using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EXShaders
{
    public class EXToonGUI : ShaderGUI
    {
        private enum RenderingMode
        {
            Opaque,
            Cutout,
            Transparent
        }

        private static readonly string[] blendNames = Enum.GetNames(typeof(RenderingMode));
        private static readonly string[] blendModeNames = Enum.GetNames(typeof(UnityEngine.Rendering.BlendMode));
        private static readonly string[] blendOpNames = Enum.GetNames(typeof(UnityEngine.Rendering.BlendOp));
        private static readonly string[] compareFunctionNames = Enum.GetNames(typeof(UnityEngine.Rendering.CompareFunction));
        private static readonly string[] stencilOpNames = Enum.GetNames(typeof(UnityEngine.Rendering.StencilOp));
        private static readonly string[] cullNames = {"Off","Front","Back"};
        private static readonly string[] colorMaskNames = {"None","A","B","BA","G","GA","GB","GBA","R","RA","RB","RBA","RG","RGA","RGB","RGBA"};
        private static readonly string[] layerNames = {"None","1 Layer","2 Layers","3 Layers"};
        private static readonly string[] layerPresetNames = {"Cancel","Emission","Detail","Decal","MatCap","AngelRing","Rim Light","Specular","Distance Fade"};
        private static GUIStyle foldout = new GUIStyle();
        private static MaterialEditor m_MaterialEditor;
        private static bool isShowBase = false;
        private static bool isShowOutline = false;
        private static bool isShowShadow = false;
        private static bool isShowLayers = false;
        private static bool isShowRim = false;
        private static bool isShowSpecular = false;
        private static bool isShowMatCap = false;
        private static bool isShowLighting = false;
        private static bool isShowStencil = false;
        private static bool isShowRendering = false;

        private static bool isShow_MainTex;
        private static bool isShow_OutlineTex;
        private static bool isShow_OutlineMaskTex;
        private static bool isShow_NormalMap;
        private static bool isShow_ShadowPositionTex;
        private static bool isShow_Shadow1stColorTex;
        private static bool isShow_Shadow2ndColorTex;
        private static bool isShow_Shadow3rdColorTex;
        private static bool isShow_LayerMaskTex;
        private static bool isShow_Layer1stColorTex;
        private static bool isShow_Layer2ndColorTex;
        private static bool isShow_Layer3rdColorTex;

        private MaterialProperty _MainTex;
        private MaterialProperty _OutlineTex;
        private MaterialProperty _OutlineMaskTex;
        private MaterialProperty _NormalMap;
        private MaterialProperty _ShadowPositionTex;
        private MaterialProperty _Shadow1stColorTex;
        private MaterialProperty _Shadow2ndColorTex;
        private MaterialProperty _Shadow3rdColorTex;
        private MaterialProperty _LayerMaskTex;
        private MaterialProperty _Layer1stColorTex;
        private MaterialProperty _Layer2ndColorTex;
        private MaterialProperty _Layer3rdColorTex;

        private MaterialProperty _Color;
        private MaterialProperty _OutlineColor;
        private MaterialProperty _Shadow1stColor;
        private MaterialProperty _Shadow2ndColor;
        private MaterialProperty _Shadow3rdColor;
        private MaterialProperty _Rim1stColor;
        private MaterialProperty _Rim2ndColor;
        private MaterialProperty _Layer1stColor;
        private MaterialProperty _Layer2ndColor;
        private MaterialProperty _Layer3rdColor;

        private MaterialProperty _UVScrollMaNo;
        private MaterialProperty _UVScrollSPS1;
        private MaterialProperty _UVScrollS2S3;
        private MaterialProperty _UVScrollOCOM;
        private MaterialProperty _UVScrollLML1;
        private MaterialProperty _UVScrollL2L3;

        private MaterialProperty _LightParams;
        private MaterialProperty _Shadow1stParams;
        private MaterialProperty _Shadow2ndParams;
        private MaterialProperty _Shadow3rdParams;
        private MaterialProperty _Shadow1stParams2;
        private MaterialProperty _Shadow2ndParams2;
        private MaterialProperty _Shadow3rdParams2;
        private MaterialProperty _RimParams;
        private MaterialProperty _Rim1stParams;
        private MaterialProperty _Rim2ndParams;
        private MaterialProperty _SpecularParams;
        private MaterialProperty _Layer1stParams;
        private MaterialProperty _Layer2ndParams;
        private MaterialProperty _Layer3rdParams;
        private MaterialProperty _Layer1stBlink;
        private MaterialProperty _Layer2ndBlink;
        private MaterialProperty _Layer3rdBlink;
        private MaterialProperty _Layer1stUV01Blend;
        private MaterialProperty _Layer2ndUV01Blend;
        private MaterialProperty _Layer3rdUV01Blend;
        private MaterialProperty _Layer1stUVMSBlend;
        private MaterialProperty _Layer2ndUVMSBlend;
        private MaterialProperty _Layer3rdUVMSBlend;
        private MaterialProperty _Layer1stFadeParams;
        private MaterialProperty _Layer2ndFadeParams;
        private MaterialProperty _Layer3rdFadeParams;

        private MaterialProperty _Mode;
        private MaterialProperty _Layers;
        private MaterialProperty _Cutoff;
        private MaterialProperty _MatCapNormal;
        private MaterialProperty _NormalScale;
        private MaterialProperty _OutlineWidth;

        private MaterialProperty _OutlineVCControll;
        private MaterialProperty _OutlineZeroDelete;
        private MaterialProperty _UseShadow;
        private MaterialProperty _UsePositionTex;
        private MaterialProperty _UseShadowColorTex;
        private MaterialProperty _MatCapStabilize;
        private MaterialProperty _MatCapPerspective;
        private MaterialProperty _Layer1stBlendMode;
        private MaterialProperty _Layer2ndBlendMode;
        private MaterialProperty _Layer3rdBlendMode;
        private MaterialProperty _Layer1stRim;
        private MaterialProperty _Layer2ndRim;
        private MaterialProperty _Layer3rdRim;
        private MaterialProperty _Layer1stSpecular;
        private MaterialProperty _Layer2ndSpecular;
        private MaterialProperty _Layer3rdSpecular;

        private MaterialProperty _EXCull;
        private MaterialProperty _EXSrcBlend;
        private MaterialProperty _EXDstBlend;
        private MaterialProperty _EXSrcBlendAlpha;
        private MaterialProperty _EXDstBlendAlpha;
        private MaterialProperty _EXBlendOp;
        private MaterialProperty _EXBlendOpAlpha;
        private MaterialProperty _EXSrcBlendFA;
        private MaterialProperty _EXDstBlendFA;
        private MaterialProperty _EXBlendOpFA;
        private MaterialProperty _EXBlendOpAlphaFA;
        private MaterialProperty _EXZClip;
        private MaterialProperty _EXZWrite;
        private MaterialProperty _EXZTest;
        private MaterialProperty _EXStencilRef;
        private MaterialProperty _EXStencilReadMask;
        private MaterialProperty _EXStencilWriteMask;
        private MaterialProperty _EXStencilComp;
        private MaterialProperty _EXStencilPass;
        private MaterialProperty _EXStencilFail;
        private MaterialProperty _EXStencilZFail;
        private MaterialProperty _EXOffsetFactor;
        private MaterialProperty _EXOffsetUnits;
        private MaterialProperty _EXColorMask;
        private MaterialProperty _EXAlphaToMask;

        private MaterialProperty _EXOLCull;
        private MaterialProperty _EXOLSrcBlend;
        private MaterialProperty _EXOLDstBlend;
        private MaterialProperty _EXOLSrcBlendAlpha;
        private MaterialProperty _EXOLDstBlendAlpha;
        private MaterialProperty _EXOLBlendOp;
        private MaterialProperty _EXOLBlendOpAlpha;
        private MaterialProperty _EXOLSrcBlendFA;
        private MaterialProperty _EXOLDstBlendFA;
        private MaterialProperty _EXOLBlendOpFA;
        private MaterialProperty _EXOLBlendOpAlphaFA;
        private MaterialProperty _EXOLZClip;
        private MaterialProperty _EXOLZWrite;
        private MaterialProperty _EXOLZTest;
        private MaterialProperty _EXOLStencilRef;
        private MaterialProperty _EXOLStencilReadMask;
        private MaterialProperty _EXOLStencilWriteMask;
        private MaterialProperty _EXOLStencilComp;
        private MaterialProperty _EXOLStencilPass;
        private MaterialProperty _EXOLStencilFail;
        private MaterialProperty _EXOLStencilZFail;
        private MaterialProperty _EXOLOffsetFactor;
        private MaterialProperty _EXOLOffsetUnits;
        private MaterialProperty _EXOLColorMask;
        private MaterialProperty _EXOLAlphaToMask;


        //------------------------------------------------------------------------------------------------------------------------------
        // GUI
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
	    {
            foldout = new GUIStyle("ShurikenModuleTitle")
            {
                fontSize = EditorStyles.label.fontSize,
                border = new RectOffset(15, 7, 4, 4),
                contentOffset = new Vector2(20f, -2f),
                fixedHeight = 22
            };

            FindProperties(props);
            m_MaterialEditor = materialEditor;
            var materials = m_MaterialEditor.targets.Select(target => target as Material).ToArray();

            EditorGUI.BeginChangeCheck();
            DrawRenderingMode();

            isShowBase = Foldout("Base", isShowBase);
            if(isShowBase)
            {
                DrawTextureAndUV(ref isShow_MainTex, new GUIContent("Color / Alpha", "Albedo (RGB), Alpha (A)"), _MainTex, _Color, _UVScrollMaNo, true);
                DrawTextureAndUV(ref isShow_NormalMap, new GUIContent("Normal Map", "Normal (RGB)"), _NormalMap, _NormalScale, _UVScrollMaNo, false);
                if(_Mode.floatValue != 0) m_MaterialEditor.ShaderProperty(_Cutoff, "Cutoff");
                DrawEnum(_EXCull, "Cull Mode", cullNames);
                DrawToggle(_EXZWrite, "ZWrite");
                m_MaterialEditor.RenderQueueField();
            }

            isShowOutline = Foldout("Outline", isShowOutline);
            if(isShowOutline)
            {
                DrawTextureAndUV(ref isShow_OutlineTex, new GUIContent("Color", "Color (RGB), Alpha (A)"), _OutlineTex, _OutlineColor, _UVScrollOCOM, true);
                DrawTextureAndUV(ref isShow_OutlineMaskTex, new GUIContent("Width / Mask", "Width (R)"), _OutlineMaskTex, _OutlineWidth, _UVScrollOCOM, false);
                DrawToggle(_OutlineVCControll, "Use Vertex Color");
                DrawToggle(_OutlineZeroDelete, "Delete Width 0 Vertex");
            }

            isShowShadow = Foldout("Shadow", isShowShadow);
            if(isShowShadow)
            {
                DrawToggle(_UseShadow, "Use Shadow");

                GUI.enabled = _UseShadow.floatValue > 0;
                {
                    DrawToggle(_UseShadowColorTex, "Use Custom Tex");
                    bool useSCT = _UseShadowColorTex.floatValue != 0.0f;
                    EditorGUILayout.LabelField("1st Shadow", EditorStyles.boldLabel);
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    if(useSCT)  DrawTextureAndUV(ref isShow_Shadow1stColorTex, new GUIContent("Color", "Color (RGB), Alpha (A)"), _Shadow1stColorTex, _Shadow1stColor, _UVScrollSPS1, false);
                    else        m_MaterialEditor.ShaderProperty(_Shadow1stColor, "Color");
                    DrawRemap(_Shadow1stParams);
                    DrawShadowParams2(_Shadow1stParams2);
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.LabelField("2nd Shadow", EditorStyles.boldLabel);
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    if(useSCT)  DrawTextureAndUV(ref isShow_Shadow2ndColorTex, new GUIContent("Color", "Color (RGB), Alpha (A)"), _Shadow2ndColorTex, _Shadow2ndColor, _UVScrollS2S3, true);
                    else        m_MaterialEditor.ShaderProperty(_Shadow2ndColor, "Color");
                    DrawRemap(_Shadow2ndParams);
                    DrawShadowParams2(_Shadow2ndParams2);
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.LabelField("3rd Shadow", EditorStyles.boldLabel);
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    if(useSCT)  DrawTextureAndUV(ref isShow_Shadow3rdColorTex, new GUIContent("Color", "Color (RGB), Alpha (A)"), _Shadow3rdColorTex, _Shadow3rdColor, _UVScrollS2S3, false);
                    else        m_MaterialEditor.ShaderProperty(_Shadow3rdColor, "Color");
                    DrawRemap(_Shadow3rdParams);
                    DrawShadowParams2(_Shadow3rdParams2);
                    EditorGUILayout.EndVertical();
                }
                GUI.enabled = true;

                DrawToggle(_UsePositionTex, "Use Position Map");
                GUI.enabled = _UsePositionTex.floatValue > 0;
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                DrawTextureAndUV(ref isShow_ShadowPositionTex, new GUIContent("Position Map", "1st (R), 2nd (G), 3rd (B)"), _ShadowPositionTex, _UVScrollSPS1, true);
                DrawRemapZW(_Shadow1stParams, "1st");
                DrawRemapZW(_Shadow2ndParams, "2nd");
                DrawRemapZW(_Shadow3rdParams, "3rd");
                EditorGUILayout.EndVertical();
                GUI.enabled = true;
            }

            isShowLayers = Foldout("Layers", isShowLayers);
            if(isShowLayers)
            {
                DrawLayers();
                GUI.enabled = _Layers.floatValue > 0;
                DrawTextureAndUV(ref isShow_LayerMaskTex, new GUIContent("Layer Mask", "1st (R), 2nd (G), 3rd (B)"), _LayerMaskTex, _UVScrollLML1, true);

                EditorGUILayout.LabelField("1st Layer", EditorStyles.boldLabel);
                DrawLayer(
                    ref isShow_Layer1stColorTex,
                    _Layer1stColorTex,
                    _Layer1stColor,
                    _UVScrollLML1,
                    false,
                    _Layer1stUV01Blend,
                    _Layer1stUVMSBlend,
                    _Layer1stBlink,
                    _Layer1stParams,
                    _Layer1stFadeParams,
                    _Layer1stRim,
                    _Layer1stSpecular,
                    _Layer1stBlendMode
                );

                GUI.enabled = _Layers.floatValue > 1;
                EditorGUILayout.LabelField("2nd Layer", EditorStyles.boldLabel);
                DrawLayer(
                    ref isShow_Layer2ndColorTex,
                    _Layer2ndColorTex,
                    _Layer2ndColor,
                    _UVScrollL2L3,
                    true,
                    _Layer2ndUV01Blend,
                    _Layer2ndUVMSBlend,
                    _Layer2ndBlink,
                    _Layer2ndParams,
                    _Layer2ndFadeParams,
                    _Layer2ndRim,
                    _Layer2ndSpecular,
                    _Layer2ndBlendMode
                );

                GUI.enabled = _Layers.floatValue > 2;
                EditorGUILayout.LabelField("3rd Layer", EditorStyles.boldLabel);
                DrawLayer(
                    ref isShow_Layer3rdColorTex,
                    _Layer3rdColorTex,
                    _Layer3rdColor,
                    _UVScrollL2L3,
                    false,
                    _Layer3rdUV01Blend,
                    _Layer3rdUVMSBlend,
                    _Layer3rdBlink,
                    _Layer3rdParams,
                    _Layer3rdFadeParams,
                    _Layer3rdRim,
                    _Layer3rdSpecular,
                    _Layer3rdBlendMode
                );
                GUI.enabled = true;
            }

            GUI.enabled = _Layers.floatValue > 0;
            isShowRim = Foldout("Rim Light (for Layers)", isShowRim) && GUI.enabled;
            if(isShowRim)
            {
                EditorGUILayout.LabelField("1st Rim", EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                m_MaterialEditor.ShaderProperty(_Rim1stColor, "Color");
                DrawRim1stFresnel(_RimParams);
                DrawRemap(_Rim1stParams);
                DrawRimDirectionParams(_Rim1stParams);
                EditorGUILayout.EndVertical();

                EditorGUILayout.LabelField("2nd Rim", EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                m_MaterialEditor.ShaderProperty(_Rim2ndColor, "Color");
                DrawRim2ndFresnel(_RimParams);
                DrawRemap(_Rim2ndParams);
                DrawRimDirectionParams(_Rim2ndParams);
                EditorGUILayout.EndVertical();
                DrawRimParams(_RimParams);

            }

            isShowSpecular = Foldout("Specular (for Layers)", isShowSpecular) && GUI.enabled;
            if(isShowSpecular)
            {
                DrawRemap(_SpecularParams);
                DrawSpecularNS(_SpecularParams);
            }

            isShowMatCap = Foldout("MatCap UV (for Layers)", isShowMatCap) && GUI.enabled;
            if(isShowMatCap)
            {
                m_MaterialEditor.ShaderProperty(_MatCapNormal, "Normal Strength");
                DrawToggle(_MatCapStabilize, "Stabilize");
                DrawToggle(_MatCapPerspective, "Perspective");
            }
            GUI.enabled = true;

            isShowLighting = Foldout("Lighting", isShowLighting);
            if(isShowLighting)
            {
                DrawLightParams(_LightParams);
            }

            isShowStencil = Foldout("Stencil", isShowStencil);
            if(isShowStencil)
            {
                m_MaterialEditor.RenderQueueField();
                EditorGUILayout.LabelField("Main", EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                DrawStencilRange(_EXStencilRef, "Ref");
                DrawStencilRange(_EXStencilReadMask, "ReadMask");
                DrawStencilRange(_EXStencilWriteMask, "WriteMask");
                DrawEnum(_EXStencilComp, "Comp", compareFunctionNames);
                DrawEnum(_EXStencilPass, "Pass", stencilOpNames);
                DrawEnum(_EXStencilFail, "Fail", stencilOpNames);
                DrawEnum(_EXStencilZFail, "ZFail", stencilOpNames);
                EditorGUILayout.EndVertical();

                EditorGUILayout.LabelField("Outline", EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                DrawStencilRange(_EXOLStencilRef, "Ref");
                DrawStencilRange(_EXOLStencilReadMask, "ReadMask");
                DrawStencilRange(_EXOLStencilWriteMask, "WriteMask");
                DrawEnum(_EXOLStencilComp, "Comp", compareFunctionNames);
                DrawEnum(_EXOLStencilPass, "Pass", stencilOpNames);
                DrawEnum(_EXOLStencilFail, "Fail", stencilOpNames);
                DrawEnum(_EXOLStencilZFail, "ZFail", stencilOpNames);
                EditorGUILayout.EndVertical();
            }

            isShowRendering = Foldout("Rendering", isShowRendering);
            if(isShowRendering)
            {
                EditorGUILayout.LabelField("Main", EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                DrawEnum(_EXCull, "Cull Mode", cullNames);
                DrawEnum(_EXSrcBlend, "SrcBlend", blendModeNames);
                DrawEnum(_EXDstBlend, "DstBlend", blendModeNames);
                DrawEnum(_EXSrcBlendAlpha, "SrcBlendAlpha", blendModeNames);
                DrawEnum(_EXDstBlendAlpha, "DstBlendAlpha", blendModeNames);
                DrawEnum(_EXBlendOp, "BlendOp", blendOpNames);
                DrawEnum(_EXBlendOpAlpha, "BlendOpAlpha", blendOpNames);
                DrawEnum(_EXSrcBlendFA, "SrcBlendFA", blendModeNames);
                DrawEnum(_EXDstBlendFA, "DstBlendFA", blendModeNames);
                DrawEnum(_EXBlendOpFA, "BlendOpFA", blendOpNames);
                DrawEnum(_EXBlendOpAlphaFA, "BlendOpAlphaFA", blendOpNames);
                DrawToggle(_EXZClip, "ZClip");
                DrawToggle(_EXZWrite, "ZWrite");
                DrawEnum(_EXZTest, "ZTest", compareFunctionNames);
                m_MaterialEditor.ShaderProperty(_EXOffsetFactor, "OffsetFactor");
                m_MaterialEditor.ShaderProperty(_EXOffsetUnits, "OffsetUnits");
                DrawEnum(_EXColorMask, "ColorMask", colorMaskNames);
                DrawToggle(_EXAlphaToMask, "AlphaToMask");
                EditorGUILayout.EndVertical();

                EditorGUILayout.LabelField("Outline", EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                DrawEnum(_EXOLCull, "Cull Mode", cullNames);
                DrawEnum(_EXOLSrcBlend, "SrcBlend", blendModeNames);
                DrawEnum(_EXOLDstBlend, "DstBlend", blendModeNames);
                DrawEnum(_EXOLSrcBlendAlpha, "SrcBlendAlpha", blendModeNames);
                DrawEnum(_EXOLDstBlendAlpha, "DstBlendAlpha", blendModeNames);
                DrawEnum(_EXOLBlendOp, "BlendOp", blendOpNames);
                DrawEnum(_EXOLBlendOpAlpha, "BlendOpAlpha", blendOpNames);
                DrawEnum(_EXOLSrcBlendFA, "SrcBlendFA", blendModeNames);
                DrawEnum(_EXOLDstBlendFA, "DstBlendFA", blendModeNames);
                DrawEnum(_EXOLBlendOpFA, "BlendOpFA", blendOpNames);
                DrawEnum(_EXOLBlendOpAlphaFA, "BlendOpAlphaFA", blendOpNames);
                DrawToggle(_EXOLZClip, "ZClip");
                DrawToggle(_EXOLZWrite, "ZWrite");
                DrawEnum(_EXOLZTest, "ZTest", compareFunctionNames);
                m_MaterialEditor.ShaderProperty(_EXOLOffsetFactor, "OffsetFactor");
                m_MaterialEditor.ShaderProperty(_EXOLOffsetUnits, "OffsetUnits");
                DrawEnum(_EXOLColorMask, "ColorMask", colorMaskNames);
                DrawToggle(_EXOLAlphaToMask, "AlphaToMask");
                EditorGUILayout.EndVertical();

                m_MaterialEditor.EnableInstancingField();
                m_MaterialEditor.DoubleSidedGIField();
                m_MaterialEditor.RenderQueueField();
            }

            if(EditorGUI.EndChangeCheck())
            {
                foreach(var obj in _Mode.targets)
                    MaterialChanged((Material)obj);
            }
        }

        private void FindProperties(MaterialProperty[] props)
        {
            _MainTex = FindProperty("_MainTex", props, false);
            _OutlineTex = FindProperty("_OutlineTex", props, false);
            _OutlineMaskTex = FindProperty("_OutlineMaskTex", props, false);
            _NormalMap = FindProperty("_NormalMap", props, false);
            _ShadowPositionTex = FindProperty("_ShadowPositionTex", props, false);
            _Shadow1stColorTex = FindProperty("_Shadow1stColorTex", props, false);
            _Shadow2ndColorTex = FindProperty("_Shadow2ndColorTex", props, false);
            _Shadow3rdColorTex = FindProperty("_Shadow3rdColorTex", props, false);
            _LayerMaskTex = FindProperty("_LayerMaskTex", props, false);
            _Layer1stColorTex = FindProperty("_Layer1stColorTex", props, false);
            _Layer2ndColorTex = FindProperty("_Layer2ndColorTex", props, false);
            _Layer3rdColorTex = FindProperty("_Layer3rdColorTex", props, false);
            _Color = FindProperty("_Color", props, false);
            _OutlineColor = FindProperty("_OutlineColor", props, false);
            _Shadow1stColor = FindProperty("_Shadow1stColor", props, false);
            _Shadow2ndColor = FindProperty("_Shadow2ndColor", props, false);
            _Shadow3rdColor = FindProperty("_Shadow3rdColor", props, false);
            _Rim1stColor = FindProperty("_Rim1stColor", props, false);
            _Rim2ndColor = FindProperty("_Rim2ndColor", props, false);
            _Layer1stColor = FindProperty("_Layer1stColor", props, false);
            _Layer2ndColor = FindProperty("_Layer2ndColor", props, false);
            _Layer3rdColor = FindProperty("_Layer3rdColor", props, false);
            _UVScrollMaNo = FindProperty("_UVScrollMaNo", props, false);
            _UVScrollSPS1 = FindProperty("_UVScrollSPS1", props, false);
            _UVScrollS2S3 = FindProperty("_UVScrollS2S3", props, false);
            _UVScrollOCOM = FindProperty("_UVScrollOCOM", props, false);
            _UVScrollLML1 = FindProperty("_UVScrollLML1", props, false);
            _UVScrollL2L3 = FindProperty("_UVScrollL2L3", props, false);
            _LightParams = FindProperty("_LightParams", props, false);
            _Shadow1stParams = FindProperty("_Shadow1stParams", props, false);
            _Shadow2ndParams = FindProperty("_Shadow2ndParams", props, false);
            _Shadow3rdParams = FindProperty("_Shadow3rdParams", props, false);
            _Shadow1stParams2 = FindProperty("_Shadow1stParams2", props, false);
            _Shadow2ndParams2 = FindProperty("_Shadow2ndParams2", props, false);
            _Shadow3rdParams2 = FindProperty("_Shadow3rdParams2", props, false);
            _RimParams = FindProperty("_RimParams", props, false);
            _Rim1stParams = FindProperty("_Rim1stParams", props, false);
            _Rim2ndParams = FindProperty("_Rim2ndParams", props, false);
            _SpecularParams = FindProperty("_SpecularParams", props, false);
            _Layer1stParams = FindProperty("_Layer1stParams", props, false);
            _Layer2ndParams = FindProperty("_Layer2ndParams", props, false);
            _Layer3rdParams = FindProperty("_Layer3rdParams", props, false);
            _Layer1stBlink = FindProperty("_Layer1stBlink", props, false);
            _Layer2ndBlink = FindProperty("_Layer2ndBlink", props, false);
            _Layer3rdBlink = FindProperty("_Layer3rdBlink", props, false);
            _Layer1stUV01Blend = FindProperty("_Layer1stUV01Blend", props, false);
            _Layer2ndUV01Blend = FindProperty("_Layer2ndUV01Blend", props, false);
            _Layer3rdUV01Blend = FindProperty("_Layer3rdUV01Blend", props, false);
            _Layer1stUVMSBlend = FindProperty("_Layer1stUVMSBlend", props, false);
            _Layer2ndUVMSBlend = FindProperty("_Layer2ndUVMSBlend", props, false);
            _Layer3rdUVMSBlend = FindProperty("_Layer3rdUVMSBlend", props, false);
            _Layer1stFadeParams = FindProperty("_Layer1stFadeParams", props, false);
            _Layer2ndFadeParams = FindProperty("_Layer2ndFadeParams", props, false);
            _Layer3rdFadeParams = FindProperty("_Layer3rdFadeParams", props, false);
            _Mode = FindProperty("_Mode", props, false);
            _Layers = FindProperty("_Layers", props, false);
            _Cutoff = FindProperty("_Cutoff", props, false);
            _MatCapNormal = FindProperty("_MatCapNormal", props, false);
            _NormalScale = FindProperty("_NormalScale", props, false);
            _OutlineWidth = FindProperty("_OutlineWidth", props, false);
            _OutlineVCControll = FindProperty("_OutlineVCControll", props, false);
            _OutlineZeroDelete = FindProperty("_OutlineZeroDelete", props, false);
            _UseShadow = FindProperty("_UseShadow", props, false);
            _UsePositionTex = FindProperty("_UsePositionTex", props, false);
            _UseShadowColorTex = FindProperty("_UseShadowColorTex", props, false);
            _MatCapStabilize = FindProperty("_MatCapStabilize", props, false);
            _MatCapPerspective = FindProperty("_MatCapPerspective", props, false);
            _Layer1stBlendMode = FindProperty("_Layer1stBlendMode", props, false);
            _Layer2ndBlendMode = FindProperty("_Layer2ndBlendMode", props, false);
            _Layer3rdBlendMode = FindProperty("_Layer3rdBlendMode", props, false);
            _Layer1stRim = FindProperty("_Layer1stRim", props, false);
            _Layer2ndRim = FindProperty("_Layer2ndRim", props, false);
            _Layer3rdRim = FindProperty("_Layer3rdRim", props, false);
            _Layer1stSpecular = FindProperty("_Layer1stSpecular", props, false);
            _Layer2ndSpecular = FindProperty("_Layer2ndSpecular", props, false);
            _Layer3rdSpecular = FindProperty("_Layer3rdSpecular", props, false);
            _EXCull = FindProperty("_EXCull", props, false);
            _EXSrcBlend = FindProperty("_EXSrcBlend", props, false);
            _EXDstBlend = FindProperty("_EXDstBlend", props, false);
            _EXSrcBlendAlpha = FindProperty("_EXSrcBlendAlpha", props, false);
            _EXDstBlendAlpha = FindProperty("_EXDstBlendAlpha", props, false);
            _EXBlendOp = FindProperty("_EXBlendOp", props, false);
            _EXBlendOpAlpha = FindProperty("_EXBlendOpAlpha", props, false);
            _EXSrcBlendFA = FindProperty("_EXSrcBlendFA", props, false);
            _EXDstBlendFA = FindProperty("_EXDstBlendFA", props, false);
            _EXBlendOpFA = FindProperty("_EXBlendOpFA", props, false);
            _EXBlendOpAlphaFA = FindProperty("_EXBlendOpAlphaFA", props, false);
            _EXZClip = FindProperty("_EXZClip", props, false);
            _EXZWrite = FindProperty("_EXZWrite", props, false);
            _EXZTest = FindProperty("_EXZTest", props, false);
            _EXStencilRef = FindProperty("_EXStencilRef", props, false);
            _EXStencilReadMask = FindProperty("_EXStencilReadMask", props, false);
            _EXStencilWriteMask = FindProperty("_EXStencilWriteMask", props, false);
            _EXStencilComp = FindProperty("_EXStencilComp", props, false);
            _EXStencilPass = FindProperty("_EXStencilPass", props, false);
            _EXStencilFail = FindProperty("_EXStencilFail", props, false);
            _EXStencilZFail = FindProperty("_EXStencilZFail", props, false);
            _EXOffsetFactor = FindProperty("_EXOffsetFactor", props, false);
            _EXOffsetUnits = FindProperty("_EXOffsetUnits", props, false);
            _EXColorMask = FindProperty("_EXColorMask", props, false);
            _EXAlphaToMask = FindProperty("_EXAlphaToMask", props, false);
            _EXOLCull = FindProperty("_EXOLCull", props, false);
            _EXOLSrcBlend = FindProperty("_EXOLSrcBlend", props, false);
            _EXOLDstBlend = FindProperty("_EXOLDstBlend", props, false);
            _EXOLSrcBlendAlpha = FindProperty("_EXOLSrcBlendAlpha", props, false);
            _EXOLDstBlendAlpha = FindProperty("_EXOLDstBlendAlpha", props, false);
            _EXOLBlendOp = FindProperty("_EXOLBlendOp", props, false);
            _EXOLBlendOpAlpha = FindProperty("_EXOLBlendOpAlpha", props, false);
            _EXOLSrcBlendFA = FindProperty("_EXOLSrcBlendFA", props, false);
            _EXOLDstBlendFA = FindProperty("_EXOLDstBlendFA", props, false);
            _EXOLBlendOpFA = FindProperty("_EXOLBlendOpFA", props, false);
            _EXOLBlendOpAlphaFA = FindProperty("_EXOLBlendOpAlphaFA", props, false);
            _EXOLZClip = FindProperty("_EXOLZClip", props, false);
            _EXOLZWrite = FindProperty("_EXOLZWrite", props, false);
            _EXOLZTest = FindProperty("_EXOLZTest", props, false);
            _EXOLStencilRef = FindProperty("_EXOLStencilRef", props, false);
            _EXOLStencilReadMask = FindProperty("_EXOLStencilReadMask", props, false);
            _EXOLStencilWriteMask = FindProperty("_EXOLStencilWriteMask", props, false);
            _EXOLStencilComp = FindProperty("_EXOLStencilComp", props, false);
            _EXOLStencilPass = FindProperty("_EXOLStencilPass", props, false);
            _EXOLStencilFail = FindProperty("_EXOLStencilFail", props, false);
            _EXOLStencilZFail = FindProperty("_EXOLStencilZFail", props, false);
            _EXOLOffsetFactor = FindProperty("_EXOLOffsetFactor", props, false);
            _EXOLOffsetUnits = FindProperty("_EXOLOffsetUnits", props, false);
            _EXOLColorMask = FindProperty("_EXOLColorMask", props, false);
            _EXOLAlphaToMask = FindProperty("_EXOLAlphaToMask", props, false);
        }

        //------------------------------------------------------------------------------------------------------------------------------
        // Base
        private bool Foldout(string title, bool display)
        {
            Rect rect = GUILayoutUtility.GetRect(16f, 20f, foldout);
			rect.width += 8f;
			rect.x -= 8f;
            GUI.Box(rect, new GUIContent(title, ""), foldout);

            Event e = Event.current;

            Rect toggleRect = new Rect(rect.x + 4f, rect.y + 2f, 13f, 13f);
            if(e.type == EventType.Repaint) {
                EditorStyles.foldout.Draw(toggleRect, false, false, display, false);
            }

            rect.width -= 24;
            if(e.type == EventType.MouseDown && rect.Contains(e.mousePosition)) {
                display = !display;
                e.Use();
            }

            return display;
        }

        private static Vector2 Vec2Field(string label, Vector2 vec, bool fixForScroll = false)
        {
            if(fixForScroll) GUILayout.Space(-2);

            const float indentPerLevel = 15;
            int indentLevel = EditorGUI.indentLevel;
            float labelWidth = EditorGUIUtility.labelWidth;
            float lineHeight = EditorGUIUtility.singleLineHeight;

            EditorGUI.indentLevel = 0;

            Rect position = EditorGUILayout.GetControlRect(true, lineHeight, EditorStyles.layerMaskField);
            float labelStartX = position.x + indentLevel * indentPerLevel;
            float valueStartX = position.x + labelWidth;
            Rect labelRect = new Rect(labelStartX, position.y, labelWidth, position.height);
            Rect valueRect = new Rect(valueStartX, position.y, position.width - labelWidth, position.height);
            EditorGUI.PrefixLabel(labelRect, new GUIContent(label));
            vec = EditorGUI.Vector2Field(valueRect, GUIContent.none, vec);

            EditorGUI.indentLevel = indentLevel;

            return vec;
        }

        private void DrawToggle(MaterialProperty prop, string label)
        {
            bool value = prop.floatValue != 0.0f;

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            value = EditorGUILayout.Toggle(label, value);
            EditorGUI.showMixedValue = false;
            if(EditorGUI.EndChangeCheck())
            {
                prop.floatValue = value ? 1.0f : 0.0f;
            }
        }

        private void DrawRenderingMode()
        {
            int mode = (int)_Mode.floatValue;

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = _Mode.hasMixedValue;
            mode = EditorGUILayout.Popup("Rendering Mode", mode, blendNames);
            EditorGUI.showMixedValue = false;
            if(EditorGUI.EndChangeCheck())
            {
                m_MaterialEditor.RegisterPropertyChangeUndo("Rendering Mode");
                _Mode.floatValue = mode;
            }
        }

        private void DrawLightParams(MaterialProperty prop)
        {
            float min = prop.vectorValue.x;
            float max = prop.vectorValue.y;
            float unlit = prop.vectorValue.z;
            float sh = prop.vectorValue.w;

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            min = EditorGUILayout.Slider("Min Brightness", min, 0.0f, 1.0f);
            max = EditorGUILayout.Slider("Max Brightness", max, 0.0f, 10.0f);
            unlit = EditorGUILayout.Slider("Unlitness", unlit, 0.0f, 1.0f);
            sh = EditorGUILayout.Slider("SH Intensity", sh, 0.0f, 10.0f);
            EditorGUI.showMixedValue = false;
            if(EditorGUI.EndChangeCheck())
            {
                prop.vectorValue = new Vector4(min, max, unlit, sh);
            }
        }

        private void DrawScrollXY(MaterialProperty prop)
        {
            Vector2 scroll = new Vector2(prop.vectorValue.x, prop.vectorValue.y);

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            scroll = Vec2Field("Scroll", scroll, true);
            EditorGUI.showMixedValue = false;
            if(EditorGUI.EndChangeCheck())
            {
                prop.vectorValue = new Vector4(
                    scroll.x,
                    scroll.y,
                    prop.vectorValue.z,
                    prop.vectorValue.w);
            }
        }

        private void DrawScrollZW(MaterialProperty prop)
        {
            Vector2 scroll = new Vector2(prop.vectorValue.z, prop.vectorValue.w);

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            scroll = Vec2Field("Scroll", scroll, true);
            EditorGUI.showMixedValue = false;
            if(EditorGUI.EndChangeCheck())
            {
                prop.vectorValue = new Vector4(
                    prop.vectorValue.x,
                    prop.vectorValue.y,
                    scroll.x,
                    scroll.y);
            }
        }

        private void DrawRemap(MaterialProperty prop)
        {
            float blur = 1.0f / prop.vectorValue.x;
            float border = -prop.vectorValue.y / prop.vectorValue.x;

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            border = EditorGUILayout.Slider("Border", border, -1.1f, 1.1f);
            blur = EditorGUILayout.Slider("Blur", blur, -1.0f, 1.0f);
            EditorGUI.showMixedValue = false;
            if(EditorGUI.EndChangeCheck())
            {
                if(blur == 0.0f) blur += 0.001f;
                prop.vectorValue = new Vector4(
                    1.0f / blur,
                    -border / blur,
                    prop.vectorValue.z,
                    prop.vectorValue.w);
            }
        }

        private void DrawRemapZW(MaterialProperty prop, string label)
        {
            float blur = 1.0f / prop.vectorValue.z;
            float border = -prop.vectorValue.w / prop.vectorValue.z;

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            border = EditorGUILayout.Slider(label + " Border", border, -1.1f, 1.1f);
            blur = EditorGUILayout.Slider(label + " Blur", blur, -1.0f, 1.0f);
            EditorGUI.showMixedValue = false;
            if(EditorGUI.EndChangeCheck())
            {
                if(blur == 0.0f) blur += 0.001f;
                prop.vectorValue = new Vector4(
                    prop.vectorValue.x,
                    prop.vectorValue.y,
                    1.0f / blur,
                    -border / blur);
            }
        }

        private void DrawShadowParams2(MaterialProperty prop)
        {
            float normal = prop.vectorValue.x;
            float receive = prop.vectorValue.y;
            float boost = prop.vectorValue.z;

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            normal = EditorGUILayout.Slider("Normal Map Strength", normal, 0.0f, 1.0f);
            receive = EditorGUILayout.Slider("Shadow Receive", receive, 0.0f, 1.0f);
            boost = EditorGUILayout.Slider("Remove Shadow Noise", boost, 1.0f, 10.0f);
            EditorGUI.showMixedValue = false;
            if(EditorGUI.EndChangeCheck())
            {
                prop.vectorValue = new Vector4(
                    normal,
                    receive,
                    boost,
                    prop.vectorValue.w);
            }
        }

        private void DrawEnum(MaterialProperty prop, string label, string[] names)
        {
            int value = (int)prop.floatValue;

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            value = EditorGUILayout.Popup(label, value, names);
            EditorGUI.showMixedValue = false;
            if(EditorGUI.EndChangeCheck())
            {
                prop.floatValue = value;
            }
        }

        private void DrawFoldoutTextureGUI(ref bool isShow, GUIContent guiContent, MaterialProperty tex, MaterialProperty col)
        {
            EditorGUI.indentLevel++;
            Rect rect = m_MaterialEditor.TexturePropertySingleLine(guiContent, tex, col);
            EditorGUI.indentLevel--;
            rect.x += 10;
            isShow = EditorGUI.Foldout(rect, isShow, "");
        }

        private void DrawTextureAndUV(ref bool isShow, GUIContent guiContent, MaterialProperty tex, MaterialProperty col, MaterialProperty scroll, bool isXY)
        {
            EditorGUI.indentLevel++;
            Rect rect = m_MaterialEditor.TexturePropertySingleLine(guiContent, tex, col);
            EditorGUI.indentLevel--;
            rect.x += 10;
            isShow = EditorGUI.Foldout(rect, isShow, "");

            if(isShow)
            {
                EditorGUI.indentLevel++;
                m_MaterialEditor.TextureScaleOffsetProperty(tex);
                if(isXY) DrawScrollXY(scroll);
                else     DrawScrollZW(scroll);
                EditorGUI.indentLevel--;
            }
        }

        private void DrawTextureAndUV(ref bool isShow, GUIContent guiContent, MaterialProperty tex, MaterialProperty scroll, bool isXY)
        {
            EditorGUI.indentLevel++;
            Rect rect = m_MaterialEditor.TexturePropertySingleLine(guiContent, tex);
            EditorGUI.indentLevel--;
            rect.x += 10;
            isShow = EditorGUI.Foldout(rect, isShow, "");

            if(isShow)
            {
                EditorGUI.indentLevel++;
                m_MaterialEditor.TextureScaleOffsetProperty(tex);
                if(isXY) DrawScrollXY(scroll);
                else     DrawScrollZW(scroll);
                EditorGUI.indentLevel--;
            }
        }

        private void DrawStencilRange(MaterialProperty prop, string label)
        {
            int value = (int)prop.floatValue;

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            value = EditorGUILayout.IntSlider(label, value, 0, 255);
            EditorGUI.showMixedValue = false;
            if(EditorGUI.EndChangeCheck())
            {
                prop.floatValue = value;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------
        // Rim
        private void DrawRim1stFresnel(MaterialProperty prop)
        {
            float width1 = prop.vectorValue.x;

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            width1 = EditorGUILayout.Slider("Fresnel", width1, 0.0f, 100.0f);
            EditorGUI.showMixedValue = prop.hasMixedValue;
            if(EditorGUI.EndChangeCheck())
            {
                prop.vectorValue = new Vector4(
                    width1,
                    prop.vectorValue.y,
                    prop.vectorValue.z,
                    prop.vectorValue.w);
            }
        }

        private void DrawRim2ndFresnel(MaterialProperty prop)
        {
            float width2 = prop.vectorValue.y;

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            width2 = EditorGUILayout.Slider("Fresnel", width2, 0.0f, 100.0f);
            EditorGUI.showMixedValue = false;
            if(EditorGUI.EndChangeCheck())
            {
                prop.vectorValue = new Vector4(
                    prop.vectorValue.x,
                    width2,
                    prop.vectorValue.z,
                    prop.vectorValue.w);
            }
        }

        private void DrawRimParams(MaterialProperty prop)
        {
            float normal = prop.vectorValue.z;
            float back = prop.vectorValue.w;

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            normal = EditorGUILayout.Slider("Normal Map Strength", normal, 0.0f, 1.0f);
            back = EditorGUILayout.Slider("Background", back, 0.0f, 1.0f);
            EditorGUI.showMixedValue = false;
            if(EditorGUI.EndChangeCheck())
            {
                prop.vectorValue = new Vector4(
                    prop.vectorValue.x,
                    prop.vectorValue.y,
                    normal,
                    back);
            }
        }

        private void DrawRimDirectionParams(MaterialProperty prop)
        {
            float dir = prop.vectorValue.z;

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            dir = EditorGUILayout.Slider("Direction", dir, -1.0f, 1.0f);
            EditorGUI.showMixedValue = false;
            if(EditorGUI.EndChangeCheck())
            {
                prop.vectorValue = new Vector4(
                    prop.vectorValue.x,
                    prop.vectorValue.y,
                    dir,
                    Mathf.Abs(dir));
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------
        // Specular
        private void DrawSpecularNS(MaterialProperty prop)
        {
            float normal = prop.vectorValue.z;
            float smooth = 1.0f - (1.0f / prop.vectorValue.w);

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            normal = EditorGUILayout.Slider("Normal Map Strength", normal, 0.0f, 1.0f);
            smooth = EditorGUILayout.Slider("Smoothness", smooth, 0.0f, 0.999f);
            EditorGUI.showMixedValue = false;
            if(EditorGUI.EndChangeCheck())
            {
                prop.vectorValue = new Vector4(
                    prop.vectorValue.x,
                    prop.vectorValue.y,
                    normal,
                    1.0f / (1.0f - smooth));
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------
        // Layer
        private void DrawLayers()
        {
            int layers = (int)_Layers.floatValue;

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = _Layers.hasMixedValue;
            layers = EditorGUILayout.Popup("Layers", layers, layerNames);
            EditorGUI.showMixedValue = false;
            if(EditorGUI.EndChangeCheck())
            {
                m_MaterialEditor.RegisterPropertyChangeUndo("Layers");
                _Layers.floatValue = layers;
            }
        }

        private void DrawBlink(MaterialProperty prop)
        {
            float strength = prop.vectorValue.x;
            float type = prop.vectorValue.y;
            float speed = prop.vectorValue.z / Mathf.PI;
            float offset = prop.vectorValue.w / Mathf.PI;

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            strength = EditorGUILayout.Slider("Blink Strength", strength, 0.0f, 1.0f);
            if(strength != 0.0f)
            {
                EditorGUI.indentLevel++;
                type    = EditorGUILayout.Toggle("Blink Type", type > 0.5f) ? 1.0f : 0.0f;
                speed   = EditorGUILayout.FloatField("Blink Speed", speed);
                offset  = EditorGUILayout.FloatField("Blink Delay", offset);
                EditorGUI.indentLevel--;
            }
            EditorGUI.showMixedValue = false;

            if(EditorGUI.EndChangeCheck())
            {
                prop.vectorValue = new Vector4(strength, type, speed * Mathf.PI, offset * Mathf.PI);
            }
        }

        private void DrawLayerParams(MaterialProperty prop)
        {
            float lighting = prop.vectorValue.x;
            float shadowmask = prop.vectorValue.y;
            bool clip = prop.vectorValue.z != 0.0f;
            bool fade = prop.vectorValue.w != 0.0f;

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            lighting = EditorGUILayout.Slider("Lighting", lighting, 0.0f, 1.0f);
            shadowmask = EditorGUILayout.Slider("Shadow Mask", shadowmask, 0.0f, 1.0f);
            clip = EditorGUILayout.Toggle("Clip UV", clip);
            fade = EditorGUILayout.Toggle("Distance Fade", fade);
            EditorGUI.showMixedValue = false;
            if(EditorGUI.EndChangeCheck())
            {
                prop.vectorValue = new Vector4(
                    lighting,
                    shadowmask,
                    clip ? 1.0f : 0.0f,
                    fade ? 1.0f : 0.0f);
            }
        }

        private void DrawLayerAlphaRemap(MaterialProperty prop)
        {
            float blur = 1.0f / prop.vectorValue.x;
            float border = -prop.vectorValue.y / prop.vectorValue.x;

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            border = EditorGUILayout.Slider("Alpha Border", border, -1.1f, 1.1f);
            blur = EditorGUILayout.Slider("Alpha Blur", blur, -1.0f, 1.0f);
            EditorGUI.showMixedValue = false;
            if(EditorGUI.EndChangeCheck())
            {
                if(blur == 0.0f) blur += 0.001f;
                prop.vectorValue = new Vector4(
                    1.0f / blur,
                    -border / blur,
                    prop.vectorValue.z,
                    prop.vectorValue.w);
            }
        }

        private void DrawLayerDistanceFadeRemap(MaterialProperty prop)
        {
            float start = -prop.vectorValue.w / prop.vectorValue.z;
            float end = (1.0f - prop.vectorValue.w) / prop.vectorValue.z;

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            start = EditorGUILayout.FloatField("Start Distance", start);
            end = EditorGUILayout.FloatField("End Distance", end);
            EditorGUI.showMixedValue = false;
            if(EditorGUI.EndChangeCheck())
            {
                if(start == end) end += 0.001f;
                prop.vectorValue = new Vector4(
                    prop.vectorValue.x,
                    prop.vectorValue.y,
                    1.0f / (end - start),
                    start / (start - end));
            }
        }

        private void DrawUVBlend(MaterialProperty prop, string label1, string label2)
        {
            Vector2 scale1 = new Vector2(prop.vectorValue.x, prop.vectorValue.y);
            Vector2 scale2 = new Vector2(prop.vectorValue.z, prop.vectorValue.w);

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            scale1 = Vec2Field(label1, scale1);
            scale2 = Vec2Field(label2, scale2);
            EditorGUI.showMixedValue = false;
            if(EditorGUI.EndChangeCheck())
            {
                prop.vectorValue = new Vector4(
                    scale1.x,
                    scale1.y,
                    scale2.x,
                    scale2.y);
            }
        }

        private void DrawBlendMode(MaterialProperty prop)
        {
            int value = (int)prop.floatValue;

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            value = EditorGUILayout.Popup("Blend Mode", value, new[]{"Alpha", "Add", "Screen", "Mul"});
            EditorGUI.showMixedValue = false;
            if(EditorGUI.EndChangeCheck())
            {
                prop.floatValue = value;
            }
        }

        private void DrawLayer(
            ref bool isShow_LayerColorTex,
            MaterialProperty _LayerColorTex,
            MaterialProperty _LayerColor,
            MaterialProperty _UVScroll,
            bool isXY,
            MaterialProperty _LayerUV01Blend,
            MaterialProperty _LayerUVMSBlend,
            MaterialProperty _LayerBlink,
            MaterialProperty _LayerParams,
            MaterialProperty _LayerFadeParams,
            MaterialProperty _LayerRim,
            MaterialProperty _LayerSpecular,
            MaterialProperty _LayerBlendMode)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            DrawFoldoutTextureGUI(ref isShow_LayerColorTex, new GUIContent("Color / Mask", "Color (RGB), Alpha (A)"), _LayerColorTex, _LayerColor);
            if(isShow_LayerColorTex)
            {
                EditorGUI.indentLevel++;
                m_MaterialEditor.TextureScaleOffsetProperty(_LayerColorTex);
                if(isXY) DrawScrollXY(_UVScroll);
                else     DrawScrollZW(_UVScroll);
                EditorGUILayout.LabelField("UV Blending", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                DrawUVBlend(_LayerUV01Blend, "UV0", "UV1");
                DrawUVBlend(_LayerUVMSBlend, "UV MatCap", "UV Screen");
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
            }
            DrawLayerAlphaRemap(_LayerFadeParams);
            DrawLayerParams(_LayerParams);
            if(_LayerParams.vectorValue.w != 0.0f)
            {
                EditorGUI.indentLevel++;
                DrawLayerDistanceFadeRemap(_LayerFadeParams);
                EditorGUI.indentLevel--;
            }
            DrawBlink(_LayerBlink);
            DrawToggle(_LayerRim, "Apply Rim");
            DrawToggle(_LayerSpecular, "Apply Specular");
            DrawBlendMode(_LayerBlendMode);

            EditorGUI.BeginChangeCheck();
            int presetType = EditorGUILayout.Popup("Apply Preset", 0, layerPresetNames);
            if(EditorGUI.EndChangeCheck())
            {
                ApplyLayerPreset(
                    _LayerUV01Blend,
                    _LayerUVMSBlend,
                    _LayerParams,
                    _LayerRim,
                    _LayerSpecular,
                    _LayerBlendMode,
                    presetType);
            }

            EditorGUILayout.EndVertical();
        }

        private void ApplyLayerPreset(
            MaterialProperty _LayerUV01Blend,
            MaterialProperty _LayerUVMSBlend,
            MaterialProperty _LayerParams,
            MaterialProperty _LayerRim,
            MaterialProperty _LayerSpecular,
            MaterialProperty _LayerBlendMode,
            int presetType)
        {
            switch(presetType)
            {
                case 0: // None
                    break;
                case 1: // Emission
                    _LayerUV01Blend.vectorValue = new Vector4(1.0f,1.0f,0.0f,0.0f);
                    _LayerUVMSBlend.vectorValue = new Vector4(0.0f,0.0f,0.0f,0.0f);
                    _LayerParams.vectorValue = new Vector4(0.0f,0.0f,0.0f,0.0f);
                    _LayerRim.floatValue = 0.0f;
                    _LayerSpecular.floatValue = 0.0f;
                    _LayerBlendMode.floatValue = 1.0f;
                    break;
                case 2: // Detail
                    _LayerUV01Blend.vectorValue = new Vector4(1.0f,1.0f,0.0f,0.0f);
                    _LayerUVMSBlend.vectorValue = new Vector4(0.0f,0.0f,0.0f,0.0f);
                    _LayerParams.vectorValue = new Vector4(0.0f,0.0f,0.0f,0.0f);
                    _LayerRim.floatValue = 0.0f;
                    _LayerSpecular.floatValue = 0.0f;
                    _LayerBlendMode.floatValue = 3.0f;
                    break;
                case 3: // Decal
                    _LayerUV01Blend.vectorValue = new Vector4(1.0f,1.0f,0.0f,0.0f);
                    _LayerUVMSBlend.vectorValue = new Vector4(0.0f,0.0f,0.0f,0.0f);
                    _LayerParams.vectorValue = new Vector4(0.0f,0.0f,1.0f,0.0f);
                    _LayerRim.floatValue = 0.0f;
                    _LayerSpecular.floatValue = 0.0f;
                    _LayerBlendMode.floatValue = 0.0f;
                    break;
                case 4: // MatCap
                    _LayerUV01Blend.vectorValue = new Vector4(0.0f,0.0f,0.0f,0.0f);
                    _LayerUVMSBlend.vectorValue = new Vector4(1.0f,1.0f,0.0f,0.0f);
                    _LayerParams.vectorValue = new Vector4(1.0f,0.0f,0.0f,0.0f);
                    _LayerRim.floatValue = 0.0f;
                    _LayerSpecular.floatValue = 0.0f;
                    _LayerBlendMode.floatValue = 1.0f;
                    break;
                case 5: // AngelRing
                    _LayerUV01Blend.vectorValue = new Vector4(0.0f,0.0f,0.0f,1.0f);
                    _LayerUVMSBlend.vectorValue = new Vector4(1.0f,0.0f,0.0f,0.0f);
                    _LayerParams.vectorValue = new Vector4(1.0f,0.0f,0.0f,0.0f);
                    _LayerRim.floatValue = 0.0f;
                    _LayerSpecular.floatValue = 0.0f;
                    _LayerBlendMode.floatValue = 1.0f;
                    break;
                case 6: // Rim Light
                    _LayerUV01Blend.vectorValue = new Vector4(1.0f,1.0f,0.0f,0.0f);
                    _LayerUVMSBlend.vectorValue = new Vector4(0.0f,0.0f,0.0f,0.0f);
                    _LayerParams.vectorValue = new Vector4(1.0f,0.0f,0.0f,0.0f);
                    _LayerRim.floatValue = 1.0f;
                    _LayerSpecular.floatValue = 0.0f;
                    _LayerBlendMode.floatValue = 1.0f;
                    break;
                case 7: // Specular
                    _LayerUV01Blend.vectorValue = new Vector4(1.0f,1.0f,0.0f,0.0f);
                    _LayerUVMSBlend.vectorValue = new Vector4(0.0f,0.0f,0.0f,0.0f);
                    _LayerParams.vectorValue = new Vector4(1.0f,0.0f,0.0f,0.0f);
                    _LayerRim.floatValue = 0.0f;
                    _LayerSpecular.floatValue = 1.0f;
                    _LayerBlendMode.floatValue = 1.0f;
                    break;
                case 8: // Distance Fade
                    _LayerUV01Blend.vectorValue = new Vector4(1.0f,1.0f,0.0f,0.0f);
                    _LayerUVMSBlend.vectorValue = new Vector4(0.0f,0.0f,0.0f,0.0f);
                    _LayerParams.vectorValue = new Vector4(0.0f,0.0f,0.0f,1.0f);
                    _LayerRim.floatValue = 0.0f;
                    _LayerSpecular.floatValue = 0.0f;
                    _LayerBlendMode.floatValue = 3.0f;
                    break;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------
        // Material
        private static void MaterialChanged(Material material)
        {
            SetupMaterialWithBlendMode(material, (RenderingMode)material.GetFloat("_Mode"));
            SetMaterialKeywords(material);
        }

        private static void SetupMaterialWithBlendMode(Material material, RenderingMode blendMode)
        {
            switch(blendMode)
            {
                case RenderingMode.Opaque:
                    material.SetOverrideTag("RenderType", "");
                    material.SetFloat("_EXSrcBlend", (float)UnityEngine.Rendering.BlendMode.One);
                    material.SetFloat("_EXDstBlend", (float)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetFloat("_EXAlphaToMask", 0);
                    material.SetFloat("_EXOLSrcBlend", (float)UnityEngine.Rendering.BlendMode.One);
                    material.SetFloat("_EXOLDstBlend", (float)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetFloat("_EXOLAlphaToMask", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.renderQueue = -1;
                    break;
                case RenderingMode.Cutout:
                    material.SetOverrideTag("RenderType", "TransparentCutout");
                    material.SetFloat("_EXSrcBlend", (float)UnityEngine.Rendering.BlendMode.One);
                    material.SetFloat("_EXDstBlend", (float)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetFloat("_EXAlphaToMask", 1);
                    material.SetFloat("_EXOLSrcBlend", (float)UnityEngine.Rendering.BlendMode.One);
                    material.SetFloat("_EXOLDstBlend", (float)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetFloat("_EXOLAlphaToMask", 1);
                    material.EnableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
                    break;
                case RenderingMode.Transparent:
                    material.SetOverrideTag("RenderType", "Transparent");
                    material.SetFloat("_EXSrcBlend", (float)UnityEngine.Rendering.BlendMode.One);
                    material.SetFloat("_EXDstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetFloat("_EXAlphaToMask", 0);
                    material.SetFloat("_EXOLSrcBlend", (float)UnityEngine.Rendering.BlendMode.One);
                    material.SetFloat("_EXOLDstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetFloat("_EXOLAlphaToMask", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.EnableKeyword("_ALPHABLEND_ON");
                    material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                    break;
            }
        }

        private static void SetMaterialKeywords(Material material)
        {
            int layers = (int)material.GetFloat("_Layers");
            switch(layers)
            {
                case 0:
                    material.DisableKeyword("_EMISSION");
                    material.DisableKeyword("_DETAIL_MULX2");
                    material.DisableKeyword("_PARALLAXMAP");
                    break;
                case 1:
                    material.EnableKeyword("_EMISSION");
                    material.DisableKeyword("_DETAIL_MULX2");
                    material.DisableKeyword("_PARALLAXMAP");
                    break;
                case 2:
                    material.DisableKeyword("_EMISSION");
                    material.EnableKeyword("_DETAIL_MULX2");
                    material.DisableKeyword("_PARALLAXMAP");
                    break;
                case 3:
                    material.DisableKeyword("_EMISSION");
                    material.DisableKeyword("_DETAIL_MULX2");
                    material.EnableKeyword("_PARALLAXMAP");
                    break;
            }
        }
    }

    public class EXHDRDrawer : MaterialPropertyDrawer
    {
        public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
        {
            float xMax = position.xMax;
            position.width = string.IsNullOrEmpty(label) ? Mathf.Min(50.0f, position.width) : EditorGUIUtility.labelWidth + 50.0f;
            Color value = prop.colorValue;
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            #if UNITY_2018_1_OR_NEWER
                value = EditorGUI.ColorField(position, new GUIContent(label), value, true, true, true);
            #else
                value = EditorGUI.ColorField(position, new GUIContent(label), value, true, true, true, null);
            #endif
            EditorGUI.showMixedValue = false;

            if(EditorGUI.EndChangeCheck())
            {
                prop.colorValue = value;
            }

            // Hex
            EditorGUI.BeginChangeCheck();
            float intensity = value.maxColorComponent > 1.0f ? value.maxColorComponent : 1.0f;
            Color value2 = new Color(value.r / intensity, value.g / intensity, value.b / intensity, 1.0f);
            string hex = ColorUtility.ToHtmlStringRGB(value2);
            int indentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            position.x += position.width + 4.0f;
            #if UNITY_2021_2_OR_NEWER
                position.width = Mathf.Min(50.0f, xMax - position.x);
                if(position.width > 10.0f)
                {
                    EditorGUI.showMixedValue = prop.hasMixedValue;
                    hex = "#" + EditorGUI.TextField(position, GUIContent.none, hex);
                    EditorGUI.showMixedValue = false;
                }
            #else
                position.width = 50.0f;
                EditorGUI.showMixedValue = prop.hasMixedValue;
                hex = "#" + EditorGUI.TextField(position, GUIContent.none, hex);
                EditorGUI.showMixedValue = false;
            #endif
            EditorGUI.indentLevel = indentLevel;
            if(EditorGUI.EndChangeCheck())
            {
                if(!ColorUtility.TryParseHtmlString(hex, out value2)) return;
                value.r = value2.r * intensity;
                value.g = value2.g * intensity;
                value.b = value2.b * intensity;
                prop.colorValue = value;
            }
        }
    }
}