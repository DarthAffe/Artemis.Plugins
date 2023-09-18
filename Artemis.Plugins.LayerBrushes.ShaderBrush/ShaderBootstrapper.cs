using Artemis.Core;
using Artemis.Plugins.LayerBrushes.ShaderBrush.OpenGL;

namespace Artemis.Plugins.LayerBrushes.ShaderBrush;

public class ShaderBootstrapper : PluginBootstrapper
{
    #region Properties & Fields

    internal static ShaderRenderService? ShaderRenderService { get; private set; }

    #endregion

    #region Methods

    public override void OnPluginEnabled(Plugin plugin)
    {
        ShaderRenderService ??= new ShaderRenderService();
    }

    public override void OnPluginDisabled(Plugin plugin)
    {
        ShaderRenderService?.Dispose();
        ShaderRenderService = null;
    }

    #endregion
}