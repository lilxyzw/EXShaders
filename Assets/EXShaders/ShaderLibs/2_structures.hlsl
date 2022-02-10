//------------------------------------------------------------------------------------------------------------------------------
// Structs

// Inputs of fragment shader
struct EXVertexDatas
{
    float3 positionOS;
    float3 positionWS;
    float3 positionVS;
    float4 positionCS;
    float depth;
    half fogFactor;

    float2 uv0;
    float2 uv1;
    float2 uvScn;

    half3 normalWS;
    half3 tangentWS;
    half3 bitangentWS;
    half3x3 tbnWS;
    half3 N;
    half3 V;
    float2 parallax;

    bool facing;

    uint renderingLayers;
    uint featureFlags;
    uint2 tileIndex;
};

// Lighting datas
struct EXLightDatas
{
    half3 L;
    half3 col;
    half3 additionalLight;
    half attenuation;
};