using System;
using Artemis.Core.LayerBrushes;
using SkiaSharp;
using Artemis.Plugins.LayerBrushes.ShaderBrush.LayerBrushes.PropertyGroups;
using Artemis.Plugins.LayerBrushes.ShaderBrush.OpenGL;

namespace Artemis.Plugins.LayerBrushes.ShaderBrush.LayerBrushes;

public class ShaderBrushLayerBrush : LayerBrush<ShaderBrushPropertyGroup>
{
    #region Properties & Fields

    private ShaderRenderService? ShaderRenderService => ShaderBootstrapper.ShaderRenderService;

    private ShaderEntry? _shader;

    #endregion

    #region Constructors

    public ShaderBrushLayerBrush()
    { }

    #endregion

    #region Methods

    public override void EnableLayerBrush()
    {
        if ((_shader != null) || (ShaderRenderService == null)) return;

        _shader = ShaderRenderService!.RegisterShader(Properties.Shader.Shader.CurrentValue, Properties.Shader.Width.CurrentValue, Properties.Shader.Height.CurrentValue);
    }

    public override void DisableLayerBrush()
    {
        if ((_shader == null) || (ShaderRenderService == null)) return;

        ShaderRenderService!.UnregisterShader(_shader.Value);
        _shader = null;
    }

    public override void Update(double deltaTime) { }

    public override unsafe void Render(SKCanvas canvas, SKRect bounds, SKPaint paint)
    {
        if ((_shader == null) || (ShaderRenderService == null)) return;

        Span<byte> buffer = ShaderRenderService.Update(_shader.Value);

        fixed (byte* img = buffer)
        {
            using SKImage skImage = SKImage.FromPixels(new SKImageInfo(_shader.Value.Width, _shader.Value.Height, SKColorType.Bgra8888, SKAlphaType.Opaque), (nint)img, _shader.Value.Width * 4);
            canvas.DrawImage(skImage, bounds, paint);
        }
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (_shader != null)
            ShaderRenderService!.UnregisterShader(_shader.Value);

        _shader = null;
    }

    #endregion
}

