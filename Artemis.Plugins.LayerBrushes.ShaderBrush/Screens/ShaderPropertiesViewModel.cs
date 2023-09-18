using System;
using System.ComponentModel;
using System.Reactive.Disposables;
using Artemis.Plugins.LayerBrushes.ShaderBrush.LayerBrushes;
using Artemis.Plugins.LayerBrushes.ShaderBrush.LayerBrushes.PropertyGroups;
using Artemis.Plugins.LayerBrushes.ShaderBrush.OpenGL;
using Artemis.UI.Shared.LayerBrushes;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using ReactiveUI;

namespace Artemis.Plugins.LayerBrushes.ShaderBrush.Screens;

public class ShaderPropertiesViewModel : BrushConfigurationViewModel
{
    #region Properties & Fields

    private readonly object _previewLock = new();

    private ShaderRenderService? ShaderRenderService => ShaderBootstrapper.ShaderRenderService;
    private ShaderEntry? _previewShader;

    private readonly ShaderBrushPropertyGroup _properties;
    private readonly DispatcherTimer _updateTimer;

    private string _shader = string.Empty;
    public string Shader
    {
        get => _shader;
        set => RaiseAndSetIfChanged(ref _shader, value);
    }

    private int _width;
    public int Width
    {
        get => _width;
        set => RaiseAndSetIfChanged(ref _width, value);
    }

    private int _height;
    public int Height
    {
        get => _height;
        set => RaiseAndSetIfChanged(ref _height, value);
    }

    private string _shaderException = string.Empty;
    public string ShaderException
    {
        get => _shaderException;
        set => RaiseAndSetIfChanged(ref _shaderException, value);
    }

    public Image? DisplayPreviewImage { get; set; }

    private WriteableBitmap? _previewImage;
    public WriteableBitmap? PreviewImage
    {
        get => _previewImage;
        set => RaiseAndSetIfChanged(ref _previewImage, value);
    }

    public ShaderBrushLayerBrush ShaderBrushLayerBrush { get; }

    #endregion

    #region Constructors

    public ShaderPropertiesViewModel(ShaderBrushLayerBrush layerBrush)
        : base(layerBrush)
    {
        this.ShaderBrushLayerBrush = layerBrush;

        _properties = layerBrush.Properties;

        _updateTimer = new DispatcherTimer(TimeSpan.FromSeconds(1 / 30.0), DispatcherPriority.Normal, (_, _) => UpdatePreview());
        _updateTimer.Start();

        _properties.Shader.Width.PropertyChanged += OnSizeChangedChanged;
        _properties.Shader.Height.PropertyChanged += OnSizeChangedChanged;
        _properties.Shader.Shader.PropertyChanged += OnShaderChangedChanged;

        this.WhenActivated(Initialize);
    }

    #endregion

    #region Methods

    public void Load()
    {
        Shader = _properties.Shader.Shader.CurrentValue;
        Width = _properties.Shader.Width;
        Height = _properties.Shader.Height;
    }

    public void Save()
    {
        _properties.Shader.Shader.SetCurrentValue(Shader);
        _properties.Shader.Width.SetCurrentValue(Width);
        _properties.Shader.Height.SetCurrentValue(Height);

        ShaderBrushLayerBrush.RecreateShader();
    }

    private void Initialize(CompositeDisposable d)
    {
        Load();

        RecreatePreviewImage();
        RecreatePreviewShader();

        Disposable.Create(() =>
        {
            _updateTimer.Stop();

            if (_previewShader != null)
                ShaderRenderService!.UnregisterShader(_previewShader.Value);

        }).DisposeWith(d);
    }

    private unsafe void UpdatePreview()
    {
        lock (_previewLock)
        {
            if ((_previewShader == null) || (PreviewImage == null)) return;

            Span<byte> pixels = ShaderRenderService!.Update(_previewShader.Value);

            using ILockedFramebuffer image = PreviewImage.Lock();
            pixels.CopyTo(new Span<byte>((void*)image.Address, pixels.Length));
        }

        DisplayPreviewImage?.InvalidateVisual();
    }

    private void RecreatePreviewShader()
    {
        lock (_previewLock)
        {
            if (_previewShader != null)
                ShaderRenderService!.UnregisterShader(_previewShader.Value);

            try
            {
                _previewShader = ShaderRenderService!.RegisterShader(_properties.Shader.Shader.CurrentValue, _properties.Shader.Width, _properties.Shader.Height);
                ShaderException = string.Empty;
            }
            catch (Exception ex)
            {
                _previewShader = null;
                ShaderException = ex.Message;
                RecreatePreviewImage();
            }
        }
    }

    private void RecreatePreviewImage()
    {
        lock (_previewLock)
        {
            PreviewImage?.Dispose();
            PreviewImage = new WriteableBitmap(new PixelSize(_properties.Shader.Width, _properties.Shader.Height), new Vector(96, 96), PixelFormat.Bgra8888, AlphaFormat.Opaque);
        }
    }

    private void OnSizeChangedChanged(object? sender, PropertyChangedEventArgs e)
    {
        RecreatePreviewImage();
        RecreatePreviewShader();
    }

    private void OnShaderChangedChanged(object? sender, PropertyChangedEventArgs e)
    {
        RecreatePreviewShader();
    }

    #endregion
}