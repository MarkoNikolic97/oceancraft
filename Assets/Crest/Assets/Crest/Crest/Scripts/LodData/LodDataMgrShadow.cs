// Crest Ocean System

// This file is subject to the MIT License as seen in the root of this folder structure (LICENSE)

using UnityEngine;
using UnityEngine.Rendering;

namespace Crest
{
    /// <summary>
    /// Stores shadowing data to use during ocean shading. Shadowing is persistent and supports sampling across
    /// many frames and jittered sampling for (very) soft shadows.
    /// </summary>
    public class LodDataMgrShadow : LodDataMgr
    {
        public override string SimName { get { return "Shadow"; } }
        public override RenderTextureFormat TextureFormat { get { return RenderTextureFormat.RG16; } }

        public static bool s_processData = true;

        Light _mainLight;
        Camera _cameraMain;

        CommandBuffer _bufCopyShadowMap = null;
        RenderTexture[] _sources;
        PropertyWrapperMaterial[] _renderMaterial;

        static int sp_CenterPos = Shader.PropertyToID("_CenterPos");
        static int sp_Scale = Shader.PropertyToID("_Scale");
        static int sp_CamPos = Shader.PropertyToID("_CamPos");
        static int sp_CamForward = Shader.PropertyToID("_CamForward");
        static int sp_JitterDiameters_CurrentFrameWeights = Shader.PropertyToID("_JitterDiameters_CurrentFrameWeights");
        static int sp_MainCameraProjectionMatrix = Shader.PropertyToID("_MainCameraProjectionMatrix");
        static int sp_SimDeltaTime = Shader.PropertyToID("_SimDeltaTime");

        SimSettingsShadow Settings { get { return OceanRenderer.Instance._simSettingsShadow; } }
        public override void UseSettings(SimSettingsBase settings) { OceanRenderer.Instance._simSettingsShadow = settings as SimSettingsShadow; }
        public override SimSettingsBase CreateDefaultSettings()
        {
            var settings = ScriptableObject.CreateInstance<SimSettingsShadow>();
            settings.name = SimName + " Auto-generated Settings";
            return settings;
        }

        protected override void Start()
        {
            base.Start();

            {
                _renderMaterial = new PropertyWrapperMaterial[OceanRenderer.Instance.CurrentLodCount];
                var shader = Shader.Find("Hidden/Crest/Simulation/Update Shadow");
                for (int i = 0; i < _renderMaterial.Length; i++)
                {
                    _renderMaterial[i] = new PropertyWrapperMaterial(shader);
                }
            }

            _cameraMain = Camera.main;
            if (_cameraMain == null)
            {
                var viewpoint = OceanRenderer.Instance.Viewpoint;
                _cameraMain = viewpoint != null ? viewpoint.GetComponent<Camera>() : null;

                if (_cameraMain == null)
                {
                    Debug.LogError("Could not find main camera, disabling shadow data", this);
                    enabled = false;
                    return;
                }
            }

#if UNITY_EDITOR
            if (!OceanRenderer.Instance.OceanMaterial.IsKeywordEnabled("_SHADOWS_ON"))
            {
                Debug.LogWarning("Shadowing is not enabled on the current ocean material and will not be visible.", this);
            }
#endif
        }

        protected override void InitData()
        {
            base.InitData();

            Debug.Assert(SystemInfo.SupportsRenderTextureFormat(TextureFormat), "The graphics device does not support the render texture format " + TextureFormat.ToString());

            int resolution = OceanRenderer.Instance.LodDataResolution;
            var desc = new RenderTextureDescriptor(resolution, resolution, TextureFormat, 0);

            _sources = new RenderTexture[OceanRenderer.Instance.CurrentLodCount];
            for (int i = 0; i < _sources.Length; i++)
            {
                _sources[i] = new RenderTexture(desc);
                _sources[i].wrapMode = TextureWrapMode.Clamp;
                _sources[i].antiAliasing = 1;
                _sources[i].filterMode = FilterMode.Bilinear;
                _sources[i].anisoLevel = 0;
                _sources[i].useMipMap = false;
                _sources[i].name = SimName + "_" + i + "_1";
            }
        }

        bool StartInitLight()
        {
            _mainLight = OceanRenderer.Instance._primaryLight;

            if (_mainLight.type != LightType.Directional)
            {
                Debug.LogError("Primary light must be of type Directional.", this);
                return false;
            }

            if (_mainLight.shadows == LightShadows.None)
            {
                Debug.LogError("Shadows must be enabled on primary light to enable ocean shadowing (types Hard and Soft are equivalent for the ocean system).", this);
                return false;
            }

            return true;
        }

