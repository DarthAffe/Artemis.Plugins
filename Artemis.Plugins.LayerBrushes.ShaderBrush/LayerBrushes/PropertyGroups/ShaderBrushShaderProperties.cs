using Artemis.Core;
using Artemis.Plugins.LayerBrushes.ShaderBrush.OpenGL;

namespace Artemis.Plugins.LayerBrushes.ShaderBrush.LayerBrushes.PropertyGroups;

public class ShaderBrushShaderProperties : LayerPropertyGroup
{
    #region Properties & Fields

    public LayerProperty<string> Shader { get; set; }
    public IntLayerProperty Width { get; set; }
    public IntLayerProperty Height { get; set; }

    #endregion

    #region Methods

    protected override void PopulateDefaults()
    {
        Shader.DefaultValue = ShaderRenderService.SHADER_TEMPLATE;
        Width.DefaultValue = 512;
        Height.DefaultValue = 512;
    }

    protected override void EnableProperties()
    { }

    protected override void DisableProperties()
    { }

    #endregion
}