//------------------------------------------------------------------------------------------------------------------------------
// Normal
half3 EXTransformDirOStoWS(half3 directionOS)
{
    return mul((half3x3)EX_MATRIX_M, directionOS);
}

half3 EXTransformDirWStoOS(half3 directionWS)
{
    return mul((half3x3)EX_MATRIX_I_M, directionWS);
}

half3 EXTransformNormalOStoWS(half3 normalOS)
{
    #ifdef UNITY_ASSUME_UNIFORM_SCALING
        return EXTransformDirOStoWS(normalOS);
    #else
        return mul(normalOS, (half3x3)EX_MATRIX_I_M);
    #endif
}

float3 EXViewDirection(float3 positionWS)
{
    return _WorldSpaceCameraPos.xyz - positionWS;
}

float3 EXHeadDirection(float3 positionWS)
{
    #if defined(USING_STEREO_MATRICES)
        return (unity_StereoWorldSpaceCameraPos[0] + unity_StereoWorldSpaceCameraPos[1]) * 0.5 - positionWS;
    #else
        return EXViewDirection(positionWS);
    #endif
}

//------------------------------------------------------------------------------------------------------------------------------
// Math
float EXMedian(float r, float g, float b)
{
    return max(min(r, g), min(max(r, g), b));
}

float EXRemap(float val, float scale, float offset)
{
    return saturate(val * scale + offset);
}

float2 EXCalcUV(float2 uv, float4 st)
{
    return uv * st.xy + st.zw;
}

float2 EXCalcUV(float2 uv, float4 st, float2 sc)
{
    return uv * st.xy + st.zw + _Time.y * sc;
}

float3 EXOrthoNormalize(float3 tangent, float3 normal)
{
    return normalize(tangent - normal * dot(normal, tangent));
}

half3 EXUnpackNormalScale(half4 normalTex, half scale)
{
    half3 normal;
    #if defined(UNITY_NO_DXT5nm)
        normal = normalTex.rgb * 2.0 - 1.0;
        normal.xy *= scale;
    #else
        #if !defined(UNITY_ASTC_NORMALMAP_ENCODING)
            normalTex.a *= normalTex.r;
        #endif
        normal.xy = normalTex.ag * 2.0 - 1.0;
        normal.xy *= scale;
        normal.z = sqrt(1.0 - saturate(dot(normal.xy, normal.xy)));
    #endif
    return normal;
}

float3 EXBlendNormal(float3 dstNormal, float3 srcNormal)
{
    return float3(dstNormal.xy + srcNormal.xy, dstNormal.z * srcNormal.z);
}

float2 EXParallax(half3x3 TBN, half3 V)
{
    float3 parallaxV = mul(TBN, V);
    return parallaxV.xy / (parallaxV.z + 0.5);
}

//------------------------------------------------------------------------------------------------------------------------------
// Color
float EXLuminance(float3 rgb)
{
    return dot(rgb, float3(0.22, 0.707, 0.071));
}

float EXGray(float3 rgb)
{
    return dot(rgb, float3(1.0/3.0, 1.0/3.0, 1.0/3.0));
}

// http://chilliant.blogspot.com/2012/08/srgb-approximations-for-hlsl.html?m=1
float3 EXLinearToSRGB(float3 col)
{
    return saturate(1.055 * pow(abs(col), 0.416666667) - 0.055);
}

float3 EXSRGBToLinear(float3 col)
{
    return col * (col * (col * 0.305306011 + 0.682171111) + 0.012522878);
}