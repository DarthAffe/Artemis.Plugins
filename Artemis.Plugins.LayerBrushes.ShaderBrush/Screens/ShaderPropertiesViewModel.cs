using System.Reactive.Disposables;
using Artemis.Plugins.LayerBrushes.ShaderBrush.LayerBrushes;
using Artemis.Plugins.LayerBrushes.ShaderBrush.LayerBrushes.PropertyGroups;
using Artemis.UI.Shared.LayerBrushes;
using Artemis.UI.Shared.Services;
using Avalonia.Threading;
using ReactiveUI;

namespace Artemis.Plugins.LayerBrushes.ShaderBrush.Screens;

public class ShaderPropertiesViewModel : BrushConfigurationViewModel
{
    #region Properties & Fields

    private readonly ShaderBrushPropertyGroup _properties;
    private readonly DispatcherTimer _updateTimer;
    private readonly IWindowService _windowService;
    private bool _saveOnChange;

    private string _shader;
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

    public ShaderBrushLayerBrush ShaderBrushLayerBrush { get; }

    #endregion

    #region Constructors

    public ShaderPropertiesViewModel(ShaderBrushLayerBrush layerBrush, IWindowService windowService)
        : base(layerBrush)
    {
        this._windowService = windowService;
        this.ShaderBrushLayerBrush = layerBrush;

        _properties = layerBrush.Properties;

        //_updateTimer = new DispatcherTimer(TimeSpan.FromMilliseconds(1 / 30.0), DispatcherPriority.Normal, (_, _) => Update());
        //_updateTimer.Start();

        this.WhenActivated(d =>
                           {
                               Initialize(d);
                               //this.WhenAnyValue(vm => vm.SelectedCaptureScreen).Subscribe(OnSelectedCaptureScreenChanged);

                               _saveOnChange = true;
                           });
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

        //Task.Run(() => AmbilightLayerBrush.RecreateCaptureZone()).ConfigureAwait(false);
    }

    private void Initialize(CompositeDisposable d)
    {
        //if (!await CreateCaptureScreens())
        //    return;

        Load();
        //EnableValidation = true;

        //Disposable.Create(() =>
        //{
        //    CaptureScreens.Clear();
        //    _updateTimer.Stop();
        //}).DisposeWith(d);
    }

    #endregion
}