using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace AreaBulldozer.Tools
{
    internal sealed class AreaBulldozerMarkerFocusPass : CustomPass
    {
        private Material m_Material;
        private Mesh m_FullscreenMesh;
        private int m_ForwardPass;
        private bool m_FirstDrawLogged;
        private readonly Vector3[] m_FrustumCorners = new Vector3[4];
        private readonly MaterialPropertyBlock m_Properties =
            new MaterialPropertyBlock();

        public Camera TargetCamera { get; set; }
        public float BrightnessMultiplier { get; set; } = 1f;

        public AreaBulldozerMarkerFocusPass()
        {
            name = "Area Bulldozer background dimming";
            targetColorBuffer = TargetBuffer.Camera;
            targetDepthBuffer = TargetBuffer.None;
            clearFlags = ClearFlag.None;
        }

        public bool TryInitialize()
        {
            if (m_Material != null && m_FullscreenMesh != null)
            {
                return true;
            }

            ReleaseResources();

            Shader shader = Shader.Find("HDRP/Unlit");
            if (shader == null || !shader.isSupported)
            {
                return false;
            }

            m_Material = new Material(shader)
            {
                hideFlags = HideFlags.HideAndDontSave
            };
            m_ForwardPass = m_Material.FindPass("ForwardOnly");
            if (m_ForwardPass < 0)
            {
                ReleaseResources();
                return false;
            }

            // Multiply the destination color directly.

            m_Material.SetFloat("_SurfaceType", 0f);
            m_Material.SetFloat("_BlendMode", 0f);
            m_Material.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
            m_Material.DisableKeyword("_ENABLE_FOG_ON_TRANSPARENT");
            m_Material.DisableKeyword("_EMISSIVE_COLOR_MAP");
            m_Material.SetInt("_SrcBlend", (int)BlendMode.Zero);
            m_Material.SetInt("_DstBlend", (int)BlendMode.SrcColor);
            m_Material.SetInt("_AlphaSrcBlend", (int)BlendMode.Zero);
            m_Material.SetInt("_AlphaDstBlend", (int)BlendMode.One);
            m_Material.SetInt("_CullMode", (int)CullMode.Off);
            m_Material.SetInt("_ZWrite", 0);
            m_Material.SetInt("_ZTestDepthEqualForOpaque", (int)CompareFunction.Always);
            m_Material.SetInt("_ZTestTransparent", (int)CompareFunction.Always);
            m_Material.SetInt("_StencilWriteMask", 0);
            m_Material.SetColor("_UnlitColor", Color.black);
            // HDRP cancels exposure for emission when this weight is zero.
            // The multiplier therefore stays stable as daylight/exposure changes.
            m_Material.SetFloat("_EmissiveExposureWeight", 0f);

            m_FullscreenMesh = new Mesh
            {
                name = "Area Bulldozer dimming quad",
                hideFlags = HideFlags.HideAndDontSave,
                vertices = new[]
                {
                    new Vector3(-1f, -1f, 0f),
                    new Vector3(-1f, 1f, 0f),
                    new Vector3(1f, 1f, 0f),
                    new Vector3(1f, -1f, 0f)
                },
                uv = new[]
                {
                    Vector2.zero, Vector2.up, Vector2.one, Vector2.right
                },
                triangles = new[] { 0, 1, 2, 0, 2, 3 }
            };
            m_FullscreenMesh.MarkDynamic();
            return true;
        }

        protected override void Execute(CustomPassContext ctx)
        {
            if (TargetCamera == null || ctx.hdCamera.camera != TargetCamera ||
                BrightnessMultiplier >= 1f || m_Material == null ||
                m_FullscreenMesh == null)
            {
                return;
            }

            float brightness = Mathf.Clamp01(BrightnessMultiplier);
            m_Properties.SetVector("_EmissiveColor",
                new Vector4(brightness, brightness, brightness, 1f));

            Camera camera = ctx.hdCamera.camera;
            camera.CalculateFrustumCorners(new Rect(-0.01f, -0.01f, 1.02f, 1.02f),
                Mathf.Max(camera.nearClipPlane * 2f, 1f),
                Camera.MonoOrStereoscopicEye.Mono, m_FrustumCorners);
            m_FullscreenMesh.vertices = m_FrustumCorners;
            ctx.cmd.DrawMesh(m_FullscreenMesh, camera.transform.localToWorldMatrix,
                m_Material, 0, m_ForwardPass, m_Properties);

            if (!m_FirstDrawLogged)
            {
                Mod.LogDiagnosticInfo(
                    $"Native background pass executed for camera {camera.name}.");
                m_FirstDrawLogged = true;
            }
        }

        public void ReleaseResources()
        {
            CoreUtils.Destroy(m_Material);
            CoreUtils.Destroy(m_FullscreenMesh);
            m_Material = null;
            m_FullscreenMesh = null;
            m_FirstDrawLogged = false;
        }

        protected override void Cleanup()
        {
            ReleaseResources();
        }
    }
}
