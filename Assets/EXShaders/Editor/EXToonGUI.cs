using System;
using UnityEditor;
using UnityEngine;
using static EXShaders.EXShadersUtil;

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
        private static readonly string[] layerEditModeNames = {"Full","Emission","Detail","Decal","MatCap","AngelRing","Rim Light","Specular","Distance Fade"};
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
        private static bool isShowBlending = false;
        private static bool isShowBlendingFA = false;
        private static bool isShowOLBlending = false;
        private static bool isShowOLBlendingFA = false;

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
        private MaterialProperty _Layer1stParams2;
        private MaterialProperty _Layer2ndParams2;
        private MaterialProperty _Layer3rdParams2;
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
        private MaterialProperty _Layer1stEditMode;
        private MaterialProperty _Layer2ndEditMode;
        private MaterialProperty _Layer3rdEditMode;
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
            Material material = materialEditor.target as Material;

            EditorGUI.BeginChangeCheck();
            bool renderingModeChanged = DrawRenderingMode();

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
                LayerDatas layerDatas1st = new LayerDatas(){
                    isXY = false,
                    _UVScroll           = _UVScrollLML1,
                    _LayerEditMode      = _Layer1stEditMode,
                    _LayerColorTex      = _Layer1stColorTex,
                    _LayerColor         = _Layer1stColor,
                    _LayerUV01Blend     = _Layer1stUV01Blend,
                    _LayerUVMSBlend     = _Layer1stUVMSBlend,
                    _LayerParams        = _Layer1stParams,
                    _LayerParams2       = _Layer1stParams2,
                    _LayerBlink         = _Layer1stBlink,
                    _LayerFadeParams    = _Layer1stFadeParams,
                    _LayerRim           = _Layer1stRim,
                    _LayerSpecular      = _Layer1stSpecular,
                    _LayerBlendMode     = _Layer1stBlendMode
                };
                LayerDatas layerDatas2nd = new LayerDatas(){
                    isXY = true,
                    _UVScroll           = _UVScrollLML1,
                    _LayerEditMode      = _Layer2ndEditMode,
                    _LayerColorTex      = _Layer2ndColorTex,
                    _LayerColor         = _Layer2ndColor,
                    _LayerUV01Blend     = _Layer2ndUV01Blend,
                    _LayerUVMSBlend     = _Layer2ndUVMSBlend,
                    _LayerParams        = _Layer2ndParams,
                    _LayerParams2       = _Layer2ndParams2,
                    _LayerBlink         = _Layer2ndBlink,
                    _LayerFadeParams    = _Layer2ndFadeParams,
                    _LayerRim           = _Layer2ndRim,
                    _LayerSpecular      = _Layer2ndSpecular,
                    _LayerBlendMode     = _Layer2ndBlendMode
                };
                LayerDatas layerDatas3rd = new LayerDatas(){
                    isXY = false,
                    _UVScroll           = _UVScrollLML1,
                    _LayerEditMode      = _Layer3rdEditMode,
                    _LayerColorTex      = _Layer3rdColorTex,
                    _LayerColor         = _Layer3rdColor,
                    _LayerUV01Blend     = _Layer3rdUV01Blend,
                    _LayerUVMSBlend     = _Layer3rdUVMSBlend,
                    _LayerParams        = _Layer3rdParams,
                    _LayerParams2       = _Layer3rdParams2,
                    _LayerBlink         = _Layer3rdBlink,
                    _LayerFadeParams    = _Layer3rdFadeParams,
                    _LayerRim           = _Layer3rdRim,
                    _LayerSpecular      = _Layer3rdSpecular,
                    _LayerBlendMode     = _Layer3rdBlendMode
                };

                DrawLayers();
                GUI.enabled = _Layers.floatValue > 0;
                DrawTextureAndUV(ref isShow_LayerMaskTex, new GUIContent("Layer Mask", "1st (R), 2nd (G), 3rd (B)"), _LayerMaskTex, _UVScrollLML1, true);
                DrawLayer(ref isShow_Layer1stColorTex, "1st Layer", layerDatas1st);
                GUI.enabled = _Layers.floatValue > 1;
                DrawLayer(ref isShow_Layer2ndColorTex, "2nd Layer", layerDatas2nd);
                GUI.enabled = _Layers.floatValue > 2;
                DrawLayer(ref isShow_Layer3rdColorTex, "3rd Layer", layerDatas3rd);
                GUI.enabled = true;
            }

            GUI.enabled = _Layers.floatValue > 0;
            isShowRim = Foldout("Rim Light (for Layers)", isShowRim) && GUI.enabled;
            if(isShowRim)
            {
                DoRimGUI();
            }

            isShowSpecular = Foldout("Specular (for Layers)", isShowSpecular) && GUI.enabled;
            if(isShowSpecular)
            {
                DoSpecularGUI();
            }

            isShowMatCap = Foldout("MatCap UV (for Layers)", isShowMatCap) && GUI.enabled;
            if(isShowMatCap)
            {
                DoMatCapGUI();
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
                EditorGUILayout.BeginHorizontal();
                if(GUILayout.Button("Set Writer"))
                {
                    _EXStencilRef.floatValue = 1;
                    _EXStencilReadMask.floatValue = 255.0f;
                    _EXStencilWriteMask.floatValue = 255.0f;
                    _EXStencilComp.floatValue = (float)UnityEngine.Rendering.CompareFunction.Always;
                    _EXStencilPass.floatValue = (float)UnityEngine.Rendering.StencilOp.Replace;
                    _EXStencilFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                    _EXStencilZFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                    _EXOLStencilRef.floatValue = 1;
                    _EXOLStencilReadMask.floatValue = 255.0f;
                    _EXOLStencilWriteMask.floatValue = 255.0f;
                    _EXOLStencilComp.floatValue = (float)UnityEngine.Rendering.CompareFunction.Always;
                    _EXOLStencilPass.floatValue = (float)UnityEngine.Rendering.StencilOp.Replace;
                    _EXOLStencilFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                    _EXOLStencilZFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                    if(_Mode.floatValue == 2.0f)      material.renderQueue = 3001;
                    else if(_Mode.floatValue == 1.0f) material.renderQueue = 2451;
                    else                              material.renderQueue = 2451;
                }
                if(GUILayout.Button("Set Reader"))
                {
                    _EXStencilRef.floatValue = 1;
                    _EXStencilReadMask.floatValue = 255.0f;
                    _EXStencilWriteMask.floatValue = 255.0f;
                    _EXStencilComp.floatValue = (float)UnityEngine.Rendering.CompareFunction.NotEqual;
                    _EXStencilPass.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                    _EXStencilFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                    _EXStencilZFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                    _EXOLStencilRef.floatValue = 1;
                    _EXOLStencilReadMask.floatValue = 255.0f;
                    _EXOLStencilWriteMask.floatValue = 255.0f;
                    _EXOLStencilComp.floatValue = (float)UnityEngine.Rendering.CompareFunction.NotEqual;
                    _EXOLStencilPass.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                    _EXOLStencilFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                    _EXOLStencilZFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                    if(_Mode.floatValue == 2.0f)      material.renderQueue = 3002;
                    else if(_Mode.floatValue == 1.0f) material.renderQueue = 2452;
                    else                              material.renderQueue = 2452;
                }
                if(GUILayout.Button("Reset"))
                {
                    _EXStencilRef.floatValue = 0;
                    _EXStencilReadMask.floatValue = 255.0f;
                    _EXStencilWriteMask.floatValue = 255.0f;
                    _EXStencilComp.floatValue = (float)UnityEngine.Rendering.CompareFunction.Always;
                    _EXStencilPass.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                    _EXStencilFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                    _EXStencilZFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                    _EXOLStencilRef.floatValue = 0;
                    _EXOLStencilReadMask.floatValue = 255.0f;
                    _EXOLStencilWriteMask.floatValue = 255.0f;
                    _EXOLStencilComp.floatValue = (float)UnityEngine.Rendering.CompareFunction.Always;
                    _EXOLStencilPass.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                    _EXOLStencilFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                    _EXOLStencilZFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                    if(_Mode.floatValue == 2.0f)      material.renderQueue = 3000;
                    else if(_Mode.floatValue == 1.0f) material.renderQueue = 2450;
                    else                              material.renderQueue = -1;
                }
                EditorGUILayout.EndHorizontal();

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

                m_MaterialEditor.RenderQueueField();
            }

            isShowRendering = Foldout("Rendering", isShowRendering);
            if(isShowRendering)
            {
                EditorGUILayout.LabelField("Main", EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                DrawEnum(_EXCull, "Cull Mode", cullNames);
                DrawToggle(_EXZClip, "ZClip");
                DrawToggle(_EXZWrite, "ZWrite");
                DrawEnum(_EXZTest, "ZTest", compareFunctionNames);
                m_MaterialEditor.ShaderProperty(_EXOffsetFactor, "OffsetFactor");
                m_MaterialEditor.ShaderProperty(_EXOffsetUnits, "OffsetUnits");
                DrawEnum(_EXColorMask, "ColorMask", colorMaskNames);
                DrawToggle(_EXAlphaToMask, "AlphaToMask");
                EditorGUI.indentLevel++;
                isShowBlending = EditorGUILayout.Foldout(isShowBlending, "Forward Blending");
                if(isShowBlending)
                {
                    DrawEnum(_EXSrcBlend, "SrcBlend", blendModeNames);
                    DrawEnum(_EXDstBlend, "DstBlend", blendModeNames);
                    DrawEnum(_EXSrcBlendAlpha, "SrcBlendAlpha", blendModeNames);
                    DrawEnum(_EXDstBlendAlpha, "DstBlendAlpha", blendModeNames);
                    DrawEnum(_EXBlendOp, "BlendOp", blendOpNames);
                    DrawEnum(_EXBlendOpAlpha, "BlendOpAlpha", blendOpNames);
                }
                isShowBlendingFA = EditorGUILayout.Foldout(isShowBlendingFA, "ForwardAdd Blending");
                if(isShowBlendingFA)
                {
                    DrawEnum(_EXSrcBlendFA, "SrcBlendFA", blendModeNames);
                    DrawEnum(_EXDstBlendFA, "DstBlendFA", blendModeNames);
                    DrawEnum(_EXBlendOpFA, "BlendOpFA", blendOpNames);
                    DrawEnum(_EXBlendOpAlphaFA, "BlendOpAlphaFA", blendOpNames);
                }
                EditorGUI.indentLevel--;
                EditorGUILayout.EndVertical();

                EditorGUILayout.LabelField("Outline", EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                DrawEnum(_EXOLCull, "Cull Mode", cullNames);
                DrawToggle(_EXOLZClip, "ZClip");
                DrawToggle(_EXOLZWrite, "ZWrite");
                DrawEnum(_EXOLZTest, "ZTest", compareFunctionNames);
                m_MaterialEditor.ShaderProperty(_EXOLOffsetFactor, "OffsetFactor");
                m_MaterialEditor.ShaderProperty(_EXOLOffsetUnits, "OffsetUnits");
                DrawEnum(_EXOLColorMask, "ColorMask", colorMaskNames);
                DrawToggle(_EXOLAlphaToMask, "AlphaToMask");
                EditorGUI.indentLevel++;
                isShowOLBlending = EditorGUILayout.Foldout(isShowOLBlending, "Forward Blending");
                if(isShowOLBlending)
                {
                    DrawEnum(_EXOLSrcBlend, "SrcBlend", blendModeNames);
                    DrawEnum(_EXOLDstBlend, "DstBlend", blendModeNames);
                    DrawEnum(_EXOLSrcBlendAlpha, "SrcBlendAlpha", blendModeNames);
                    DrawEnum(_EXOLDstBlendAlpha, "DstBlendAlpha", blendModeNames);
                    DrawEnum(_EXOLBlendOp, "BlendOp", blendOpNames);
                    DrawEnum(_EXOLBlendOpAlpha, "BlendOpAlpha", blendOpNames);
                }
                isShowOLBlendingFA = EditorGUILayout.Foldout(isShowOLBlendingFA, "ForwardAdd Blending");
                if(isShowOLBlendingFA)
                {
                    DrawEnum(_EXOLSrcBlendFA, "SrcBlendFA", blendModeNames);
                    DrawEnum(_EXOLDstBlendFA, "DstBlendFA", blendModeNames);
                    DrawEnum(_EXOLBlendOpFA, "BlendOpFA", blendOpNames);
                    DrawEnum(_EXOLBlendOpAlphaFA, "BlendOpAlphaFA", blendOpNames);
                }
                EditorGUI.indentLevel--;
                EditorGUILayout.EndVertical();

                m_MaterialEditor.EnableInstancingField();
                m_MaterialEditor.DoubleSidedGIField();
                m_MaterialEditor.RenderQueueField();
            }

            if(EditorGUI.EndChangeCheck())
            {
                foreach(var obj in _Mode.targets) MaterialChanged((Material)obj, renderingModeChanged);
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
            _Layer1stParams2 = FindProperty("_Layer1stParams2", props, false);
            _Layer2ndParams2 = FindProperty("_Layer2ndParams2", props, false);
            _Layer3rdParams2 = FindProperty("_Layer3rdParams2", props, false);
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
            _Layer1stEditMode = FindProperty("_Layer1stEditMode", props, false);
            _Layer2ndEditMode = FindProperty("_Layer2ndEditMode", props, false);
            _Layer3rdEditMode = FindProperty("_Layer3rdEditMode", props, false);
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
        private static bool Foldout(string title, bool display)
        {
            return FoldoutGUI(title, display, foldout);
        }

        private void DrawFoldoutTexture(ref bool isShow, GUIContent guiContent, MaterialProperty tex, MaterialProperty col)
        {
            DrawFoldoutTextureGUI(ref isShow, guiContent, tex, col, m_MaterialEditor);
        }

        private void DrawTextureAndUV(ref bool isShow, GUIContent guiContent, MaterialProperty tex, MaterialProperty col, MaterialProperty scroll, bool isScrollXY)
        {
            DrawTextureAndUVGUI(ref isShow, guiContent, tex, col, scroll, isScrollXY, m_MaterialEditor);
        }

        private void DrawTextureAndUV(ref bool isShow, GUIContent guiContent, MaterialProperty tex, MaterialProperty scroll, bool isScrollXY)
        {
            DrawTextureAndUVGUI(ref isShow, guiContent, tex, scroll, isScrollXY, m_MaterialEditor);
        }

        private bool DrawRenderingMode()
        {
            int mode = (int)_Mode.floatValue;

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = _Mode.hasMixedValue;
            mode = EditorGUILayout.Popup("Rendering Mode", mode, blendNames);
            EditorGUI.showMixedValue = false;
            bool changed = EditorGUI.EndChangeCheck();
            if(changed)
            {
                m_MaterialEditor.RegisterPropertyChangeUndo("Rendering Mode");
                _Mode.floatValue = mode;
            }

            return changed;
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

        private void DoRimGUI()
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

        private void DoSpecularGUI()
        {
            DrawRemap(_SpecularParams);
            DrawSpecularNS(_SpecularParams);
        }

        //------------------------------------------------------------------------------------------------------------------------------
        // MatCap
        private void DoMatCapGUI()
        {
            m_MaterialEditor.ShaderProperty(_MatCapNormal, "Normal Strength");
            DrawToggle(_MatCapStabilize, "Stabilize");
            DrawToggle(_MatCapPerspective, "Perspective");
        }

        //------------------------------------------------------------------------------------------------------------------------------
        // Layer
        private struct LayerDatas
        {
            public bool isXY;
            public MaterialProperty _UVScroll;
            public MaterialProperty _LayerEditMode;
            public MaterialProperty _LayerColorTex;
            public MaterialProperty _LayerColor;
            public MaterialProperty _LayerUV01Blend;
            public MaterialProperty _LayerUVMSBlend;
            public MaterialProperty _LayerParams;
            public MaterialProperty _LayerParams2;
            public MaterialProperty _LayerBlink;
            public MaterialProperty _LayerFadeParams;
            public MaterialProperty _LayerRim;
            public MaterialProperty _LayerSpecular;
            public MaterialProperty _LayerBlendMode;
        };

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

        private void DrawLayerParams(MaterialProperty prop, bool drawLighting, bool drawInvLighting, bool drawLightmask, bool drawShadowmask)
        {
            float lighting = prop.vectorValue.x;
            float invlighting = prop.vectorValue.y;
            float lightmask = prop.vectorValue.z;
            float shadowmask = prop.vectorValue.w;

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            if(drawLighting)    lighting = EditorGUILayout.Slider("Lighting", lighting, 0.0f, 1.0f);
            if(drawInvLighting) invlighting = EditorGUILayout.Slider("Inverse Lighting", invlighting, 0.0f, 1.0f);
            if(drawLightmask)   lightmask = EditorGUILayout.Slider("Disable in lit", lightmask, 0.0f, 1.0f);
            if(drawShadowmask)  shadowmask = EditorGUILayout.Slider("Disable in shadow", shadowmask, 0.0f, 1.0f);
            EditorGUI.showMixedValue = false;
            if(EditorGUI.EndChangeCheck())
            {
                prop.vectorValue = new Vector4(
                    lighting,
                    invlighting,
                    lightmask,
                    shadowmask);
            }
        }

        private void DrawLayerParams2(MaterialProperty prop, bool drawParallax, bool drawEmpty, bool drawClip, bool drawDistance)
        {
            float parallax = prop.vectorValue.x;
            float empty = prop.vectorValue.y;
            bool clip = prop.vectorValue.z != 0.0f;
            bool dist = prop.vectorValue.w != 0.0f;

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            if(drawParallax)    parallax = EditorGUILayout.Slider("Parallax", parallax, -1.0f, 1.0f);
            if(drawClip)        clip = EditorGUILayout.Toggle("UV Clip", clip);
            if(drawDistance)    dist = EditorGUILayout.Toggle("Distance Fade", dist);
            EditorGUI.showMixedValue = false;
            if(EditorGUI.EndChangeCheck())
            {
                prop.vectorValue = new Vector4(
                    parallax,
                    empty,
                    clip ? 1.0f : 0.0f,
                    dist ? 1.0f : 0.0f);
            }
        }

        private void DrawLayerAlphaRemap(MaterialProperty prop)
        {
            DrawRemap(prop, "Alpha");
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

        private void DrawAngelRingUV(MaterialProperty uv01, MaterialProperty uvms)
        {
            float blendUV1 = uv01.vectorValue.w;

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = uv01.hasMixedValue || uvms.hasMixedValue;
            blendUV1 = EditorGUILayout.Slider("BlendUV1", blendUV1, 0.0f, 1.0f);
            EditorGUI.showMixedValue = false;
            if(EditorGUI.EndChangeCheck())
            {
                uv01.vectorValue = new Vector4(
                    uv01.vectorValue.x,
                    uv01.vectorValue.y,
                    uv01.vectorValue.z,
                    blendUV1);
                uvms.vectorValue = new Vector4(
                    uvms.vectorValue.x,
                    1.0f - blendUV1,
                    uvms.vectorValue.z,
                    uvms.vectorValue.w);
            }
        }

        private void DrawLayer(ref bool isShow_LayerColorTex, string label, LayerDatas datas)
        {
            EditorGUI.BeginChangeCheck();
            DrawEnumBold(datas._LayerEditMode, label, layerEditModeNames);
            if(EditorGUI.EndChangeCheck())
            {
                ApplyLayerPreset(datas, (int)datas._LayerEditMode.floatValue);
            }

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            switch((int)datas._LayerEditMode.floatValue)
            {
                case 0:
                    DrawLayerFull(ref isShow_LayerColorTex, datas);
                    break;
                case 1:
                    DrawLayerAsEmission(ref isShow_LayerColorTex, datas);
                    break;
                case 2:
                    DrawLayerAsDetail(ref isShow_LayerColorTex, datas);
                    break;
                case 3:
                    DrawLayerAsDecal(ref isShow_LayerColorTex, datas);
                    break;
                case 4:
                    DrawLayerAsMatCap(ref isShow_LayerColorTex, datas);
                    break;
                case 5:
                    DrawLayerAsAngelRing(ref isShow_LayerColorTex, datas);
                    break;
                case 6:
                    DrawLayerAsRimLight(ref isShow_LayerColorTex, datas);
                    break;
                case 7:
                    DrawLayerAsSpecular(ref isShow_LayerColorTex, datas);
                    break;
                case 8:
                    DrawLayerAsDistanceFade(ref isShow_LayerColorTex, datas);
                    break;
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawLayerFull(ref bool isShow_LayerColorTex, LayerDatas datas)
        {
            DrawFoldoutTexture(ref isShow_LayerColorTex, new GUIContent("Color / Mask", "Color (RGB), Alpha (A)"), datas._LayerColorTex, datas._LayerColor);
            if(isShow_LayerColorTex)
            {
                EditorGUI.indentLevel++;
                m_MaterialEditor.TextureScaleOffsetProperty(datas._LayerColorTex);
                if(datas.isXY) DrawScrollXY(datas._UVScroll);
                else           DrawScrollZW(datas._UVScroll);
                EditorGUILayout.LabelField("UV Blending", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                DrawUVBlend(datas._LayerUV01Blend, "UV0", "UV1");
                DrawUVBlend(datas._LayerUVMSBlend, "UV MatCap", "UV Screen");
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
            }
            DrawLayerAlphaRemap(datas._LayerFadeParams);
            DrawLayerParams(datas._LayerParams, true, true, true, true);
            DrawLayerParams2(datas._LayerParams2, true, true, true, true);
            if(datas._LayerParams2.vectorValue.w != 0.0f)
            {
                EditorGUI.indentLevel++;
                DrawLayerDistanceFadeRemap(datas._LayerFadeParams);
                EditorGUI.indentLevel--;
            }
            DrawBlink(datas._LayerBlink);
            DrawToggle(datas._LayerRim, "Apply Rim");
            DrawToggle(datas._LayerSpecular, "Apply Specular");
            DrawBlendMode(datas._LayerBlendMode);

            EditorGUI.BeginChangeCheck();
            int presetType = EditorGUILayout.Popup("Apply Preset", 0, layerPresetNames);
            if(EditorGUI.EndChangeCheck())
            {
                ApplyLayerPreset(datas, presetType);
            }
        }

        private void DrawLayerAsEmission(ref bool isShow_LayerColorTex, LayerDatas datas)
        {
            DrawFoldoutTexture(ref isShow_LayerColorTex, new GUIContent("Color / Mask", "Color (RGB), Alpha (A)"), datas._LayerColorTex, datas._LayerColor);
            if(isShow_LayerColorTex)
            {
                EditorGUI.indentLevel++;
                m_MaterialEditor.TextureScaleOffsetProperty(datas._LayerColorTex);
                if(datas.isXY) DrawScrollXY(datas._UVScroll);
                else           DrawScrollZW(datas._UVScroll);
                EditorGUILayout.LabelField("UV Blending", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                DrawUVBlend(datas._LayerUV01Blend, "UV0", "UV1");
                DrawUVBlend(datas._LayerUVMSBlend, "UV MatCap", "UV Screen");
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
            }
            DrawLayerParams(datas._LayerParams, true, true, false, false);
            DrawLayerParams2(datas._LayerParams2, true, false, true, false);
            DrawBlink(datas._LayerBlink);
        }

        private void DrawLayerAsDetail(ref bool isShow_LayerColorTex, LayerDatas datas)
        {
            DrawFoldoutTexture(ref isShow_LayerColorTex, new GUIContent("Color / Mask", "Color (RGB), Alpha (A)"), datas._LayerColorTex, datas._LayerColor);
            if(isShow_LayerColorTex)
            {
                EditorGUI.indentLevel++;
                m_MaterialEditor.TextureScaleOffsetProperty(datas._LayerColorTex);
                DrawDetailUV(datas._LayerUV01Blend);
                EditorGUI.indentLevel--;
            }
            DrawLayerParams(datas._LayerParams, true, false, false, false);
            DrawBlendMode(datas._LayerBlendMode);
        }

        private void DrawLayerAsDecal(ref bool isShow_LayerColorTex, LayerDatas datas)
        {
            DrawFoldoutTexture(ref isShow_LayerColorTex, new GUIContent("Color / Mask", "Color (RGB), Alpha (A)"), datas._LayerColorTex, datas._LayerColor);
            if(isShow_LayerColorTex)
            {
                EditorGUI.indentLevel++;
                m_MaterialEditor.TextureScaleOffsetProperty(datas._LayerColorTex);
                DrawDetailUV(datas._LayerUV01Blend);
                EditorGUI.indentLevel--;
            }
            DrawLayerAlphaRemap(datas._LayerFadeParams);
            DrawLayerParams(datas._LayerParams, true, true, true, true);
            DrawLayerParams2(datas._LayerParams2, false, false, true, true);
            if(datas._LayerParams2.vectorValue.w != 0.0f)
            {
                EditorGUI.indentLevel++;
                DrawLayerDistanceFadeRemap(datas._LayerFadeParams);
                EditorGUI.indentLevel--;
            }
            DrawBlendMode(datas._LayerBlendMode);
        }

        private void DrawLayerAsMatCap(ref bool isShow_LayerColorTex, LayerDatas datas)
        {
            DrawFoldoutTexture(ref isShow_LayerColorTex, new GUIContent("Color / Mask", "Color (RGB), Alpha (A)"), datas._LayerColorTex, datas._LayerColor);
            if(isShow_LayerColorTex)
            {
                EditorGUI.indentLevel++;
                m_MaterialEditor.TextureScaleOffsetProperty(datas._LayerColorTex);
                EditorGUI.indentLevel--;
            }
            DoMatCapGUI();
            DrawLayerParams(datas._LayerParams, true, true, true, true);
            DrawBlendMode(datas._LayerBlendMode);
        }

        private void DrawLayerAsAngelRing(ref bool isShow_LayerColorTex, LayerDatas datas)
        {
            DrawFoldoutTexture(ref isShow_LayerColorTex, new GUIContent("Color / Mask", "Color (RGB), Alpha (A)"), datas._LayerColorTex, datas._LayerColor);
            if(isShow_LayerColorTex)
            {
                EditorGUI.indentLevel++;
                m_MaterialEditor.TextureScaleOffsetProperty(datas._LayerColorTex);
                DrawAngelRingUV(datas._LayerUV01Blend, datas._LayerUVMSBlend);
                EditorGUI.indentLevel--;
            }
            DoMatCapGUI();
            DrawLayerParams(datas._LayerParams, true, true, true, true);
            DrawBlendMode(datas._LayerBlendMode);
        }

        private void DrawLayerAsRimLight(ref bool isShow_LayerColorTex, LayerDatas datas)
        {
            DrawFoldoutTexture(ref isShow_LayerColorTex, new GUIContent("Color / Mask", "Color (RGB), Alpha (A)"), datas._LayerColorTex, datas._LayerColor);
            if(isShow_LayerColorTex)
            {
                EditorGUI.indentLevel++;
                m_MaterialEditor.TextureScaleOffsetProperty(datas._LayerColorTex);
                if(datas.isXY) DrawScrollXY(datas._UVScroll);
                else           DrawScrollZW(datas._UVScroll);
                EditorGUILayout.LabelField("UV Blending", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                DrawUVBlend(datas._LayerUV01Blend, "UV0", "UV1");
                DrawUVBlend(datas._LayerUVMSBlend, "UV MatCap", "UV Screen");
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
            }
            DoRimGUI();
            DrawLayerParams(datas._LayerParams, true, true, true, true);
            DrawBlendMode(datas._LayerBlendMode);
        }

        private void DrawLayerAsSpecular(ref bool isShow_LayerColorTex, LayerDatas datas)
        {
            DrawFoldoutTexture(ref isShow_LayerColorTex, new GUIContent("Color / Mask", "Color (RGB), Alpha (A)"), datas._LayerColorTex, datas._LayerColor);
            if(isShow_LayerColorTex)
            {
                EditorGUI.indentLevel++;
                m_MaterialEditor.TextureScaleOffsetProperty(datas._LayerColorTex);
                if(datas.isXY) DrawScrollXY(datas._UVScroll);
                else           DrawScrollZW(datas._UVScroll);
                EditorGUILayout.LabelField("UV Blending", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                DrawUVBlend(datas._LayerUV01Blend, "UV0", "UV1");
                DrawUVBlend(datas._LayerUVMSBlend, "UV MatCap", "UV Screen");
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
            }
            DoSpecularGUI();
            DrawLayerParams(datas._LayerParams, true, true, true, true);
            DrawBlendMode(datas._LayerBlendMode);
        }

        private void DrawLayerAsDistanceFade(ref bool isShow_LayerColorTex, LayerDatas datas)
        {
            DrawFoldoutTexture(ref isShow_LayerColorTex, new GUIContent("Color / Mask", "Color (RGB), Alpha (A)"), datas._LayerColorTex, datas._LayerColor);
            if(isShow_LayerColorTex)
            {
                EditorGUI.indentLevel++;
                m_MaterialEditor.TextureScaleOffsetProperty(datas._LayerColorTex);
                if(datas.isXY) DrawScrollXY(datas._UVScroll);
                else           DrawScrollZW(datas._UVScroll);
                EditorGUILayout.LabelField("UV Blending", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                DrawUVBlend(datas._LayerUV01Blend, "UV0", "UV1");
                DrawUVBlend(datas._LayerUVMSBlend, "UV MatCap", "UV Screen");
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
            }
            DrawLayerParams2(datas._LayerParams2, false, false, false, true);
            if(datas._LayerParams2.vectorValue.w != 0.0f)
            {
                EditorGUI.indentLevel++;
                DrawLayerDistanceFadeRemap(datas._LayerFadeParams);
                EditorGUI.indentLevel--;
            }
        }

        private void ApplyLayerPreset(LayerDatas datas, int presetType)
        {
            switch(presetType)
            {
                case 0: // None
                    break;
                case 1: // Emission
                    datas._LayerUV01Blend.vectorValue = new Vector4(1.0f,1.0f,0.0f,0.0f);
                    datas._LayerUVMSBlend.vectorValue = new Vector4(0.0f,0.0f,0.0f,0.0f);
                    datas._LayerParams.vectorValue = new Vector4(0.0f,0.0f,0.0f,0.0f);
                    datas._LayerParams2.vectorValue = new Vector4(0.0f,0.0f,0.0f,0.0f);
                    datas._LayerRim.floatValue = 0.0f;
                    datas._LayerSpecular.floatValue = 0.0f;
                    datas._LayerBlendMode.floatValue = 1.0f;
                    break;
                case 2: // Detail
                    datas._LayerUV01Blend.vectorValue = new Vector4(1.0f,1.0f,0.0f,0.0f);
                    datas._LayerUVMSBlend.vectorValue = new Vector4(0.0f,0.0f,0.0f,0.0f);
                    datas._LayerParams.vectorValue = new Vector4(0.0f,0.0f,0.0f,0.0f);
                    datas._LayerParams2.vectorValue = new Vector4(0.0f,0.0f,0.0f,0.0f);
                    datas._LayerRim.floatValue = 0.0f;
                    datas._LayerSpecular.floatValue = 0.0f;
                    datas._LayerBlendMode.floatValue = 3.0f;
                    break;
                case 3: // Decal
                    datas._LayerUV01Blend.vectorValue = new Vector4(1.0f,1.0f,0.0f,0.0f);
                    datas._LayerUVMSBlend.vectorValue = new Vector4(0.0f,0.0f,0.0f,0.0f);
                    datas._LayerParams.vectorValue = new Vector4(0.0f,0.0f,0.0f,0.0f);
                    datas._LayerParams2.vectorValue = new Vector4(0.0f,0.0f,1.0f,0.0f);
                    datas._LayerRim.floatValue = 0.0f;
                    datas._LayerSpecular.floatValue = 0.0f;
                    datas._LayerBlendMode.floatValue = 0.0f;
                    break;
                case 4: // MatCap
                    datas._LayerUV01Blend.vectorValue = new Vector4(0.0f,0.0f,0.0f,0.0f);
                    datas._LayerUVMSBlend.vectorValue = new Vector4(1.0f,1.0f,0.0f,0.0f);
                    datas._LayerParams.vectorValue = new Vector4(1.0f,0.0f,0.0f,0.0f);
                    datas._LayerParams2.vectorValue = new Vector4(0.0f,0.0f,0.0f,0.0f);
                    datas._LayerRim.floatValue = 0.0f;
                    datas._LayerSpecular.floatValue = 0.0f;
                    datas._LayerBlendMode.floatValue = 1.0f;
                    break;
                case 5: // AngelRing
                    datas._LayerUV01Blend.vectorValue = new Vector4(0.0f,0.0f,0.0f,1.0f);
                    datas._LayerUVMSBlend.vectorValue = new Vector4(1.0f,0.0f,0.0f,0.0f);
                    datas._LayerParams.vectorValue = new Vector4(1.0f,0.0f,0.0f,0.0f);
                    datas._LayerParams2.vectorValue = new Vector4(0.0f,0.0f,0.0f,0.0f);
                    datas._LayerRim.floatValue = 0.0f;
                    datas._LayerSpecular.floatValue = 0.0f;
                    datas._LayerBlendMode.floatValue = 1.0f;
                    break;
                case 6: // Rim Light
                    datas._LayerUV01Blend.vectorValue = new Vector4(1.0f,1.0f,0.0f,0.0f);
                    datas._LayerUVMSBlend.vectorValue = new Vector4(0.0f,0.0f,0.0f,0.0f);
                    datas._LayerParams.vectorValue = new Vector4(1.0f,0.0f,0.0f,0.0f);
                    datas._LayerParams2.vectorValue = new Vector4(0.0f,0.0f,0.0f,0.0f);
                    datas._LayerRim.floatValue = 1.0f;
                    datas._LayerSpecular.floatValue = 0.0f;
                    datas._LayerBlendMode.floatValue = 1.0f;
                    break;
                case 7: // Specular
                    datas._LayerUV01Blend.vectorValue = new Vector4(1.0f,1.0f,0.0f,0.0f);
                    datas._LayerUVMSBlend.vectorValue = new Vector4(0.0f,0.0f,0.0f,0.0f);
                    datas._LayerParams.vectorValue = new Vector4(1.0f,0.0f,0.0f,0.0f);
                    datas._LayerParams2.vectorValue = new Vector4(0.0f,0.0f,0.0f,0.0f);
                    datas._LayerRim.floatValue = 0.0f;
                    datas._LayerSpecular.floatValue = 1.0f;
                    datas._LayerBlendMode.floatValue = 1.0f;
                    break;
                case 8: // Distance Fade
                    datas._LayerUV01Blend.vectorValue = new Vector4(1.0f,1.0f,0.0f,0.0f);
                    datas._LayerUVMSBlend.vectorValue = new Vector4(0.0f,0.0f,0.0f,0.0f);
                    datas._LayerParams.vectorValue = new Vector4(0.0f,0.0f,0.0f,0.0f);
                    datas._LayerParams2.vectorValue = new Vector4(0.0f,0.0f,0.0f,1.0f);
                    datas._LayerRim.floatValue = 0.0f;
                    datas._LayerSpecular.floatValue = 0.0f;
                    datas._LayerBlendMode.floatValue = 3.0f;
                    break;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------
        // Material
        private static void MaterialChanged(Material material, bool changed)
        {
            if(changed) SetupMaterialWithBlendMode(material, (RenderingMode)material.GetFloat("_Mode"));
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
}