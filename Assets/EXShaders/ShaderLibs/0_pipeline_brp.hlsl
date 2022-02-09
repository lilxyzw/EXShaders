//------------------------------------------------------------------------------------------------------------------------------
// Libraries
#include "UnityCG.cginc"
#include "AutoLight.cginc"
#include "Lighting.cginc"
#include "UnityMetaPass.cginc"

#if !defined(half)
#define half float
#define half2 float2
#define half3 float3
#define half4 float4
#define half3x3 float3x3
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Version
#if !defined(SHADER_LIBRARY_VERSION_MAJOR)
    #if UNITY_VERSION < 201820
        #define SHADER_LIBRARY_VERSION_MAJOR 1
    #elif UNITY_VERSION < 201830
        #define SHADER_LIBRARY_VERSION_MAJOR 2
    #elif UNITY_VERSION < 201840
        #define SHADER_LIBRARY_VERSION_MAJOR 3
    #elif UNITY_VERSION < 201910
        #define SHADER_LIBRARY_VERSION_MAJOR 4
    #elif UNITY_VERSION < 201920
        #define SHADER_LIBRARY_VERSION_MAJOR 5
    #elif UNITY_VERSION < 201930
        #define SHADER_LIBRARY_VERSION_MAJOR 6
    #elif UNITY_VERSION < 201940
        #define SHADER_LIBRARY_VERSION_MAJOR 7
    #elif UNITY_VERSION < 202010
        #define SHADER_LIBRARY_VERSION_MAJOR 8
    #elif UNITY_VERSION < 202020
        #define SHADER_LIBRARY_VERSION_MAJOR 9
    #elif UNITY_VERSION < 202030
        #define SHADER_LIBRARY_VERSION_MAJOR 10
    #elif UNITY_VERSION < 202110
        #define SHADER_LIBRARY_VERSION_MAJOR 11
    #elif UNITY_VERSION < 202120
        #define SHADER_LIBRARY_VERSION_MAJOR 12
    #elif UNITY_VERSION < 202210
        #define SHADER_LIBRARY_VERSION_MAJOR 13
    #else
        #define SHADER_LIBRARY_VERSION_MAJOR 0
    #endif
#endif

#if !defined(SHADER_LIBRARY_VERSION_MINOR)
    #define SHADER_LIBRARY_VERSION_MINOR 99
#endif

#if !defined(VERSION_GREATER_EQUAL)
    #define VERSION_GREATER_EQUAL(major, minor) ((SHADER_LIBRARY_VERSION_MAJOR > major) || ((SHADER_LIBRARY_VERSION_MAJOR == major) && (SHADER_LIBRARY_VERSION_MINOR >= minor)))
#endif

#if !defined(VERSION_GREATER_EQUAL)
    #define VERSION_LOWER(major, minor) ((SHADER_LIBRARY_VERSION_MAJOR < major) || ((SHADER_LIBRARY_VERSION_MAJOR == major) && (SHADER_LIBRARY_VERSION_MINOR < minor)))
#endif

#if !defined(VERSION_GREATER_EQUAL)
    #define VERSION_EQUAL(major, minor) ((SHADER_LIBRARY_VERSION_MAJOR == major) && (SHADER_LIBRARY_VERSION_MINOR == minor))
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Macros
#define EX_BRP
#define EX_MATRIX_M        unity_ObjectToWorld
#define EX_MATRIX_I_M      unity_WorldToObject
#define EX_MATRIX_V        UNITY_MATRIX_V
#define EX_MATRIX_VP       UNITY_MATRIX_VP
#define EX_MATRIX_P        UNITY_MATRIX_P
#define EX_NEGATIVE_SCALE  unity_WorldTransformParams.w

#define EX_DEEXPOSURE(col)
#define EX_INVDEEXPOSURE(col)
#define EX_POSITION_INPUTS(positionWS,positionCS,vd)
#define EX_POSITION_INPUTS_FUNC
#define EX_POSITION_INPUTS_FUNC_IN

