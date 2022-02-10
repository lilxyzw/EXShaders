using UnityEditor;
using UnityEngine;

namespace EXShaders
{
    public class EXShadersUtil
    {
        private const string VRC_FALLBACK_TAG = "VRCFallback";

        public static bool FoldoutGUI(string title, bool display, GUIStyle foldout)
        {
            Rect rect = GUILayoutUtility.GetRect(16f, 20f, foldout);
			rect.width += 8f;
			rect.x -= 8f;
            GUI.Box(rect, new GUIContent(title, ""), foldout);

            Event e = Event.current;

            Rect toggleRect = new Rect(rect.x + 4f, rect.y + 2f, 13f, 13f);
            if(e.type == EventType.Repaint) {
                EditorStyles.foldout.Draw(toggleRect, false, false, display, false);
            }

            rect.width -= 24;
            if(e.type == EventType.MouseDown && rect.Contains(e.mousePosition)) {
                display = !display;
                e.Use();
            }

            return display;
        }

        public static Vector2 Vec2Field(string label, Vector2 vec, bool fixForScroll = false)
        {
            if(fixForScroll) GUILayout.Space(-2);

            const float indentPerLevel = 15;
            int indentLevel = EditorGUI.indentLevel;
            float labelWidth = EditorGUIUtility.labelWidth;
            float lineHeight = EditorGUIUtility.singleLineHeight;

            EditorGUI.indentLevel = 0;

            Rect position = EditorGUILayout.GetControlRect(true, lineHeight, EditorStyles.layerMaskField);
            float labelStartX = position.x + indentLevel * indentPerLevel;
            float valueStartX = position.x + labelWidth;
            Rect labelRect = new Rect(labelStartX, position.y, labelWidth, position.height);
            Rect valueRect = new Rect(valueStartX, position.y, position.width - labelWidth, position.height);
            EditorGUI.PrefixLabel(labelRect, new GUIContent(label));
            vec = EditorGUI.Vector2Field(valueRect, GUIContent.none, vec);

            EditorGUI.indentLevel = indentLevel;

            return vec;
        }

        public static void DrawToggle(MaterialProperty prop, string label)
        {
            bool value = prop.floatValue != 0.0f;

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            value = EditorGUILayout.Toggle(label, value);
            EditorGUI.showMixedValue = false;
            if(EditorGUI.EndChangeCheck())
            {
                prop.floatValue = value ? 1.0f : 0.0f;
            }
        }

        public static void DrawRemap(MaterialProperty prop, string label)
        {
            float blur = 1.0f / prop.vectorValue.x;
            float border = -prop.vectorValue.y / prop.vectorValue.x;

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            border = EditorGUILayout.Slider(label + " Border", border, -1.1f, 1.1f);
            blur = EditorGUILayout.Slider(label + " Blur", blur, -1.0f, 1.0f);
            EditorGUI.showMixedValue = false;
            if(EditorGUI.EndChangeCheck())
            {
                if(blur == 0.0f) blur += 0.001f;
                prop.vectorValue = new Vector4(
                    1.0f / blur,
                    -border / blur,
                    prop.vectorValue.z,
                    prop.vectorValue.w);
            }
        }

        public static void DrawRemap(MaterialProperty prop)
        {
            float blur = 1.0f / prop.vectorValue.x;
            float border = -prop.vectorValue.y / prop.vectorValue.x;

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            border = EditorGUILayout.Slider("Border", border, -1.1f, 1.1f);
            blur = EditorGUILayout.Slider("Blur", blur, -1.0f, 1.0f);
            EditorGUI.showMixedValue = false;
            if(EditorGUI.EndChangeCheck())
            {
                if(blur == 0.0f) blur += 0.001f;
                prop.vectorValue = new Vector4(
                    1.0f / blur,
                    -border / blur,
                    prop.vectorValue.z,
                    prop.vectorValue.w);
            }
        }

        public static void DrawRemapZW(MaterialProperty prop, string label)
        {
            float blur = 1.0f / prop.vectorValue.z;
            float border = -prop.vectorValue.w / prop.vectorValue.z;

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            border = EditorGUILayout.Slider(label + " Border", border, -1.1f, 1.1f);
            blur = EditorGUILayout.Slider(label + " Blur", blur, -1.0f, 1.0f);
            EditorGUI.showMixedValue = false;
            if(EditorGUI.EndChangeCheck())
            {
                if(blur == 0.0f) blur += 0.001f;
                prop.vectorValue = new Vector4(
                    prop.vectorValue.x,
                    prop.vectorValue.y,
                    1.0f / blur,
                    -border / blur);
            }
        }

