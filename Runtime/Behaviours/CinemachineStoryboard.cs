#if CINEMACHINE_UGUI
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace Unity.Cinemachine
{
    /// <summary>
    /// An add-on module for CinemachineCamera that places an image in screen space
    /// over the camera's output.
    /// </summary>
    [SaveDuringPlay]
    [AddComponentMenu("Cinemachine/Procedural/Extensions/Cinemachine Storyboard")]
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [HelpURL(Documentation.BaseURL + "manual/CinemachineStoryboard.html")]
    public class CinemachineStoryboard : CinemachineExtension
    {
        /// <summary>
        /// 如果勾选，所有故事板将被全局静音
        /// </summary>
        [Tooltip("如果勾选，所有故事板将被全局静音")]
        public static bool s_StoryboardGlobalMute;

        /// <summary>
        /// 如果勾选，指定图像将作为覆盖层显示在虚拟相机输出之上
        /// </summary>
        [Tooltip("如果勾选，指定图像将作为覆盖层显示在虚拟相机输出之上")]
        [FormerlySerializedAs("m_ShowImage")]
        public bool ShowImage = true;

        /// <summary>
        /// 要显示的图像
        /// </summary>
        [Tooltip("要显示的图像")]
        [FormerlySerializedAs("m_Image")]
        public Texture Image;

        /// <summary>当图像宽高比与屏幕不匹配时，如何适配图像</summary>
        public enum FillStrategy
        {
            /// <summary>图像将尽可能大但不被裁剪</summary>
            BestFit,
            /// <summary>如有必要将裁剪图像以完全填充屏幕</summary>
            CropImageToFit,
            /// <summary>图像将被拉伸以覆盖屏幕的任何宽高比不匹配</summary>
            StretchToFit
        };

        /// <summary>
        /// 如何处理图像宽高比与屏幕宽高比的差异
        /// </summary>
        [Tooltip("如何处理图像宽高比与屏幕宽高比的差异")]
        [FormerlySerializedAs("m_Aspect")]
        public FillStrategy Aspect = FillStrategy.BestFit;

        /// <summary>
        /// 图像的不透明度。0为完全透明，1为完全不透明
        /// </summary>
        [Tooltip("图像的不透明度。0为完全透明，1为完全不透明")]
        [FormerlySerializedAs("m_Alpha")]
        [Range(0, 1)]
        public float Alpha = 1;

        /// <summary>
        /// 显示图像的屏幕空间位置。零点为中心
        /// </summary>
        [Tooltip("显示图像的屏幕空间位置。零点为中心")]
        [FormerlySerializedAs("m_Center")]
        public Vector2 Center = Vector2.zero;

        /// <summary>
        /// 应用于图像的屏幕空间旋转
        /// </summary>
        [Tooltip("应用于图像的屏幕空间旋转")]
        [FormerlySerializedAs("m_Rotation")]
        public Vector3 Rotation = Vector3.zero;

        /// <summary>
        /// 应用于图像的屏幕空间缩放
        /// </summary>
        [Tooltip("应用于图像的屏幕空间缩放")]
        [FormerlySerializedAs("m_Scale")]
        public Vector2 Scale = Vector3.one;

        /// <summary>
        /// 如果勾选，X和Y轴缩放将同步
        /// </summary>
        [Tooltip("如果勾选，X和Y轴缩放将同步")]
        [FormerlySerializedAs("m_SyncScale")]
        public bool SyncScale = true;

        /// <summary>
        /// 如果勾选，相机变换将不受此虚拟相机控制
        /// </summary>
        [Tooltip("如果勾选，相机变换将不受此虚拟相机控制")]
        [FormerlySerializedAs("m_MuteCamera")]
        public bool MuteCamera;

        /// <summary>
        /// 水平方向擦除图像的显示和隐藏
        /// </summary>
        [Range(-1, 1)]
        [Tooltip("水平方向擦除图像的显示和隐藏")]
        [FormerlySerializedAs("m_SplitView")]
        public float SplitView = 0f;

        /// <summary>
        /// 绘制故事板的画布渲染模式
        /// </summary>
        [Tooltip("绘制故事板的画布渲染模式")]
        [FormerlySerializedAs("m_RenderMode")]
        public StoryboardRenderMode RenderMode = StoryboardRenderMode.ScreenSpaceOverlay;

        /// <summary>
        /// 允许对画布进行排序，以在其他画布之上或之下渲染
        /// </summary>
        [Tooltip("允许对画布进行排序，以在其他画布之上或之下渲染")]
        [FormerlySerializedAs("m_SortingOrder")]
        public int SortingOrder;

        /// <summary>
        /// 故事板画布距离相机多远生成
        /// </summary>
        [Tooltip("故事板画布距离相机多远生成")]
        [FormerlySerializedAs("m_PlaneDistance")]
        public float PlaneDistance = 100;


        class CanvasInfo
        {
            public GameObject Canvas;
            public Canvas CanvasComponent;
            public CinemachineBrain CanvasParent;
            public RectTransform Viewport; // for mViewport clipping
            public UnityEngine.UI.RawImage RawImage;
        }
        List<CanvasInfo> m_CanvasInfo = new List<CanvasInfo>();

        /// <summary>Callback to display the image</summary>
        /// <param name="vcam">The virtual camera being processed</param>
        /// <param name="stage">The current pipeline stage</param>
        /// <param name="state">The current virtual camera state</param>
        /// <param name="deltaTime">The current applicable deltaTime</param>
        protected override void PostPipelineStageCallback(
            CinemachineVirtualCameraBase vcam,
            CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {
            // Apply to this vcam only, not the children
            if (vcam != ComponentOwner || stage != CinemachineCore.Stage.Finalize)
                return;

            UpdateRenderCanvas();

            if (ShowImage)
                state.AddCustomBlendable(new CameraState.CustomBlendableItems.Item { Custom = this, Weight = 1});
            if (MuteCamera)
                state.BlendHint |= CameraState.BlendHints.NoTransform | CameraState.BlendHints.NoLens;
        }

        /// <summary>
        /// Camera render modes supported by CinemachineStoryboard.
        /// </summary>
        public enum StoryboardRenderMode
        {
            /// <summary>
            /// Renders in camera screen space. This means, that the storyboard is going to be displayed in front of
            /// any objects in the scene. Equivalent to Unity's RenderMode.ScreenSpaceOverlay.
            /// </summary>
            ScreenSpaceOverlay = UnityEngine.RenderMode.ScreenSpaceOverlay,
            /// <summary>
            /// Render using the vcam on which the storyboard is on. This is useful, if you'd like to render the
            /// storyboard at a specific distance from the vcam. Equivalent to Unity's RenderMode.ScreenSpaceCamera.
            /// </summary>
            ScreenSpaceCamera = UnityEngine.RenderMode.ScreenSpaceCamera
        };

        void UpdateRenderCanvas()
        {
            for (int i = 0; i < m_CanvasInfo.Count; ++i)
            {
                if (m_CanvasInfo[i] == null || m_CanvasInfo[i].CanvasComponent == null)
                    m_CanvasInfo.RemoveAt(i--);
                else
                {
                    m_CanvasInfo[i].CanvasComponent.renderMode = (RenderMode)RenderMode;
                    m_CanvasInfo[i].CanvasComponent.planeDistance = PlaneDistance;
                    m_CanvasInfo[i].CanvasComponent.sortingOrder = SortingOrder;
                }
            }
        }


        /// <summary>Connect to virtual camera.  Adds/removes listener</summary>
        /// <param name="connect">True if connecting, false if disconnecting</param>
        protected override void ConnectToVcam(bool connect)
        {
            base.ConnectToVcam(connect);
            CinemachineCore.CameraUpdatedEvent.RemoveListener(CameraUpdatedCallback);
            if (connect)
                CinemachineCore.CameraUpdatedEvent.AddListener(CameraUpdatedCallback);
            else
                DestroyCanvas();
        }

        string CanvasName => "_CM_canvas" + gameObject.GetInstanceID();

        void CameraUpdatedCallback(CinemachineBrain brain)
        {
            var owner = ComponentOwner;
            if (owner == null)
                return;
            var showIt = enabled && ShowImage && CinemachineCore.IsLive(owner);
            var channel = (uint)owner.OutputChannel;
            if (s_StoryboardGlobalMute || ((uint)brain.ChannelMask & channel) == 0)
                showIt = false;
            var ci = LocateMyCanvas(brain, showIt);
            if (ci != null && ci.Canvas != null)
                ci.Canvas.SetActive(showIt);
        }

        CanvasInfo LocateMyCanvas(CinemachineBrain parent, bool createIfNotFound)
        {
            CanvasInfo ci = null;
            for (int i = 0; ci == null && i < m_CanvasInfo.Count; ++i)
                if (m_CanvasInfo[i] != null && m_CanvasInfo[i].CanvasParent == parent)
                    ci = m_CanvasInfo[i];
            if (createIfNotFound)
            {
                if (ci == null)
                {
                    ci = new CanvasInfo() { CanvasParent = parent };
                    int numChildren = parent.transform.childCount;
                    for (int i = 0; ci.Canvas == null && i < numChildren; ++i)
                    {
                        var child = parent.transform.GetChild(i) as RectTransform;
                        if (child != null && child.name == CanvasName)
                        {
                            ci.Canvas = child.gameObject;
                            var kids = ci.Canvas.GetComponentsInChildren<RectTransform>();
                            ci.Viewport = kids.Length > 1 ? kids[1] : null; // 0 is mCanvas
                            ci.RawImage = ci.Canvas.GetComponentInChildren<UnityEngine.UI.RawImage>();
                            ci.CanvasComponent = ci.Canvas.GetComponent<Canvas>();
                        }
                    }
                    m_CanvasInfo.Add(ci);
                }
                if (ci.Canvas == null || ci.Viewport == null || ci.RawImage == null || ci.CanvasComponent == null)
                    CreateCanvas(ci);
            }
            return ci;
        }

        void CreateCanvas(CanvasInfo ci)
        {
            ci.Canvas = new GameObject(CanvasName, typeof(RectTransform));
            ci.Canvas.layer = gameObject.layer;
            ci.Canvas.hideFlags = HideFlags.HideAndDontSave;
            ci.Canvas.transform.SetParent(ci.CanvasParent.transform);
#if UNITY_EDITOR
            // Workaround for Unity bug case Case 1004117
            CanvasesAndTheirOwners.AddCanvas(ci.Canvas, this);
#endif

            var c = ci.CanvasComponent = ci.Canvas.AddComponent<Canvas>();
            c.renderMode = (RenderMode)RenderMode;
            c.sortingOrder = SortingOrder;
            c.planeDistance = PlaneDistance;
            c.worldCamera = ci.CanvasParent.OutputCamera;

            var go = new GameObject("Viewport", typeof(RectTransform));
            go.transform.SetParent(ci.Canvas.transform);
            ci.Viewport = (RectTransform)go.transform;
            go.AddComponent<UnityEngine.UI.RectMask2D>();

            go = new GameObject("RawImage", typeof(RectTransform));
            go.transform.SetParent(ci.Viewport.transform);
            ci.RawImage = go.AddComponent<UnityEngine.UI.RawImage>();
        }

        void DestroyCanvas()
        {
            int numBrains = CinemachineBrain.ActiveBrainCount;
            for (int i = 0; i < numBrains; ++i)
            {
                var parent = CinemachineBrain.GetActiveBrain(i);
                int numChildren = parent.transform.childCount;
                for (int j = numChildren - 1; j >= 0; --j)
                {
                    var child = parent.transform.GetChild(j) as RectTransform;
                    if (child != null && child.name == CanvasName)
                    {
                        var canvas = child.gameObject;
                        RuntimeUtility.DestroyObject(canvas);
#if UNITY_EDITOR
                        // Workaround for Unity bug case Case 1004117
                        CanvasesAndTheirOwners.RemoveCanvas(canvas);
#endif
                    }
                }
            }
            m_CanvasInfo.Clear();
        }

        void PlaceImage(CanvasInfo ci, float alpha)
        {
            if (ci.RawImage != null && ci.Viewport != null)
            {
                var screen = new Rect(0, 0, Screen.width, Screen.height);
                if (ci.CanvasParent.OutputCamera != null)
                    screen = ci.CanvasParent.OutputCamera.pixelRect;
                screen.x -= (float)Screen.width/2;
                screen.y -= (float)Screen.height/2;

                // Apply Split View
                var wipeAmount = -Mathf.Clamp(SplitView, -1, 1) * screen.width;

                var pos = screen.center;
                pos.x -= wipeAmount/2;
                ci.Viewport.localPosition = pos;
                ci.Viewport.localRotation = Quaternion.identity;
                ci.Viewport.localScale = Vector3.one;
                ci.Viewport.ForceUpdateRectTransforms();
                ci.Viewport.sizeDelta = new Vector2(screen.width + 1 - Mathf.Abs(wipeAmount), screen.height + 1);

                var scale = Vector2.one;
                if (Image != null
                    && Image.width > 0 && Image.width > 0
                    && screen.width > 0 && screen.height > 0)
                {
                    float f = (screen.height * Image.width) / (screen.width * Image.height);
                    switch (Aspect)
                    {
                        case FillStrategy.BestFit:
                            if (f >= 1)
                                scale.y /= f;
                            else
                                scale.x *= f;
                            break;
                        case FillStrategy.CropImageToFit:
                            if (f >= 1)
                                scale.x *= f;
                            else
                                scale.y /= f;
                            break;
                        case FillStrategy.StretchToFit:
                            break;
                    }
                }
                scale.x *= Scale.x;
                scale.y *= SyncScale ? Scale.x : Scale.y;

                ci.RawImage.texture = Image;
                var tintColor = Color.white;
                tintColor.a = Alpha * alpha;
                ci.RawImage.color = tintColor;

                pos = new Vector2(screen.width * Center.x, screen.height * Center.y);
                pos.x += wipeAmount/2;
                ci.RawImage.rectTransform.localPosition = pos;
                ci.RawImage.rectTransform.localRotation = Quaternion.Euler(Rotation);
                ci.RawImage.rectTransform.localScale = scale;
                ci.RawImage.rectTransform.ForceUpdateRectTransforms();
                ci.RawImage.rectTransform.sizeDelta = screen.size;
            }
        }

        static void StaticBlendingHandler(CinemachineBrain brain)
        {
            var state = brain.State;
            int numBlendables = state.GetNumCustomBlendables();
            for (int i = 0; i < numBlendables; ++i)
            {
                var b = state.GetCustomBlendable(i);
                var src = b.Custom as CinemachineStoryboard;
                if (src != null && src.ComponentOwner != null) // in case it was deleted
                {
                    bool showIt = true;
                    var channel = (uint)src.ComponentOwner.OutputChannel;
                    if (s_StoryboardGlobalMute || ((uint)brain.ChannelMask & channel) == 0)
                        showIt = false;
                    var ci = src.LocateMyCanvas(brain, showIt);
                    if (ci != null)
                        src.PlaceImage(ci, b.Weight);
                }
            }
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoad]
        class EditorInitialize { static EditorInitialize() { InitializeModule(); } }
#endif
        [RuntimeInitializeOnLoadMethod]
        static void InitializeModule()
        {
            CinemachineCore.CameraUpdatedEvent.RemoveListener(StaticBlendingHandler);
            CinemachineCore.CameraUpdatedEvent.AddListener(StaticBlendingHandler);
        }


#if UNITY_EDITOR
        // Workaround for the Unity bug where OnDestroy doesn't get called if Undo
        // bug case Case 1004117
        [UnityEditor.InitializeOnLoad]
        class CanvasesAndTheirOwners
        {
            static Dictionary<UnityEngine.Object, UnityEngine.Object> s_CanvasesAndTheirOwners;
            static CanvasesAndTheirOwners()
            {
                UnityEditor.Undo.undoRedoPerformed -= OnUndoRedoPerformed;
                UnityEditor.Undo.undoRedoPerformed += OnUndoRedoPerformed;
            }
            static void OnUndoRedoPerformed()
            {
                if (s_CanvasesAndTheirOwners != null)
                {
                    List<UnityEngine.Object> toDestroy = null;
                    var iter = s_CanvasesAndTheirOwners.GetEnumerator();
                    while (iter.MoveNext())
                    {
                        var v = iter.Current;
                        if (v.Value == null)
                        {
                            toDestroy ??= new List<UnityEngine.Object>();
                            toDestroy.Add(v.Key);
                        }
                    }
                    for (int i = 0; toDestroy != null && i < toDestroy.Count; ++i)
                    {
                        var o = toDestroy[i];
                        RemoveCanvas(o);
                        RuntimeUtility.DestroyObject(o);
                    }
                }
            }
            public static void RemoveCanvas(UnityEngine.Object canvas)
            {
                if (s_CanvasesAndTheirOwners != null && s_CanvasesAndTheirOwners.ContainsKey(canvas))
                    s_CanvasesAndTheirOwners.Remove(canvas);
            }
            public static void AddCanvas(UnityEngine.Object canvas, UnityEngine.Object owner)
            {
                s_CanvasesAndTheirOwners ??= new ();
                s_CanvasesAndTheirOwners.Add(canvas, owner);
            }
        }
#endif
    }
}
#endif
