using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;

namespace Respatch.Converters;

/// <summary>
/// Converts bool to Visibility. Pass ConverterParameter="Invert" to reverse.
/// Also used as bool→bool inverter when target type is bool (IsEnabled bindings).
/// </summary>
public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var boolValue = value is bool b && b;
        var invert = parameter is string s && s.Equals("Invert", StringComparison.OrdinalIgnoreCase);

        if (invert) boolValue = !boolValue;

        if (targetType == typeof(bool)) return boolValue;
        return boolValue ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        var invert = parameter is string s && s.Equals("Invert", StringComparison.OrdinalIgnoreCase);
        var result = value is Visibility v ? v == Visibility.Visible : value is bool b && b;
        return invert ? !result : result;
    }
}
