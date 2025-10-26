#if CINEMACHINE_PHYSICS || CINEMACHINE_PHYSICS_2D
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

#if CINEMACHINE_TIMELINE
using UnityEngine.Playables;
#endif

namespace Unity.Cinemachine
{
    /// <summary>
    /// A multi-purpose script which causes an action to occur when
    /// a trigger collider is entered and exited.
    /// </summary>
    [SaveDuringPlay]
    [AddComponentMenu("Cinemachine/Helpers/Cinemachine Trigger Action")]
    [HelpURL(Documentation.BaseURL + "manual/CinemachineTriggerAction.html")]
    public class CinemachineTriggerAction : MonoBehaviour
    {
        /// <summary>只有由这些层上的对象生成的触发器才会被考虑</summary>
        [Header("触发器对象过滤器")]
        [Tooltip("只有由这些层上的对象生成的触发器才会被考虑")]
        [FormerlySerializedAs("m_LayerMask")]
        public LayerMask LayerMask = 1;

        /// <summary>如果设置，只有具有此标签的对象生成的触发器才会被考虑</summary>
        [TagField]
        [Tooltip("如果设置，只有具有此标签的对象生成的触发器才会被考虑")]
        [FormerlySerializedAs("m_WithTag")]
        public string WithTag = string.Empty;

        /// <summary>具有此标签的对象生成的触发器将被忽略</summary>
        [TagField]
        [Tooltip("具有此标签的对象生成的触发器将被忽略")]
        [FormerlySerializedAs("m_WithoutTag")]
        public string WithoutTag = string.Empty;

        /// <summary>在执行操作前跳过指定数量的触发器条目</summary>
        [NoSaveDuringPlay]
        [Tooltip("在执行操作前跳过指定数量的触发器条目")]
        [FormerlySerializedAs("m_SkipFirst")]
        public int SkipFirst = 0;

        /// <summary>为所有后续触发器条目重复执行操作</summary>
        [Tooltip("为所有后续触发器条目重复执行操作")]
        [FormerlySerializedAs("m_Repeating")]
        public bool Repeating = true;

        /// <summary>当符合条件的对象进入碰撞体或触发器区域时要采取的操作</summary>
        [Tooltip("当符合条件的对象进入碰撞体或触发器区域时要采取的操作")]
        [FormerlySerializedAs("m_OnObjectEnter")]
        public ActionSettings OnObjectEnter = new(ActionSettings.ActionModes.EventOnly);

        /// <summary>当符合条件的对象离开碰撞体或触发器区域时要采取的操作</summary>
        [Tooltip("当符合条件的对象离开碰撞体或触发器区域时要采取的操作")]
        [FormerlySerializedAs("m_OnObjectExit")]
        public ActionSettings OnObjectExit = new(ActionSettings.ActionModes.EventOnly);

        HashSet<GameObject> m_ActiveTriggerObjects = new();

        /// <summary>定义在触发器进入/退出时要采取的操作</summary>
        [Serializable]
        public struct ActionSettings
        {
            /// <summary>要采取的操作</summary>
            public enum ActionModes
            {
                /// <summary>仅使用事件</summary>
                EventOnly,
                /// <summary>提升虚拟摄像机目标的优先级</summary>
                PriorityBoost,
                /// <summary>激活目标GameObject</summary>
                Activate,
                /// <summary>停用目标GameObject</summary>
                Deactivate,
                /// <summary>启用组件</summary>
                Enable,
                /// <summary>禁用组件</summary>
                Disable,
        #if CINEMACHINE_TIMELINE
                /// <summary>在目标上开始动画</summary>
                Play,
                /// <summary>在目标上停止动画</summary>
                Stop
        #endif
            }

            /// <summary>可序列化的无参数游戏事件</summary>
            [Serializable] public class TriggerEvent : UnityEvent {}

            /// <summary>要采取的操作</summary>
            [Tooltip("要采取的操作")]
            [FormerlySerializedAs("m_Action")]
            public ActionModes Action;

            /// <summary>要操作的目标对象。如果为空，则将使用当前行为/GameObject</summary>
            [Tooltip("要操作的目标对象。如果为空，则将使用当前行为/GameObject")]
            [FormerlySerializedAs("m_Target")]
            public UnityEngine.Object Target;

            /// <summary>如果是PriorityBoost，此值将添加到虚拟摄像机的优先级中</summary>
            [Tooltip("如果是PriorityBoost，此值将添加到虚拟摄像机的优先级中")]
            [FormerlySerializedAs("m_BoostAmount")]
            public int BoostAmount;

            /// <summary>如果播放时间轴，从此时间开始</summary>
            [Tooltip("如果播放时间轴，从此时间开始")]
            [FormerlySerializedAs("m_StartTime")]
            public float StartTime;

            /// <summary>如何解释开始时间</summary>
            public enum TimeModes
            {
                /// <summary>从时间轴开始处的偏移</summary>
                FromStart,
                /// <summary>从时间轴结束处的偏移</summary>
                FromEnd,
                /// <summary>当前时间轴时间之前的偏移</summary>
                BeforeNow,
                /// <summary>当前时间轴时间之后的偏移</summary>
                AfterNow
            };

            /// <summary>如何解释开始时间</summary>
            [Tooltip("如何解释开始时间")]
            [FormerlySerializedAs("m_Mode")]
            public TimeModes Mode;

