using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.Helpers;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI;
using Windows.UI.ViewManagement;

namespace XamlDiff.Helpers;

public static class TitleBarEx {

    private static AppWindowTitleBar? GetTitleBar() => XamlDiff.App.Window!.AppWindow.TitleBar;


    #region ButtonForeground Attached Property

    /// <summary>
    /// Identifies the ButtonForeground attachted property. This enables animation, styling, binding, etc...
    /// </summary>
    public static readonly DependencyProperty ButtonForegroundProperty =
        DependencyProperty.RegisterAttached("ButtonForeground",
                                            typeof(object),
                                            typeof(TitleBarEx),
                                            new PropertyMetadata(null, OnButtonForegroundChanged));

    /// <summary>
    /// ButtonForeground changed handler.
    /// </summary>
    /// <param name="d">FrameworkElement that changed its ButtonForeground attached property.</param>
    /// <param name="e">DependencyPropertyChangedEventArgs with the new and old value.</param>
    private static async void OnButtonForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if (d is TitleBar tbc) {
            Color value = GetColor(e.NewValue);
            if (XamlDiff.App.Window is null) {
                tbc.Loaded += async (_, _) => {
                    GetTitleBar()?.ButtonForegroundColor = Microsoft.UI.Colors.Transparent;
                    await Task.Delay(50);
                    GetTitleBar()?.ButtonForegroundColor = value;
                };
            } else {
                GetTitleBar()?.ButtonForegroundColor = value;
            }
        }
    }

    private static void Tbc_Loaded(object sender, RoutedEventArgs e) {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Gets the value of the ButtonForeground attached property from the specified FrameworkElement.
    /// </summary>
    public static object GetButtonForeground(DependencyObject obj) {
        return (object)obj.GetValue(ButtonForegroundProperty);
    }


    /// <summary>
    /// Sets the value of the ButtonForeground attached property to the specified FrameworkElement.
    /// </summary>
    /// <param name="obj">The object on which to set the ButtonForeground attached property.</param>
    /// <param name="value">The property value to set.</param>
    public static void SetButtonForeground(DependencyObject obj, object value) {
        obj.SetValue(ButtonForegroundProperty, value);
    }

    #endregion ButtonForeground Attached Property


    private static Color GetColor(object color) {
        return (color) switch {
            string s => s.ToColor(),
            SolidColorBrush b => b.Color,
            { } x => (Color)x,
        };
    }
}
