//------------------------------------------------------------------------------------------------------------------------------
// Libraries
#if !defined(LIGHTLOOP_DISABLE_TILE_AND_CLUSTER)
    #define LIGHTLOOP_DISABLE_TILE_AND_CLUSTER
#endif
#if !defined(SHADOW_LOW) && !defined(SHADOW_MEDIUM) && !defined(SHADOW_HIGH) && !defined(SHADOW_VERY_HIGH)
    #define SHADOW_LOW
#endif
#if !defined(USE_FPTL_LIGHTLIST) && !defined(USE_CLUSTERED_LIGHTLIST)
    #define USE_FPTL_LIGHTLIST
#endif
#if defined(SHADOW_LOW)
    #define PUNCTUAL_SHADOW_LOW
    #define DIRECTIONAL_SHADOW_LOW
#elif defined(SHADOW_MEDIUM)
    #define PUNCTUAL_SHADOW_MEDIUM
    #define DIRECTIONAL_SHADOW_MEDIUM
#elif defined(SHADOW_HIGH)
    #define PUNCTUAL_SHADOW_HIGH
    #define DIRECTIONAL_SHADOW_HIGH
#endif

#include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Lighting/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Lighting/LightLoop/LightLoopDef.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Lighting/LightEvaluation.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialEvaluation.hlsl"

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
#define EX_HDRP
#if defined(SHADER_STAGE_RAY_TRACING)
    #define EX_MATRIX_M        ObjectToWorld3x4()
    #define EX_MATRIX_I_M      WorldToObject3x4()
#else
    #define EX_MATRIX_M        GetObjectToWorldMatrix()
    #define EX_MATRIX_I_M      GetWorldToObjectMatrix()
#endif
#define EX_MATRIX_V        GetWorldToViewMatrix()
#define EX_MATRIX_VP       GetWorldToHClipMatrix()
#define EX_MATRIX_P        GetViewToHClipMatrix()
#define EX_NEGATIVE_SCALE  GetOddNegativeScale()

#if VERSION_LOWER(11, 0)
    #define EX_DEEXPOSURE(col)
    #define EX_INVDEEXPOSURE(col)
#else
    #define EX_DEEXPOSURE(col) col.rgb *= _DeExposureMultiplier
    #define EX_INVDEEXPOSURE(col) col.rgb /= _DeExposureMultiplier
#endif

#define EX_POSITION_INPUTS(positionWS,positionCS,vd) PositionInputs posInput = GetPositionInput(positionCS.xy, _ScreenSize.zw, positionCS.z, positionCS.w, positionWS.xyz, vd.tileIndex)
#define EX_POSITION_INPUTS_FUNC , PositionInputs posInput
#define EX_POSITION_INPUTS_FUNC_IN , posInput

//------------------------------------------------------------------------------------------------------------------------------
// Subpass
#if defined(WRITE_MSAA_DEPTH)
    #define EX_MSAA_OUT(i) \
        depthColor = i.positionCS.z; \
        depthColor.a = 1.0;
#else
    #define EX_MSAA_OUT(i)
#endif

#if defined(WRITE_NORMAL_BUFFER)
    #define EX_NORMAL_OUT(i) \
        float3 N = vd.N; \
        N.z = CopySign(max(1.0 / 1024.0, abs(N.z)), N.z); \
        float2 octNormalWS = PackNormalOctQuadEncode(N); \
        float3 packNormalWS = PackFloat2To888(saturate(octNormalWS * 0.5 + 0.5)); \
        outNormalBuffer = float4(packNormalWS, 1.0);
#else
    #define EX_NORMAL_OUT(i)
#endif

