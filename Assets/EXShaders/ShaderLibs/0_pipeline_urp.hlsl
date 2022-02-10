//------------------------------------------------------------------------------------------------------------------------------
// Libraries
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"

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
#define EX_URP
#define EX_MATRIX_M        GetObjectToWorldMatrix()
#define EX_MATRIX_I_M      GetWorldToObjectMatrix()
#define EX_MATRIX_V        GetWorldToViewMatrix()
#define EX_MATRIX_VP       GetWorldToHClipMatrix()
#define EX_MATRIX_P        GetViewToHClipMatrix()
#define EX_NEGATIVE_SCALE  GetOddNegativeScale()

#define EX_DEEXPOSURE(col)
#define EX_INVDEEXPOSURE(col)
#define EX_POSITION_INPUTS(positionWS,positionCS,vd)
#define EX_POSITION_INPUTS_FUNC
#define EX_POSITION_INPUTS_FUNC_IN

//------------------------------------------------------------------------------------------------------------------------------
// Subpass
#define EX_SUBPASS_OUTPUTS , out float4 outColor : SV_Target0
#define EX_SUBPASS_CUSTOM_COORD(idx)

#if defined(EX_PASS_SHADOWCASTER)
    #define EX_SUBPASS_CUSTOM_TRANSFER(v,o)  o.positionCS = URPShadowPos(v.positionOS, v.normalOS)
#else
    #define EX_SUBPASS_CUSTOM_TRANSFER(v,o)
#endif

#if VERSION_GREATER_EQUAL(10, 1) && defined(EX_PASS_DEPTHNORMALS)
    #define EX_OUTPUT_SUBPASS(i) outColor = float4(PackNormalOctRectEncode(normalize(mul((float3x3)EX_MATRIX_V, i.normalWS.xyz))), 0.0, 0.0)
#elif defined(EX_PASS_META)
    #define EX_OUTPUT_SUBPASS(i) outColor = col
#else
    #define EX_OUTPUT_SUBPASS(i) outColor = 0
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Rendering Layer
#if VERSION_GREATER_EQUAL(12, 0)
    uint EXGetRenderingLayer()
    {
        #if defined(_LIGHT_LAYERS)
            return (asuint(unity_RenderingLayer.x) & RENDERING_LIGHT_LAYERS_MASK) >> RENDERING_LIGHT_LAYERS_MASK_SHIFT;
        #else
            return DEFAULT_LIGHT_LAYERS;
        #endif
    }
#else
    uint EXGetRenderingLayer()
    {
        return 0;
    }
#endif

uint EXGetFeatureFlags()
{
    return 0;
}

//------------------------------------------------------------------------------------------------------------------------------
// Support for old version
#if VERSION_GREATER_EQUAL(12, 0) && defined(_LIGHT_LAYERS)
    #define EX_MAINLIGHT_COLOR ((_MainLightLayerMask & vd.renderingLayers) != 0 ? _MainLightColor.rgb : 0.0)
    #define EX_EV_LIGHT_LAYERS(layerMask,renderingLayers) if((layerMask & renderingLayers) != 0)
#else
    #define EX_MAINLIGHT_COLOR _MainLightColor.rgb
    #define EX_EV_LIGHT_LAYERS(layerMask,renderingLayers)
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Position
float3 EXTransformOStoWS(float4 positionOS)
{
    return TransformObjectToWorld(positionOS.xyz).xyz;
}

float3 EXTransformOStoWS(float3 positionOS)
{
    return mul(EX_MATRIX_M, float4(positionOS,1.0)).xyz;
}

float3 EXTransformWStoOS(float3 positionWS)
{
    return TransformWorldToObject(positionWS).xyz;
}

float3 EXTransformWStoVS(float3 positionWS)
{
    return TransformWorldToView(positionWS).xyz;
}

float4 EXTransformWStoCS(float3 positionWS)
{
    return TransformWorldToHClip(positionWS);
}

float4 EXTransformVStoCS(float3 positionVS)
{
    return TransformWViewToHClip(positionVS);
}