//------------------------------------------------------------------------------------------------------------------------------
// Subpass
#define EX_SUBPASS_OUTPUTS , out float4 outColor : SV_Target0

#if defined(EX_PASS_SHADOWCASTER)
    #if defined(SHADOWS_CUBE) && !defined(SHADOWS_CUBE_IN_DEPTH_TEX)
        #define EX_SUBPASS_CUSTOM_COORD(idx) float3 vec : TEXCOORD##idx;
        #define EX_SUBPASS_CUSTOM_TRANSFER(i,o) \
            o.vec = mul(unity_ObjectToWorld, i.positionOS).xyz - _LightPositionRange.xyz; \
            o.positionCS = UnityObjectToClipPos(i.positionOS)
        #define EX_SHADOW_CASTER_FRAGMENT(i) UnityEncodeCubeShadowDepth((length(i.vec) + unity_LightShadowBias.x) * _LightPositionRange.w)
    #else
        #define EX_SUBPASS_CUSTOM_COORD(idx)
        #define EX_SUBPASS_CUSTOM_TRANSFER(i,o) \
            o.positionCS = UnityClipSpaceShadowCasterPos(i.positionOS, i.normalOS); \
            o.positionCS = UnityApplyLinearShadowBias(o.positionCS)
        #define EX_SHADOW_CASTER_FRAGMENT(i) 0
    #endif
#else
    #define EX_SUBPASS_CUSTOM_COORD(idx)
    #define EX_SUBPASS_CUSTOM_TRANSFER(i,o)
    #define EX_SHADOW_CASTER_FRAGMENT(i) 0
#endif

#if defined(EX_PASS_SHADOWCASTER)
    #define EX_OUTPUT_SUBPASS(i) outColor = EX_SHADOW_CASTER_FRAGMENT(i)
#elif defined(EX_PASS_META)
    #define EX_OUTPUT_SUBPASS(i) outColor = col
#else
    #define EX_OUTPUT_SUBPASS(i) outColor = 0
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Rendering Layer
uint EXGetRenderingLayer()
{
    return 0;
}

uint EXGetFeatureFlags()
{
    return 0;
}

//------------------------------------------------------------------------------------------------------------------------------
// Position
float3 EXTransformOStoWS(float4 positionOS)
{
    return mul(EX_MATRIX_M, positionOS).xyz;
}

float3 EXTransformOStoWS(float3 positionOS)
{
    return mul(EX_MATRIX_M, float4(positionOS, 1.0)).xyz;
}

float3 EXTransformWStoOS(float3 positionWS)
{
    return mul(EX_MATRIX_I_M, float4(positionWS, 1.0)).xyz;
}

float3 EXTransformWStoVS(float3 positionWS)
{
    return UnityWorldToViewPos(positionWS).xyz;
}

float4 EXTransformWStoCS(float3 positionWS)
{
    return UnityWorldToClipPos(positionWS);
}

float4 EXTransformVStoCS(float3 positionVS)
{
    return UnityViewToClipPos(positionVS);
}

float4 EXTransformCStoSS(float4 positionCS)
{
    return ComputeGrabScreenPos(positionCS);
}

float4 EXTransformCStoSSFrag(float4 positionCS)
{
    float4 positionSS = float4(positionCS.xyz * positionCS.w, positionCS.w);
    positionSS.xy = positionSS.xy / _ScreenParams.xy;
    return positionSS;
}

float3 EXToAbsolutePositionWS(float3 positionRWS)
{
    return positionRWS;
}

//------------------------------------------------------------------------------------------------------------------------------
// Fog
#if defined(EX_PASS_FORWARD)
    #define EX_APPLY_FOG(col,factor) UNITY_FOG_LERP_COLOR(col, unity_FogColor, factor);
#elif defined(EX_PASS_FORWARDADD)
    #define EX_APPLY_FOG(col,factor) UNITY_FOG_LERP_COLOR(col, half4(0,0,0,0), factor);
#endif

