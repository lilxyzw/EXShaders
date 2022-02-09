//------------------------------------------------------------------------------------------------------------------------------
// Skip warning
#pragma warning(disable: 3568 4008)

//------------------------------------------------------------------------------------------------------------------------------
// Replace shader keywords
#if defined(_EMISSION)
    #undef _EMISSION
    #define EX_1LAYER
#endif
#if defined(_DETAIL_MULX2)
    #undef _DETAIL_MULX2
    #define EX_2LAYER
#endif
#if defined(_PARALLAXMAP)
    #undef _PARALLAXMAP
    #define EX_3LAYER
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Inputs

// Name                 x           y           z           w
// -------------------- ----------- ----------- ----------- -----------
// _LightParams         min limit   max limit   unlit       sh strength
// _Shadow1stParams     sharpness   position    sharpness   position
// _Shadow1stParams2    normal      receive     boost       -
// _RimParams           1st width   2nd width   normal      backcolor
// _Rim1stParams        sharpness   position    direction   offset
// _SpecularParams      sharpness   position    normal      smoothness
// _Layer1stBlink       strength    mode        speed       delay
// _Layer1stParams      lighting    shadow      uv clip     dist fade
// _Layer1stUVAnims     scroll      scroll      angle       rotate
// _Layer1stUV01Blend   uv0         uv0         uv1         uv1
// _Layer1stUVMSBlend   uvMat       uvMat       uvScn       uvScn
// _Layer1stFadeParams  sharpness   position    sharpness   position

CBUFFER_START(UnityPerMaterial)
float4 _MainTex_ST;
float4 _OutlineTex_ST;
float4 _OutlineMaskTex_ST;
float4 _NormalMap_ST;
float4 _ShadowPositionTex_ST;
float4 _Shadow1stColorTex_ST;
float4 _Shadow2ndColorTex_ST;
float4 _Shadow3rdColorTex_ST;
float4 _LayerMaskTex_ST;
float4 _Layer1stColorTex_ST;
float4 _Layer2ndColorTex_ST;
float4 _Layer3rdColorTex_ST;
float4 _UVScrollMaNo;
float4 _UVScrollSPS1;
float4 _UVScrollS2S3;
float4 _UVScrollOCOM;
float4 _UVScrollLML1;
float4 _UVScrollL2L3;
float4 _Layer1stBlink;
float4 _Layer2ndBlink;
float4 _Layer3rdBlink;
float4 _Layer1stUV01Blend;
float4 _Layer2ndUV01Blend;
float4 _Layer3rdUV01Blend;
float4 _Layer1stUVMSBlend;
float4 _Layer2ndUVMSBlend;
float4 _Layer3rdUVMSBlend;

half4 _Color;
half4 _OutlineColor;
half4 _Shadow1stColor;
half4 _Shadow2ndColor;
half4 _Shadow3rdColor;
half4 _Rim1stColor;
half4 _Rim2ndColor;
half4 _Layer1stColor;
half4 _Layer2ndColor;
half4 _Layer3rdColor;
half4 _LightParams;
half4 _Shadow1stParams;
half4 _Shadow2ndParams;
half4 _Shadow3rdParams;
half4 _Shadow1stParams2;
half4 _Shadow2ndParams2;
half4 _Shadow3rdParams2;
half4 _RimParams;
half4 _Rim1stParams;
half4 _Rim2ndParams;
half4 _SpecularParams;
half4 _Layer1stParams;
half4 _Layer2ndParams;
half4 _Layer3rdParams;
half4 _Layer1stFadeParams;
half4 _Layer2ndFadeParams;
half4 _Layer3rdFadeParams;

float _OutlineWidth;
half _Cutoff;
half _MatCapNormal;
half _NormalScale;

uint _OutlineVCControll;
uint _OutlineZeroDelete;
uint _UseShadow;
uint _UsePositionTex;
uint _UseShadowColorTex;
uint _MatCapStabilize;
uint _MatCapPerspective;
uint _Layer1stBlendMode;
uint _Layer2ndBlendMode;
uint _Layer3rdBlendMode;
uint _Layer1stRim;
uint _Layer2ndRim;
uint _Layer3rdRim;
uint _Layer1stSpecular;
uint _Layer2ndSpecular;
uint _Layer3rdSpecular;
uint _EXCull;
uint _EXOLCull;
CBUFFER_END

