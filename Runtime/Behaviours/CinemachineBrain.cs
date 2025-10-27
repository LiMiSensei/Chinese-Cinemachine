//#define CINEMACHINE_RESET_PROJECTION_MATRIX // GML todo: decide on the correct solution

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Unity.Cinemachine
{
    /// <summary>
    /// CinemachineBrain 是 Unity 摄像机与场景中 Cinemachine 摄像机之间的桥梁。
    /// 它监控优先级堆栈以选择当前的 Cinemachine 摄像机，并在需要时与另一个摄像机进行混合。
    /// 最后也是最重要的，它将 CinemachineCamera 状态应用到所连接的 Unity 摄像机上。
    ///
    /// CinemachineBrain 也是定义 Cinemachine 摄像机之间混合规则的地方。
    /// 摄像机混合是指一个 Cinemachine 摄像机的位置和状态随时间插值过渡到另一个摄像机。
    /// 如果将 CinemachineCamera 视为摄影师，那么混合就像是一个摄影师将摄像机平稳地传递给
    /// 另一个摄影师。您可以指定混合的时间长度以及混合曲线的形状。
    /// 请注意，摄像机剪切就是零时间混合。
    /// </summary>
    [DisallowMultipleComponent]
    [ExecuteAlways]
    [AddComponentMenu("Cinemachine/Cinemachine Brain")]
    [HelpURL(Documentation.BaseURL + "manual/CinemachineBrain.html")]
    public class CinemachineBrain : MonoBehaviour, ICameraOverrideStack, ICinemachineMixer
    {
        [RedHeader("Help：CinemachineBrain 也是定义 Cinemachine 摄像机之间混合规则的地方")]
        [Space(30)]
        /// <summary>
        /// When enabled, the current camera and blend are indicated in the game window,
        /// for debugging.
        /// </summary>
        [Tooltip("启用后，游戏窗口中会显示当前相机和混合过渡效果，用于调试。")]
        [FormerlySerializedAs("m_ShowDebugText")]
        public bool ShowDebugText = false;

        /// <summary>
        /// When enabled, shows the camera's frustum in the Scene view.
        /// </summary>
        [Tooltip("启用后，场景视图中会始终显示相机的视锥体。")]
        [FormerlySerializedAs("m_ShowCameraFrustum")]
        public bool ShowCameraFrustum = true;

        /// <summary>
        /// When enabled, the cameras always respond in real-time to user input and damping,
        /// even if the game is running in slow motion.
        /// </summary>
        [Tooltip("启用后，即使游戏以慢动作运行，相机也会始终实时时间响应用户输入和阻尼做出实时响应。")]
        [FormerlySerializedAs("m_IgnoreTimeScale")]
        public bool IgnoreTimeScale = false;

        /// <summary>
        /// If set, this GameObject's Y axis defines the world-space Up vector for all the
        /// CinemachineCameras.  This is useful in top-down game environments.  If not set,
        /// Up is world-space Y.
        /// </summary>
        [Tooltip("若设置此选项，该游戏对象（GameObject）的 Y 轴将为所有 Cinemachine 相机（CinemachineCameras）定义世界空间（world-space）中的上方向向量（Up vector）。例如，在俯视角（top-down）游戏场景中，此设置十分实用。若未设置，上方向（Up）将默认采用世界空间的 Y 轴。恰当设置此选项至关重要，因为 Cinemachine 相机不适合直接朝上或朝下拍摄。")]
        [FormerlySerializedAs("m_WorldUpOverride")]
        public Transform WorldUpOverride;

        /// <summary>
        /// The CinemachineBrain finds the highest-priority CinemachineCamera that outputs
        /// to any of the channels selected.  CinemachineCameras that do not output to one
        /// of these channels are ignored.  Use this in situations where multiple
        /// CinemachineBrains are needed (for example, Split-screen).
        /// </summary>
        [Tooltip("Cinemachine 控制器（CinemachineBrain）会查找优先级最高且输出到所选任一通道的 Cinemachine 相机（CinemachineCamera）。未输出到这些通道之一的 Cinemachine 相机会被忽略。此功能适用于需要多个 Cinemachine 控制器的场景（例如分屏（Split-screen）场景）。")]
        public OutputChannels ChannelMask = (OutputChannels)(-1);  // default is Everything

        /// <summary>The options available for the update method.</summary>
        public enum UpdateMethods
        {
            /// <summary>CinemachineCameras are updated in sync with the Physics module, in FixedUpdate.</summary>
            FixedUpdate,
            /// <summary>CinemachineCameras are updated in MonoBehaviour LateUpdate.</summary>
            LateUpdate,
            /// <summary>CinemachineCameras are updated according to how the target is updated.</summary>
            SmartUpdate,
            /// <summary>CinemachineCameras are not automatically updated, client must explicitly call
            /// the CinemachineBrain's ManualUpdate() method.</summary>
            ManualUpdate
        };

        /// <summary>
        /// Depending on how the target GameObjects are animated, adjust the update method to
        /// minimize the potential jitter.  Use FixedUpdate if all your targets are animated
        /// with for RigidBody animation.  SmartUpdate chooses the best method for each
        /// CinemachineCamera, depending on how the target is animated.
        /// </summary>
        [Tooltip("Cinemachine 相机（CinemachineCameras）的更新时间设置。"
+ "若所有目标在 FixedUpdate 期间进行动画（例如刚体（RigidBodies）），请使用 FixedUpdate；"
+ "若所有目标在常规 Update 循环期间进行动画，请使用 LateUpdate；"
+ "若希望 Cinemachine 根据每个目标的情况自动采取适当的更新方式，请使用 SmartUpdate。"
+ "推荐设置为 SmartUpdate。")]
        [FormerlySerializedAs("m_UpdateMethod")]
        public UpdateMethods UpdateMethod = UpdateMethods.SmartUpdate;

        /// <summary>The options available for the update method.</summary>
        public enum BrainUpdateMethods
        {
            /// <summary>Camera is updated in sync with the Physics module, in FixedUpdate.</summary>
            FixedUpdate,
            /// <summary>Camera is updated in MonoBehaviour LateUpdate (or when ManualUpdate is called).</summary>
            LateUpdate
        };

        /// <summary>
        /// The update time for the Brain, i.e. when the blends are evaluated and the
        /// brain's transform is updated.
        /// </summary>
        [Tooltip("“大脑” 的更新时间，即对混合内容（或 “融合物”，具体含义需结合上下文）进行评估并更新 “大脑” 转换功能的时间。")]
        [FormerlySerializedAs("m_BlendUpdateMethod")]
        public BrainUpdateMethods BlendUpdateMethod = BrainUpdateMethods.LateUpdate;

        /// <summary>Defines the settings for Lens Mode overriding.</summary>
        [Serializable]
        public struct LensModeOverrideSettings
        {
            /// <summary>如果启用，允许Cinemachine相机覆盖相机的镜头模式。</summary>
        [Tooltip("如果启用，允许Cinemachine相机覆盖相机的镜头模式")]
        public bool Enabled;

        /// <summary>当没有激活模式覆盖时使用的镜头模式。</summary>
        [Tooltip("当没有激活模式覆盖时使用的镜头模式")]
            public LensSettings.OverrideModes DefaultMode;
        }

        /// <summary>Controls whether CinemachineCameras can change the lens mode.</summary>
        [FoldoutWithEnabledButton]
        public LensModeOverrideSettings LensModeOverride
            = new () { DefaultMode = LensSettings.OverrideModes.Perspective };

        /// <summary>
        /// The blend that is used if you don't explicitly define a blend between two CinemachineCameras.
        /// </summary>
        [Tooltip("当未明确定义两个Cinemachine相机之间的混合时使用的默认混合设置")]
        [FormerlySerializedAs("m_DefaultBlend")]
        public CinemachineBlendDefinition DefaultBlend = new (CinemachineBlendDefinition.Styles.EaseInOut, 2f);

        /// <summary>
        /// 该资源包含场景中特定Cinemachine相机之间
        /// 混合的自定义设置。
        /// </summary>
        [Tooltip("该资源包含场景中特定Cinemachine相机之间混合的自定义设置")]
        [FormerlySerializedAs("m_CustomBlends")]
        [EmbeddedBlenderSettingsProperty]
        public CinemachineBlenderSettings CustomBlends = null;

        Camera m_OutputCamera = null; // never use directly - use accessor
        GameObject m_TargetOverride = null; // never use directly - use accessor

        int m_LastFrameUpdated;
        Coroutine m_PhysicsCoroutine;
        readonly WaitForFixedUpdate m_WaitForFixedUpdate = new ();
        readonly BlendManager m_BlendManager = new ();
        static readonly List<CinemachineBrain> s_ActiveBrains = new ();
        CameraState m_CameraState; // Cached camera state

#if CINEMACHINE_UIELEMENTS && UNITY_EDITOR
        DebugText m_DebugText;
#endif

        void OnValidate()
        {
            DefaultBlend.Time = Mathf.Max(0, DefaultBlend.Time);
        }

        void Reset()
        {
            DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Styles.EaseInOut, 2f);
            CustomBlends = null;
            ShowDebugText = false;
            ShowCameraFrustum = true;
            IgnoreTimeScale = false;
            WorldUpOverride = null;
            ChannelMask = (OutputChannels)(-1);
            UpdateMethod = UpdateMethods.SmartUpdate;
            BlendUpdateMethod = BrainUpdateMethods.LateUpdate;
            LensModeOverride = new LensModeOverrideSettings { DefaultMode = LensSettings.OverrideModes.Perspective };
        }

        void Awake()
        {
            ControlledObject.TryGetComponent(out m_OutputCamera);
        }

        void Start()
        {
            m_LastFrameUpdated = -1;
            UpdateVirtualCameras(CameraUpdateManager.UpdateFilter.Late, -1f);
        }

        void OnEnable()
        {
            m_BlendManager.OnEnable();
            m_BlendManager.LookupBlendDelegate = LookupBlend;

            s_ActiveBrains.Add(this);
#if UNITY_EDITOR && CINEMACHINE_UIELEMENTS
            CinemachineDebug.OnGUIHandlers -= OnGuiHandler;
            CinemachineDebug.OnGUIHandlers += OnGuiHandler;
#endif

            // We check in after the physics system has had a chance to move things
            m_PhysicsCoroutine = StartCoroutine(AfterPhysics());

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;

#if UNITY_EDITOR && CINEMACHINE_UIELEMENTS
            CinemachineDebug.OnGUIHandlers -= OnGuiHandler;
            m_DebugText?.Dispose();
            m_DebugText = null;
#endif
            s_ActiveBrains.Remove(this);

            m_BlendManager.OnDisable();
            StopCoroutine(m_PhysicsCoroutine);
            UpdateTracker.ForgetContext(this);
            CameraUpdateManager.ForgetContext(this);
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (Time.frameCount == m_LastFrameUpdated 
                    && m_BlendManager.IsInitialized && UpdateMethod != UpdateMethods.ManualUpdate)
                DoNonFixedUpdate(Time.frameCount);
        }

        void OnSceneUnloaded(Scene scene)
        {
            if (Time.frameCount == m_LastFrameUpdated 
                    && m_BlendManager.IsInitialized && UpdateMethod != UpdateMethods.ManualUpdate)
                DoNonFixedUpdate(Time.frameCount);
        }

        void LateUpdate()
        {
            if (UpdateMethod != UpdateMethods.ManualUpdate)
                DoNonFixedUpdate(Time.frameCount);
        }

        // Instead of FixedUpdate() we have this, to ensure that it happens
        // after all physics updates have taken place
        IEnumerator AfterPhysics()
        {
            while (true)
            {
                // FixedUpdate can be called multiple times per frame
                yield return m_WaitForFixedUpdate;
                DoFixedUpdate();
            }
        }

#if UNITY_EDITOR
        /// This is only needed in editor mode to force timeline to call OnGUI while
        /// timeline is up and the game is not running, in order to allow dragging
        /// the composer guide in the game view.
        void OnPreCull()
        {
            if (!Application.isPlaying)
            {
                // Note: this call causes any screen canvas attached to the camera
                // to be painted one frame out of sync.  It only happens in the editor when not playing.
                ProcessActiveCamera(GetEffectiveDeltaTime(false));
            }
        }

        // We don't want this in runtime because it's only for debugging and it can generate garbage
        void OnGUI()
        {
            if (CinemachineDebug.OnGUIHandlers != null && Event.current.type != EventType.Layout)
                CinemachineDebug.OnGUIHandlers(this);
        }

    #if CINEMACHINE_UIELEMENTS
        void OnGuiHandler(CinemachineBrain brain)
        {
            if (!ShowDebugText && m_DebugText != null)
            {
                m_DebugText.Dispose();
                m_DebugText = null;
            }

            if (!ShowDebugText || brain != this)
                return;

            m_DebugText ??= new DebugText(OutputCamera);

            // Show the active camera and blend
            var sb = CinemachineDebug.SBFromPool();
            sb.Length = 0;
            sb.Append("CM ");
            sb.Append(gameObject.name);
            sb.Append(": ");
            if (CinemachineCore.SoloCamera != null)
            {
                sb.Append("SOLO ");
                m_DebugText.SetTextColor(CinemachineCore.SoloGUIColor());
            }
            else
                m_DebugText.RestoreOriginalTextColor();

            if (IsBlending)
                sb.Append(ActiveBlend.Description);
            else
            {
                var vcam = ActiveVirtualCamera;
                if (vcam == null)
                    sb.Append("(none)");
                else
                {
                    sb.Append(vcam.Name);
                    var desc = vcam.Description;
                    if (!string.IsNullOrEmpty(desc))
                    {
                        sb.Append(" ");
                        sb.Append(desc);
                    }
                }
            }

            m_DebugText.SetText(sb.ToString());
            CinemachineDebug.ReturnToPool(sb);
        }
    #endif
#endif

        // ============ ICameraOverrideStack implementation ================

        /// <inheritdoc />
        public int SetCameraOverride(
            int overrideId, int priority,
            ICinemachineCamera camA, ICinemachineCamera camB,
            float weightB, float deltaTime)
                => m_BlendManager.SetCameraOverride(overrideId, priority, camA, camB, weightB, deltaTime);

        /// <inheritdoc />
        public void ReleaseCameraOverride(int overrideId) => m_BlendManager.ReleaseCameraOverride(overrideId);

        /// <summary>Get the default world up for the CinemachineCameras.</summary>
        public Vector3 DefaultWorldUp => (WorldUpOverride != null) ? WorldUpOverride.transform.up : Vector3.up;

        // ============ ICinemachineMixer implementation ================

        /// <inheritdoc />
        public string Name => name;

        /// <inheritdoc />
        public string Description
        {
            get
            {
                if (ActiveVirtualCamera == null)
                    return "(none)";
                if (IsBlending)
                    return ActiveBlend.Description;
                var sb = CinemachineDebug.SBFromPool();
                sb.Append(ActiveVirtualCamera.Name);
                sb.Append(" ");
                sb.Append(ActiveVirtualCamera.Description);
                var text = sb.ToString();
                CinemachineDebug.ReturnToPool(sb);
                return text;
            }
        }

        /// <inheritdoc />
        public CameraState State => m_CameraState;

        /// <inheritdoc />
        public bool IsValid => this != null;

        /// <inheritdoc />
        public ICinemachineMixer ParentCamera => null; // GML todo: think about this

        /// <inheritdoc />
        public void UpdateCameraState(Vector3 up, float deltaTime) {} // GML todo: think about this

        /// <inheritdoc />
        public void OnCameraActivated(ICinemachineCamera.ActivationEventParams evt) {} // GML todo: think about this

        /// <inheritdoc />
        public bool IsLiveChild(ICinemachineCamera cam, bool dominantChildOnly = false)
        {
            if (CinemachineCore.SoloCamera == cam || m_BlendManager.IsLive(cam))
                return true;

            // Walk up the parents
            var parent = cam.ParentCamera;
            if (parent != null && parent.IsLiveChild(cam, dominantChildOnly))
                return IsLiveChild(parent, dominantChildOnly);
            return false;
        }

        // ============ Global Brain cache ================

        /// <summary>Access the array of active CinemachineBrains in the Scene</summary>
        public static int ActiveBrainCount => s_ActiveBrains.Count;

        /// <summary>
        /// Access the array of active CinemachineBrains in the Scene without generating garbage.
        /// </summary>
        /// <param name="index">Index of the brain to access, range 0-ActiveBrainCount.</param>
        /// <returns>The brain at the specified index.</returns>
        public static CinemachineBrain GetActiveBrain(int index) => s_ActiveBrains[index];

        // ============================

        /// <summary>
        /// CinemachineBrain controls this GameObject.  Normally, this is the GameObject to which
        /// the CinemachineBrain component is attached.  However, it is possible to override this
        /// by setting this property to another GameObject.  If a Camera component is attached to the
        /// Controlled GameObject, then that Camera component's lens settings is also driven
        /// by the CinemachineBrain.
        /// If this property is set to null, then CinemachineBrain is controlling the GameObject
        /// to which it is attached.  The value of this property always reports as non-null.
        /// </summary>
        public GameObject ControlledObject
        {
            get => m_TargetOverride == null ? gameObject : m_TargetOverride;
            set
            {
                if (!ReferenceEquals(m_TargetOverride, value))
                {
                    m_TargetOverride = value;
                    ControlledObject.TryGetComponent(out m_OutputCamera); // update output camera when target changes
                }
            }
        }

        /// <summary>
        /// Get the Unity Camera that is attached to this GameObject.  This is the camera
        /// that is controlled by the CinemachineBrain.
        /// </summary>
        public Camera OutputCamera
        {
            get
            {
                if (m_OutputCamera == null && !Application.isPlaying)
                    ControlledObject.TryGetComponent(out m_OutputCamera);
                return m_OutputCamera;
            }
        }

        /// <summary>
        /// Get the current active CinemachineCamera.
        /// </summary>
        public ICinemachineCamera ActiveVirtualCamera
            => CinemachineCore.SoloCamera ?? m_BlendManager.ActiveVirtualCamera;

        /// <summary>
        /// Call this to reset the current active camera, causing the brain to choose a new
        /// one without blending.  It is useful, for example, when you want to restart a game level.
        /// </summary>
        public void ResetState() => m_BlendManager.ResetRootFrame();

        /// <summary>
        /// Indicates if there is a blend in progress.
        /// </summary>
        public bool IsBlending => m_BlendManager.IsBlending;

        /// <summary>
        /// Get the current blend in progress.  Returns null if none.
        /// It is also possible to set the current blend, but this is not a recommended usage
        /// unless it is to set the active blend to null, which forces the completion of the blend.
        /// </summary>
        public CinemachineBlend ActiveBlend
        {
            get => m_BlendManager.ActiveBlend;
            set => m_BlendManager.ActiveBlend = value;
        }

        /// <summary>
        /// Returns true if the CinemachineCamera is on a channel that is handled
        /// by this CinemachineBrain.
        /// </summary>
        /// <param name="vcam">The CinemachineCamera to check.</param>
        /// <returns>True if the CinemachineCamera is on a channel that is handled by this Brain.</returns>
        public bool IsValidChannel(CinemachineVirtualCameraBase vcam)
            => vcam != null && ((uint)vcam.OutputChannel & (uint)ChannelMask) != 0;

        /// <summary>
        /// Checks if the CinemachineCamera is live as part of an outgoing blend.
        /// Does not check whether the CinemachineCamera is also the current active CinemachineCamera.
        /// </summary>
        /// <param name="cam">The CinemachineCamera to check.</param>
        /// <returns>True if the CinemachineCamera is part of a live outgoing blend, false otherwise.</returns>
        public bool IsLiveInBlend(ICinemachineCamera cam)
        {
            if (m_BlendManager.IsLiveInBlend(cam))
                return true;

            // Walk up the parents
            var parent = cam.ParentCamera;
            if (parent != null && parent.IsLiveChild(cam, false))
                return IsLiveInBlend(parent);
            return false;
        }

        /// <summary>
        /// Updates CinemachineCameras and positions the main camera when UpdateMode is set to ManualUpdate.
        /// This method should only be called in ManualUpdate mode. For other modes, updates occur 
        /// automatically and this method should not be called explicitly.
        /// </summary>
        /// <param name="currentFrame">The current update frmae.  This is a substitute for Time.frameCount.  
        /// If you're controlling your own player loop and timestep, this parameter indicates the current frame.  
        /// Each call should increase this number by 1.</param>
        /// <param name="deltaTime">The game time that elapsed since the last call to this method.  A value of -1 will 
        /// cancel previous frame state, effectively canceling damping and making the CinemachineCameras snap to position.</param>
        /// <remarks>
        /// Important usage notes:
        /// <list type="bullet">
        /// <item>Never call this method from FixedUpdate.</item>
        /// <item>This version of the method allows you to explicitly control the update frame count and the deltaTime.</item>
        /// <item>This method must be called exactly once per update frame - more frequent calls 
        /// will not update the cameras, and less frequent calls may cause jerky camera movement.</item>
        /// </list>
        /// </remarks>
        public void ManualUpdate(int currentFrame, float deltaTime)
        {
#if UNITY_EDITOR
            if (UpdateMethod != UpdateMethods.ManualUpdate)
                Debug.LogError("CinemachineBrain.ManualUpdate was called but CinemachineBrain is not in ManualUpdate mode");
            if (Time.inFixedTimeStep)
                Debug.LogError("CinemachineBrain.ManualUpdate was called from FixedUpdate");
#endif
            var prev = CinemachineCore.UniformDeltaTimeOverride;
            CinemachineCore.UniformDeltaTimeOverride = deltaTime;
            DoNonFixedUpdate(currentFrame);
            CinemachineCore.UniformDeltaTimeOverride = prev;
        }

        /// <summary>
        /// Updates CinemachineCameras and positions the main camera when UpdateMode is set to ManualUpdate.
        /// This method should only be called in ManualUpdate mode. For other modes, updates occur 
        /// automatically and this method should not be called explicitly.
        /// </summary>
        /// <remarks>
        /// Important usage notes:
        /// <list type="bullet">
        /// <item>Never call this method from FixedUpdate.</item>
        /// <item>This method must be called exactly once per render frame - more frequent calls 
        /// will not update the cameras, and less frequent calls may cause jerky camera movement.</item>
        /// <item>This version of the method will automatically track the update frame count and the current delta time.  
        /// If you want to explicity control deltaTime and update frame count, use the overload of this method that 
        /// allows you to specify those values.</item>
        /// </list>
        /// </remarks>
        public void ManualUpdate()
        {
#if UNITY_EDITOR
            if (UpdateMethod != UpdateMethods.ManualUpdate)
                Debug.LogError("CinemachineBrain.ManualUpdate was called but CinemachineBrain is not in ManualUpdate mode");
            if (Time.inFixedTimeStep)
                Debug.LogError("CinemachineBrain.ManualUpdate was called from FixedUpdate");
#endif
            DoNonFixedUpdate(Time.frameCount);
        }

        void DoNonFixedUpdate(int updateFrame)
        {
            m_LastFrameUpdated = CinemachineCore.CurrentUpdateFrame = updateFrame;

            float deltaTime = GetEffectiveDeltaTime(false);
            if (Application.isPlaying && (UpdateMethod == UpdateMethods.FixedUpdate || Time.inFixedTimeStep))
            {
                CameraUpdateManager.s_CurrentUpdateFilter = CameraUpdateManager.UpdateFilter.Fixed;

                // Special handling for fixed update: cameras that have been enabled
                // since the last physics frame must be updated now
                if (BlendUpdateMethod != BrainUpdateMethods.FixedUpdate && CinemachineCore.SoloCamera == null)
                    m_BlendManager.RefreshCurrentCameraState(DefaultWorldUp, GetEffectiveDeltaTime(true));
            }
            else
            {
                var filter = CameraUpdateManager.UpdateFilter.Late;
                if (UpdateMethod == UpdateMethods.SmartUpdate)
                {
                    // Track the targets
                    UpdateTracker.OnUpdate(UpdateTracker.UpdateClock.Late, this);
                    filter = CameraUpdateManager.UpdateFilter.SmartLate;
                }
                UpdateVirtualCameras(filter, deltaTime);
            }

            if (!Application.isPlaying || BlendUpdateMethod != BrainUpdateMethods.FixedUpdate)
                m_BlendManager.UpdateRootFrame(this, TopCameraFromPriorityQueue(), DefaultWorldUp, deltaTime);

            m_BlendManager.ComputeCurrentBlend();

            // Choose the active CinemachineCamera and apply it to the Unity camera
            if (!Application.isPlaying || BlendUpdateMethod != BrainUpdateMethods.FixedUpdate)
                ProcessActiveCamera(deltaTime);
        }

        /// Called in the place of FixedUpdate
        void DoFixedUpdate()
        {
            if (UpdateMethod == UpdateMethods.FixedUpdate
                || UpdateMethod == UpdateMethods.SmartUpdate)
            {
                var filter = CameraUpdateManager.UpdateFilter.Fixed;
                if (UpdateMethod == UpdateMethods.SmartUpdate)
                {
                    // Track the targets
                    UpdateTracker.OnUpdate(UpdateTracker.UpdateClock.Fixed, this);
                    filter = CameraUpdateManager.UpdateFilter.SmartFixed;
                }
                UpdateVirtualCameras(filter, GetEffectiveDeltaTime(true));
            }

            // Choose the active CinemachineCamera and apply it to the Unity camera
            if (BlendUpdateMethod == BrainUpdateMethods.FixedUpdate)
            {
                m_BlendManager.UpdateRootFrame(this, TopCameraFromPriorityQueue(), DefaultWorldUp, Time.fixedDeltaTime);
                ProcessActiveCamera(Time.fixedDeltaTime);
            }
        }

        float GetEffectiveDeltaTime(bool fixedDelta)
        {
            if (CinemachineCore.UniformDeltaTimeOverride >= 0)
                return CinemachineCore.UniformDeltaTimeOverride;

            if (CinemachineCore.SoloCamera != null)
                return Time.unscaledDeltaTime;

            if (!Application.isPlaying)
                return m_BlendManager.GetDeltaTimeOverride();

            if (IgnoreTimeScale)
                return fixedDelta ? Time.fixedDeltaTime : Time.unscaledDeltaTime;

            return fixedDelta ? Time.fixedDeltaTime : Time.deltaTime;
        }

        void UpdateVirtualCameras(CameraUpdateManager.UpdateFilter updateFilter, float deltaTime)
        {
            // We always update all active CinemachineCameras
            CameraUpdateManager.s_CurrentUpdateFilter = updateFilter;
            CameraUpdateManager.UpdateAllActiveVirtualCameras((uint)ChannelMask, DefaultWorldUp, deltaTime, this);

            // Make sure all live cameras get updated, in case some of them are deactivated
            if (CinemachineCore.SoloCamera != null)
                CinemachineCore.SoloCamera.UpdateCameraState(DefaultWorldUp, deltaTime);
            m_BlendManager.RefreshCurrentCameraState(DefaultWorldUp, deltaTime);

            // Restore the filter for general use
            updateFilter = CameraUpdateManager.UpdateFilter.Late;
            if (Application.isPlaying)
            {
                if (UpdateMethod == UpdateMethods.SmartUpdate)
                    updateFilter |= CameraUpdateManager.UpdateFilter.Smart;
                else if (UpdateMethod == UpdateMethods.FixedUpdate)
                    updateFilter = CameraUpdateManager.UpdateFilter.Fixed;
            }
            CameraUpdateManager.s_CurrentUpdateFilter = updateFilter;
        }

        /// <summary>
        /// Chooses the default active CinemachineCamera in the case there is no camera override.
        /// </summary>
        /// <returns>The highest-priority Enabled ICinemachineCamera that is in my Channel Mask.</returns>
        protected virtual ICinemachineCamera TopCameraFromPriorityQueue()
        {
            int numCameras = CameraUpdateManager.VirtualCameraCount;
            for (int i = 0; i < numCameras; ++i)
            {
                var cam = CameraUpdateManager.GetVirtualCamera(i);
                if (IsValidChannel(cam))
                    return cam;
            }
            return null;
        }

        CinemachineBlendDefinition LookupBlend(ICinemachineCamera fromKey, ICinemachineCamera toKey)
            => CinemachineBlenderSettings.LookupBlend(fromKey, toKey, DefaultBlend, CustomBlends, this);

        void ProcessActiveCamera(float deltaTime)
        {
            if (CinemachineCore.SoloCamera != null)
            {
                var state = CinemachineCore.SoloCamera.State;
                PushStateToUnityCamera(ref state);
            }
            else if (m_BlendManager.ProcessActiveCamera(this, DefaultWorldUp, deltaTime) != null)
            {
                // Apply the CinemachineCamera state to the Unity camera
                var state = m_BlendManager.CameraState;
                PushStateToUnityCamera(ref state);
            }
            else
            {
                // No active CinemachineCamera.  We create a state representing its position
                // and call the callback, but we don't actively set the transform or lens
                var state = CameraState.Default;
                var target = ControlledObject.transform;
                state.RawPosition = target.position;
                state.RawOrientation = target.rotation;
                state.Lens = LensSettings.FromCamera(m_OutputCamera);
                state.BlendHint |= CameraState.BlendHints.NoTransform | CameraState.BlendHints.NoLens;
                PushStateToUnityCamera(ref state);
            }
        }

        /// <summary> Applies a <see cref="CameraState"/> to the GameOject.</summary>
        void PushStateToUnityCamera(ref CameraState state)
        {
            m_CameraState = state;
            var target = ControlledObject.transform;

            var pos = target.position;
            var rot = target.rotation;
            if ((state.BlendHint & CameraState.BlendHints.NoPosition) == 0)
                pos = state.GetFinalPosition();
            if ((state.BlendHint & CameraState.BlendHints.NoOrientation) == 0)
                rot = state.GetFinalOrientation();
            target.ConservativeSetPositionAndRotation(pos, rot);

            if ((state.BlendHint & CameraState.BlendHints.NoLens) == 0)
            {
                Camera cam = OutputCamera;
                if (cam != null)
                {
                    bool isPhysical = cam.usePhysicalProperties;
#if CINEMACHINE_RESET_PROJECTION_MATRIX
                    cam.ResetProjectionMatrix();
#endif
                    cam.nearClipPlane = state.Lens.NearClipPlane;
                    cam.farClipPlane = state.Lens.FarClipPlane;
                    cam.orthographicSize = state.Lens.OrthographicSize;
                    cam.fieldOfView = state.Lens.FieldOfView;

#if CINEMACHINE_RESET_PROJECTION_MATRIX
                    if (!LensModeOverride.Enabled)
                        cam.usePhysicalProperties = isPhysical; // because ResetProjectionMatrix resets it
                    else
#else
                    if (LensModeOverride.Enabled)
#endif
                    {
                        if (state.Lens.ModeOverride != LensSettings.OverrideModes.None)
                        {
                            isPhysical = state.Lens.IsPhysicalCamera;
                            cam.orthographic = state.Lens.ModeOverride == LensSettings.OverrideModes.Orthographic;
                        }
                        else if (LensModeOverride.DefaultMode != LensSettings.OverrideModes.None)
                        {
                            isPhysical = LensModeOverride.DefaultMode == LensSettings.OverrideModes.Physical;
                            cam.orthographic = LensModeOverride.DefaultMode == LensSettings.OverrideModes.Orthographic;
                        }
                        cam.usePhysicalProperties = isPhysical;
                    }

                    if (isPhysical)
                    {
                        cam.sensorSize = state.Lens.PhysicalProperties.SensorSize;
                        cam.gateFit = state.Lens.PhysicalProperties.GateFit;
                        cam.focalLength = Camera.FieldOfViewToFocalLength(
                            state.Lens.FieldOfView, state.Lens.PhysicalProperties.SensorSize.y);
                        cam.lensShift = state.Lens.PhysicalProperties.LensShift;
                        cam.focusDistance = state.Lens.PhysicalProperties.FocusDistance;
                        cam.iso = state.Lens.PhysicalProperties.Iso;
                        cam.shutterSpeed = state.Lens.PhysicalProperties.ShutterSpeed;
                        cam.aperture = state.Lens.PhysicalProperties.Aperture;
                        cam.bladeCount = state.Lens.PhysicalProperties.BladeCount;
                        cam.curvature = state.Lens.PhysicalProperties.Curvature;
                        cam.barrelClipping = state.Lens.PhysicalProperties.BarrelClipping;
                        cam.anamorphism = state.Lens.PhysicalProperties.Anamorphism;
                    }
                }
            }
            // Send the camera updated event
            CinemachineCore.CameraUpdatedEvent.Invoke(this);
        }
    }
}