half EXFog(float depth)
{
    #if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
        UNITY_CALC_FOG_FACTOR(depth);
        return unityFogFactor;
    #else
        return 1.0;
    #endif
}

//------------------------------------------------------------------------------------------------------------------------------
// Main Light
#if defined(EX_PASS_FORWARD)
    #define EX_GET_MAINLIGHT(dir,col,vd,posInput) dir = _WorldSpaceLightPos0.xyz; col = _LightColor0.rgb
#elif defined(DIRECTIONAL) || defined(DIRECTIONAL_COOKIE)
    #define EX_GET_MAINLIGHT(dir,col,vd,posInput) dir = _WorldSpaceLightPos0.xyz; col = _LightColor0.rgb
#elif defined(POINT) || defined(SPOT) || defined(POINT_COOKIE)
    #define EX_GET_MAINLIGHT(dir,col,vd,posInput) dir = normalize(_WorldSpaceLightPos0.xyz - vd.positionWS); col = _LightColor0.rgb
#elif defined(EX_PASS_FORWARDADD)
    #define EX_GET_MAINLIGHT(dir,col,vd,posInput) dir = normalize(_WorldSpaceLightPos0.xyz - vd.positionWS * _WorldSpaceLightPos0.w); col = _LightColor0.rgb
#else
    #define EX_GET_MAINLIGHT(dir,col,vd,posInput) dir = _WorldSpaceLightPos0.xyz; col = _LightColor0.rgb
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Shadow
#if defined(EX_PASS_FORWARD) && defined(SHADOWS_SCREEN)
    #define EX_USE_SHADOWS
    #define EX_SHADOW_COORDS(idx) UNITY_SHADOW_COORDS(idx)
    #define EX_TRANSFER_SHADOW(vi,uv,o) \
        DummyStructure v; \
        v.vertex = i.positionOS; \
        BRPShadowCoords brpShadowCoords; \
        brpShadowCoords.pos = vi.positionCS; \
        UNITY_TRANSFER_LIGHTING(brpShadowCoords, uv) \
        o._ShadowCoord = brpShadowCoords._ShadowCoord
    #define EX_LIGHT_ATTENUATION(atten,i) \
        BRPShadowCoords brpShadowCoords; \
        brpShadowCoords.pos = i.positionCS; \
        brpShadowCoords._ShadowCoord = i._ShadowCoord; \
        UNITY_LIGHT_ATTENUATION(attenuationOrig, brpShadowCoords, i.positionWS.xyz); \
        atten = attenuationOrig
#elif defined(EX_PASS_FORWARDADD)
    #if defined(POINT)
        #define EX_CALC_LIGHT_COORDS(o,i) o._LightCoord = mul(unity_WorldToLight, float4(i.positionWS.xyz, 1.0)).xyz;
    #elif defined(SPOT)
        #define EX_CALC_LIGHT_COORDS(o,i) o._LightCoord = mul(unity_WorldToLight, float4(i.positionWS.xyz, 1.0));
    #elif defined(POINT_COOKIE)
        #define EX_CALC_LIGHT_COORDS(o,i) o._LightCoord = mul(unity_WorldToLight, float4(i.positionWS.xyz, 1.0)).xyz;
    #elif defined(DIRECTIONAL_COOKIE)
        #define EX_CALC_LIGHT_COORDS(o,i) o._LightCoord = mul(unity_WorldToLight, float4(i.positionWS.xyz, 1.0)).xy;
    #else
        #define EX_CALC_LIGHT_COORDS(o,i)
    #endif

    #define EX_SHADOW_COORDS(idx)
    #define EX_TRANSFER_SHADOW(vi,uv,o)
    #define EX_LIGHT_ATTENUATION(atten,i) \
        BRPShadowCoords brpShadowCoords; \
        brpShadowCoords.pos = i.positionCS; \
        EX_CALC_LIGHT_COORDS(brpShadowCoords,i) \
        UNITY_LIGHT_ATTENUATION(attenuationOrig, brpShadowCoords, i.positionWS.xyz); \
        atten = attenuationOrig