#if defined(EX_PASS_DEPTHONLY) || defined(EX_PASS_SCENESELECTION)
    // DepthOnly
    #if defined(SCENESELECTIONPASS) || defined(SCENEPICKINGPASS)
        #define EX_SUBPASS_OUTPUTS , out float4 outColor : SV_Target0
    #else
        #if defined(WRITE_MSAA_DEPTH) && defined(WRITE_NORMAL_BUFFER)
            #define EX_SUBPASS_OUTPUTS , out float4 depthColor : SV_Target0, out float4 outNormalBuffer : SV_Target1
        #elif defined(WRITE_MSAA_DEPTH)
            #define EX_SUBPASS_OUTPUTS , out float4 depthColor : SV_Target0
        #elif defined(WRITE_NORMAL_BUFFER)
            #define EX_SUBPASS_OUTPUTS , out float4 outNormalBuffer : SV_Target0
        #else
            #define EX_SUBPASS_OUTPUTS
        #endif
    #endif
    #define EX_SUBPASS_CUSTOM_COORD(idx)
    #if defined(UNITY_REVERSED_Z)
        // DirectX
        #define EX_SUBPASS_CUSTOM_TRANSFER(i,o) o.positionCS.z -= 0.0001;
    #else
        // OpenGL
        #define EX_SUBPASS_CUSTOM_TRANSFER(i,o) o.positionCS.z += 0.0001;
    #endif

    #if defined(SCENESELECTIONPASS)
        int _ObjectId;
        int _PassValue;
        #define EX_OUTPUT_SUBPASS(i) outColor = float4(_ObjectId, _PassValue, 1.0, 1.0);
    #elif defined(SCENEPICKINGPASS)
        #define EX_OUTPUT_SUBPASS(i) outColor = _SelectionID;
    #else
        #define EX_OUTPUT_SUBPASS(i) EX_MSAA_OUT(i) EX_NORMAL_OUT(i)
    #endif
#elif defined(EX_PASS_MOTIONVECTORS)
    // MotionVectors
    #if defined(WRITE_MSAA_DEPTH) && defined(WRITE_NORMAL_BUFFER)
        #define EX_SUBPASS_OUTPUTS , out float4 depthColor : SV_Target0, out float4 outMotionVector : SV_Target1, out float4 outNormalBuffer : SV_Target2
    #elif defined(WRITE_MSAA_DEPTH)
        #define EX_SUBPASS_OUTPUTS , out float4 depthColor : SV_Target0, out float4 outMotionVector : SV_Target1
    #elif defined(WRITE_NORMAL_BUFFER)
        #define EX_SUBPASS_OUTPUTS , out float4 outMotionVector : SV_Target0, out float4 outNormalBuffer : SV_Target1
    #else
        #define EX_SUBPASS_OUTPUTS , out float4 outMotionVector : SV_Target0
    #endif
    #define EX_SUBPASS_CUSTOM_COORD(idx) float4 previousPositionCS : TEXCOORD##idx;
    #define EX_SUBPASS_CUSTOM_TRANSFER(i,o) \
        i.previousPositionOS = unity_MotionVectorsParams.x > 0.0 ? i.previousPositionOS : i.positionOS.xyz; \
        float3 previousPositionWS = TransformPreviousObjectToWorld(i.previousPositionOS); \
        o.previousPositionCS = mul(UNITY_MATRIX_PREV_VP, float4(previousPositionWS, 1.0))

    #define EX_OUTPUT_SUBPASS(i) \
        float2 motionVector = EXCalculateMotionVector(i.positionCS, i.previousPositionCS); \
        outMotionVector = float4(motionVector * 0.5, 0.0, 0.0); \
        bool forceNoMotion = unity_MotionVectorsParams.y == 0.0; \
        if(forceNoMotion) outMotionVector = float4(2.0, 0.0, 0.0, 0.0); \
        EX_MSAA_OUT(i) \
        EX_NORMAL_OUT(i)

    float2 EXCalculateMotionVector(float4 positionCS, float4 previousPositionCS)
    {
        positionCS.xy = positionCS.xy / _ScreenParams.xy * 2.0 - 1.0;
        #if UNITY_UV_STARTS_AT_TOP
            positionCS.y = -positionCS.y;
        #endif
        previousPositionCS.xy = previousPositionCS.xy / previousPositionCS.w;
        float2 motionVec = (positionCS.xy - previousPositionCS.xy);

        float2 microThreshold = 0.01f * _ScreenSize.zw;
        motionVec.x = abs(motionVec.x) < microThreshold.x ? 0 : motionVec.x;
        motionVec.y = abs(motionVec.y) < microThreshold.y ? 0 : motionVec.y;

        motionVec = clamp(motionVec, -1.0f + microThreshold, 1.0f - microThreshold);

        #if UNITY_UV_STARTS_AT_TOP
            motionVec.y = -motionVec.y;
        #endif
        return motionVec;
    }
