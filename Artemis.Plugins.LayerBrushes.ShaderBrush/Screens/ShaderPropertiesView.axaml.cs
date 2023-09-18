using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using ReactiveUI;

namespace Artemis.Plugins.LayerBrushes.ShaderBrush.Screens;

public partial class ShaderPropertiesView : ReactiveUserControl<ShaderPropertiesViewModel>
{
    public ShaderPropertiesView()
    {
        InitializeComponent();

        this.WhenActivated(d =>
                           {
                               ViewModel!.DisplayPreviewImage = DisplayPreviewImage;
                           });
    }

    private void InputFinished(object? sender, RoutedEventArgs e)
    {
        ViewModel?.Save();
    }
}