        public override void UpdateLodData()
        {
            base.UpdateLodData();

            if (_mainLight != OceanRenderer.Instance._primaryLight)
            {
                if (_mainLight)
                {
                    _mainLight.RemoveCommandBuffer(LightEvent.BeforeScreenspaceMask, _bufCopyShadowMap);
                    _bufCopyShadowMap = null;
                    foreach (var source in _sources)
                    {
                        Graphics.Blit(Texture2D.blackTexture, source);
                    }
                    foreach (var target in _targets)
                    {
                        Graphics.Blit(Texture2D.blackTexture, target);
                    }
                }
                _mainLight = null;
            }

            if (!OceanRenderer.Instance._primaryLight)
            {
                if (!Settings._allowNullLight)
                {
                    Debug.LogWarning("Primary light must be specified on OceanRenderer script to enable shadows.", this);
                }
                return;
            }

            if (!_mainLight)
            {
                if (!StartInitLight())
                {
                    enabled = false;
                    return;
                }
            }

            if (_bufCopyShadowMap == null && s_processData)
            {
                _bufCopyShadowMap = new CommandBuffer();
                _bufCopyShadowMap.name = "Shadow data";
                _mainLight.AddCommandBuffer(LightEvent.BeforeScreenspaceMask, _bufCopyShadowMap);
            }
            else if (!s_processData && _bufCopyShadowMap != null)
            {
                _mainLight.RemoveCommandBuffer(LightEvent.BeforeScreenspaceMask, _bufCopyShadowMap);
                _bufCopyShadowMap = null;
            }

            if (!s_processData)
                return;


            var lodCount = OceanRenderer.Instance.CurrentLodCount;

            for (var lodIdx = lodCount - 1; lodIdx >= 0; lodIdx--)
            {
                SwapRTs(ref _sources[lodIdx], ref _targets[lodIdx]);
            }

            _bufCopyShadowMap.Clear();

            for (var lodIdx = OceanRenderer.Instance.CurrentLodCount - 1; lodIdx >= 0; lodIdx--)
            {
                // clear the shadow collection. it will be overwritten with shadow values IF the shadows render,
                // which only happens if there are (nontransparent) shadow receivers around
                Graphics.Blit(Texture2D.blackTexture, _targets[lodIdx]);

                var lt = OceanRenderer.Instance._lods[lodIdx];

                lt._renderData.Validate(0, this);
                _renderMaterial[lodIdx].SetVector(sp_CenterPos, lt._renderData._posSnapped);
                _renderMaterial[lodIdx].SetVector(sp_Scale, lt.transform.lossyScale);
                _renderMaterial[lodIdx].SetVector(sp_CamPos, OceanRenderer.Instance.Viewpoint.position);
                _renderMaterial[lodIdx].SetVector(sp_CamForward, OceanRenderer.Instance.Viewpoint.forward);
                _renderMaterial[lodIdx].SetVector(sp_JitterDiameters_CurrentFrameWeights, new Vector4(Settings._jitterDiameterSoft, Settings._jitterDiameterHard, Settings._currentFrameWeightSoft, Settings._currentFrameWeightHard));
                _renderMaterial[lodIdx].SetMatrix(sp_MainCameraProjectionMatrix, _cameraMain.projectionMatrix * _cameraMain.worldToCameraMatrix);
                _renderMaterial[lodIdx].SetFloat(sp_SimDeltaTime, Time.deltaTime);

                // compute which lod data we are sampling previous frame shadows from. if a scale change has happened this can be any lod up or down the chain.
                var srcDataIdx = lt.LodIndex + ScaleDifferencePow2;
                srcDataIdx = Mathf.Clamp(srcDataIdx, 0, lt.LodCount - 1);
                // bind data to slot 0 - previous frame data
                BindSourceData(srcDataIdx, 0, _renderMaterial[lodIdx], false);
                _bufCopyShadowMap.Blit(Texture2D.blackTexture, _targets[lodIdx], _renderMaterial[lodIdx].material);
            }
        }

        public void BindSourceData(int lodIdx, int slot, PropertyWrapperMaterial simMaterial, bool paramsOnly)
        {
            var rd = OceanRenderer.Instance._lods[lodIdx]._renderDataPrevFrame.Validate(BuildCommandBufferBase._lastUpdateFrame - Time.frameCount, this);
            BindData(lodIdx, slot, simMaterial, paramsOnly ? Texture2D.blackTexture : (_sources[lodIdx] as Texture), true, ref rd);
        }

        void OnEnable()
        {
            RemoveCommandBuffers();
        }

        void OnDisable()
        {
            RemoveCommandBuffers();
        }

        void RemoveCommandBuffers()
        {
            if (_bufCopyShadowMap != null)
            {
                if (_mainLight)
                {
                    _mainLight.RemoveCommandBuffer(LightEvent.BeforeScreenspaceMask, _bufCopyShadowMap);
                }
                _bufCopyShadowMap = null;
            }
        }

        static int[] _paramsSampler;
        public static int ParamIdSampler(int slot)
        {
            if (_paramsSampler == null)
                LodTransform.CreateParamIDs(ref _paramsSampler, "_LD_Sampler_Shadow_");
            return _paramsSampler[slot];
        }
        protected override int GetParamIdSampler(int slot)
        {
            return ParamIdSampler(slot);
        }
        public static void BindNull(int shapeSlot, IPropertyWrapper properties)
        {
            properties.SetTexture(ParamIdSampler(shapeSlot), Texture2D.blackTexture);
        }
    }
}