        public static void DrawRemapZW(MaterialProperty prop)
        {
            float blur = 1.0f / prop.vectorValue.z;
            float border = -prop.vectorValue.w / prop.vectorValue.z;

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            border = EditorGUILayout.Slider("Border", border, -1.1f, 1.1f);
            blur = EditorGUILayout.Slider("Blur", blur, -1.0f, 1.0f);
            EditorGUI.showMixedValue = false;
            if(EditorGUI.EndChangeCheck())
            {
                if(blur == 0.0f) blur += 0.001f;
                prop.vectorValue = new Vector4(
                    prop.vectorValue.x,
                    prop.vectorValue.y,
                    1.0f / blur,
                    -border / blur);
            }
        }

        public static void DrawScrollXY(MaterialProperty prop)
        {
            Vector2 scroll = new Vector2(prop.vectorValue.x, prop.vectorValue.y);

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            scroll = Vec2Field("Scroll", scroll, true);
            EditorGUI.showMixedValue = false;
            if(EditorGUI.EndChangeCheck())
            {
                prop.vectorValue = new Vector4(
                    scroll.x,
                    scroll.y,
                    prop.vectorValue.z,
                    prop.vectorValue.w);
            }
        }

        public static void DrawScrollZW(MaterialProperty prop)
        {
            Vector2 scroll = new Vector2(prop.vectorValue.z, prop.vectorValue.w);

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            scroll = Vec2Field("Scroll", scroll, true);
            EditorGUI.showMixedValue = false;
            if(EditorGUI.EndChangeCheck())
            {
                prop.vectorValue = new Vector4(
                    prop.vectorValue.x,
                    prop.vectorValue.y,
                    scroll.x,
                    scroll.y);
            }
        }

        public static void DrawEnum(MaterialProperty prop, string label, string[] names)
        {
            int value = (int)prop.floatValue;

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            value = EditorGUILayout.Popup(label, value, names);
            EditorGUI.showMixedValue = false;
            if(EditorGUI.EndChangeCheck())
            {
                prop.floatValue = value;
            }
        }

        public static void DrawEnumBold(MaterialProperty prop, string label, string[] names)
        {
            int value = (int)prop.floatValue;

            Rect position = EditorGUILayout.GetControlRect();
            float labelWidth = EditorGUIUtility.labelWidth + 2;
            Rect labelRect = new Rect(position.x, position.y, labelWidth, position.height);
            Rect valueRect = new Rect(position.x + labelWidth, position.y, position.width - labelWidth, position.height);

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            EditorGUI.PrefixLabel(labelRect, new GUIContent(label), EditorStyles.boldLabel);
            value = EditorGUI.Popup(valueRect, value, names);
            EditorGUI.showMixedValue = false;
            if(EditorGUI.EndChangeCheck())
            {
                prop.floatValue = value;
            }
        }

        public static void DrawStencilRange(MaterialProperty prop, string label)
        {
            int value = (int)prop.floatValue;

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            value = EditorGUILayout.IntSlider(label, value, 0, 255);
            EditorGUI.showMixedValue = false;
            if(EditorGUI.EndChangeCheck())
            {
                prop.floatValue = value;
            }
        }

        public static void DrawFoldoutTextureGUI(ref bool isShow, GUIContent guiContent, MaterialProperty tex, MaterialProperty col, MaterialEditor m_MaterialEditor)
        {
            EditorGUI.indentLevel++;
            Rect rect = m_MaterialEditor.TexturePropertySingleLine(guiContent, tex, col);
            EditorGUI.indentLevel--;
            rect.x += 10;
            isShow = EditorGUI.Foldout(rect, isShow, "");
        }

        public static void DrawTextureAndUVGUI(ref bool isShow, GUIContent guiContent, MaterialProperty tex, MaterialProperty col, MaterialProperty scroll, bool isScrollXY, MaterialEditor m_MaterialEditor)
        {
            EditorGUI.indentLevel++;
            Rect rect = m_MaterialEditor.TexturePropertySingleLine(guiContent, tex, col);
            EditorGUI.indentLevel--;
            rect.x += 10;
            isShow = EditorGUI.Foldout(rect, isShow, "");

            if(isShow)
            {
                EditorGUI.indentLevel++;
                m_MaterialEditor.TextureScaleOffsetProperty(tex);
                if(isScrollXY) DrawScrollXY(scroll);
                else           DrawScrollZW(scroll);
                EditorGUI.indentLevel--;
            }
        }

        public static void DrawTextureAndUVGUI(ref bool isShow, GUIContent guiContent, MaterialProperty tex, MaterialProperty scroll, bool isScrollXY, MaterialEditor m_MaterialEditor)
        {
            EditorGUI.indentLevel++;
            Rect rect = m_MaterialEditor.TexturePropertySingleLine(guiContent, tex);
            EditorGUI.indentLevel--;
            rect.x += 10;
            isShow = EditorGUI.Foldout(rect, isShow, "");

            if(isShow)
            {
                EditorGUI.indentLevel++;
                m_MaterialEditor.TextureScaleOffsetProperty(tex);
                if(isScrollXY) DrawScrollXY(scroll);
                else           DrawScrollZW(scroll);
                EditorGUI.indentLevel--;
            }
        }