#elif defined(EX_PASS_META)
    // Meta
    #define EX_SUBPASS_OUTPUTS , out float4 outColor : SV_Target0
    #define EX_SUBPASS_CUSTOM_COORD(idx)
    #define EX_SUBPASS_CUSTOM_TRANSFER(i,o)
    #define EX_OUTPUT_SUBPASS(i) outColor = col
#else
    #define EX_SUBPASS_OUTPUTS , out float4 outColor : SV_Target0
    #define EX_SUBPASS_CUSTOM_COORD(idx)
    #define EX_SUBPASS_CUSTOM_TRANSFER(i,o)
    #define EX_OUTPUT_SUBPASS(i) outColor = 0
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Rendering Layer
uint EXGetRenderingLayer()
{
    #if defined(RENDERING_LIGHT_LAYERS_MASK)
        return _EnableLightLayers ? (asuint(unity_RenderingLayer.x) & RENDERING_LIGHT_LAYERS_MASK) >> RENDERING_LIGHT_LAYERS_MASK_SHIFT : DEFAULT_LIGHT_LAYERS;
    #else
        return _EnableLightLayers ? asuint(unity_RenderingLayer.x) : DEFAULT_LIGHT_LAYERS;
    #endif
}

uint EXGetFeatureFlags()
{
    return LIGHT_FEATURE_MASK_FLAGS_OPAQUE;
}

//------------------------------------------------------------------------------------------------------------------------------
// Support for old version
#if VERSION_LOWER(7, 0)
    half3 UnpackNormalScale(half4 packedNormal, half bumpScale)
    {
        #if defined(UNITY_NO_DXT5nm)
            return UnpackNormalRGB(packedNormal, bumpScale);
        #else
            return UnpackNormalmapRGorAG(packedNormal, bumpScale);
        #endif
    }
#endif

#if VERSION_LOWER(7, 1)
    float3 TransformPreviousObjectToWorld(float3 positionOS)
    {
        float4x4 previousModelMatrix = ApplyCameraTranslationToMatrix(unity_MatrixPreviousM);
        return mul(previousModelMatrix, float4(positionOS, 1.0)).xyz;
    }
#endif

#if VERSION_LOWER(7, 2)
    #define EX_EV_COOKIE(light) if(light.cookieIndex >= 0)
#else
    #define EX_EV_COOKIE(light) if(light.cookieMode != COOKIEMODE_NONE)
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
    return GetAbsolutePositionWS(positionRWS);
}

//------------------------------------------------------------------------------------------------------------------------------
// Fog
#define EX_APPLY_FOG(col,factor) col = EvaluateAtmosphericScattering(posInput, vd.V, col);

half EXFog(float depth)
{
    return 0.0;
}

//------------------------------------------------------------------------------------------------------------------------------
// Direction Light
#define LIGHT_SIMULATE_HQ
#define EX_GET_MAINLIGHT(dir,col,vd,posInput) EXGetLightDirectionAndColor(dir, col, vd.renderingLayers, vd.featureFlags, posInput)

struct EXLightingData
{
    half3 color;
    half3 direction;
};

LightLoopContext EXInitLightLoopContext()
{
    LightLoopContext lightLoopContext;
    lightLoopContext.shadowContext    = InitShadowContext();
    lightLoopContext.shadowValue      = 1;
    lightLoopContext.sampleReflection = 0;
    lightLoopContext.contactShadow    = 0;
    return lightLoopContext;
}

EXLightingData EXGetNPRDirectionalLight(PositionInputs posInput, DirectionalLightData light)
{
    EXLightingData lighting = (EXLightingData)0;
    half3 L = -light.forward;
    #if !defined(LIL_HDRP_IGNORE_LIGHTDIMMER)
    if(light.lightDimmer > 0)
    #endif
    {
        LightLoopContext lightLoopContext = EXInitLightLoopContext();
        half4 lightColor = EvaluateLight_Directional(lightLoopContext, posInput, light);
        lightColor.rgb *= lightColor.a;

        lighting.direction = L;
        lighting.color = lightColor.rgb;
    }
    return lighting;
}

