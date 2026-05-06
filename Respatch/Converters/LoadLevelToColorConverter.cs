using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;

namespace Respatch.Converters;

/// <summary>
/// Converts a worker load fraction (0.0–1.0) to a SolidColorBrush.
/// Load > 0.8 → red (high-load), otherwise accent green.
/// Replaces .worker-progress.high-load CSS class.
/// </summary>
public class LoadLevelToColorConverter : IValueConverter
{
    private static readonly SolidColorBrush HighLoadBrush = new(Colors.Crimson);
    private static readonly SolidColorBrush NormalBrush = new(Colors.SeaGreen);

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is double fraction)
            return fraction > 0.8 ? HighLoadBrush : NormalBrush;
        return NormalBrush;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}
