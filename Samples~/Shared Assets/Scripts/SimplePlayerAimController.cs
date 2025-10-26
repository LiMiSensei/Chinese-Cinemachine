using System.Collections.Generic;
using UnityEngine;

namespace Unity.Cinemachine.Samples
{
    /// <summary>
    /// This is an add-on for SimplePlayerController that controls the player's Aiming Core.
    ///
    /// This component expects to be in a child object of a player that has a SimplePlayerController
    /// behaviour.  It works intimately with that component.
    //
    /// The purpose of the aiming core is to decouple the camera rotation from the player rotation.
    /// Camera rotation is determined by the rotation of the player core GameObject, and this behaviour
    /// provides input axes for controlling it.  When the player core is used as the target for
    /// a CinemachineCamera with a ThirdPersonFollow component, the camera will look along the core's
    /// forward axis, and pivot around the core's origin.
    ///
    /// The aiming core is also used to define the origin and direction of player shooting, if player
    /// has that ability.
    ///
    /// To implement player shooting, add a SimplePlayerShoot behaviour to this GameObject.
    /// </summary>
    public class SimplePlayerAimController : MonoBehaviour, Unity.Cinemachine.IInputAxisOwner
    {
        public enum CouplingMode { Coupled, CoupledWhenMoving, Decoupled }

        [Tooltip("玩家旋转与摄像机旋转的耦合方式。提供三种模式：\n"
        + "<b>耦合</b>：玩家随摄像机旋转。横向移动将导致侧向移动。\n"
        + "<b>移动时耦合</b>：玩家静止时摄像机可自由环绕玩家旋转，"
            + "但玩家开始移动时会旋转到面向摄像机前方。\n"
        + "<b>解耦</b>：玩家的旋转独立于摄像机的旋转。")]
        public CouplingMode PlayerRotation;

        [Tooltip("玩家开始移动时旋转到面向摄像机方向的速度。"
            + "仅在玩家旋转模式为“移动时耦合”时使用。")]
        public float RotationDamping = 0.2f;

        [Tooltip("水平视角旋转。值为角度，0 表示居中。")]
        public InputAxis HorizontalLook = new () { Range = new Vector2(-180, 180), Wrap = true, Recentering = InputAxis.RecenteringSettings.Default };

        [Tooltip("垂直视角旋转。值为角度，0 表示居中。")]
        public InputAxis VerticalLook = new () { Range = new Vector2(-70, 70), Recentering = InputAxis.RecenteringSettings.Default };

        SimplePlayerControllerBase m_Controller;
        Transform m_ControllerTransform;    // cached for efficiency
        Quaternion m_DesiredWorldRotation;

        /// Report the available input axes to the input axis controller.
        /// We use the Input Axis Controller because it works with both the Input package
        /// and the Legacy input system.  This is sample code and we
        /// want it to work everywhere.
        void IInputAxisOwner.GetInputAxes(List<IInputAxisOwner.AxisDescriptor> axes)
        {
            axes.Add(new () { DrivenAxis = () => ref HorizontalLook, Name = "Horizontal Look", Hint = IInputAxisOwner.AxisDescriptor.Hints.X });
            axes.Add(new () { DrivenAxis = () => ref VerticalLook, Name = "Vertical Look", Hint = IInputAxisOwner.AxisDescriptor.Hints.Y });
        }

        void OnValidate()
        {
            HorizontalLook.Validate();
            VerticalLook.Range.x = Mathf.Clamp(VerticalLook.Range.x, -90, 90);
            VerticalLook.Range.y = Mathf.Clamp(VerticalLook.Range.y, -90, 90);
            VerticalLook.Validate();
        }

        void OnEnable()
        {
            m_Controller = GetComponentInParent<SimplePlayerControllerBase>();
            if (m_Controller == null)
                Debug.LogError("SimplePlayerController not found on parent object");
            else
            {
                m_Controller.PreUpdate -= UpdatePlayerRotation;
                m_Controller.PreUpdate += UpdatePlayerRotation;
                m_Controller.PostUpdate -= PostUpdate;
                m_Controller.PostUpdate += PostUpdate;
                m_ControllerTransform = m_Controller.transform;
            }
        }

