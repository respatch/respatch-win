using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Respatch.Navigation;

namespace Respatch.ViewModels;

public partial class WelcomeViewModel : ObservableObject
{
    private readonly NavigationService _navigation;

    public WelcomeViewModel(NavigationService navigation)
    {
        _navigation = navigation;
    }

    [RelayCommand]
    private async Task OpenAddProjectDialogAsync()
    {
        await _navigation.ShowAddProjectDialogAsync();
    }
}