EXLightingData EXGetDirectionalLightSum(PositionInputs posInput, uint renderingLayers, uint featureFlags)
{
    EXLightingData lightingData;
    lightingData.color = 0.0;
    lightingData.direction = half3(0.0, 0.001, 0.0);
    if(featureFlags & LIGHTFEATUREFLAGS_DIRECTIONAL)
    {
        for(uint i = 0; i < _DirectionalLightCount; ++i)
        {
            if((_DirectionalLightDatas[i].lightLayers & renderingLayers) != 0)
            {
                EXLightingData lighting = EXGetNPRDirectionalLight(posInput, _DirectionalLightDatas[i]);
                lightingData.color += lighting.color;
                lightingData.direction += lighting.direction * Luminance(lighting.color);
            }
        }
    }

    lightingData.direction = normalize(lightingData.direction);

    #ifdef LIGHT_SIMULATE_HQ
        lightingData.color = 0.0;
        if(featureFlags & LIGHTFEATUREFLAGS_DIRECTIONAL)
        {
            for(uint i = 0; i < _DirectionalLightCount; ++i)
            {
                if((_DirectionalLightDatas[i].lightLayers & renderingLayers) != 0)
                {
                    EXLightingData lighting = EXGetNPRDirectionalLight(posInput, _DirectionalLightDatas[i]);
                    lightingData.color += lighting.color * saturate(dot(lightingData.direction, lighting.direction));
                }
            }
        }
    #endif

    return lightingData;
}

void EXGetLightDirectionAndColor(out half3 lightDirection, out half3 lightColor, uint renderingLayers, uint featureFlags, PositionInputs posInput)
{
    EXLightingData lightingData = EXGetDirectionalLightSum(posInput, renderingLayers, featureFlags);
    lightDirection = lightingData.direction;
    lightColor = lightingData.color * GetCurrentExposureMultiplier();
}

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
    return sh * GetCurrentExposureMultiplier();
}

half3 EXGetEnvironmentDirection()
{
    half3 lightDirection = unity_SHAr.xyz + unity_SHAg.xyz + unity_SHAb.xyz;
    return lightDirection * GetCurrentExposureMultiplier();
}

//------------------------------------------------------------------------------------------------------------------------------
// Punctual Light (Point / Spot)
half4 EvaluateLight_Punctual(LightLoopContext lightLoopContext, float3 positionWS, LightData light, half3 L, float4 distances)
{
    half4 color = half4(light.color, 1.0);
    color.a *= PunctualLightAttenuation(distances, light.rangeAttenuationScale, light.rangeAttenuationBias, light.angleScale, light.angleOffset);

    #if !defined(LIGHT_EVALUATION_NO_HEIGHT_FOG)
        float cosZenithAngle = L.y;
        float distToLight = (light.lightType == GPULIGHTTYPE_PROJECTOR_BOX) ? distances.w : distances.x;
        float fragmentHeight = positionWS.y;
        color.a *= TransmittanceHeightFog(_HeightFogBaseExtinction, _HeightFogBaseHeight, _HeightFogExponents, cosZenithAngle, fragmentHeight, distToLight);
    #endif

    EX_EV_COOKIE(light)
    {
        float3 lightToSample = positionWS - light.positionRWS;
        half4 cookie = EvaluateCookie_Punctual(lightLoopContext, light, lightToSample);
        color *= cookie;
    }

    return color;
}

