using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel;
using Windows.Services.Store;
using Windows.System;
using XamlDiff.ViewModels;

namespace XamlDiff.Views.Pages;

public sealed partial class SettingsPage : Page {

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "x:Bind must be instance")]
    private string AppVersion => GetAppVersion(false);

    public MainViewModel ViewModel { get; } = MainViewModel.Current;

    public SettingsPage() {
        InitializeComponent();
    }

    private async void ButtonSupporMail_Click(object sender, RoutedEventArgs e) {
        try {
            var url = $"mailto:support@reflectionit.nl?subject=Feedback on XAML Diff";
            await Launcher.LaunchUriAsync(new Uri(url));
        } catch { }
    }

    private async void ButtonRate_Click(object sender, RoutedEventArgs e) {
        try {
            //var result = await StoreContext.GetDefault().RequestRateAndReviewAppAsync();

            var url = $"ms-windows-store://review/?ProductId={Package.Current.Id.ProductId}";
            await Launcher.LaunchUriAsync(new Uri(url));
        } catch { }
    }

    public static string GetAppVersion(bool showAppName = false) {
        var version = Package.Current.Id.Version;
        var displayName = showAppName ? Package.Current.DisplayName + ": " : string.Empty;
        return $"{displayName} {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
    }
}
