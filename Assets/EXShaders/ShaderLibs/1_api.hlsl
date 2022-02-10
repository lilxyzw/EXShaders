//------------------------------------------------------------------------------------------------------------------------------
// API Macro
#if defined(SHADER_API_D3D11) || defined(SHADER_API_METAL) || defined(SHADER_API_SWITCH) || defined(SHADER_API_VULKAN) || defined(SHADER_API_XBOXONE) || defined(SHADER_API_PS4) || defined(SHADER_API_PS5) || defined(SHADER_API_GAMECORE)
    #define EX_VFACE(facing) , bool facing : SV_IsFrontFace
    #define EX_VFACE_COMP(facing) facing
#elif defined(SHADER_API_GLCORE) || defined(SHADER_API_GLES) || defined(SHADER_API_GLES3)
    #define EX_VFACE(facing) , float facing : VFACE
    #define EX_VFACE_COMP(facing) facing > 0.0
#else // defined(SHADER_API_D3D11_9X)
    #define EX_VFACE(facing)
    #define EX_VFACE_COMP(facing) true
#endif

#if defined(ZERO_INITIALIZE)
    #define EX_INIT_STRUCT(type, o) ZERO_INITIALIZE(type, o)
#elif defined(UNITY_INITIALIZE_OUTPUT)
    #define EX_INIT_STRUCT(type, o) UNITY_INITIALIZE_OUTPUT(type, o)
#else
    #define EX_INIT_STRUCT(type, o)
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
#if defined(SAMPLE3D)
    #undef SAMPLE3D
#endif
#if defined(TEXTURE2D)
    #undef TEXTURE2D
#endif
#if defined(TEXTURE3D)
    #undef TEXTURE3D
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

#if defined(SHADER_API_D3D9) || defined(SHADER_API_GLES) || (defined(SHADER_TARGET_SURFACE_ANALYSIS) && defined(SHADER_TARGET_SURFACE_ANALYSIS_MOJOSHADER)) || defined(SHADER_TARGET_SURFACE_ANALYSIS)
    #define SAMPLE2D(tex,samp,uv)           tex2D(tex,uv)
    #define SAMPLE2DLOD(tex,samp,uv,lod)    tex2Dlod(tex,float4(uv,0,lod))
    #define SAMPLE3D(tex,samp,uv)           tex3D(tex,uv)
    #define TEXTURE2D(tex)                  sampler2D tex
    #define TEXTURE3D(tex)                  sampler3D tex
    #define SAMPLER(samp)
    #define SAMPLER_IN_FUNC(samp)
    #define SAMPLER_IN(samp)
#else
    #define SAMPLE2D(tex,samp,uv)           tex.Sample(samp,uv)
    #define SAMPLE2D_LOD(tex,samp,uv,lod)   tex.SampleLevel(samp,uv,lod)
    #define SAMPLE3D(tex,samp,uv)           tex.Sample(samp,uv)
    #define TEXTURE2D(tex)                  Texture2D tex
    #define TEXTURE3D(tex)                  Texture3D tex
    #define SAMPLER(samp)                   SamplerState samp
    #define SAMPLER_IN_FUNC(samp)           , SamplerState samp
    #define SAMPLER_IN(samp)                , samp
#endif