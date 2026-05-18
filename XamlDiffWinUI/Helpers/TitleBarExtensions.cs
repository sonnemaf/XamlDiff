using CommunityToolkit.WinUI.Helpers;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace XamlDiff.Helpers;

public static class TitleBarExtensions {

    #region ButtonForegroundColor Attached Property

    /// <summary>
    /// Identifies the ButtonForeground attachted property. This enables animation, styling, binding, etc...
    /// </summary>
    public static readonly DependencyProperty ButtonForegroundColorProperty =
        DependencyProperty.RegisterAttached("ButtonForegroundColor",
                                            typeof(object),
                                            typeof(TitleBarExtensions),
                                            new PropertyMetadata(null, OnButtonForegroundColorChanged));

    /// <summary>
    /// ButtonForeground changed handler.
    /// </summary>
    /// <param name="d">FrameworkElement that changed its ButtonForeground attached property.</param>
    /// <param name="e">DependencyPropertyChangedEventArgs with the new and old value.</param>
    private static void OnButtonForegroundColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        d.DispatcherQueue.TryEnqueue(() => {
            GetTitleBar()?.ButtonForegroundColor = GetColor(e.NewValue);
        });
    }

    private static void Tbc_Loaded(object sender, RoutedEventArgs e) {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Gets the value of the ButtonForeground attached property from the specified FrameworkElement.
    /// </summary>
    public static object GetButtonForegroundColor(DependencyObject obj) {
        return (object)obj.GetValue(ButtonForegroundColorProperty);
    }


    /// <summary>
    /// Sets the value of the ButtonForeground attached property to the specified FrameworkElement.
    /// </summary>
    /// <param name="obj">The object on which to set the ButtonForeground attached property.</param>
    /// <param name="value">The property value to set.</param>
    public static void SetButtonForegroundColor(DependencyObject obj, object value) {
        obj.SetValue(ButtonForegroundColorProperty, value);
    }

    #endregion ButtonForeground Attached Property


    #region ButtonBackgroundColor Attached Property

    /// <summary>
    /// Identifies the ButtonBackgroundColor attachted property. This enables animation, styling, binding, etc...
    /// </summary>
    public static readonly DependencyProperty ButtonBackgroundColorProperty =
        DependencyProperty.RegisterAttached("ButtonBackgroundColor",
                                            typeof(object),
                                            typeof(TitleBarExtensions),
                                            new PropertyMetadata(null, OnButtonBackgroundColorChanged));

    /// <summary>
    /// ButtonBackgroundColor changed handler.
    /// </summary>
    /// <param name="d">FrameworkElement that changed its ButtonBackgroundColor attached property.</param>
    /// <param name="e">DependencyPropertyChangedEventArgs with the new and old value.</param>
    private static void OnButtonBackgroundColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        d.DispatcherQueue.TryEnqueue(() => {
            GetTitleBar()?.ButtonBackgroundColor = GetColor(e.NewValue);
        });
    }

    /// <summary>
    /// Gets the value of the ButtonBackgroundColor attached property from the specified FrameworkElement.
    /// </summary>
    public static object GetButtonBackgroundColor(DependencyObject obj) {
        return (object)obj.GetValue(ButtonBackgroundColorProperty);
    }


    /// <summary>
    /// Sets the value of the ButtonBackgroundColor attached property to the specified FrameworkElement.
    /// </summary>
    /// <param name="obj">The object on which to set the ButtonBackgroundColor attached property.</param>
    /// <param name="value">The property value to set.</param>
    public static void SetButtonBackgroundColor(DependencyObject obj, object value) {
        obj.SetValue(ButtonBackgroundColorProperty, value);
    }

    #endregion ButtonBackgroundColor Attached Property


    #region ButtonHoverBackgroundColor Attached Property

    /// <summary>
    /// Identifies the ButtonHoverBackgroundColor attachted property. This enables animation, styling, binding, etc...
    /// </summary>
    public static readonly DependencyProperty ButtonHoverBackgroundColorProperty =
        DependencyProperty.RegisterAttached("ButtonHoverBackgroundColor",
                                            typeof(object),
                                            typeof(TitleBarExtensions),
                                            new PropertyMetadata(null, OnButtonHoverBackgroundColorChanged));

    /// <summary>
    /// ButtonHoverBackgroundColor changed handler.
    /// </summary>
    /// <param name="d">FrameworkElement that changed its ButtonHoverBackgroundColor attached property.</param>
    /// <param name="e">DependencyPropertyChangedEventArgs with the new and old value.</param>
    private static async void OnButtonHoverBackgroundColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        d.DispatcherQueue.TryEnqueue(() => {
            GetTitleBar()?.ButtonHoverBackgroundColor = GetColor(e.NewValue);
        });
    }

    /// <summary>
    /// Gets the value of the ButtonHoverBackgroundColor attached property from the specified FrameworkElement.
    /// </summary>
    public static object GetButtonHoverBackgroundColor(DependencyObject obj) => (object)obj.GetValue(ButtonHoverBackgroundColorProperty);


    /// <summary>
    /// Sets the value of the ButtonHoverBackgroundColor attached property to the specified FrameworkElement.
    /// </summary>
    /// <param name="obj">The object on which to set the ButtonHoverBackgroundColor attached property.</param>
    /// <param name="value">The property value to set.</param>
    public static void SetButtonHoverBackgroundColor(DependencyObject obj, object value) => obj.SetValue(ButtonHoverBackgroundColorProperty, value);

    #endregion ButtonHoverBackgroundColor Attached Property


    #region ButtonHoverForegroundColor Attached Property

    /// <summary>
    /// Identifies the ButtonHoverForegroundColor attachted property. This enables animation, styling, binding, etc...
    /// </summary>
    public static readonly DependencyProperty ButtonHoverForegroundColorProperty =
        DependencyProperty.RegisterAttached("ButtonHoverForegroundColor",
                                            typeof(object),
                                            typeof(TitleBarExtensions),
                                            new PropertyMetadata(null, OnButtonHoverForegroundColorChanged));

    /// <summary>
    /// ButtonHoverForegroundColor changed handler.
    /// </summary>
    /// <param name="d">FrameworkElement that changed its ButtonHoverForegroundColor attached property.</param>
    /// <param name="e">DependencyPropertyChangedEventArgs with the new and old value.</param>
    private static async void OnButtonHoverForegroundColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        d.DispatcherQueue.TryEnqueue(() => {
            GetTitleBar()?.ButtonHoverForegroundColor = GetColor(e.NewValue);
        });
    }

    /// <summary>
    /// Gets the value of the ButtonHoverForegroundColor attached property from the specified FrameworkElement.
    /// </summary>
    public static object GetButtonHoverForegroundColor(DependencyObject obj) => (object)obj.GetValue(ButtonHoverForegroundColorProperty);


    /// <summary>
    /// Sets the value of the ButtonHoverForegroundColor attached property to the specified FrameworkElement.
    /// </summary>
    /// <param name="obj">The object on which to set the ButtonHoverForegroundColor attached property.</param>
    /// <param name="value">The property value to set.</param>
    public static void SetButtonHoverForegroundColor(DependencyObject obj, object value) => obj.SetValue(ButtonHoverForegroundColorProperty, value);

    #endregion ButtonHoverForegroundColor Attached Property


    #region ButtonPressedForegroundColor Attached Property

    /// <summary>
    /// Identifies the ButtonPressedForeground attachted property. This enables animation, styling, binding, etc...
    /// </summary>
    public static readonly DependencyProperty ButtonPressedForegroundColorProperty =
        DependencyProperty.RegisterAttached("ButtonPressedForegroundColor",
                                            typeof(object),
                                            typeof(TitleBarExtensions),
                                            new PropertyMetadata(null, OnButtonPressedForegroundColorChanged));

    /// <summary>
    /// ButtonPressedForeground changed handler.
    /// </summary>
    /// <param name="d">FrameworkElement that changed its ButtonPressedForeground attached property.</param>
    /// <param name="e">DependencyPropertyChangedEventArgs with the new and old value.</param>
    private static async void OnButtonPressedForegroundColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        d.DispatcherQueue.TryEnqueue(() => {
            GetTitleBar()?.ButtonPressedForegroundColor = GetColor(e.NewValue);
        });
    }

    /// <summary>
    /// Gets the value of the ButtonPressedForeground attached property from the specified FrameworkElement.
    /// </summary>
    public static object GetButtonPressedForegroundColor(DependencyObject obj) => (object)obj.GetValue(ButtonPressedForegroundColorProperty);


    /// <summary>
    /// Sets the value of the ButtonPressedForeground attached property to the specified FrameworkElement.
    /// </summary>
    /// <param name="obj">The object on which to set the ButtonPressedForeground attached property.</param>
    /// <param name="value">The property value to set.</param>
    public static void SetButtonPressedForegroundColor(DependencyObject obj, object value) => obj.SetValue(ButtonPressedForegroundColorProperty, value);

    #endregion ButtonPressedForeground Attached Property


    #region ButtonPressedBackgroundColor Attached Property

    /// <summary>
    /// Identifies the ButtonPressedBackgroundColor attachted property. This enables animation, styling, binding, etc...
    /// </summary>
    public static readonly DependencyProperty ButtonPressedBackgroundColorProperty =
        DependencyProperty.RegisterAttached("ButtonPressedBackgroundColor",
                                            typeof(object),
                                            typeof(TitleBarExtensions),
                                            new PropertyMetadata(null, OnButtonPressedBackgroundColorChanged));

    /// <summary>
    /// ButtonPressedBackgroundColor changed handler.
    /// </summary>
    /// <param name="d">FrameworkElement that changed its ButtonPressedBackgroundColor attached property.</param>
    /// <param name="e">DependencyPropertyChangedEventArgs with the new and old value.</param>
    private static async void OnButtonPressedBackgroundColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        d.DispatcherQueue.TryEnqueue(() => {
            GetTitleBar()?.ButtonPressedBackgroundColor = GetColor(e.NewValue);
        });
    }

    /// <summary>
    /// Gets the value of the ButtonPressedBackgroundColor attached property from the specified FrameworkElement.
    /// </summary>
    public static object GetButtonPressedBackgroundColor(DependencyObject obj) => (object)obj.GetValue(ButtonPressedBackgroundColorProperty);


    /// <summary>
    /// Sets the value of the ButtonPressedBackgroundColor attached property to the specified FrameworkElement.
    /// </summary>
    /// <param name="obj">The object on which to set the ButtonPressedBackgroundColor attached property.</param>
    /// <param name="value">The property value to set.</param>
    public static void SetButtonPressedBackgroundColor(DependencyObject obj, object value) => obj.SetValue(ButtonPressedBackgroundColorProperty, value);

    #endregion ButtonPressedBackgroundColor Attached Property


    #region ButtonInactiveForegroundColor Attached Property

    /// <summary>
    /// Identifies the ButtonInactiveForegroundColor attachted property. This enables animation, styling, binding,
    /// etc...
    /// </summary>
    public static readonly DependencyProperty ButtonInactiveForegroundColorProperty =
        DependencyProperty.RegisterAttached("ButtonInactiveForegroundColor",
                                            typeof(object),
                                            typeof(TitleBarExtensions),
                                            new PropertyMetadata(null, OnButtonInactiveForegroundColorChanged));

    /// <summary>
    /// ButtonInactiveForegroundColor changed handler.
    /// </summary>
    /// <param name="d">FrameworkElement that changed its ButtonInactiveForegroundColor attached property.</param>
    /// <param name="e">DependencyPropertyChangedEventArgs with the new and old value.</param>
    private static void OnButtonInactiveForegroundColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        d.DispatcherQueue.TryEnqueue(() => {
            GetTitleBar()?.ButtonInactiveForegroundColor = GetColor(e.NewValue);
        });
    }

    /// <summary>
    /// Gets the value of the ButtonInactiveForegroundColor attached property from the specified FrameworkElement.
    /// </summary>
    public static object GetButtonInactiveForegroundColor(DependencyObject obj) {
        return (object)obj.GetValue(ButtonInactiveForegroundColorProperty);
    }


    /// <summary>
    /// Sets the value of the ButtonInactiveForegroundColor attached property to the specified FrameworkElement.
    /// </summary>
    /// <param name="obj">The object on which to set the ButtonInactiveForegroundColor attached property.</param>
    /// <param name="value">The property value to set.</param>
    public static void SetButtonInactiveForegroundColor(DependencyObject obj, object value) {
        obj.SetValue(ButtonInactiveForegroundColorProperty, value);
    }

    #endregion ButtonInactiveForegroundColor Attached Property


    #region ButtonInactiveBackgroundColor Attached Property

    /// <summary>
    /// Identifies the ButtonInactiveBackgroundColor attachted property. This enables animation, styling, binding,
    /// etc...
    /// </summary>
    public static readonly DependencyProperty ButtonInactiveBackgroundColorProperty =
        DependencyProperty.RegisterAttached("ButtonInactiveBackgroundColor",
                                            typeof(object),
                                            typeof(TitleBarExtensions),
                                            new PropertyMetadata(null, OnButtonInactiveBackgroundColorChanged));

    /// <summary>
    /// ButtonInactiveBackgroundColor changed handler.
    /// </summary>
    /// <param name="d">FrameworkElement that changed its ButtonInactiveBackgroundColor attached property.</param>
    /// <param name="e">DependencyPropertyChangedEventArgs with the new and old value.</param>
    private static void OnButtonInactiveBackgroundColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        d.DispatcherQueue.TryEnqueue(() => {
            GetTitleBar()?.ButtonInactiveBackgroundColor = GetColor(e.NewValue);
        });
    }

    /// <summary>
    /// Gets the value of the ButtonInactiveBackgroundColor attached property from the specified FrameworkElement.
    /// </summary>
    public static object GetButtonInactiveBackgroundColor(DependencyObject obj) {
        return (object)obj.GetValue(ButtonInactiveBackgroundColorProperty);
    }


    /// <summary>
    /// Sets the value of the ButtonInactiveBackgroundColor attached property to the specified FrameworkElement.
    /// </summary>
    /// <param name="obj">The object on which to set the ButtonInactiveBackgroundColor attached property.</param>
    /// <param name="value">The property value to set.</param>
    public static void SetButtonInactiveBackgroundColor(DependencyObject obj, object value) {
        obj.SetValue(ButtonInactiveBackgroundColorProperty, value);
    }

    #endregion ButtonInactiveBackgroundColor Attached Property


    private static Color GetColor(object color) =>
        color switch {
            string colorText => colorText.ToColor(),
            SolidColorBrush b => b.Color,
            { } x => (Color)x,
        };

    private static AppWindowTitleBar? GetTitleBar() => XamlDiff.App.Window!.AppWindow.TitleBar;

}
