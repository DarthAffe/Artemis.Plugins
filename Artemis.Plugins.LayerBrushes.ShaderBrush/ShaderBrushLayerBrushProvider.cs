using Artemis.Core.LayerBrushes;
using Artemis.Plugins.LayerBrushes.ShaderBrush.LayerBrushes;

namespace Artemis.Plugins.LayerBrushes.ShaderBrush;

public class ShaderBrushLayerBrushProvider : LayerBrushProvider
{
    #region Methods

    public override void Enable()
    {
        RegisterLayerBrushDescriptor<ShaderBrushLayerBrush>("Shader Brush", "Renders GLSL shaders", "React");
    }

    public override void Disable()
    { }

    #endregion
}
