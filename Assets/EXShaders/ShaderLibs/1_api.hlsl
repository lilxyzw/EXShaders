//------------------------------------------------------------------------------------------------------------------------------
// API Macro
#if defined(SHADER_API_D3D11_9X)
    #define EX_VFACE(facing)
    #define EX_VFACE_FALLBACK(facing) float facing = 1
#else
    #define EX_VFACE(facing) , float facing : VFACE
    #define EX_VFACE_FALLBACK(facing)
#endif

#if defined(UNITY_INITIALIZE_OUTPUT)
    #define EX_INIT_STRUCT(type, o)  UNITY_INITIALIZE_OUTPUT(type, o)
#else
    #define EX_INIT_STRUCT(type, o)  o = (type)0
#endif

#if !defined(CBUFFER_START)
    #define CBUFFER_START()
#endif

#if !defined(CBUFFER_END)
    #define CBUFFER_END
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Texture
#if defined(SAMPLE2D)
    #undef SAMPLE2D
#endif
#if defined(SAMPLE2DLOD)
    #undef SAMPLE2DLOD
#endif
#if defined(TEXTURE2D)
    #undef TEXTURE2D
#endif
#if defined(SAMPLER)
    #undef SAMPLER
#endif
#if defined(SAMPLER_IN_FUNC)
    #undef SAMPLER_IN_FUNC
#endif
#if defined(SAMPLER_IN)
    #undef SAMPLER_IN
#endif

#if defined(SHADER_API_D3D9) || (UNITY_VERSION < 201800 && defined(SHADER_API_GLES)) || (defined(SHADER_TARGET_SURFACE_ANALYSIS) && defined(SHADER_TARGET_SURFACE_ANALYSIS_MOJOSHADER)) || defined(SHADER_TARGET_SURFACE_ANALYSIS)
    #define SAMPLE2D(tex,samp,uv)           tex2D(tex,uv)
    #define SAMPLE2DLOD(tex,samp,uv,lod)    tex2Dlod(tex,float4(uv,0,lod))
    #define TEXTURE2D(tex)                  sampler2D tex
    #define SAMPLER(samp)
    #define SAMPLER_IN_FUNC(samp)
    #define SAMPLER_IN(samp)
#else
    #define SAMPLE2D(tex,samp,uv)           tex.Sample(samp,uv)
    #define SAMPLE2D_LOD(tex,samp,uv,lod)   tex.SampleLevel(samp,uv,lod)
    #define TEXTURE2D(tex)                  Texture2D tex
    #define SAMPLER(samp)                   SamplerState samp
    #define SAMPLER_IN_FUNC(samp)           , SamplerState samp
    #define SAMPLER_IN(samp)                , samp
#endif