namespace MovieQuotes.UI.Views.Controls;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using System;

public class TimeSpanControl : TemplatedControl
{
    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<TimeSpanControl, string>(
            nameof(Text),
            string.Empty);

    public static readonly StyledProperty<TimeSpan> TimeProperty =
        AvaloniaProperty.Register<TimeSpanControl, TimeSpan>(
            nameof(Time),
            defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);
    public TimeSpan Time
    {
        get => GetValue(TimeProperty);
        set => SetValue(TimeProperty, value);
    }
    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
    public TimeSpanControl()
    {
        Time = TimeSpan.Zero;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        var incrementBtn = e.NameScope.Find<Button>("IncrementBtn");
        var decrementBtn = e.NameScope.Find<Button>("DecrementBtn");

        incrementBtn!.Click += IncrementBtn_Click;
        decrementBtn!.Click += DecrementBtn_Click;
        base.OnApplyTemplate(e);
    }
    private void IncrementBtn_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Time = Time.Add(TimeSpan.FromMilliseconds(100));
    }

    public void DecrementBtn_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Time = Time.Subtract(TimeSpan.FromMilliseconds(100));
    }

    
}