TEXTURE2D(_MainTex);
TEXTURE2D(_OutlineTex);
TEXTURE2D(_OutlineMaskTex);
TEXTURE2D(_NormalMap);
TEXTURE2D(_ShadowPositionTex);
TEXTURE2D(_Shadow1stColorTex);
TEXTURE2D(_Shadow2ndColorTex);
TEXTURE2D(_Shadow3rdColorTex);
TEXTURE2D(_LayerMaskTex);
TEXTURE2D(_Layer1stColorTex);
TEXTURE2D(_Layer2ndColorTex);
TEXTURE2D(_Layer3rdColorTex);
SAMPLER(sampler_linear_repeat);
SAMPLER(sampler_MainTex);
SAMPLER(sampler_OutlineTex);
SAMPLER(sampler_NormalMap);
SAMPLER(sampler_LayerMaskTex);
SAMPLER(sampler_Layer1stColorTex);
SAMPLER(sampler_Layer2ndColorTex);
SAMPLER(sampler_Layer3rdColorTex);

//------------------------------------------------------------------------------------------------------------------------------
// Structs
struct appdata
{
    float4 positionOS   : POSITION;
    float2 uv0          : TEXCOORD0;
    float2 uv1          : TEXCOORD1;
    float3 normalOS     : NORMAL;
    float4 tangentOS    : TANGENT;
    float4 color        : COLOR;
    #if defined(EX_PASS_MOTIONVECTORS)
        float3 previousPositionOS : TEXCOORD4;
    #endif
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

#if defined(EX_PASS_FORWARD) || defined(EX_PASS_FORWARDADD)
    #define EX_V2F_POSITION_CS
    #define EX_V2F_POSITION_WS
    #define EX_V2F_TEXCOORD0
    #define EX_V2F_TEXCOORD1
    #define EX_V2F_NORMAL
    #define EX_V2F_TANGENT
    #define EX_V2F_FOG
    #if !defined(EX_PASS_FORWARDADD)
        #define EX_V2F_LIGHTDIRECTION
        #define EX_V2F_LIGHTCOLOR
        #define EX_V2F_SHADOW
    #endif
    #if !defined(EX_PASS_FORWARDADD) && defined(EX_USE_ADDLIGHT)
        #define EX_V2F_ADDLIGHT
    #endif

    struct v2f
    {
        float4 positionCS   : SV_POSITION;
        float4 positionWS   : TEXCOORD0;
        float4 uv01         : TEXCOORD1;
        half3 normalWS      : TEXCOORD2;
        half4 tangentWS     : TEXCOORD3;
        #if !defined(EX_PASS_FORWARDADD)
            half3 lightDir      : TEXCOORD4;
            half3 lightCol      : TEXCOORD5;
            EX_SHADOW_COORDS(6)     // _ShadowCoord
            EX_ADDLIGHT_COORDS(7)   // additionalLight
        #endif
        UNITY_VERTEX_INPUT_INSTANCE_ID
        UNITY_VERTEX_OUTPUT_STEREO
    };
#else
    #define EX_V2F_POSITION_CS
    #define EX_V2F_POSITION_WS
    #define EX_V2F_TEXCOORD0
    #define EX_V2F_TEXCOORD1
    #define EX_V2F_NORMAL

