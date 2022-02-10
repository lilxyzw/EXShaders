using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEditor;
#if UNITY_2020_2_OR_NEWER
    using UnityEditor.AssetImporters;
#else
    using UnityEditor.Experimental.AssetImporters;
#endif

namespace EXShaders
{
    [ScriptedImporter(0, "excontainer")]
    public class EXShaderContainerImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            EXShaderContainer container = JsonUtility.FromJson<EXShaderContainer>(File.ReadAllText(ctx.assetPath));
            Shader shader = ShaderUtil.CreateShaderAsset(ctx, container.UnpackContainer(ctx.assetPath, ctx), false);

            ctx.AddObjectToAsset("main obj", shader);
            ctx.SetMainObject(shader);
        }
    }

    [CustomEditor(typeof(EXShaderContainerImporter))]
    public class EXShaderContainerImporterEditor : ScriptedImporterEditor
    {
        public override void OnInspectorGUI()
        {
            if(GUILayout.Button("Export Shader"))
            {
                string assetPath = AssetDatabase.GetAssetPath(target);
                EXShaderContainer container = JsonUtility.FromJson<EXShaderContainer>(File.ReadAllText(assetPath));
                string shaderText = container.UnpackContainer(assetPath, null);
                string exportPath = EditorUtility.SaveFilePanel("Export Shader", Path.GetDirectoryName(assetPath), Path.GetFileNameWithoutExtension(assetPath), "shader");
                if(string.IsNullOrEmpty(exportPath)) return;
                File.WriteAllText(exportPath, shaderText);
            }
            ApplyRevertGUI();
        }
    }

    public class EXShaderContainerAssetPostprocessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach(string path in importedAssets)
            {
                if(!path.EndsWith("excontainer", StringComparison.InvariantCultureIgnoreCase)) continue;

                var mainobj = AssetDatabase.LoadMainAssetAtPath(path);
                if(mainobj is Shader) ShaderUtil.RegisterShader((Shader)mainobj);

                foreach(var obj in AssetDatabase.LoadAllAssetRepresentationsAtPath(path))
                {
                    if(obj is Shader) ShaderUtil.RegisterShader((Shader)obj);
                }
            }
        }
    }

    public class EXShaderContainer
    {
        private const string MULTI_COMPILE_FORWARD          = "#pragma ex_multi_compile_forward";
        private const string MULTI_COMPILE_FORWARDADD       = "#pragma ex_multi_compile_forwardadd";
        private const string MULTI_COMPILE_SHADOWCASTER     = "#pragma ex_multi_compile_shadowcaster";
        private const string MULTI_COMPILE_DEPTHONLY        = "#pragma ex_multi_compile_depthonly";
        private const string MULTI_COMPILE_DEPTHNORMALS     = "#pragma ex_multi_compile_depthnormals";
        private const string MULTI_COMPILE_MOTIONVECTORS    = "#pragma ex_multi_compile_motionvectors";
        private const string MULTI_COMPILE_SCENESELECTION   = "#pragma ex_multi_compile_sceneselection";
        private const string MULTI_COMPILE_INSTANCING       = "#pragma multi_compile_instancing";
        private const string SKIP_VARIANTS_SHADOWS          = "#pragma ex_skip_variants_shadows";
        private const string SKIP_VARIANTS_LIGHTMAPS        = "#pragma ex_skip_variants_lightmaps";
        private const string SKIP_VARIANTS_DECALS           = "#pragma ex_skip_variants_decals";
        private const string SKIP_VARIANTS_ADDLIGHTSHADOWS  = "#pragma ex_skip_variants_addlightshadows";
        private const string SKIP_VARIANTS_PROBEVOLUMES     = "#pragma ex_skip_variants_probevolumes";
        private const string SKIP_VARIANTS_AO               = "#pragma ex_skip_variants_ao";
        private const string SKIP_VARIANTS_LIGHTLISTS       = "#pragma ex_skip_variants_lightlists";
        private const string SKIP_VARIANTS_REFLECTIONS      = "#pragma ex_skip_variants_reflections";
        public string Name = "";
        public string PropertiesPath = "";
        public string HLSLIncludePath = "";
        public string BRPPath = "";
        public string URPPath = "";
        public string HDRPPath = "";
        public string CustomEditor = "";
        private static string rpinc = "";

        public string UnpackContainer(string assetPath, AssetImportContext ctx)
        {
            string assetFolderPath = Path.GetDirectoryName(assetPath) + "/";
            string propPath = assetFolderPath + PropertiesPath;
            string incPath = assetFolderPath + HLSLIncludePath;
            string brpPath = assetFolderPath + BRPPath;
            string urpPath = assetFolderPath + URPPath;
            string hdrpPath = assetFolderPath + HDRPPath;
            ctx?.DependsOnSourceAsset(propPath);
            ctx?.DependsOnSourceAsset(incPath);
            ctx?.DependsOnSourceAsset(brpPath);
            ctx?.DependsOnSourceAsset(urpPath);
            ctx?.DependsOnSourceAsset(hdrpPath);
            StringBuilder sb = new StringBuilder();
            sb.Append("Shader \"" + Name + "\"\r\n");
            sb.Append("{\r\n");

            int indent = 4;
            sb.Append(GetIndent(indent) + "Properties\r\n");
            sb.Append(GetIndent(indent) + "{\r\n");
            sb.Append(ReadTextFile(propPath));
            sb.Append("\r\n");
            sb.Append(GetIndent(indent) + "}\r\n");
            sb.Append("\r\n");

            sb.Append(ReadTextFile(incPath) + "\r\n");
            sb.Append("\r\n");

            string renderPipelineName = UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset?.ToString() ?? "";

            PackageVersionInfos version = new PackageVersionInfos()
            {
                RP = RenderPipeline.BRP,
                Major = 12,
                Minor = 0,
                Patch = 0
            };

            string shaderLibsPath = GetShaderLibsPath(assetFolderPath) + "/";
            indent = 12;
            if(renderPipelineName.Contains("Universal"))
            {
                version = GetURPVersion();
                rpinc = GenerateIndentText(indent,
                    "#include \"" + shaderLibsPath + "0_pipeline_urp.hlsl\"",
                    "#include \"" + shaderLibsPath + "1_api.hlsl\"",
                    "#include \"" + shaderLibsPath + "2_structures.hlsl\"",
                    "#include \"" + shaderLibsPath + "3_functions.hlsl\"");
            }
            else if(renderPipelineName.Contains("HDRenderPipeline"))
            {
                version = GetHDRPVersion();
                rpinc = GenerateIndentText(indent,
                    "#include \"" + shaderLibsPath + "0_pipeline_hdrp.hlsl\"",
                    "#include \"" + shaderLibsPath + "1_api.hlsl\"",
                    "#include \"" + shaderLibsPath + "2_structures.hlsl\"",
                    "#include \"" + shaderLibsPath + "3_functions.hlsl\"");
            }
            else
            {
                rpinc = GenerateIndentText(indent,
                    "#include \"" + shaderLibsPath + "0_pipeline_brp.hlsl\"",
                    "#include \"" + shaderLibsPath + "1_api.hlsl\"",
                    "#include \"" + shaderLibsPath + "2_structures.hlsl\"",
                    "#include \"" + shaderLibsPath + "3_functions.hlsl\"");
            }

            switch(version.RP)
            {
                case RenderPipeline.BRP:
                    sb.Append(ReadTextFile(brpPath));
                    break;
                case RenderPipeline.URP:
                    sb.Append(ReadTextFile(urpPath));
                    break;
                case RenderPipeline.HDRP:
                    sb.Append(ReadTextFile(hdrpPath));
                    break;
            }

            sb.Replace(MULTI_COMPILE_FORWARDADD, GetMultiCompileForwardAdd(version, indent));
            sb.Replace(MULTI_COMPILE_FORWARD, GetMultiCompileForward(version, indent));
            sb.Replace(MULTI_COMPILE_SHADOWCASTER, GetMultiCompileShadowCaster(version, indent));
            sb.Replace(MULTI_COMPILE_DEPTHONLY, GetMultiCompileDepthOnly(version, indent));
            sb.Replace(MULTI_COMPILE_DEPTHNORMALS, GetMultiCompileDepthNormals(version, indent));
            sb.Replace(MULTI_COMPILE_MOTIONVECTORS, GetMultiCompileMotionVectors(version, indent));
            sb.Replace(MULTI_COMPILE_SCENESELECTION, GetMultiCompileSceneSelection(version, indent));
            sb.Replace(MULTI_COMPILE_INSTANCING, GetMultiCompileInstancingLayer(version, indent));

            sb.Replace(SKIP_VARIANTS_SHADOWS, GetSkipVariantsShadows());
            sb.Replace(SKIP_VARIANTS_LIGHTMAPS, GetSkipVariantsLightmaps());
            sb.Replace(SKIP_VARIANTS_DECALS, GetSkipVariantsDecals());
            sb.Replace(SKIP_VARIANTS_ADDLIGHTSHADOWS, GetSkipVariantsAddLightShadows());
            sb.Replace(SKIP_VARIANTS_PROBEVOLUMES, GetSkipVariantsProbeVolumes());
            sb.Replace(SKIP_VARIANTS_AO, GetSkipVariantsAO());
            sb.Replace(SKIP_VARIANTS_LIGHTLISTS, GetSkipVariantsLightLists());
            sb.Replace(SKIP_VARIANTS_REFLECTIONS, GetSkipVariantsReflections());

            indent = 4;
            sb.Append("\r\n");
            sb.Append("\r\n");
            sb.Append(GetIndent(indent) + "CustomEditor \"");
            sb.Append(CustomEditor);
            sb.Append("\"\r\n");
            sb.Append("}");

            Debug.LogFormat(Name + " Info: {0} {1}.{2}.{3}", version.RP, version.Major, version.Minor, version.Patch);

            return sb.ToString();
        }

        public string ReadTextFile(string path)
        {
            StreamReader sr = new StreamReader(path);
            string text = sr.ReadToEnd();
            sr.Close();
            return text;
        }

        //------------------------------------------------------------------------------------------------------------------------------
        // Utils
        private string GetIndent(int indent)
        {
            return new string(' ', indent);
        }

        private string GenerateIndentText(int indent, params string[] texts)
        {
            string ind = "\r\n" + GetIndent(indent);
            return string.Join(ind, texts);

        }

        private string GetShaderLibsPath(string assetPath)
        {
            return GetRelativePath(assetPath, AssetDatabase.GUIDToAssetPath("a4b4b7de0d50f494494a30a207b1882a"));
        }

        private static string GetRelativePath(string fromPath, string toPath)
        {
            Uri fromUri = new Uri(Path.GetFullPath(fromPath));
            Uri toUri = new Uri(Path.GetFullPath(toPath));

            string relativePath = Uri.UnescapeDataString(fromUri.MakeRelativeUri(toUri).ToString());
            relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, '/');

            return relativePath;
        }

        //------------------------------------------------------------------------------------------------------------------------------
        // Render Pipeline
        private PackageVersionInfos GetURPVersion()
        {
            PackageVersionInfos version = ReadVersion("30648b8d550465f4bb77f1e1afd0b37d");
            version.RP = RenderPipeline.URP;
            return version;
        }

        private PackageVersionInfos GetHDRPVersion()
        {
            PackageVersionInfos version = ReadVersion("6f54db4299717fc4ca37866c6afa0905");
            version.RP = RenderPipeline.HDRP;
            return version;
        }

        private PackageVersionInfos ReadVersion(string guid)
        {
            string version = "";
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if(!string.IsNullOrEmpty(path))
            {
                PackageInfos package = JsonUtility.FromJson<PackageInfos>(File.ReadAllText(path));
                version = package.version;
            }

            PackageVersionInfos infos;
            infos.RP = RenderPipeline.BRP;
            if(string.IsNullOrEmpty(version))
            {
                infos.Major = 12;
                infos.Minor = 0;
                infos.Patch = 0;
            }
            else
            {
                string[] parts = version.Split('.');
                infos.Major = int.Parse(parts[0]);
                infos.Minor = int.Parse(parts[1]);
                infos.Patch = int.Parse(parts[2]);
            }
            return infos;
        }

        private enum RenderPipeline
        {
            BRP,
            URP,
            HDRP
        }

        private struct PackageVersionInfos
        {
            public RenderPipeline RP;
            public int Major;
            public int Minor;
            public int Patch;
        }

        private class PackageInfos
        {
            public string version;
        }

        //------------------------------------------------------------------------------------------------------------------------------
        // Multi Compile
        private string GetMultiCompileForward(PackageVersionInfos version, int indent)
        {
            if(version.RP == RenderPipeline.URP)
            {
                if(version.Major >= 12)
                {
                    return GenerateIndentText(indent,
                        "#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN",
                        "#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS",
                        "#pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS",
                        "#pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING",
                        "#pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION",
                        "#pragma multi_compile_fragment _ _SHADOWS_SOFT",
                        "#pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION",
                        "#pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3",
                        "#pragma multi_compile_fragment _ _LIGHT_LAYERS",
                        "#pragma multi_compile_fragment _ _LIGHT_COOKIES",
                        "#pragma multi_compile _ _CLUSTERED_RENDERING",
                        "#pragma multi_compile _ LIGHTMAP_SHADOW_MIXING",
                        "#pragma multi_compile _ SHADOWS_SHADOWMASK",
                        "#pragma multi_compile _ DIRLIGHTMAP_COMBINED",
                        "#pragma multi_compile _ LIGHTMAP_ON",
                        "#pragma multi_compile _ DYNAMICLIGHTMAP_ON",
                        "#pragma multi_compile_vertex _ FOG_LINEAR FOG_EXP FOG_EXP2",
                        "#pragma multi_compile_instancing",
                        "#define EX_PASS_FORWARD",
                        rpinc);
                }
                if(version.Major >= 11)
                {
                    return GenerateIndentText(indent,
                        "#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN",
                        "#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS",
                        "#pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS",
                        "#pragma multi_compile_fragment _ _SHADOWS_SOFT",
                        "#pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION",
                        "#pragma multi_compile _ LIGHTMAP_SHADOW_MIXING",
                        "#pragma multi_compile _ SHADOWS_SHADOWMASK",
                        "#pragma multi_compile _ DIRLIGHTMAP_COMBINED",
                        "#pragma multi_compile _ LIGHTMAP_ON",
                        "#pragma multi_compile_vertex _ FOG_LINEAR FOG_EXP FOG_EXP2",
                        "#pragma multi_compile_instancing",
                        "#define EX_PASS_FORWARD",
                        rpinc);
                }
                if(version.Major >= 10)
                {
                    return GenerateIndentText(indent,
                        "#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE",
                        "#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS",
                        "#pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS",
                        "#pragma multi_compile_fragment _ _SHADOWS_SOFT",
                        "#pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION",
                        "#pragma multi_compile _ LIGHTMAP_SHADOW_MIXING",
                        "#pragma multi_compile _ SHADOWS_SHADOWMASK",
                        "#pragma multi_compile _ DIRLIGHTMAP_COMBINED",
                        "#pragma multi_compile _ LIGHTMAP_ON",
                        "#pragma multi_compile_vertex _ FOG_LINEAR FOG_EXP FOG_EXP2",
                        "#pragma multi_compile_instancing",
                        "#define EX_PASS_FORWARD",
                        rpinc);
                }
                else
                {
                    return GenerateIndentText(indent,
                        "#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE",
                        "#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS",
                        "#pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS",
                        "#pragma multi_compile_fragment _ _SHADOWS_SOFT",
                        "#pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE",
                        "#pragma multi_compile _ DIRLIGHTMAP_COMBINED",
                        "#pragma multi_compile _ LIGHTMAP_ON",
                        "#pragma multi_compile_vertex _ FOG_LINEAR FOG_EXP FOG_EXP2",
                        "#pragma multi_compile_instancing",
                        "#define EX_PASS_FORWARD",
                        rpinc);
                }
            }
            else if(version.RP == RenderPipeline.HDRP)
            {
                if(version.Major >= 12)
                {
                    return GenerateIndentText(indent,
                        "#pragma multi_compile _ LIGHTMAP_ON",
                        "#pragma multi_compile _ DIRLIGHTMAP_COMBINED",
                        "#pragma multi_compile _ DYNAMICLIGHTMAP_ON",
                        "#pragma multi_compile_fragment _ SHADOWS_SHADOWMASK",
                        "#pragma multi_compile_fragment PROBE_VOLUMES_OFF PROBE_VOLUMES_L1 PROBE_VOLUMES_L2",
                        "#pragma multi_compile_fragment SCREEN_SPACE_SHADOWS_OFF SCREEN_SPACE_SHADOWS_ON",
                        "#pragma multi_compile_fragment DECALS_OFF DECALS_3RT DECALS_4RT",
                        "#pragma multi_compile_fragment _ DECAL_SURFACE_GRADIENT",
                        "#pragma multi_compile_fragment SHADOW_LOW SHADOW_MEDIUM SHADOW_HIGH SHADOW_VERY_HIGH",
                        "#pragma multi_compile_fragment USE_FPTL_LIGHTLIST USE_CLUSTERED_LIGHTLIST",
                        "#pragma multi_compile_instancing",
                        "#define SHADERPASS SHADERPASS_FORWARD",
                        "#define HAS_LIGHTLOOP",
                        "#define EX_PASS_FORWARD",
                        rpinc);
                }
                if(version.Major >= 10)
                {
                    return GenerateIndentText(indent,
                        "#pragma multi_compile _ LIGHTMAP_ON",
                        "#pragma multi_compile _ DIRLIGHTMAP_COMBINED",
                        "#pragma multi_compile _ DYNAMICLIGHTMAP_ON",
                        "#pragma multi_compile_fragment _ SHADOWS_SHADOWMASK",
                        "#pragma multi_compile_fragment SCREEN_SPACE_SHADOWS_OFF SCREEN_SPACE_SHADOWS_ON",
                        "#pragma multi_compile_fragment DECALS_OFF DECALS_3RT DECALS_4RT",
                        "#pragma multi_compile_fragment SHADOW_LOW SHADOW_MEDIUM SHADOW_HIGH SHADOW_VERY_HIGH",
                        "#pragma multi_compile_fragment USE_FPTL_LIGHTLIST USE_CLUSTERED_LIGHTLIST",
                        "#pragma multi_compile_instancing",
                        "#define SHADERPASS SHADERPASS_FORWARD",
                        "#define HAS_LIGHTLOOP",
                        "#define EX_PASS_FORWARD",
                        rpinc);
                }
                else
                {
                    return GenerateIndentText(indent,
                        "#pragma multi_compile _ LIGHTMAP_ON",
                        "#pragma multi_compile _ DIRLIGHTMAP_COMBINED",
                        "#pragma multi_compile _ DYNAMICLIGHTMAP_ON",
                        "#pragma multi_compile_fragment _ SHADOWS_SHADOWMASK",
                        "#pragma multi_compile_fragment DECALS_OFF DECALS_3RT DECALS_4RT",
                        "#pragma multi_compile_fragment SHADOW_LOW SHADOW_MEDIUM SHADOW_HIGH SHADOW_VERY_HIGH",
                        "#pragma multi_compile_fragment USE_FPTL_LIGHTLIST USE_CLUSTERED_LIGHTLIST",
                        "#pragma multi_compile_instancing",
                        "#define SHADERPASS SHADERPASS_FORWARD",
                        "#define HAS_LIGHTLOOP",
                        "#define EX_PASS_FORWARD",
                        rpinc);
                }
            }
            else
            {
                return GenerateIndentText(indent,
                    "#pragma multi_compile_fwdbase",
                    "#pragma multi_compile_vertex _ FOG_LINEAR FOG_EXP FOG_EXP2",
                    "#pragma multi_compile_instancing",
                    "#define EX_PASS_FORWARD",
                    rpinc);
            }
        }

        private string GetMultiCompileForwardAdd(PackageVersionInfos version, int indent)
        {
            if(version.RP == RenderPipeline.URP)
            {
                return GenerateIndentText(indent,
                    "#pragma multi_compile_instancing",
                    "#define EX_PASS_FORWARDADD",
                    rpinc);
            }
            else if(version.RP == RenderPipeline.HDRP)
            {
                return GenerateIndentText(indent,
                    "#pragma multi_compile_instancing",
                    "#define EX_PASS_FORWARDADD",
                    rpinc);
            }
            else
            {
                return GenerateIndentText(indent,
                    "#pragma multi_compile_fragment POINT DIRECTIONAL SPOT POINT_COOKIE DIRECTIONAL_COOKIE",
                    "#pragma multi_compile_vertex _ FOG_LINEAR FOG_EXP FOG_EXP2",
                    "#pragma multi_compile_instancing",
                    "#define EX_PASS_FORWARDADD",
                    rpinc);
            }
        }

        private string GetMultiCompileShadowCaster(PackageVersionInfos version, int indent)
        {
            if(version.RP == RenderPipeline.URP)
            {
                if(version.Major >= 11)
                {
                    return GenerateIndentText(indent,
                        "#pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW",
                        "#pragma multi_compile_instancing",
                        "#define EX_PASS_SHADOWCASTER",
                        rpinc);
                }
                else
                {
                    return GenerateIndentText(indent,
                        "#pragma multi_compile_instancing",
                        "#define EX_PASS_SHADOWCASTER",
                        rpinc);
                }
            }
            else if(version.RP == RenderPipeline.HDRP)
            {
                return GenerateIndentText(indent,
                    "#pragma multi_compile_instancing",
                    "#define SHADERPASS SHADERPASS_SHADOWS",
                    "#define EX_PASS_SHADOWCASTER",
                    rpinc);
            }
            else
            {
                return GenerateIndentText(indent,
                    "#pragma multi_compile_shadowcaster",
                    "#pragma multi_compile_instancing",
                    "#define EX_PASS_SHADOWCASTER",
                    rpinc);
            }
        }

        private string GetMultiCompileDepthOnly(PackageVersionInfos version, int indent)
        {
            if(version.RP == RenderPipeline.URP)
            {
                return GenerateIndentText(indent,
                    "#pragma multi_compile_instancing",
                    "#define EX_PASS_DEPTHONLY",
                    rpinc);
            }
            else if(version.RP == RenderPipeline.HDRP)
            {
                if(version.Major >= 10)
                {
                    return GenerateIndentText(indent,
                        "#pragma multi_compile _ WRITE_NORMAL_BUFFER",
                        "#pragma multi_compile_fragment _ WRITE_MSAA_DEPTH",
                        "#pragma multi_compile _ WRITE_DECAL_BUFFER",
                        "#pragma multi_compile_instancing",
                        "#define SHADERPASS SHADERPASS_DEPTH_ONLY",
                        "#define EX_PASS_DEPTHONLY",
                        rpinc);
                }
                else
                {
                    return GenerateIndentText(indent,
                        "#pragma multi_compile _ WRITE_NORMAL_BUFFER",
                        "#pragma multi_compile_fragment _ WRITE_MSAA_DEPTH",
                        "#pragma multi_compile_instancing",
                        "#define SHADERPASS SHADERPASS_DEPTH_ONLY",
                        "#define EX_PASS_DEPTHONLY",
                        rpinc);
                }
            }
            else
            {
                return GenerateIndentText(indent,
                    "#pragma multi_compile_instancing",
                    "#define EX_PASS_DEPTHONLY",
                    rpinc);
            }
        }

        private string GetMultiCompileDepthNormals(PackageVersionInfos version, int indent)
        {
            if(version.RP == RenderPipeline.URP)
            {
                return GenerateIndentText(indent,
                    "#pragma multi_compile_instancing",
                    "#define EX_PASS_DEPTHNORMALS",
                    rpinc);
            }
            else if(version.RP == RenderPipeline.HDRP)
            {
                return GenerateIndentText(indent,
                    "#pragma multi_compile_instancing",
                    "#define EX_PASS_DEPTHNORMALS",
                    rpinc);
            }
            else
            {
                return GenerateIndentText(indent,
                    "#pragma multi_compile_instancing",
                    "#define EX_PASS_DEPTHNORMALS",
                    rpinc);
            }
        }

        private string GetMultiCompileMotionVectors(PackageVersionInfos version, int indent)
        {
            if(version.RP == RenderPipeline.URP)
            {
                return GenerateIndentText(indent,
                    "#pragma multi_compile_instancing",
                    "#define EX_PASS_MOTIONVECTORS",
                    rpinc);
            }
            else if(version.RP == RenderPipeline.HDRP)
            {
                if(version.Major >= 10)
                {
                    return GenerateIndentText(indent,
                        "#pragma multi_compile _ WRITE_NORMAL_BUFFER",
                        "#pragma multi_compile_fragment _ WRITE_MSAA_DEPTH",
                        "#pragma multi_compile _ WRITE_DECAL_BUFFER",
                        "#pragma multi_compile_instancing",
                        "#define SHADERPASS SHADERPASS_MOTION_VECTORS",
                        "#define EX_PASS_MOTIONVECTORS",
                        rpinc);
                }
                else
                {
                    return GenerateIndentText(indent,
                        "#pragma multi_compile _ WRITE_NORMAL_BUFFER",
                        "#pragma multi_compile_fragment _ WRITE_MSAA_DEPTH",
                        "#pragma multi_compile_instancing",
                        "#define SHADERPASS SHADERPASS_MOTION_VECTORS",
                        "#define EX_PASS_MOTIONVECTORS",
                        rpinc);
                }
            }
            else
            {
                return GenerateIndentText(indent,
                    "#pragma multi_compile_instancing",
                    "#define EX_PASS_MOTIONVECTORS",
                    rpinc);
            }
        }

        private string GetMultiCompileSceneSelection(PackageVersionInfos version, int indent)
        {
            if(version.RP == RenderPipeline.URP)
            {
                return GenerateIndentText(indent,
                    "#pragma multi_compile_instancing",
                    "#define EX_PASS_SCENESELECTION",
                    rpinc);
            }
            else if(version.RP == RenderPipeline.HDRP)
            {
                return GenerateIndentText(indent,
                    "#pragma multi_compile_instancing",
                    "#pragma editor_sync_compilation",
                    "#define SHADERPASS SHADERPASS_DEPTH_ONLY",
                    "#define SCENESELECTIONPASS",
                    "#define EX_PASS_SCENESELECTION",
                    rpinc);
            }
            else
            {
                return GenerateIndentText(indent,
                    "#pragma multi_compile_instancing",
                    "#define EX_PASS_SCENESELECTION",
                    rpinc);
            }
        }

        private string GetMultiCompileInstancingLayer(PackageVersionInfos version, int indent)
        {
            if(version.RP == RenderPipeline.URP)
            {
                if(version.Major >= 12)
                {
                    return GenerateIndentText(indent,
                        "#pragma multi_compile_instancing",
                        "#pragma instancing_options renderinglayer");
                }
                else
                {
                    return GenerateIndentText(indent,
                        "#pragma multi_compile_instancing");
                }
            }
            else if(version.RP == RenderPipeline.HDRP)
            {
                return GenerateIndentText(indent,
                    "#pragma multi_compile_instancing",
                    "#pragma instancing_options renderinglayer");
            }
            else
            {
                return GenerateIndentText(indent,
                    "#pragma multi_compile_instancing");
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------
        // Skip Variants
        private string GetSkipVariantsShadows()
        {
            return "#pragma skip_variants SHADOWS_SCREEN _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN _ADDITIONAL_LIGHT_SHADOWS SCREEN_SPACE_SHADOWS_ON SHADOW_LOW SHADOW_MEDIUM SHADOW_HIGH SHADOW_VERY_HIGH";
        }

        private string GetSkipVariantsLightmaps()
        {
            return "#pragma skip_variants LIGHTMAP_ON DYNAMICLIGHTMAP_ON LIGHTMAP_SHADOW_MIXING SHADOWS_SHADOWMASK DIRLIGHTMAP_COMBINED _MIXED_LIGHTING_SUBTRACTIVE";
        }

        private string GetSkipVariantsDecals()
        {
            return "#pragma skip_variants DECALS_OFF DECALS_3RT DECALS_4RT DECAL_SURFACE_GRADIENT _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3";
        }

        private string GetSkipVariantsAddLightShadows()
        {
            return "#pragma skip_variants _ADDITIONAL_LIGHT_SHADOWS";
        }

        private string GetSkipVariantsProbeVolumes()
        {
            return "#pragma skip_variants PROBE_VOLUMES_OFF PROBE_VOLUMES_L1 PROBE_VOLUMES_L2";
        }

        private string GetSkipVariantsAO()
        {
            return "#pragma skip_variants _SCREEN_SPACE_OCCLUSION";
        }

        private string GetSkipVariantsLightLists()
        {
            return "#pragma skip_variants USE_FPTL_LIGHTLIST USE_CLUSTERED_LIGHTLIST";
        }

        private string GetSkipVariantsReflections()
        {
            return "#pragma skip_variants _REFLECTION_PROBE_BLENDING _REFLECTION_PROBE_BOX_PROJECTION";
        }
    }
}