        void OnDisable()
        {
            if (m_Controller != null)
            {
                m_Controller.PreUpdate -= UpdatePlayerRotation;
                m_Controller.PostUpdate -= PostUpdate;
                m_ControllerTransform = null;
            }
        }

        /// <summary>Recenters the player to match my rotation</summary>
        /// <param name="damping">How long the recentering should take</param>
        public void RecenterPlayer(float damping = 0)
        {
            if (m_ControllerTransform == null)
                return;

            // Get my rotation relative to parent
            var rot = transform.localRotation.eulerAngles;
            rot.y = NormalizeAngle(rot.y);
            var delta = rot.y;
            delta = Damper.Damp(delta, damping, Time.deltaTime);

            // Rotate the parent towards me
            m_ControllerTransform.rotation = Quaternion.AngleAxis(
                delta, m_ControllerTransform.up) * m_ControllerTransform.rotation;

            // Rotate me in the opposite direction
            HorizontalLook.Value -= delta;
            rot.y -= delta;
            transform.localRotation = Quaternion.Euler(rot);
        }

        /// <summary>
        /// Set my rotation to look in this direction, without changing player rotation.
        /// Here we only set the axis values, we let the player controller do the actual rotation.
        /// </summary>
        /// <param name="worldspaceDirection">Direction to look in, in worldspace</param>
        public void SetLookDirection(Vector3 worldspaceDirection)
        {
            if (m_ControllerTransform == null)
                return;
            var rot = (Quaternion.Inverse(m_ControllerTransform.rotation)
                * Quaternion.LookRotation(worldspaceDirection, m_ControllerTransform.up)).eulerAngles;
            HorizontalLook.Value = HorizontalLook.ClampValue(rot.y);
            VerticalLook.Value = VerticalLook.ClampValue(NormalizeAngle(rot.x));
        }

        // This is called by the player controller before it updates its own rotation.
        void UpdatePlayerRotation()
        {
            var t = transform;
            t.localRotation = Quaternion.Euler(VerticalLook.Value, HorizontalLook.Value, 0);
            m_DesiredWorldRotation = t.rotation;
            switch (PlayerRotation)
            {
                case CouplingMode.Coupled:
                {
                    m_Controller.SetStrafeMode(true);
                    RecenterPlayer();
                    break;
                }
                case CouplingMode.CoupledWhenMoving:
                {
                    // If the player is moving, rotate its yaw to match the camera direction,
                    // otherwise let the camera orbit
                    m_Controller.SetStrafeMode(true);
                    if (m_Controller.IsMoving)
                        RecenterPlayer(RotationDamping);
                    break;
                }
                case CouplingMode.Decoupled:
                {
                    m_Controller.SetStrafeMode(false);
                    break;
                }
            }
            VerticalLook.UpdateRecentering(Time.deltaTime, VerticalLook.TrackValueChange());
            HorizontalLook.UpdateRecentering(Time.deltaTime, HorizontalLook.TrackValueChange());
        }

        // Callback for player controller to update our rotation after it has updated its own.
        void PostUpdate(Vector3 vel, float speed)
        {
            if (PlayerRotation == CouplingMode.Decoupled)
            {
                // After player has been rotated, we subtract any rotation change
                // from our own transform, to maintain our world rotation
                transform.rotation = m_DesiredWorldRotation;
                var delta = (Quaternion.Inverse(m_ControllerTransform.rotation) * m_DesiredWorldRotation).eulerAngles;
                VerticalLook.Value = NormalizeAngle(delta.x);
                HorizontalLook.Value = NormalizeAngle(delta.y);
            }
        }

        float NormalizeAngle(float angle)
        {
            while (angle > 180)
                angle -= 360;
            while (angle < -180)
                angle += 360;
            return angle;
        }
    }
}