    struct v2f
    {
        float4 positionCS   : SV_POSITION;
        float3 positionWS   : TEXCOORD0;
        float4 uv01         : TEXCOORD1;
        half3 normalWS      : TEXCOORD2;
        EX_SUBPASS_CUSTOM_COORD(3)
        UNITY_VERTEX_INPUT_INSTANCE_ID
        UNITY_VERTEX_OUTPUT_STEREO
    };
#endif

struct EXCustomDatas
{
    float2 uvMat;
    half3 origN;
};

struct EXLayerDatas
{
    half3 rim;
    half specular;
};

//------------------------------------------------------------------------------------------------------------------------------
// Unpack v2f struct
void EXUnpackVertexData(inout EXVertexDatas vd, v2f i, float facing)
{
    EX_INIT_STRUCT(EXVertexDatas, vd);

    vd.renderingLayers = EXGetRenderingLayer();
    vd.featureFlags = EXGetFeatureFlags();
    vd.tileIndex = uint2(0,0);

    // Copy
    vd.facing = facing > 0;
    #if defined(EX_V2F_POSITION_WS)
        vd.positionWS = EXToAbsolutePositionWS(i.positionWS.xyz);
        float3 rawV = EXHeadDirection(vd.positionWS);
        vd.depth = length(rawV);
        vd.V = vd.V / vd.depth;
    #endif
    #if defined(EX_V2F_POSITION_CS)
        vd.positionCS = i.positionCS;
        vd.uvScn = i.positionCS.xy / _ScreenParams.xy;
        #if defined(UNITY_SINGLE_PASS_STEREO)
            vd.uvScn.x *= 0.5;
        #endif
    #endif
    #if defined(EX_V2F_TEXCOORD0) || defined(EX_V2F_TEXCOORD1)
        vd.uv0 = i.uv01.xy;
        vd.uv1 = i.uv01.zw;
    #endif
    #if defined(EX_V2F_NORMAL)
        vd.normalWS = i.normalWS.xyz;
        vd.N = i.normalWS.xyz;
    #endif
    #if defined(EX_V2F_TANGENT)
        vd.tangentWS = i.tangentWS.xyz;
    #endif
    #if defined(EX_V2F_NORMAL) && defined(EX_V2F_TANGENT)
        vd.bitangentWS = cross(i.normalWS, i.tangentWS.xyz) * i.tangentWS.w;
        vd.tbnWS = half3x3(vd.tangentWS, vd.bitangentWS, vd.normalWS);
    #endif
    #if defined(EX_V2F_FOG)
        vd.fogFactor = i.positionWS.w;
    #endif
}

void EXUnpackLightData(inout EXLightDatas ld, EXVertexDatas vd, v2f i EX_POSITION_INPUTS_FUNC)
{
    EX_INIT_STRUCT(EXLightDatas, ld);
    // Light
    ld.attenuation = 1.0;
    EX_LIGHT_ATTENUATION(ld.attenuation,i);
    #if defined(EX_V2F_LIGHTDIRECTION)
        ld.L = i.lightDir;
    #endif
    #if defined(EX_V2F_LIGHTCOLOR)
        ld.col = i.lightCol;
    #endif
    #if defined(EX_PASS_FORWARDADD)
        EX_GET_MAINLIGHT(ld.L, ld.col, vd, posInput);
    #endif
    #if defined(EX_V2F_ADDLIGHT)
        EX_COPY_ADDLIGHT(i,ld);
    #endif
}

//------------------------------------------------------------------------------------------------------------------------------
// Math
half3 EXBlendColor(half3 dstCol, half3 srcCol, half3 srcA, uint blendMode)
{
    half3 ad = dstCol + srcCol;
    half3 mu = dstCol * srcCol;
    half3 outCol;
    if(blendMode == 0) outCol = srcCol;               // Normal
    if(blendMode == 1) outCol = ad;                   // Add
    if(blendMode == 2) outCol = max(ad - mu, dstCol); // Screen
    if(blendMode == 3) outCol = mu;                   // Multiply
    return lerp(dstCol, outCol, srcA);
}

half EXCalcBlink(float4 blink)
{
    half outBlink = sin(_Time.y * blink.z + blink.w) * 0.5 + 0.5;
    if(blink.y) outBlink = round(outBlink);
    return 1.0 - outBlink * blink.x;
}

float2 EXCalcMatCapUV(float3 normalWS, float3 V, bool stabilize, bool perspective)
{
    float3 normalVD = perspective ? V : UNITY_MATRIX_V._m20_m21_m22;
    float3 bitangentVD = stabilize ? float3(0,1,0) : UNITY_MATRIX_V._m10_m11_m12;
    bitangentVD = EXOrthoNormalize(bitangentVD, normalVD);
    float3 tangentVD = cross(normalVD, bitangentVD);
    float3x3 tbnVD = float3x3(tangentVD, bitangentVD, normalVD);
    return mul(tbnVD, normalWS).xy * 0.5 + 0.5;
}

float2 EXBlendUV(float2 uv0, float2 uv1, float2 uvMat, float2 uvScn, float4 factor01, float4 factorMS)
{
    return uv0 * factor01.xy + uv1 * factor01.zw + uvMat * factorMS.xy + uvScn * factorMS.zw;
}

bool EXClipUV(float2 uv)
{
    return saturate(uv.x) == uv.x && saturate(uv.y) == uv.y;
}

//------------------------------------------------------------------------------------------------------------------------------
// Layer
void BlendLayer(
    inout half4 col,
    EXVertexDatas vd,
    EXLightDatas ld,
    EXCustomDatas cd,
    EXLayerDatas layerDatas,
    half mask,
    half NdotL,
    half4 _LayerColor,
    float4 _LayerColorTex_ST,
    float2 _UVScroll,
    half4 _LayerParams,
    float4 _LayerBlink,
    float4 _LayerUV01Blend,
    float4 _LayerUVMSBlend,
    half4 _LayerFadeParams,
    uint _LayerBlendMode,
    uint _LayerRim,
    uint _LayerSpecular,
    TEXTURE2D(_LayerColorTex)
    SAMPLER_IN_FUNC(sampler_LayerColorTex)
)
{
    float2 layerUV = EXBlendUV(vd.uv0, vd.uv1, cd.uvMat, vd.uvScn, _LayerUV01Blend, _LayerUVMSBlend);
    layerUV = EXCalcUV(layerUV, _LayerColorTex_ST, _UVScroll);
    half4 layerColorTex = SAMPLE2D(_LayerColorTex, sampler_LayerColorTex, layerUV) * _LayerColor;
    layerColorTex.rgb = lerp(layerColorTex.rgb, layerColorTex.rgb * ld.col, _LayerParams.x);
    layerColorTex.rgb = _LayerRim ? layerColorTex.rgb * layerDatas.rim : layerColorTex.rgb;
    half layerBlend = EXRemap(layerColorTex.a, _LayerFadeParams.x, _LayerFadeParams.y);
    layerBlend *= mask * EXCalcBlink(_LayerBlink);
    layerBlend = lerp(layerBlend, layerBlend * NdotL, _LayerParams.y);
    layerBlend = _LayerSpecular ? layerBlend * layerDatas.specular : layerBlend;
    layerBlend = _LayerParams.z && !EXClipUV(layerUV) ? 0.0 : layerBlend;
    layerBlend = _LayerParams.w ? layerBlend * EXRemap(vd.depth, _LayerFadeParams.z, _LayerFadeParams.w) : layerBlend;
    col.rgb = EXBlendColor(col.rgb, layerColorTex.rgb, layerBlend, _LayerBlendMode);
}

//------------------------------------------------------------------------------------------------------------------------------
// Vertex Shader
v2f vert(appdata i)
{
    v2f o;
    EX_INIT_STRUCT(v2f, o);

    // Single Pass Instanced rendering
    UNITY_SETUP_INSTANCE_ID(i);
    UNITY_TRANSFER_INSTANCE_ID(i, o);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

    // Outline
    #if defined(EX_OL)
        float3 bitangentOS = cross(i.normalOS, i.tangentOS.xyz) * (i.tangentOS.w * EX_NEGATIVE_SCALE);
        float3x3 tbnOS = float3x3(i.tangentOS.xyz, bitangentOS, i.normalOS);
        float3 outlineVector = _OutlineVCControll ? mul(i.color.xyz, tbnOS) * i.color.w : i.normalOS;
        float outlineWidth = SAMPLE2D_LOD(_OutlineMaskTex, sampler_linear_repeat, EXCalcUV(i.uv0, _OutlineMaskTex_ST, _UVScrollOCOM.zw), 0).r * _OutlineWidth;
        i.positionOS.xyz += outlineVector * outlineWidth * 0.01;
    #endif

    // Transform
    float3 positionWS = EXTransformOStoWS(float4(i.positionOS.xyz, 1.0));
    float4 positionCS = EXTransformWStoCS(positionWS.xyz);
    half3 normalWS = normalize(EXTransformNormalOStoWS(i.normalOS));
    half3 tangentWS = normalize(EXTransformDirOStoWS(i.tangentOS.xyz));

    // Fog
    half fogfactor = EXFog(positionCS.w);

    // Light
    EXVertexDatas vd;
    EX_INIT_STRUCT(EXVertexDatas, vd);
    vd.renderingLayers = EXGetRenderingLayer();
    vd.featureFlags = EXGetFeatureFlags();
    vd.positionWS = positionWS;
    vd.positionCS = positionCS;
    EX_POSITION_INPUTS(positionWS, positionCS, vd);

    half3 lightCol;
    half3 lightDirection;
    half3 additionalLight;
    EX_GET_MAINLIGHT(lightDirection, lightCol, vd, posInput);
    EX_GET_ADDLIGHT(additionalLight);
    half3 envCol = EXGetEnvironmentLight();
    half3 envDirection = EXGetEnvironmentDirection();

    // Copy
    #if defined(EX_V2F_POSITION_WS)
        o.positionWS.xyz = positionWS;
    #endif
    #if defined(EX_V2F_POSITION_CS)
        o.positionCS = positionCS;
    #endif
    #if defined(EX_V2F_TEXCOORD0) || defined(EX_V2F_TEXCOORD1)
        o.uv01.xy = i.uv0;
        o.uv01.zw = i.uv1;
    #endif
    #if defined(EX_V2F_NORMAL)
        o.normalWS.xyz = normalWS;
    #endif
    #if defined(EX_V2F_TANGENT)
        o.tangentWS.xyz = tangentWS;
        o.tangentWS.w = i.tangentOS.w;
    #endif
    #if defined(EX_V2F_LIGHTDIRECTION)
        o.lightDir = half3(0.0,0.01,-0.01);
        o.lightDir = o.lightDir + lightDirection * dot(lightCol, 4.0);
        o.lightDir = o.lightDir + envDirection;
        o.lightDir = normalize(o.lightDir);
    #endif
    #if defined(EX_V2F_LIGHTCOLOR)
        o.lightCol = lightCol + envCol * _LightParams.w;
        o.lightCol = clamp(o.lightCol, _LightParams.x, _LightParams.y);
        o.lightCol = o.lightCol - o.lightCol * _LightParams.z + _LightParams.z;
    #endif
    #if defined(EX_V2F_ADDLIGHT)
        o.additionalLight = additionalLight;
    #endif
    #if defined(EX_V2F_SHADOW)
        EX_TRANSFER_SHADOW(vd, i.uv1, o);
    #endif
    #if defined(EX_V2F_FOG)
        o.positionWS.w = fogfactor;
    #endif
    EX_SUBPASS_CUSTOM_TRANSFER(i, o);

    // Outline
    #if defined(EX_V2F_POSITION_CS) && defined(EX_OL) && defined(EX_PASS_FORWARD)
        if(_OutlineWidth == 0) o.positionCS = float4(0,0,0,0);
    #endif
    #if defined(EX_OL)
        if(_OutlineZeroDelete && outlineWidth <= 0.001) o.positionCS = 0.0/0.0;
    #endif

    return o;
}

//------------------------------------------------------------------------------------------------------------------------------
// Fragment Shader
#if defined(EX_PASS_FORWARD) || defined(EX_PASS_FORWARDADD)
half4 frag(v2f i EX_VFACE(facing)) : SV_Target
{
    UNITY_SETUP_INSTANCE_ID(i);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

    EX_VFACE_FALLBACK(facing);
    EXVertexDatas vd;
    EXLightDatas ld;
    EXUnpackVertexData(vd, i, facing);
    EX_POSITION_INPUTS(i.positionWS, i.positionCS, vd);
    EXUnpackLightData(ld, vd, i EX_POSITION_INPUTS_FUNC_IN);

    EXCustomDatas cd;

    #if defined(EX_PASS_FORWARDADD)
        ld.col = min(ld.col * ld.attenuation, _LightParams.y);
        ld.col = ld.col - ld.col * _LightParams.z;
    #endif

    // Main
    #if defined(EX_OL)
        half4 mainTex = SAMPLE2D(_OutlineTex, sampler_OutlineTex, EXCalcUV(vd.uv0, _OutlineTex_ST, _UVScrollOCOM.xy)) * _OutlineColor;
        half4 col = mainTex;
    #else
        half4 mainTex = SAMPLE2D(_MainTex, sampler_MainTex, EXCalcUV(vd.uv0, _MainTex_ST, _UVScrollMaNo.xy)) * _Color;
        half4 col = mainTex;
    #endif

    #if defined(_ALPHATEST_ON)
        col.a = saturate((col.a - _Cutoff) / max(fwidth(col.a), 0.0001) + 0.5);
    #elif defined(_ALPHABLEND_ON)
        clip(col.a  - _Cutoff);
    #else
        col.a = 1.0;
    #endif

    // Normal
    #if !defined(EX_OL)
        half4 normalMap = SAMPLE2D(_NormalMap, sampler_NormalMap, EXCalcUV(vd.uv0, _NormalMap_ST, _UVScrollMaNo.zw));
        vd.N = mul(EXUnpackNormalScale(normalMap, _NormalScale), vd.tbnWS);
        vd.N = vd.facing ? vd.N : -vd.N;
        cd.origN = vd.facing ? vd.normalWS : -vd.normalWS;
        vd.N = normalize(vd.N);
        cd.origN = normalize(cd.origN);
    #endif

    #if defined(EX_1LAYER) || defined(EX_2LAYER) || defined(EX_3LAYER)
        float3 matN = lerp(cd.origN, vd.N, _MatCapNormal);
        cd.uvMat = EXCalcMatCapUV(matN, vd.V, _MatCapStabilize, _MatCapPerspective);
    #endif

    // Shading
    #if !defined(EX_OL)
        half NdotL1st = 1.0;
        if(_UseShadow)
        {
            half3 shadow1stColorTex = mainTex.rgb;
            half3 shadow2ndColorTex = mainTex.rgb;
            half3 shadow3rdColorTex = mainTex.rgb;
            if(_UseShadowColorTex)
            {
                shadow1stColorTex = SAMPLE2D(_Shadow1stColorTex, sampler_MainTex, EXCalcUV(vd.uv0, _Shadow1stColorTex_ST, _UVScrollSPS1.zw)).rgb;
                shadow2ndColorTex = SAMPLE2D(_Shadow2ndColorTex, sampler_MainTex, EXCalcUV(vd.uv0, _Shadow2ndColorTex_ST, _UVScrollS2S3.xy)).rgb;
                shadow3rdColorTex = SAMPLE2D(_Shadow3rdColorTex, sampler_MainTex, EXCalcUV(vd.uv0, _Shadow3rdColorTex_ST, _UVScrollS2S3.zw)).rgb;
            }

            half3 shadow1stN = lerp(cd.origN, vd.N, _Shadow1stParams2.x);
            half3 shadow2ndN = lerp(cd.origN, vd.N, _Shadow2ndParams2.x);
            half3 shadow3rdN = lerp(cd.origN, vd.N, _Shadow3rdParams2.x);
                 NdotL1st = dot(shadow1stN, ld.L);
            half NdotL2nd = dot(shadow2ndN, ld.L);
            half NdotL3rd = dot(shadow3rdN, ld.L);
            #if defined(EX_USE_SHADOWS)
                //NdotL1st = lerp(NdotL1st, (NdotL1st + 1.0) * saturate(ld.attenuation * _Shadow1stParams2.z) - 1.0, _Shadow1stParams2.y);
                //NdotL2nd = lerp(NdotL2nd, (NdotL2nd + 1.0) * saturate(ld.attenuation * _Shadow2ndParams2.z) - 1.0, _Shadow2ndParams2.y);
                //NdotL3rd = lerp(NdotL3rd, (NdotL3rd + 1.0) * saturate(ld.attenuation * _Shadow3rdParams2.z) - 1.0, _Shadow3rdParams2.y);
                // Optimization
                NdotL1st = NdotL1st + (NdotL1st * _Shadow1stParams2.y + _Shadow1stParams2.y) * (saturate(ld.attenuation * _Shadow1stParams2.z) - 1.0);
                NdotL2nd = NdotL2nd + (NdotL2nd * _Shadow2ndParams2.y + _Shadow2ndParams2.y) * (saturate(ld.attenuation * _Shadow2ndParams2.z) - 1.0);
                NdotL3rd = NdotL3rd + (NdotL3rd * _Shadow3rdParams2.y + _Shadow3rdParams2.y) * (saturate(ld.attenuation * _Shadow3rdParams2.z) - 1.0);
            #endif
            if(_UsePositionTex)
            {
                half4 shadowPositionTex = SAMPLE2D(_ShadowPositionTex, sampler_MainTex, EXCalcUV(vd.uv0, _ShadowPositionTex_ST, _UVScrollSPS1.xy));
                shadowPositionTex.r = shadowPositionTex.r * _Shadow1stParams.z + _Shadow1stParams.w;
                shadowPositionTex.g = shadowPositionTex.g * _Shadow2ndParams.z + _Shadow2ndParams.w;
                shadowPositionTex.b = shadowPositionTex.b * _Shadow3rdParams.z + _Shadow3rdParams.w;
                NdotL1st += shadowPositionTex.r;
                NdotL2nd += shadowPositionTex.g;
                NdotL3rd += shadowPositionTex.b;
            }
            NdotL1st = EXRemap(NdotL1st, _Shadow1stParams.x, _Shadow1stParams.y);
            NdotL2nd = EXRemap(NdotL2nd, _Shadow2ndParams.x, _Shadow2ndParams.y);
            NdotL3rd = EXRemap(NdotL3rd, _Shadow3rdParams.x, _Shadow3rdParams.y);
            #if !defined(UNITY_COLORSPACE_GAMMA)
                NdotL1st *= NdotL1st;
                NdotL2nd *= NdotL2nd;
                NdotL3rd *= NdotL3rd;
            #endif

            col.rgb = lerp(shadow1stColorTex * _Shadow1stColor.rgb, col.rgb, NdotL1st);
            col.rgb = lerp(shadow2ndColorTex * _Shadow2ndColor.rgb, col.rgb, NdotL2nd);
            col.rgb = lerp(shadow3rdColorTex * _Shadow3rdColor.rgb, col.rgb, NdotL3rd);
        }
    #endif

    // Light
    col.rgb *= ld.col;
    #if !defined(EX_PASS_FORWARDADD)
        col.rgb = min(col.rgb + mainTex.rgb * ld.additionalLight, mainTex.rgb * _LightParams.y);
    #endif

    // Layers
    #if defined(EX_1LAYER) || defined(EX_2LAYER) || defined(EX_3LAYER)
        EXLayerDatas layerDatas;

        // Rim
        half3 rimN = lerp(cd.origN, vd.N, _RimParams.z);
        half rimNdotL = dot(rimN, ld.L);
        half rimNdotV = dot(rimN, vd.V);
        half rim = saturate(1.0 - rimNdotV);
        half rim1st = pow(rim, _RimParams.x);
        half rim2nd = pow(rim, _RimParams.y);
        rim1st = saturate(rim1st + rimNdotL * _Rim1stParams.z - _Rim1stParams.w);
        rim2nd = saturate(rim2nd + rimNdotL * _Rim2ndParams.z - _Rim2ndParams.w);
        rim1st = EXRemap(rim1st, _Rim1stParams.x, _Rim1stParams.y);
        rim2nd = EXRemap(rim2nd, _Rim2ndParams.x, _Rim2ndParams.y);
        layerDatas.rim = rim1st * _Rim1stColor.rgb + rim2nd * _Rim2ndColor.rgb + saturate(_RimParams.w - rim1st - rim2nd);

        // Specular
        half3 specN = lerp(cd.origN, vd.N, _SpecularParams.z);
        half3 H = normalize(vd.V + ld.L);
        half NdotH = saturate(dot(specN, H));
        layerDatas.specular = pow(NdotH, _SpecularParams.w);
        layerDatas.specular = EXRemap(layerDatas.specular, _SpecularParams.x, _SpecularParams.y);

        // Mask
        half4 layerMaskTex = SAMPLE2D(_LayerMaskTex, sampler_LayerMaskTex, EXCalcUV(vd.uv0, _LayerMaskTex_ST, _UVScrollLML1.xy));

        BlendLayer(
            col,
            vd,
            ld,
            cd,
            layerDatas,
            layerMaskTex.r,
            NdotL1st,
            _Layer1stColor,
            _Layer1stColorTex_ST,
            _UVScrollLML1.zw,
            _Layer1stParams,
            _Layer1stBlink,
            _Layer1stUV01Blend,
            _Layer1stUVMSBlend,
            _Layer1stFadeParams,
            _Layer1stBlendMode,
            _Layer1stRim,
            _Layer1stSpecular,
            _Layer1stColorTex
            SAMPLER_IN(sampler_Layer1stColorTex)
        );
    #endif

    #if defined(EX_2LAYER) || defined(EX_3LAYER)
        BlendLayer(
            col,
            vd,
            ld,
            cd,
            layerDatas,
            layerMaskTex.g,
            NdotL1st,
            _Layer2ndColor,
            _Layer2ndColorTex_ST,
            _UVScrollL2L3.xy,
            _Layer2ndParams,
            _Layer2ndBlink,
            _Layer2ndUV01Blend,
            _Layer2ndUVMSBlend,
            _Layer2ndFadeParams,
            _Layer2ndBlendMode,
            _Layer2ndRim,
            _Layer2ndSpecular,
            _Layer2ndColorTex
            SAMPLER_IN(sampler_Layer2ndColorTex)
        );
    #endif

    #if defined(EX_3LAYER)
        BlendLayer(
            col,
            vd,
            ld,
            cd,
            layerDatas,
            layerMaskTex.b,
            NdotL1st,
            _Layer3rdColor,
            _Layer3rdColorTex_ST,
            _UVScrollL2L3.zw,
            _Layer3rdParams,
            _Layer3rdBlink,
            _Layer3rdUV01Blend,
            _Layer3rdUVMSBlend,
            _Layer3rdFadeParams,
            _Layer3rdBlendMode,
            _Layer3rdRim,
            _Layer3rdSpecular,
            _Layer3rdColorTex
            SAMPLER_IN(sampler_Layer3rdColorTex)
        );
    #endif

    EX_DEEXPOSURE(col);
    EX_APPLY_FOG(col, vd.fogFactor);

    return col;
}
#else
void frag(v2f i EX_VFACE(facing) EX_SUBPASS_OUTPUTS)
{
    UNITY_SETUP_INSTANCE_ID(i);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

    EX_VFACE_FALLBACK(facing);
    EXVertexDatas vd;
    EXLightDatas ld;
    EXUnpackVertexData(vd, i, facing);
    EX_POSITION_INPUTS(i.positionWS, i.positionCS, vd);

    // Main
    #if defined(EX_HDRP) && defined(EX_PASS_DEPTHONLY)
        half4 mainTex = SAMPLE2D(_MainTex, sampler_MainTex, EXCalcUV(vd.uv0, _MainTex_ST, _UVScrollMaNo.xy)) * _Color;
        half4 col = mainTex;
        if(_OutlineWidth != 0)
        {
            if(vd.facing) discard;
            col = SAMPLE2D(_OutlineTex, sampler_OutlineTex, EXCalcUV(vd.uv0, _OutlineTex_ST, _UVScrollOCOM.xy)) * _OutlineColor;
        }
        if(_OutlineWidth == 0)
        {
            if(_EXCull == 1 && vd.facing || _EXCull == 2 && !vd.facing) discard;
        }
    #else
        #if defined(EX_OL)
            half4 mainTex = SAMPLE2D(_OutlineTex, sampler_OutlineTex, EXCalcUV(vd.uv0, _OutlineTex_ST, _UVScrollOCOM.xy)) * _OutlineColor;
            half4 col = mainTex;
        #else
            half4 mainTex = SAMPLE2D(_MainTex, sampler_MainTex, EXCalcUV(vd.uv0, _MainTex_ST, _UVScrollMaNo.xy)) * _Color;
            half4 col = mainTex;
        #endif
    #endif

    #if defined(_ALPHATEST_ON)
        clip(col.a  - _Cutoff);
    #elif defined(_ALPHABLEND_ON)
        clip(col.a  - max(0.5, _Cutoff));
    #else
        col.a = 1.0;
    #endif

    vd.N = normalize(vd.N);

    EX_OUTPUT_SUBPASS(i);
}
#endif