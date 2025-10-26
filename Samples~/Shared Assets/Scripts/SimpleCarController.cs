using System.Collections.Generic;
using UnityEngine;

namespace Unity.Cinemachine.Samples
{
    public class SimpleCarController : MonoBehaviour, Unity.Cinemachine.IInputAxisOwner
    {
        public float MotorStrength = 2000;
        public float BrakeStrength = 5000;
        public float MaxTurnAngle = 50;

        public WheelCollider FrontLeftWheelCollider;
        public WheelCollider FrontRightWheelCollider;
        public WheelCollider RearLeftWheelCollider;
        public WheelCollider RearRightWheelCollider;

        public Transform FrontLeftWheel;
        public Transform FrontRightWhee;
        public Transform RearLeftWheel;
        public Transform RearRightWheel;

        [Header("输入轴")]
        [Tooltip("X轴移动。值为-1..1。控制转向量")]
        public InputAxis MoveX = InputAxis.DefaultMomentary;

        [Tooltip("Z轴移动。值为-1..1。控制前进加速度")]
        public InputAxis MoveZ = InputAxis.DefaultMomentary;

        [Tooltip("刹车。值为0到1。控制刹车力度")]
        public InputAxis Brake = InputAxis.DefaultMomentary;


        /// Report the available input axes to the input axis controller.
        /// We use the Input Axis Controller because it works with both the Input package
        /// and the Legacy input system.  This is sample code and we
        /// want it to work everywhere.
        void IInputAxisOwner.GetInputAxes(List<IInputAxisOwner.AxisDescriptor> axes)
        {
            axes.Add(new () { DrivenAxis = () => ref MoveX, Name = "Move X", Hint = IInputAxisOwner.AxisDescriptor.Hints.X });
            axes.Add(new () { DrivenAxis = () => ref MoveZ, Name = "Move Z", Hint = IInputAxisOwner.AxisDescriptor.Hints.Y });
            axes.Add(new () { DrivenAxis = () => ref Brake, Name = "Brake" });
        }

        void OnValidate()
        {
            MoveX.Validate();
            MoveZ.Validate();
            Brake.Validate();
        }

        void FixedUpdate()
        {
            // Acceleration
            var force = MotorStrength * MoveZ.Value;
            FrontLeftWheelCollider.motorTorque = force;
            FrontRightWheelCollider.motorTorque = force;

            // Braking
            force = BrakeStrength * Brake.Value;
            FrontRightWheelCollider.brakeTorque = force;
            FrontLeftWheelCollider.brakeTorque = force;
            RearLeftWheelCollider.brakeTorque = force;
            RearRightWheelCollider.brakeTorque = force;
            if (Brake.Value > 0.99f)
                MoveZ.Value = 0;

            // Steering
            force = MaxTurnAngle * MoveX.Value;
            FrontLeftWheelCollider.steerAngle = force;
            FrontRightWheelCollider.steerAngle = force;

            // Place the wheels
            UpdateWheel(FrontLeftWheelCollider, FrontLeftWheel);
            UpdateWheel(FrontRightWheelCollider, FrontRightWhee);
            UpdateWheel(RearRightWheelCollider, RearRightWheel);
            UpdateWheel(RearLeftWheelCollider, RearLeftWheel);
        }

        void UpdateWheel(WheelCollider c, Transform t)
        {
            c.GetWorldPose(out var pos, out var rot);
            t.SetPositionAndRotation(pos, rot);
        }
    }
}