float4 EXTransformCStoSS(float4 positionCS)
{
    float4 positionSS = positionCS * 0.5f;
    positionSS.xy = float2(positionSS.x, positionSS.y * _ProjectionParams.x) + positionSS.w;
    positionSS.zw = positionCS.zw;
    return positionSS;
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
#define EX_APPLY_FOG(col,factor) col.rgb = MixFog(col.rgb, factor)

half EXFog(float depth)
{
    return ComputeFogFactor(depth);
}

//------------------------------------------------------------------------------------------------------------------------------
// Directional Light
#define EX_GET_MAINLIGHT(dir,col,vd,posInput)   dir = _MainLightPosition.xyz; col = EX_MAINLIGHT_COLOR

//------------------------------------------------------------------------------------------------------------------------------
// Shadow
#if defined(_MAIN_LIGHT_SHADOWS_SCREEN)
    #define EX_USE_SHADOWS
    #define EX_SHADOW_COORDS(idx)              float4 _ShadowCoord : TEXCOORD##idx;
    #define EX_TRANSFER_SHADOW(vi,uv,o)        o._ShadowCoord = ComputeScreenPos(vi.positionCS);
    #define EX_LIGHT_ATTENUATION(atten,i) \
        atten = MainLightRealtimeShadow(i._ShadowCoord)
#elif defined(_MAIN_LIGHT_SHADOWS)
    #define EX_USE_SHADOWS
    #define EX_SHADOW_COORDS(idx)              float4 _ShadowCoord : TEXCOORD##idx;
    #define EX_TRANSFER_SHADOW(vi,uv,o)        o._ShadowCoord = TransformWorldToShadowCoord(vi.positionWS.xyz);
    #define EX_LIGHT_ATTENUATION(atten,i) \
        atten = MainLightRealtimeShadow(i._ShadowCoord)
#elif defined(_MAIN_LIGHT_SHADOWS_CASCADE)
    #define EX_USE_SHADOWS
    #define EX_SHADOW_COORDS(idx)
    #define EX_TRANSFER_SHADOW(vi,uv,o)
    #define EX_LIGHT_ATTENUATION(atten,i) \
        float4 _ShadowCoord = TransformWorldToShadowCoord(i.positionWS.xyz); \
        atten = MainLightRealtimeShadow(_ShadowCoord)
#else
    #define EX_SHADOW_COORDS(idx)
    #define EX_TRANSFER_SHADOW(vi,uv,o)
    #define EX_LIGHT_ATTENUATION(atten,i)
#endif

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
#if defined(_ADDITIONAL_LIGHTS) || defined(_ADDITIONAL_LIGHTS_VERTEX)
    #define EX_USE_ADDLIGHT
    #define EX_ADDLIGHT_COORDS(idx) half3 additionalLight : TEXCOORD##idx;
    #define EX_GET_ADDLIGHT(o) o = EXGetAdditionalLights(positionWS, positionCS, vd.renderingLayers, vd.featureFlags)
    #define EX_COPY_ADDLIGHT(i,ld) ld.additionalLight = i.additionalLight
#else
    #define EX_ADDLIGHT_COORDS(idx)
    #define EX_GET_ADDLIGHT(o) o = 0
    #define EX_COPY_ADDLIGHT(i,ld) ld.additionalLight = 0
#endif

half3 EXGetAdditionalLights(float3 positionWS, float4 positionCS, uint renderingLayers, uint featureFlags)
{
    half3 additionalLightColor = 0.0;

    #if defined(_ADDITIONAL_LIGHTS) || defined(_ADDITIONAL_LIGHTS_VERTEX)
        uint lightsCount = GetAdditionalLightsCount();
        #if defined(USE_CLUSTERED_LIGHTING) && USE_CLUSTERED_LIGHTING
            ClusteredLightLoop cll = ClusteredLightLoopInit(GetNormalizedScreenSpaceUV(positionCS), positionWS);
            while(ClusteredLightLoopNextWord(cll))
            {
                while(ClusteredLightLoopNextLight(cll))
                {
                    uint lightIndex = ClusteredLightLoopGetLightIndex(cll);
        #elif defined(_USE_WEBGL1_LIGHTS) && _USE_WEBGL1_LIGHTS
            for(uint lightIndex = 0; lightIndex < _WEBGL1_MAX_LIGHTS; lightIndex++)
            {
                if(lightIndex >= lightsCount) break;
        #else
            for(uint lightIndex = 0; lightIndex < lightsCount; lightIndex++)
            {
        #endif

            Light light = GetAdditionalLight(lightIndex, positionWS);
            EX_EV_LIGHT_LAYERS(light.layerMask, renderingLayers)
            additionalLightColor += light.color * light.distanceAttenuation;
        }

        #if defined(USE_CLUSTERED_LIGHTING) && USE_CLUSTERED_LIGHTING
            }
        #endif
    #endif

    #if defined(_ADDITIONAL_LIGHTS) && defined(USE_CLUSTERED_LIGHTING) && USE_CLUSTERED_LIGHTING
        for(uint lightIndex = 0; lightIndex < min(_AdditionalLightsDirectionalCount, MAX_VISIBLE_LIGHTS); lightIndex++)
        {
            Light light = GetAdditionalLight(lightIndex, positionWS);
            EX_EV_LIGHT_LAYERS(light.layerMask, renderingLayers)
            additionalLightColor += light.color * light.distanceAttenuation;
        }
    #endif

    return additionalLightColor;
}

//------------------------------------------------------------------------------------------------------------------------------
// Shadow Caster
float3 _LightDirection;
float3 _LightPosition;
float4 URPShadowPos(float4 positionOS, float3 normalOS)
{
    float3 positionWS = TransformObjectToWorld(positionOS.xyz);
    float3 normalWS = TransformObjectToWorldNormal(normalOS);

    #if defined(_CASTING_PUNCTUAL_LIGHT_SHADOW) && _CASTING_PUNCTUAL_LIGHT_SHADOW
        float3 lightDirectionWS = normalize(_LightPosition - positionWS);
    #else
        float3 lightDirectionWS = _LightDirection;
    #endif

    float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, lightDirectionWS));

    #if UNITY_REVERSED_Z
        positionCS.z = min(positionCS.z, UNITY_NEAR_CLIP_VALUE);
    #else
        positionCS.z = max(positionCS.z, UNITY_NEAR_CLIP_VALUE);
    #endif

    return positionCS;
}