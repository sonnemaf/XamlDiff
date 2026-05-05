using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using XamlDiff.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace XamlDiff.Views.Pages;

/// <summary>
/// The main content page displayed inside the application window.
/// </summary>
public sealed partial class MainPage : Page {
    public MainViewModel ViewModel { get; } = MainViewModel.Current;

    public MainPage() {
        InitializeComponent();

        var options = /*lang=json-strict*/ """
            {
                "stickyScroll": { "enabled": false },
                "fontSize": 14,
                "minimap": { "enabled": false },
                "wordWrap": "bounded",
                "wordWrapColumn": 100
            }
            """;

        editorSource.SetOptionsAsync(options);
        editorDestination.SetOptionsAsync(options);
        editorOutput.SetOptionsAsync(options);

        ViewModel.GenerateCommand.Execute(null);
    }

    private void Settings_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) {
        this.Frame.Navigate(typeof(SettingsPage));
    }

    public string EditorTheme => this.ActualTheme == ElementTheme.Dark ? "vs-dark" : "vs";

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "databinding")]
    private Visibility IsEmpty(int count) => count == 0 ? Visibility.Visible : Visibility.Collapsed;
    
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "databinding")]
    private Visibility IsNotEmpty(int count) => count != 0 ? Visibility.Visible : Visibility.Collapsed;
}
