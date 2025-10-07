namespace MovieQuotes.UI;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Primitives;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Avalonia.Media;

public class RichTextBlock : TemplatedControl
{

    public static readonly StyledProperty<string> TextProperty =
            AvaloniaProperty.Register<RichTextBlock, string>(
                nameof(Text),
                string.Empty);
    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public RichTextBlock()
    {
        Text = "hello <b>this is</b> html";
    }


    private TextBlock? _textBlock;
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _textBlock = e.NameScope.Find<TextBlock>("PART_Text");
        UpdateInlines();
    }

    private void UpdateInlines()
    {
        if (_textBlock == null || Text == null)
            return;

        _textBlock.Inlines = new InlineCollection();

        _textBlock.Inlines.AddRange(ParseHtml(Text));

    }

    private IEnumerable<Inline> ParseHtml(string html)
    {
        return new List<Inline>()
        {
            new Run(html),
            new Run {
                Text = "this is red",
                Foreground = Brushes.Red
            } ,
            new Run {
                Text = " this is bold",
                FontWeight = FontWeight.Bold
            } ,
        };
    }
}