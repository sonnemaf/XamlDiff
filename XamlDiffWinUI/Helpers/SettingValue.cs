using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Windows.Storage;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace XamlDiff.Helpers; 

public sealed partial class SettingValue<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T> : ObservableObject, ISettingValue<T> {

    private bool _hasValue;

    private Func<T, object> Serializer { get; set; }
    private Func<object, T> Deseriazlier { get; set; }

    public string Key { get; private set; }
    private Lazy<T> DefaultValue { get; set; }
    public Action<T>? OnValueChanged { get; set; }

    public ApplicationDataContainer Container { get; private set; }

    public SettingValue(ApplicationDataContainer container, string key, T defaultValue)
        : this(container, key, new Lazy<T>(() => defaultValue)) {
    }

    public SettingValue(ApplicationDataContainer container, string key, Lazy<T> defaultValue)
        : this(container, key, defaultValue, null) {
    }

    public SettingValue(ApplicationDataContainer container, string key, T defaultValue, Action<T>? onValueChanged)
        : this(container, key, new Lazy<T>(() => defaultValue), onValueChanged) {
    }

    public SettingValue(ApplicationDataContainer container, string key, Lazy<T> defaultValue, Action<T>? onValueChanged)
        : this(container, key, defaultValue, onValueChanged, GetDefaultSerializer(), o => (T)o) {
    }

    public SettingValue(ApplicationDataContainer container, string key, Lazy<T> defaultValue, Func<T, object> serializer, Func<object, T> deseriazlier) 
        : this(container, key, defaultValue, null, serializer, deseriazlier) {
    }

    private static Func<T, object> GetDefaultSerializer() {
        try {
            if (typeof(T).GetTypeInfo().IsEnum) {
                // Convert Enums to Int32
                return o => System.Convert.ToInt32(o, System.Globalization.CultureInfo.InvariantCulture);
            }
        } catch { }
        return static o => o!;
    }

#pragma warning disable CS9264 // Non-nullable property must contain a non-null value when exiting constructor. Consider adding the 'required' modifier, or declaring the property as nullable, or safely handling the case where 'field' is null in the 'get' accessor.
    public SettingValue(ApplicationDataContainer container, string key, Lazy<T> defaultValue, Action<T>? onValueChanged, Func<T, object> serializer, Func<object, T> deseriazlier) {
        ArgumentNullException.ThrowIfNull(container);
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(defaultValue);
        ArgumentNullException.ThrowIfNull(serializer);
        ArgumentNullException.ThrowIfNull(deseriazlier);

        this.Container = container;
        this.Key = key;
        this.DefaultValue = defaultValue;

        this.Serializer = serializer;
        this.Deseriazlier = deseriazlier;

        this.OnValueChanged = onValueChanged;
        OnValueChanged?.Invoke(this.Value!);
    }
#pragma warning restore CS9264 // Non-nullable property must contain a non-null value when exiting constructor. Consider adding the 'required' modifier, or declaring the property as nullable, or safely handling the case where 'field' is null in the 'get' accessor.

    [System.Diagnostics.DebuggerHidden]
    public T Value {
        get {
            if (!_hasValue) {
                _hasValue = true;
                if (Windows.ApplicationModel.DesignMode.DesignModeEnabled) {
                    field = DefaultValue.Value;
                } else {
                    try {
                        Load();
                    } catch {
                        field = DefaultValue.Value;
                        this.Save();
                    }
                }
            }
            return field;
        }
        set {
            if (!EqualityComparer<T>.Default.Equals(field, value)) {
                _hasValue = true;
                field = value;
                if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled) {
                    try {
                        this.Save();
                    } catch (Exception ex) {
                        System.Diagnostics.Debug.WriteLine(ex);
                    }
                    OnPropertyChanged(nameof(Value));
                    OnValueChanged?.Invoke(value);
                }
            }
        }
    }

    public void Load() {
        _hasValue = true;
        try {
            Value = this.Container.Values.TryGetValue(this.Key, out var o) ? Deseriazlier(o) : DefaultValue.Value;
        } catch {
            Value = DefaultValue.Value;
        }
    }

    public void Save() {
        try {
            this.Container.Values[this.Key] = Serializer(this.Value);
        } catch { }
    }
}

public interface ISettingValue<T> {
    T Value { get; set; }

    void Save();
}

public class TestSettingValue<T> : ISettingValue<T> {
    public T Value { get; set; }

    public TestSettingValue(T value) {
        this.Value = value;
    }

    public void Save() {
    }
}

