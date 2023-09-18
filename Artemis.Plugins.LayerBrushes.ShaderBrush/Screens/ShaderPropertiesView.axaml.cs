using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;

namespace Artemis.Plugins.LayerBrushes.ShaderBrush.Screens;

public partial class ShaderPropertiesView : ReactiveUserControl<ShaderPropertiesViewModel>
{
    public ShaderPropertiesView()
    {
        InitializeComponent();
    }

    private void InputFinished(object? sender, RoutedEventArgs e)
    {
        ViewModel?.Save();
    }

    private void PointerInputFinished(object? sender, PointerCaptureLostEventArgs pointerCaptureLostEventArgs)
    {
        ViewModel?.Save();
    }
}