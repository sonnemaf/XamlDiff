using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.Windows.Storage;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Popups;
using XamlDiff.Helpers;
using XamlDiff.Models;

namespace XamlDiff.ViewModels;

/// <summary>
/// Sample ViewModel using CommunityToolkit.Mvvm partial property syntax. Uses <see cref="ObservableProperty"/> for
/// change notification and <see cref="RelayCommand"/> for command binding.
/// </summary>
public sealed partial class MainViewModel : ObservableObject {

    public static MainViewModel Current { get; } = new();

    


    private Microsoft.Windows.Storage.Pickers.FileOpenPicker? _fp;

    public Diff Diff { get; } = new();

    public SettingValue<bool> GenerateStateTrigger { get; } = new SettingValue<bool>(ApplicationData.GetDefault().LocalSettings, "GenerateStateTrigger", true);
    public SettingValue<bool> GenerateVisualState { get; } = new SettingValue<bool>(ApplicationData.GetDefault().LocalSettings, "GenerateVisualState", true);
    public SettingValue<bool> GenerateVisualStateGroup { get; } = new SettingValue<bool>(ApplicationData.GetDefault().LocalSettings, "GenerateVisualStateGroup", true);
    public SettingValue<bool> GenerateVisualStateGroups { get; } = new SettingValue<bool>(ApplicationData.GetDefault().LocalSettings, "GenerateVisualStateGroups", true);
    public SettingValue<bool> IncludeSource { get; } = new SettingValue<bool>(ApplicationData.GetDefault().LocalSettings, "IncludeSource", false);
    public SettingValue<string> Source { get; } = new SettingValue<string>(ApplicationData.GetDefault().LocalSettings, "Source", """
        <Page xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
            <Grid>
                <TextBlock x:Name="TextBlock1" Text="Hello World" />
            </Grid>
        </Page>
        """);
    public SettingValue<string> Destination { get; } = new SettingValue<string>(ApplicationData.GetDefault().LocalSettings, "Destination", """
        <Page xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
            <Grid>
                <TextBlock x:Name="TextBlock1" Text="Bye Bye" Foreground="Red" />
            </Grid>
        </Page>
        """);
    public SettingValue<ElementTheme> Theme { get; }

    private MainViewModel() {
        Theme = new SettingValue<ElementTheme>(ApplicationData.GetDefault().LocalSettings, "Theme", ElementTheme.Default, v => {
            // if (Theme is not null && MasterPage.Current is not null) SetTheme();
        });
    }

    [RelayCommand]
    private void Generate() {
        Diff.Generate(Source.Value, Destination.Value, IncludeSource.Value, GenerateVisualStateGroups.Value, GenerateVisualStateGroup.Value, GenerateVisualState.Value, GenerateStateTrigger.Value);
    }

    [RelayCommand]
    private void CopyOutput() {
        try {
            var dp = new DataPackage();
            dp.SetText(Diff.Output);
            Clipboard.SetContent(dp);
        } catch { }
    }


    [RelayCommand]
    private void PasteSource() {
        Paste(Source);
    }

    [RelayCommand]
    private void OpenSource() {
        OpenFile(Source);
    }

    [RelayCommand]
    private void PasteDestination() {
        Paste(Destination);
    }

    [RelayCommand]
    private void OpenDestination() {
        OpenFile(Destination);
    }


    private static async void Paste(SettingValue<string> setting) {
        try {
            var dp = Clipboard.GetContent();
            setting.Value = await dp.GetTextAsync();
        } catch { }
    }

    private async void OpenFile(SettingValue<string> setting) {
        var picker = _fp ??= new Microsoft.Windows.Storage.Pickers.FileOpenPicker(App.Window.AppWindow.Id);

        if (picker.FileTypeFilter.Count == 0) {
            picker.FileTypeFilter.Add(".xaml");
            picker.FileTypeFilter.Add(".txt");
        }

        try {
            var file = await picker.PickSingleFileAsync();
            if (file is not null) {
                var content = await System.IO.File.ReadAllTextAsync(file.Path);
                setting.Value = content;
            }
        } catch {
            await new MessageDialog("Error reading file").ShowAsync();
        }
    }

}