            /// <summary>此事件将被调用</summary>
            [Tooltip("此事件将被调用")]
            [FormerlySerializedAs("m_Event")]
            public TriggerEvent Event;

            /// <summary>Standard Constructor</summary>
            /// <param name="action">Action to set</param>
            public ActionSettings(ActionModes action)
            {
                Action = action;
                Target = null;
                BoostAmount = 0;
                StartTime = 0;
                Mode = TimeModes.FromStart;
                Event = new TriggerEvent();
            }

            /// <summary>Invoke the action.  Depending on the mode, different action will
            /// be performed.  The embedded event will always be invoked, in addition to the
            /// action specified by the Mode.</summary>
            public void Invoke()
            {
                UnityEngine.Object currentTarget = Target;
                if (currentTarget != null)
                {
                    var targetGameObject = currentTarget as GameObject;
                    var targetBehaviour = currentTarget as Behaviour;
                    if (targetBehaviour != null)
                        targetGameObject = targetBehaviour.gameObject;

                    switch (Action)
                    {
                        case ActionModes.EventOnly:
                            break;
                        case ActionModes.PriorityBoost:
                            {
                                if (targetGameObject.TryGetComponent<CinemachineVirtualCameraBase>(out var vcam))
                                {
                                    vcam.Priority.Value += BoostAmount;
                                    vcam.Prioritize();
                                }
                                break;
                            }
                        case ActionModes.Activate:
                            if (targetGameObject != null)
                            {
                                targetGameObject.SetActive(true);
                                if (targetGameObject.TryGetComponent<CinemachineVirtualCameraBase>(out var vcam))
                                    vcam.Prioritize();
                            }
                            break;
                        case ActionModes.Deactivate:
                            if (targetGameObject != null)
                                targetGameObject.SetActive(false);
                            break;
                        case ActionModes.Enable:
                            {
                                if (targetBehaviour != null)
                                    targetBehaviour.enabled = true;
                                break;
                            }
                        case ActionModes.Disable:
                            {
                                if (targetBehaviour != null)
                                    targetBehaviour.enabled = false;
                                break;
                            }
#if CINEMACHINE_TIMELINE
                        case ActionModes.Play:
                            {
                                if (targetGameObject.TryGetComponent<PlayableDirector>(out var playable))
                                {
                                    double startTime = 0;
                                    double duration = playable.duration;
                                    double current = playable.time;
                                    switch (Mode)
                                    {
                                        default:
                                        case TimeModes.FromStart:
                                            startTime += StartTime;
                                            break;
                                        case TimeModes.FromEnd:
                                            startTime = duration - StartTime;
                                            break;
                                        case TimeModes.BeforeNow:
                                            startTime = current - StartTime;
                                            break;
                                        case TimeModes.AfterNow:
                                            startTime = current + StartTime;
                                            break;
                                    }
                                    playable.time = startTime;
                                    playable.Play();
                                }
                                else
                                {
                                    if (targetGameObject.TryGetComponent<Animation>(out var ani))
                                        ani.Play();
                                }
                                break;
                            }
                        case ActionModes.Stop:
                            {
                                if (targetGameObject.TryGetComponent<PlayableDirector>(out var playable))
                                    playable.Stop();
                                else
                                {
                                    if (targetGameObject.TryGetComponent<Animation>(out var ani))
                                        ani.Stop();
                                }
                                break;
                            }
#endif
                    }
                }
                Event.Invoke();
            }
        }

        private bool Filter(GameObject other)
        {
            if (!enabled)
                return false;
            if (((1 << other.layer) & LayerMask) == 0)
                return false;
            if (WithTag.Length != 0 && !other.CompareTag(WithTag))
                return false;
            if (WithoutTag.Length != 0 && other.CompareTag(WithoutTag))
                return false;

            return true;
        }

        void InternalDoTriggerEnter(GameObject other)
        {
            if (!Filter(other))
                return;
            --SkipFirst;
            if (SkipFirst > -1)
                return;
            if (!Repeating && SkipFirst != -1)
                return;

            m_ActiveTriggerObjects.Add(other);
            OnObjectEnter.Invoke();
        }

        void InternalDoTriggerExit(GameObject other)
        {
            if (!m_ActiveTriggerObjects.Contains(other))
                return;
            m_ActiveTriggerObjects.Remove(other);
            if (enabled)
                OnObjectExit.Invoke();
        }

#if CINEMACHINE_PHYSICS
        void OnTriggerEnter(Collider other) => InternalDoTriggerEnter(other.gameObject);
        void OnTriggerExit(Collider other) => InternalDoTriggerExit(other.gameObject);
        void OnCollisionEnter(Collision other) => InternalDoTriggerEnter(other.gameObject);
        void OnCollisionExit(Collision other) => InternalDoTriggerExit(other.gameObject);
#endif
#if CINEMACHINE_PHYSICS_2D
        void OnTriggerEnter2D(Collider2D other) => InternalDoTriggerEnter(other.gameObject);
        void OnTriggerExit2D(Collider2D other) => InternalDoTriggerExit(other.gameObject);
        void OnCollisionEnter2D(Collision2D other) => InternalDoTriggerEnter(other.gameObject);
        void OnCollisionExit2D(Collision2D other) => InternalDoTriggerExit(other.gameObject);
#endif
        void OnEnable() {} // For the Enabled checkbox
    }
}
#endif
