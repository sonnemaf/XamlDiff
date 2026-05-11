using Microsoft.UI.Xaml;
using XamlDiff.ViewModels;
using XamlDiff.Views.Pages;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace XamlDiff.Views.Windows;

/// <summary>
/// The application window. This hosts a Frame that displays pages. Add your UI and logic to MainPage.xaml /
/// MainPage.xaml.cs instead of here so you can use Page features such as navigation events and the Loaded lifecycle.
/// </summary>
public sealed partial class MainWindow : Window {

    public MainViewModel ViewModel { get; } = MainViewModel.Current;

    public MainWindow() {
        InitializeComponent();

        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);
        SetAppWindowIcon();
        AppTitleBar.ActualThemeChanged += AppTitleBar_ActualThemeChanged;

        // Navigate the root frame to the main page on startup.
        RootFrame.Navigate(typeof(MainPage));
    }

    private void SetAppWindowIcon() {
        return; // TODO: AppWindow.SetIcon() does not work with correct icon in Taskbar

        if (AppTitleBar.ActualTheme == ElementTheme.Light) {
            AppWindow.SetIcon("/Assets/Tiles/IconLight.ico");
        } else {
            AppWindow.SetIcon("/Assets/Tiles/IconDark.ico");
        }
    }

    private void AppTitleBar_ActualThemeChanged(FrameworkElement sender, object args) {
        SetAppWindowIcon();
    }

    private void RootFrame_Navigated(object sender, Microsoft.UI.Xaml.Navigation.NavigationEventArgs e) {
        AppTitleBar.IsBackButtonVisible = RootFrame.CanGoBack;
    }

    private void AppTitleBar_BackRequested(Microsoft.UI.Xaml.Controls.TitleBar sender, object args) {
        if (this.RootFrame.CanGoBack) {
            this.RootFrame.GoBack();
        }
    }
}