        public static void DrawBlink(MaterialProperty prop)
        {
            float strength = prop.vectorValue.x;
            float type = prop.vectorValue.y;
            float speed = prop.vectorValue.z / Mathf.PI;
            float offset = prop.vectorValue.w / Mathf.PI;

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            strength = EditorGUILayout.Slider("Blink Strength", strength, 0.0f, 1.0f);
            if(strength != 0.0f)
            {
                EditorGUI.indentLevel++;
                type    = EditorGUILayout.Toggle("Blink Type", type > 0.5f) ? 1.0f : 0.0f;
                speed   = EditorGUILayout.FloatField("Blink Speed", speed);
                offset  = EditorGUILayout.FloatField("Blink Delay", offset);
                EditorGUI.indentLevel--;
            }
            EditorGUI.showMixedValue = false;

            if(EditorGUI.EndChangeCheck())
            {
                prop.vectorValue = new Vector4(strength, type, speed * Mathf.PI, offset * Mathf.PI);
            }
        }

        public static void DrawUVBlend(MaterialProperty prop, string label1, string label2)
        {
            Vector2 scale1 = new Vector2(prop.vectorValue.x, prop.vectorValue.y);
            Vector2 scale2 = new Vector2(prop.vectorValue.z, prop.vectorValue.w);

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            scale1 = Vec2Field(label1, scale1);
            scale2 = Vec2Field(label2, scale2);
            EditorGUI.showMixedValue = false;
            if(EditorGUI.EndChangeCheck())
            {
                prop.vectorValue = new Vector4(
                    scale1.x,
                    scale1.y,
                    scale2.x,
                    scale2.y);
            }
        }

        public static void DrawBlendMode(MaterialProperty prop)
        {
            int value = (int)prop.floatValue;

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            value = EditorGUILayout.Popup("Blend Mode", value, new[]{"Alpha", "Add", "Screen", "Mul"});
            EditorGUI.showMixedValue = false;
            if(EditorGUI.EndChangeCheck())
            {
                prop.floatValue = value;
            }
        }

        public static void DrawDetailUV(MaterialProperty uv01)
        {
            float blendUV1 = uv01.vectorValue.w;

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = uv01.hasMixedValue;
            blendUV1 = EditorGUILayout.Slider("BlendUV1", blendUV1, 0.0f, 1.0f);
            EditorGUI.showMixedValue = false;
            if(EditorGUI.EndChangeCheck())
            {
                uv01.vectorValue = new Vector4(
                    1.0f - blendUV1,
                    1.0f - blendUV1,
                    blendUV1,
                    blendUV1);
            }
        }

        public static void DrawVRCFallbackGUI(Material material)
        {
            #if VRC_SDK_VRCSDK2 || VRC_SDK_VRCSDK3
                string tag = material.GetTag(VRC_FALLBACK_TAG, false);
                string[] sFallbackShaderTypes = {"Unlit", "Standard", "VertexLit", "Toon", "Particle", "Sprite", "Matcap", "MobileToon", "Hidden"};
                string[] sFallbackRenderTypes = {"Opaque", "Cutout", "Transparent", "Fade"};
                string[] sFallbackCullTypes = {"Default", "DoubleSided"};

                int fallbackShaderType = tag.Contains("Standard")       ? 1 : 0;
                    fallbackShaderType = tag.Contains("VertexLit")      ? 2 : fallbackShaderType;
                    fallbackShaderType = tag.Contains("Toon")           ? 3 : fallbackShaderType;
                    fallbackShaderType = tag.Contains("Particle")       ? 4 : fallbackShaderType;
                    fallbackShaderType = tag.Contains("Sprite")         ? 5 : fallbackShaderType;
                    fallbackShaderType = tag.Contains("Matcap")         ? 6 : fallbackShaderType;
                    fallbackShaderType = tag.Contains("MobileToon")     ? 7 : fallbackShaderType;
                    fallbackShaderType = tag.Contains("Hidden")         ? 8 : fallbackShaderType;

                int fallbackRenderType = tag.Contains("Cutout")         ? 1 : 0;
                    fallbackRenderType = tag.Contains("Transparent")    ? 2 : fallbackRenderType;
                    fallbackRenderType = tag.Contains("Fade")           ? 3 : fallbackRenderType;

                int fallbackCullType = tag.Contains("DoubleSided") ? 1 : 0;

                fallbackShaderType = EditorGUILayout.Popup("Shader Type", fallbackShaderType, sFallbackShaderTypes);
                fallbackRenderType = EditorGUILayout.Popup("Rendering Mode", fallbackRenderType, sFallbackRenderTypes);
                fallbackCullType = EditorGUILayout.Popup("Facing", fallbackCullType, sFallbackCullTypes);

                switch(fallbackShaderType)
                {
                    case 0: tag = "Unlit"; break;
                    case 1: tag = "Standard"; break;
                    case 2: tag = "VertexLit"; break;
                    case 3: tag = "Toon"; break;
                    case 4: tag = "Particle"; break;
                    case 5: tag = "Sprite"; break;
                    case 6: tag = "Matcap"; break;
                    case 7: tag = "MobileToon"; break;
                    case 8: tag = "Hidden"; break;
                    default: tag = "Unlit"; break;
                }
                switch(fallbackRenderType)
                {
                    case 0: break;
                    case 1: tag += "Cutout"; break;
                    case 2: tag += "Transparent"; break;
                    case 3: tag += "Fade"; break;
                    default: break;
                }
                switch(fallbackCullType)
                {
                    case 0: break;
                    case 1: tag += "DoubleSided"; break;
                    default: break;
                }
                EditorGUILayout.LabelField("Result:", '"' + tag + '"');
                material.SetOverrideTag(VRC_FALLBACK_TAG, tag);
            #endif
        }