EXLightingData EXGetNPRPunctualLight(float3 positionWS, LightData light)
{
    EXLightingData lighting = (EXLightingData)0;
    half3 L;
    float4 distances;
    GetPunctualLightVectors(positionWS, light, L, distances);
    #if !defined(LIL_HDRP_IGNORE_LIGHTDIMMER)
    if(light.lightDimmer > 0)
    #endif
    {
        LightLoopContext lightLoopContext;
        lightLoopContext.shadowContext    = InitShadowContext();
        lightLoopContext.shadowValue      = 1;
        lightLoopContext.sampleReflection = 0;
        lightLoopContext.contactShadow    = 0;

        half4 lightColor = EvaluateLight_Punctual(lightLoopContext, positionWS, light, L, distances);
        #if !defined(LIL_HDRP_IGNORE_LIGHTDIMMER)
            lightColor.a *= light.diffuseDimmer;
        #endif
        lightColor.rgb *= lightColor.a;

        lighting.direction = L;
        lighting.color = lightColor.rgb;
    }
    return lighting;
}

half3 EXGetPunctualLightColor(float3 positionWS, uint renderingLayers, uint featureFlags)
{
    half3 lightColor = 0.0;
    if(featureFlags & LIGHTFEATUREFLAGS_PUNCTUAL)
    {
        uint lightStart = 0;
        bool fastPath = false;
        #if SCALARIZE_LIGHT_LOOP
            uint lightStartLane0;
            fastPath = IsFastPath(lightStart, lightStartLane0);
            if(fastPath) lightStart = lightStartLane0;
        #endif

        uint lightListOffset = 0;
        while(lightListOffset < _PunctualLightCount)
        {
            uint v_lightIdx = FetchIndex(lightStart, lightListOffset);
            #if SCALARIZE_LIGHT_LOOP
                uint s_lightIdx = ScalarizeElementIndex(v_lightIdx, fastPath);
            #else
                uint s_lightIdx = v_lightIdx;
            #endif
            if(s_lightIdx == -1) break;

            LightData lightData = FetchLight(s_lightIdx, 0);

            if(s_lightIdx >= v_lightIdx)
            {
                lightListOffset++;
                if((lightData.lightLayers & renderingLayers) != 0)
                {
                    EXLightingData lighting = EXGetNPRPunctualLight(positionWS, lightData);
                    lightColor += lighting.color;
                }
            }
        }
    }

    return lightColor;
}

//------------------------------------------------------------------------------------------------------------------------------
// Area Light (Line / Rectangle)
half3 EXGetLineLightColor(float3 positionWS, LightData lightData)
{
    half3 lightColor = 0.0;
    half intensity = EllipsoidalDistanceAttenuation(
        lightData.positionRWS - positionWS,
        lightData.right,
        saturate(lightData.range / (lightData.range + (0.5 * lightData.size.x))),
        lightData.rangeAttenuationScale,
        lightData.rangeAttenuationBias);
        #if !defined(LIL_HDRP_IGNORE_LIGHTDIMMER)
            intensity *= lightData.diffuseDimmer;
        #endif
    lightColor = lightData.color * intensity;
    return lightColor;
}

half3 EXGetRectLightColor(float3 positionWS, LightData lightData)
{
    half3 lightColor = 0.0;
    #if SHADEROPTIONS_BARN_DOOR
        RectangularLightApplyBarnDoor(lightData, positionWS);
    #endif
    float3 unL = lightData.positionRWS - positionWS;
    if(dot(lightData.forward, unL) < FLT_EPS)
    {
        float3x3 lightToWorld = float3x3(lightData.right, lightData.up, -lightData.forward);
        unL = mul(unL, transpose(lightToWorld));
        float halfWidth  = lightData.size.x * 0.5;
        float halfHeight = lightData.size.y * 0.5;
        float3 invHalfDim = rcp(float3(lightData.range + halfWidth, lightData.range + halfHeight, lightData.range));
        #ifdef ELLIPSOIDAL_ATTENUATION
            half intensity = EllipsoidalDistanceAttenuation(unL, invHalfDim, lightData.rangeAttenuationScale, lightData.rangeAttenuationBias);
        #else
            half intensity = BoxDistanceAttenuation(unL, invHalfDim, lightData.rangeAttenuationScale, lightData.rangeAttenuationBias);
        #endif
        #if !defined(LIL_HDRP_IGNORE_LIGHTDIMMER)
            intensity *= lightData.diffuseDimmer;
        #endif
        lightColor = lightData.color * intensity;
    }
    return lightColor;
}

