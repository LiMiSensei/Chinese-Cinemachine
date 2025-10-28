using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.Cinemachine
{
    /// <summary>
    /// Cinemachine ClearShot 是一个"管理器摄像机"，它拥有并管理一组
    /// 虚拟摄像机游戏对象子项。当激活时，ClearShot 会检查所有子摄像机，
    /// 选择具有最佳拍摄质量的摄像机并将其激活。
    ///
    /// 这是一个非常强大的工具。如果子摄像机具有拍摄评估器扩展，
    /// 它们将分析场景中的目标遮挡、最佳目标距离等因素，
    /// 并将拍摄质量评估报告给 ClearShot 父对象，然后选择最佳摄像机。
    /// 您可以使用此功能设置场景的复杂多摄像机覆盖，
    /// 并确保始终可以获得目标的清晰拍摄视角。
    ///
    /// 如果多个子摄像机具有相同的拍摄质量，将选择优先级最高的摄像机。
    ///
    /// 您还可以在 ClearShot 子摄像机之间定义自定义混合效果。
    /// </summary>
    [DisallowMultipleComponent]
    [ExecuteAlways]
    [ExcludeFromPreset]
    [SaveDuringPlay]
    [AddComponentMenu("Cinemachine/Cinemachine ClearShot")]
    [HelpURL(Documentation.BaseURL + "manual/CinemachineClearShot.html")]
    public class CinemachineClearShot : CinemachineCameraManagerBase
    {
        [RedHeader("Help：管理器摄像机，评估器目标遮挡、距离等因素，然后选择最佳摄像机。")]
        [Space(30)]
        /// <summary>激活新的子相机前等待的秒数</summary>
        [Tooltip("激活新的子相机前等待的秒数")]
        [FormerlySerializedAs("m_ActivateAfter")]
        public float ActivateAfter;

        /// <summary>激活的相机必须保持激活状态至少这么多秒</summary>
        [Tooltip("激活的相机必须保持激活状态至少这么多秒")]
        [FormerlySerializedAs("m_MinDuration")]
        public float MinDuration;

        /// <summary>如果勾选，当多个相机同等理想时，相机选择将被随机化。
        /// 否则，将使用子相机列表顺序和优先级</summary>
        [Tooltip("如果勾选，当多个相机同等理想时，相机选择将被随机化。"
        + "否则，将使用子相机列表顺序和优先级。")]
        [FormerlySerializedAs("m_RandomizeChoice")]
        public bool RandomizeChoice = false;

        [SerializeField, HideInInspector, NoSaveDuringPlay, FormerlySerializedAs("m_LookAt")] Transform m_LegacyLookAt;
        [SerializeField, HideInInspector, NoSaveDuringPlay, FormerlySerializedAs("m_Follow")] Transform m_LegacyFollow;

        float m_ActivationTime = 0;
        float m_PendingActivationTime = 0;
        CinemachineVirtualCameraBase m_PendingCamera;
        bool m_RandomizeNow = false;
        List<CinemachineVirtualCameraBase> m_RandomizedChildren = null;

        /// <inheritdoc />
        protected override void Reset()
        {
            base.Reset();
            ActivateAfter = 0;
            MinDuration = 0;
            RandomizeChoice = false;
            DefaultBlend = new (CinemachineBlendDefinition.Styles.EaseInOut, 0.5f);
            CustomBlends = null;
        }

        /// <inheritdoc/>
        protected internal override void PerformLegacyUpgrade(int streamedVersion)
        {
            base.PerformLegacyUpgrade(streamedVersion);
            if (streamedVersion < 20220721)
            {
                if (m_LegacyLookAt != null || m_LegacyFollow != null)
                {
                    DefaultTarget = new DefaultTargetSettings
                    {
                        Enabled = true,
                        Target = new CameraTarget
                        {
                            LookAtTarget = m_LegacyLookAt,
                            TrackingTarget = m_LegacyFollow,
                            CustomLookAtTarget = m_LegacyLookAt != m_LegacyFollow
                        }
                    };
                    m_LegacyLookAt = m_LegacyFollow = null;
                }
            }
        }

        /// <inheritdoc />
        public override void OnTransitionFromCamera(
            ICinemachineCamera fromCam, Vector3 worldUp, float deltaTime)
        {
            if (RandomizeChoice && !IsBlending)
                m_RandomizedChildren = null;
            base.OnTransitionFromCamera(fromCam, worldUp, deltaTime);
        }

        /// <summary>If RandomizeChoice is enabled, call this to re-randomize the children next frame.
        /// This is useful if you want to freshen up the shot.</summary>
        public void ResetRandomization()
        {
            m_RandomizedChildren = null;
            m_RandomizeNow = true;
        }

        /// <inheritdoc />
        protected override CinemachineVirtualCameraBase ChooseCurrentCamera(Vector3 worldUp, float deltaTime)
        {
            if (!PreviousStateIsValid)
            {
                m_ActivationTime = 0;
                m_PendingActivationTime = 0;
                m_PendingCamera = null;
                m_RandomizedChildren = null;
            }

            var liveChild = LiveChild as CinemachineVirtualCameraBase;
            if (ChildCameras == null || ChildCameras.Count == 0)
            {
                m_ActivationTime = 0;
                return null;
            }

            var allCameras = ChildCameras;
            if (!RandomizeChoice)
                m_RandomizedChildren = null;
            else if (allCameras.Count> 1)
            {
                m_RandomizedChildren ??= Randomize(allCameras);
                allCameras = m_RandomizedChildren;
            }

            if (liveChild != null && (!liveChild.IsValid || !liveChild.gameObject.activeSelf))
                liveChild = null;

            var best = liveChild;
            for (var i = 0; i < allCameras.Count; ++i)
            {
                var vcam = allCameras[i];
                if (vcam != null && vcam.gameObject.activeInHierarchy)
                {
                    // Choose the first in the list that is better than the current
                    if (best == null
                        || vcam.State.ShotQuality > best.State.ShotQuality
                        || (vcam.State.ShotQuality == best.State.ShotQuality && vcam.Priority.Value > best.Priority.Value)
                        || (RandomizeChoice && m_RandomizeNow && vcam != liveChild
                            && vcam.State.ShotQuality == best.State.ShotQuality
                            && vcam.Priority.Value == best.Priority.Value))
                    {
                        best = vcam;
                    }
                }
            }
            m_RandomizeNow = false;

            float now = CinemachineCore.CurrentTime;
            if (m_ActivationTime != 0)
            {
                // Is it active now?
                if (liveChild == best)
                {
                    // Yes, cancel any pending
                    m_PendingActivationTime = 0;
                    m_PendingCamera = null;
                    return best;
                }

                // Is it pending?
                if (PreviousStateIsValid)
                {
                    if (m_PendingActivationTime != 0 && m_PendingCamera == best)
                    {
                        // Has it been pending long enough, and are we allowed to switch away
                        // from the active action?
                        if ((now - m_PendingActivationTime) > ActivateAfter
                            && (now - m_ActivationTime) > MinDuration)
                        {
                            // Yes, activate it now
                            m_RandomizedChildren = null; // reshuffle the children
                            m_ActivationTime = now;
                            m_PendingActivationTime = 0;
                            m_PendingCamera = null;
                            return best;
                        }
                        return liveChild;
                    }
                }
            }
            // Neither active nor pending.
            m_PendingActivationTime = 0; // cancel the pending, if any
            m_PendingCamera = null;

            // Can we activate it now?
            if (PreviousStateIsValid && m_ActivationTime > 0)
            {
                if (ActivateAfter > 0
                    || (now - m_ActivationTime) < MinDuration)
                {
                    // Too early - make it pending
                    m_PendingCamera = best;
                    m_PendingActivationTime = now;
                    return liveChild;
                }
            }
            // Activate now
            m_RandomizedChildren = null; // reshuffle the children
            m_ActivationTime = now;
            return best;
        }

        struct Pair { public int a; public float b; }

        static List<CinemachineVirtualCameraBase> Randomize(List<CinemachineVirtualCameraBase> src)
        {
            var pairs = new List<Pair>();
            for (var i = 0; i < src.Count; ++i)
            {
                var p = new Pair { a = i, b = Random.Range(0, 1000f) };
                pairs.Add(p);
            }
            pairs.Sort((p1, p2) => (int)p1.b - (int)p2.b);
            var dst = new List<CinemachineVirtualCameraBase>(src.Count);
            for (var i = 0; i < src.Count; ++i)
                dst.Add(src[pairs[i].a]);
            return dst;
        }
    }
}
