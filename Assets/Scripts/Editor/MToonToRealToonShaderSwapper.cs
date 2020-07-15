// MToon to RealToon Shader Swapper for Unity
// by Andy Miira (Andrei Müller), October 2019
//
//
// [INSTRUCTIONS]
// 0) Add this script inside an "Editor" folder in your project.
// You must create this folder if it doesn't already exist.
//
// 1) Open Unity, then click on
// "Tools -> Mirror Mirai -> RealToon Helpers -> MToon to RealToon Shader Swapper" at the menu bar.
//
// 2) Drag and drop your model's Prefab/Game Object with MToon materials in the "Model Prefab" field.
//
// 3) You can optionally override the "Environmental Lighting Intensity" property from RealToon,
// by enabling the "Set Env Light Intensity" option and typing the override value in the field below.
//
// 4) Finally, click on the "Swap Shaders" button.
// In ALL of the model's materials that use MToon shader, the shader will be swapped to RealToon.
//
// Usage example:
// https://twitter.com/andymiira/status/1189735251813294082
//
//
// Check out more Unity scripts and utilities at:
// https://gist.github.com/andreiagmu

using UnityEditor;
using UnityEngine;

namespace MirrorMirai
{
    public class MToonToRealToonShaderSwapper : ScriptableWizard
    {
        public GameObject modelPrefab;

        [Tooltip("If enabled, swap \"MToon\" shader with \"RealToon Lite\" shader.")]
        public bool useLiteShaders;

        // Shader properties overrides
        // If enabled, these overrides will be set in ALL of the prefab's materials!
        [Space]
        [Tooltip("If enabled, can override \"Main Color\" property with the value below. " +
                         "This will be applied to materials with MToon's default color value (Color.white), " +
                         "and will not be applied to materials with custom colors.")]
        public bool setMainColor;

        [Tooltip("The value to override \"Main Color\" property.")]
        public Color mainColorOverride = new Color(0.6886792f, 0.6886792f, 0.6886792f, 1);

        [Space]
        [Tooltip("If enabled, can override \"Environmental Lighting Intensity\" property (from RealToon Default) with the value below.")]
        public bool setEnvLightIntensity;

        [Tooltip("The value to override \"Environmental Lighting Intensity\" property.")]
        public float envLightIntensityOverride = 0.65f;

        // MToon cached property indexes
        private static readonly int BlendMode = Shader.PropertyToID("_BlendMode");
        private static readonly int MToonMainColor = Shader.PropertyToID("_Color");
        private static readonly int ShadeColor = Shader.PropertyToID("_ShadeColor");
        private static readonly int BumpMap = Shader.PropertyToID("_BumpMap");
        private static readonly int RimColor = Shader.PropertyToID("_RimColor");
        private static readonly int EmissionMap = Shader.PropertyToID("_EmissionMap");
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

        // RealToon cached property indexes
        private static readonly int MainColor = Shader.PropertyToID("_MainColor");
        private static readonly int OverallShadowColor = Shader.PropertyToID("_OverallShadowColor");
        private static readonly int NormalMap = Shader.PropertyToID("_NormalMap");
        private static readonly int RimLightColor = Shader.PropertyToID("_RimLightColor");
        private static readonly int EnableTextureTransparent = Shader.PropertyToID("_EnableTextureTransparent");
        private static readonly int GlossTexture = Shader.PropertyToID("_GlossTexture");
        private static readonly int GlossColor = Shader.PropertyToID("_GlossColor");
        private static readonly int EnvironmentalLightingIntensity = Shader.PropertyToID("_EnvironmentalLightingIntensity");


        [MenuItem("Tools/Mirror Mirai/RealToon Helpers/MToon to RealToon Shader Swapper")]
        static void CreateWizard()
        {
            DisplayWizard<MToonToRealToonShaderSwapper>("MToon to RealToon Shader Swapper", "Swap Shaders");
        }

        void OnWizardCreate()
        {
            var renderers = modelPrefab.GetComponentsInChildren<Renderer>();
            var shadersPath = useLiteShaders ? "RealToon/Version 5/Lite/" : "RealToon/Version 5/Default/";

            foreach (var renderer in renderers)
            {
                var sharedMaterials = renderer.sharedMaterials;

                foreach (var mat in sharedMaterials)
                {
                    if (mat.shader == Shader.Find("VRM/MToon"))
                    {
                        // Save MToon values
                        var textureMainColor = mat.GetColor(MToonMainColor);
                        var shadeColor = mat.GetColor(ShadeColor);
                        var normalMapTexture = mat.GetTexture(BumpMap);
                        var emissionMapTexture = mat.GetTexture(EmissionMap);
                        var emissionColor = mat.GetColor(EmissionColor);
                        var rimLightColor = mat.GetColor(RimColor);

                        // Swap the MToon shader to RealToon
                        // [MToon Rendering Types/BlendModes]
                        // Opaque = 0, Cutout = 1, Transparent = 2, TransparentWithZWrite = 3
                        var blendMode = (int)mat.GetFloat(BlendMode);
                        if (blendMode == 2 || blendMode == 3)
                        {
                            mat.shader = Shader.Find($"{shadersPath}Fade Transparency");
                        }
                        else
                        {
                            mat.shader = Shader.Find($"{shadersPath}Default");

                            if (blendMode == 1)
                            {
                                mat.SetFloat(EnableTextureTransparent, 1);
                            }
                        }

                        // Pass MToon values to RealToon shader
                        mat.SetColor(MainColor, textureMainColor);
                        mat.SetColor(OverallShadowColor, shadeColor);
                        mat.SetTexture(NormalMap, normalMapTexture);
                        mat.SetTexture(GlossTexture, emissionMapTexture);
                        mat.SetColor(GlossColor, emissionColor);

                        if (rimLightColor != Color.black)
                            mat.SetColor(RimLightColor, rimLightColor);


                        // Set property overrides if enabled
                        if (setMainColor && mat.GetColor(MainColor) == Color.white)
                            mat.SetColor(MainColor, mainColorOverride);

                        if (setEnvLightIntensity)
                            mat.SetFloat(EnvironmentalLightingIntensity, envLightIntensityOverride);
                    }
                }
            }
        }
    }
}