half3 EXGetAreaLightColor(float3 positionWS, uint renderingLayers, uint featureFlags)
{
    half3 lightColor = 0.0;
    #if SHADEROPTIONS_AREA_LIGHTS
        if(featureFlags & LIGHTFEATUREFLAGS_AREA)
        {
            if(_AreaLightCount > 0)
            {
                uint i = 0;
                uint last = _AreaLightCount - 1;
                LightData lightData = FetchLight(_PunctualLightCount, i);

                while(i <= last && lightData.lightType == GPULIGHTTYPE_TUBE)
                {
                    lightData.lightType = GPULIGHTTYPE_TUBE;
                    #if defined(COOKIEMODE_NONE)
                        lightData.cookieMode = COOKIEMODE_NONE;
                    #endif
                    if((lightData.lightLayers & renderingLayers) != 0)
                    {
                        lightColor += EXGetLineLightColor(positionWS, lightData);
                    }
                    lightData = FetchLight(_PunctualLightCount, min(++i, last));
                }

                while(i <= last)
                {
                    lightData.lightType = GPULIGHTTYPE_RECTANGLE;
                    if((lightData.lightLayers & renderingLayers) != 0)
                    {
                        lightColor += EXGetRectLightColor(positionWS, lightData);
                    }
                    lightData = FetchLight(_PunctualLightCount, min(++i, last));
                }
            }
        }
    #endif
    return lightColor;
}

//------------------------------------------------------------------------------------------------------------------------------
// Additional Light
#if defined(EX_PASS_FORWARD)
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
    additionalLightColor += EXGetPunctualLightColor(positionWS, renderingLayers, featureFlags);
    additionalLightColor += EXGetAreaLightColor(positionWS, renderingLayers, featureFlags);
    additionalLightColor *= 0.75 * GetCurrentExposureMultiplier();

    return additionalLightColor;
}

//------------------------------------------------------------------------------------------------------------------------------
// Shadow
#if defined(EX_PASS_FORWARD)
    #define EX_USE_SHADOWS
    #define EX_SHADOW_COORDS(idx)
    #define EX_TRANSFER_SHADOW(vi,uv,o)
    #define EX_LIGHT_ATTENUATION(atten,i) \
        atten = EXGetDirectionalShadow(posInput, i.normalWS.xyz, vd.featureFlags)
#else
    #define EX_SHADOW_COORDS(idx)
    #define EX_TRANSFER_SHADOW(vi,uv,o)
    #define EX_LIGHT_ATTENUATION(atten,i)
#endif

bool EXUseScreenSpaceShadow(int screenSpaceShadowIndex)
{
    #if defined(SCREEN_SPACE_SHADOW_INDEX_MASK) && defined(INVALID_SCREEN_SPACE_SHADOW)
        return (screenSpaceShadowIndex & SCREEN_SPACE_SHADOW_INDEX_MASK) != INVALID_SCREEN_SPACE_SHADOW;
    #else
        return screenSpaceShadowIndex >= 0;
    #endif
}

half EXGetDirectionalShadow(PositionInputs posInput, half3 normalWS, uint featureFlags)
{
    half attenuation = 1.0;
    if(featureFlags & LIGHTFEATUREFLAGS_DIRECTIONAL)
    {
        HDShadowContext shadowContext = InitShadowContext();
        if(_DirectionalShadowIndex >= 0)
        {
            DirectionalLightData light = _DirectionalLightDatas[_DirectionalShadowIndex];
            #if defined(SCREEN_SPACE_SHADOWS_ON)
            if(EXUseScreenSpaceShadow(light.screenSpaceShadowIndex))
            {
                attenuation = GetScreenSpaceShadow(posInput, light.screenSpaceShadowIndex);
            }
            else
            #endif
            {
                half3 L = -light.forward;
                #if !defined(LIL_HDRP_IGNORE_LIGHTDIMMER)
                if((light.lightDimmer > 0) && (light.shadowDimmer > 0))
                #endif
                {
                    attenuation = GetDirectionalShadowAttenuation(shadowContext, posInput.positionSS, posInput.positionWS, normalWS, light.shadowIndex, L);
                }
            }
        }
    }
    return attenuation;
}