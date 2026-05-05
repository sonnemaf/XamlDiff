using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace XamlDiff.Helpers;

public abstract class EnumToIntConverter<T> : IValueConverter where T : Enum {

    public virtual object Convert(object value, Type targetType, object parameter, string language) {
        try {
            return System.Convert.ChangeType(value, Enum.GetUnderlyingType(value.GetType()));
        } catch {
            return DependencyProperty.UnsetValue;
        }
    }

    public virtual object ConvertBack(object value, Type targetType, object parameter, string language) {
        try {
            return (T)value;
        } catch {
            return DependencyProperty.UnsetValue;
        }
    }
}