        public static void SetVRCFallback(Material material, int fallbackShaderType, int fallbackRenderType, int fallbackCullType)
        {
            #if VRC_SDK_VRCSDK2 || VRC_SDK_VRCSDK3
                string tag;
                switch(fallbackShaderType)
                {
                    case 0: tag = "Unlit"; break;
                    case 1: tag = "Standard"; break;
                    case 2: tag = "VertexLit"; break;
                    case 3: tag = "Toon"; break;
                    case 4: tag = "Particle"; break;
                    case 5: tag = "Sprite"; break;
                    case 6: tag = "Matcap"; break;
                    case 7: tag = "MobileToon"; break;
                    case 8: tag = "Hidden"; break;
                    default: tag = "Unlit"; break;
                }
                switch(fallbackRenderType)
                {
                    case 0: break;
                    case 1: tag += "Cutout"; break;
                    case 2: tag += "Transparent"; break;
                    case 3: tag += "Fade"; break;
                    default: break;
                }
                switch(fallbackCullType)
                {
                    case 0: break;
                    case 1: tag += "DoubleSided"; break;
                    default: break;
                }
                material.SetOverrideTag(VRC_FALLBACK_TAG, tag);
            #endif
        }
    }

    public class EXHDRDrawer : MaterialPropertyDrawer
    {
        public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
        {
            float xMax = position.xMax;
            position.width = string.IsNullOrEmpty(label) ? Mathf.Min(50.0f, position.width) : EditorGUIUtility.labelWidth + 50.0f;
            Color value = prop.colorValue;
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            #if UNITY_2018_1_OR_NEWER
                value = EditorGUI.ColorField(position, new GUIContent(label), value, true, true, true);
            #else
                value = EditorGUI.ColorField(position, new GUIContent(label), value, true, true, true, null);
            #endif
            EditorGUI.showMixedValue = false;

            if(EditorGUI.EndChangeCheck())
            {
                prop.colorValue = value;
            }

            // Hex
            EditorGUI.BeginChangeCheck();
            float intensity = value.maxColorComponent > 1.0f ? value.maxColorComponent : 1.0f;
            Color value2 = new Color(value.r / intensity, value.g / intensity, value.b / intensity, 1.0f);
            string hex = ColorUtility.ToHtmlStringRGB(value2);
            int indentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            position.x += position.width + 4.0f;
            #if UNITY_2021_2_OR_NEWER
                position.width = Mathf.Min(50.0f, xMax - position.x);
                if(position.width > 10.0f)
                {
                    EditorGUI.showMixedValue = prop.hasMixedValue;
                    hex = "#" + EditorGUI.TextField(position, GUIContent.none, hex);
                    EditorGUI.showMixedValue = false;
                }
            #else
                position.width = 50.0f;
                EditorGUI.showMixedValue = prop.hasMixedValue;
                hex = "#" + EditorGUI.TextField(position, GUIContent.none, hex);
                EditorGUI.showMixedValue = false;
            #endif
            EditorGUI.indentLevel = indentLevel;
            if(EditorGUI.EndChangeCheck())
            {
                if(!ColorUtility.TryParseHtmlString(hex, out value2)) return;
                value.r = value2.r * intensity;
                value.g = value2.g * intensity;
                value.b = value2.b * intensity;
                prop.colorValue = value;
            }
        }
    }
}