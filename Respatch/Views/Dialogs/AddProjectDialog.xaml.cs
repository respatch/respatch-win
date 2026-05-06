using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Respatch.Navigation;
using Respatch.ViewModels;

namespace Respatch.Views.Dialogs;

public sealed partial class AddProjectDialog : ContentDialog
{
    public AddProjectViewModel ViewModel { get; }

    public AddProjectDialog()
    {
        InitializeComponent();
        ViewModel = App.Services.GetRequiredService<AddProjectViewModel>();

        // Close dialog automatically when project is saved
        ViewModel.ProjectSaved += (_, _) =>
        {
            Hide();
            // Navigate to main after saving
            var nav = App.Services.GetRequiredService<NavigationService>();
            nav.NavigateToMain();
        };
    }
}
