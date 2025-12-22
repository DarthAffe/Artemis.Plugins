using Artemis.Plugins.Devices.Wled.ViewModels;
using ReactiveUI.Avalonia;

namespace Artemis.Plugins.Devices.Wled.Views;

public partial class WledConfigurationView : ReactiveUserControl<WledConfigurationViewModel>
{
    public WledConfigurationView()
    {
        InitializeComponent();
    }
}