using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace Respatch.Controls;

public enum BadgeStyle { Busy, Idle }

/// <summary>
/// Colored numeric badge. Replaces .numeric-badge + .is-busy/.is-idle CSS classes.
/// Busy = red, Idle = green.
/// </summary>
public sealed partial class NumericBadge : UserControl
{
    public static readonly DependencyProperty CountProperty =
        DependencyProperty.Register(nameof(Count), typeof(int), typeof(NumericBadge),
            new PropertyMetadata(0, OnCountChanged));

    public static readonly DependencyProperty BadgeStyleProperty =
        DependencyProperty.Register(nameof(BadgeStyle), typeof(BadgeStyle), typeof(NumericBadge),
            new PropertyMetadata(BadgeStyle.Idle, OnBadgeStyleChanged));

    public int Count
    {
        get => (int)GetValue(CountProperty);
        set => SetValue(CountProperty, value);
    }

    public BadgeStyle BadgeStyle
    {
        get => (BadgeStyle)GetValue(BadgeStyleProperty);
        set => SetValue(BadgeStyleProperty, value);
    }

    public NumericBadge()
    {
        InitializeComponent();
        UpdateAppearance();
    }

    private static void OnCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is NumericBadge badge)
        {
            badge.CountText.Text = e.NewValue?.ToString() ?? "0";
        }
    }

    private static void OnBadgeStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is NumericBadge badge)
            badge.UpdateAppearance();
    }

    private void UpdateAppearance()
    {
        BadgeBorder.Background = BadgeStyle switch
        {
            BadgeStyle.Busy => new SolidColorBrush(Colors.Crimson),
            BadgeStyle.Idle => new SolidColorBrush(Colors.SeaGreen),
            _ => new SolidColorBrush(Colors.Gray)
        };
    }
}
