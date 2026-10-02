using CommunityToolkit.Mvvm.ComponentModel;

namespace KrookiKoomer.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    public partial string Greeting { get; set; } = "Welcome to Avalonia!";
}
