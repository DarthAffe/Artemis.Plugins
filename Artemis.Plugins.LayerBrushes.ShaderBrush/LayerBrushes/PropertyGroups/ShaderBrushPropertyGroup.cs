using Artemis.Core;

namespace Artemis.Plugins.LayerBrushes.ShaderBrush.LayerBrushes.PropertyGroups;

public class ShaderBrushPropertyGroup : LayerPropertyGroup
{
    #region Properties & Fields

    public ShaderBrushShaderProperties Shader { get; set; }

    #endregion

    #region Methods

    protected override void PopulateDefaults() { }

    protected override void EnableProperties()
    {
        Shader.IsHidden = true;
    }

    protected override void DisableProperties() { }

    #endregion
}