#else
    #define EX_SHADOW_COORDS(idx)
    #define EX_TRANSFER_SHADOW(vi,uv,o)
    #define EX_LIGHT_ATTENUATION(atten,i)
#endif

struct BRPShadowCoords
{
    float4 pos;
    EX_SHADOW_COORDS(0)
    #if defined(DECLARE_LIGHT_COORDS)
        DECLARE_LIGHT_COORDS(1)
    #elif defined(POINT)
        unityShadowCoord3 _LightCoord : TEXCOORD1;
    #elif defined(SPOT)
        unityShadowCoord4 _LightCoord : TEXCOORD1;
    #elif defined(POINT_COOKIE)
        unityShadowCoord3 _LightCoord : TEXCOORD1;
    #elif defined(DIRECTIONAL_COOKIE)
        unityShadowCoord2 _LightCoord : TEXCOORD1;
    #endif
};

struct DummyStructure
{
    float4 vertex;
};

//------------------------------------------------------------------------------------------------------------------------------
// Environment Light (SH)
half3 EXGetEnvironmentLight()
{
    half3 sh;
    sh.r = dot(unity_SHAr.xyz, unity_SHAr.xyz);
    sh.g = dot(unity_SHAg.xyz, unity_SHAg.xyz);
    sh.b = dot(unity_SHAb.xyz, unity_SHAb.xyz);
    sh = sqrt(sh);
    sh.r += unity_SHBr.z * 0.333333;
    sh.g += unity_SHBg.z * 0.333333;
    sh.b += unity_SHBb.z * 0.333333;
    sh.r += unity_SHAr.w;
    sh.g += unity_SHAg.w;
    sh.b += unity_SHAb.w;
    return sh;
}

half3 EXGetEnvironmentDirection()
{
    half3 lightDirection = unity_SHAr.xyz + unity_SHAg.xyz + unity_SHAb.xyz;
    return lightDirection;
}

//------------------------------------------------------------------------------------------------------------------------------
// Additional Light
#if defined(EX_PASS_FORWARD) && defined(UNITY_SHOULD_SAMPLE_SH)
    #define EX_USE_ADDLIGHT
    #define EX_ADDLIGHT_COORDS(idx) half3 additionalLight : TEXCOORD##idx;
    #if defined(VERTEXLIGHT_ON)
        #define EX_GET_ADDLIGHT(o) o = saturate(EXGetAdditionalLights(positionWS, positionCS, vd.renderingLayers, vd.featureFlags))
    #else
        #define EX_GET_ADDLIGHT(o) o = 0
    #endif
#else
    #define EX_ADDLIGHT_COORDS(idx)
    #define EX_GET_ADDLIGHT(o) o = 0
#endif
#define EX_COPY_ADDLIGHT(i,ld) ld.additionalLight = i.additionalLight

half3 EXGetAdditionalLights(float3 positionWS, float4 positionCS, uint renderingLayers, uint featureFlags)
{
    float4 toLightX = unity_4LightPosX0 - positionWS.x;
    float4 toLightY = unity_4LightPosY0 - positionWS.y;
    float4 toLightZ = unity_4LightPosZ0 - positionWS.z;

    float4 lengthSq = toLightX * toLightX + 0.000001;
    lengthSq += toLightY * toLightY;
    lengthSq += toLightZ * toLightZ;

    //half4 atten = 1.0 / (1.0 + lengthSq * unity_4LightAtten0);
    half4 atten = saturate(saturate((25.0 - lengthSq * unity_4LightAtten0) * 0.111375) / (0.987725 + lengthSq * unity_4LightAtten0));

    half3 additionalLightColor;
    additionalLightColor =                                 unity_LightColor[0].rgb * atten.x;
    additionalLightColor =          additionalLightColor + unity_LightColor[1].rgb * atten.y;
    additionalLightColor =          additionalLightColor + unity_LightColor[2].rgb * atten.z;
    additionalLightColor = saturate(additionalLightColor + unity_LightColor[3].rgb * atten.w);

    return additionalLightColor;
}