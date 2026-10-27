using System.Globalization;
using System.Text.RegularExpressions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using MovieQuotes.Subtitles.Models;
using MovieQuotes.Subtitles.Srt;

namespace MovieQuotes.UI.Controls;

public sealed class SubtitleControl : Control
{
    private static readonly Regex WrappingPartsRegex = new(
        @"\S+\s*|\s+",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly Vector[] OutlineOffsets =
    [
        new(-1, 0),
        new(1, 0),
        new(0, -1),
        new(0, 1),
        new(-1, -1),
        new(1, -1),
        new(-1, 1),
        new(1, 1)
    ];

    public static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<SubtitleControl, string?>(nameof(Text));

    public static readonly StyledProperty<double> FontSizeProperty =
        AvaloniaProperty.Register<SubtitleControl, double>(
            nameof(FontSize),
            32);

    public static readonly StyledProperty<FontFamily> FontFamilyProperty =
        AvaloniaProperty.Register<SubtitleControl, FontFamily>(
            nameof(FontFamily),
            FontFamily.Default);

    public static readonly StyledProperty<IBrush?> ForegroundProperty =
        AvaloniaProperty.Register<SubtitleControl, IBrush?>(
            nameof(Foreground),
            Brushes.White);

    public static readonly StyledProperty<IBrush?> OutlineBrushProperty =
        AvaloniaProperty.Register<SubtitleControl, IBrush?>(
            nameof(OutlineBrush),
            Brushes.Black);

    public static readonly StyledProperty<double> OutlineThicknessProperty =
        AvaloniaProperty.Register<SubtitleControl, double>(
            nameof(OutlineThickness),
            2);

    public static readonly StyledProperty<double> LineSpacingProperty =
        AvaloniaProperty.Register<SubtitleControl, double>(
            nameof(LineSpacing),
            4);

    public static readonly StyledProperty<TextAlignment> TextAlignmentProperty =
        AvaloniaProperty.Register<SubtitleControl, TextAlignment>(
            nameof(TextAlignment),
            TextAlignment.Center);

    public static readonly StyledProperty<HorizontalAlignment> HorizontalTextAlignmentProperty =
        AvaloniaProperty.Register<SubtitleControl, HorizontalAlignment>(
            nameof(HorizontalTextAlignment),
            HorizontalAlignment.Center);

    public static readonly StyledProperty<VerticalAlignment> VerticalTextAlignmentProperty =
        AvaloniaProperty.Register<SubtitleControl, VerticalAlignment>(
            nameof(VerticalTextAlignment),
            VerticalAlignment.Center);

    private readonly SrtSubtitleParser _parser = new();
    private SubtitleDocument? _document;

    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public double FontSize
    {
        get => GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    public FontFamily FontFamily
    {
        get => GetValue(FontFamilyProperty);
        set => SetValue(FontFamilyProperty, value);
    }

    public IBrush? Foreground
    {
        get => GetValue(ForegroundProperty);
        set => SetValue(ForegroundProperty, value);
    }

    public IBrush? OutlineBrush
    {
        get => GetValue(OutlineBrushProperty);
        set => SetValue(OutlineBrushProperty, value);
    }

    public double OutlineThickness
    {
        get => GetValue(OutlineThicknessProperty);
        set => SetValue(OutlineThicknessProperty, value);
    }

    public double LineSpacing
    {
        get => GetValue(LineSpacingProperty);
        set => SetValue(LineSpacingProperty, value);
    }

    public TextAlignment TextAlignment
    {
        get => GetValue(TextAlignmentProperty);
        set => SetValue(TextAlignmentProperty, value);
    }

    public HorizontalAlignment HorizontalTextAlignment
    {
        get => GetValue(HorizontalTextAlignmentProperty);
        set => SetValue(HorizontalTextAlignmentProperty, value);
    }

    public VerticalAlignment VerticalTextAlignment
    {
        get => GetValue(VerticalTextAlignmentProperty);
        set => SetValue(VerticalTextAlignmentProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == TextProperty)
        {
            _document = null;
            InvalidateMeasure();
            InvalidateVisual();
        }
        else if (change.Property == FontSizeProperty ||
                 change.Property == FontFamilyProperty ||
                 change.Property == LineSpacingProperty ||
                 change.Property == FlowDirectionProperty)
        {
            InvalidateMeasure();
            InvalidateVisual();
        }
        else if (change.Property == ForegroundProperty ||
                 change.Property == OutlineBrushProperty ||
                 change.Property == OutlineThicknessProperty ||
                 change.Property == TextAlignmentProperty ||
                 change.Property == HorizontalTextAlignmentProperty ||
                 change.Property == VerticalTextAlignmentProperty)
        {
            InvalidateVisual();
        }
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var document = GetDocument();
        if (document is null || document.Runs.Count == 0)
            return;

        var lines = LayoutLines(document, Bounds.Width);
        if (lines.Count == 0)
            return;

        var lineSpacing = Math.Max(0, LineSpacing);
        var totalHeight = lines.Sum(line => line.Height)
                          + Math.Max(0, lines.Count - 1) * lineSpacing;
        var y = CalculateVerticalOffset(totalHeight);
        var blockWidth = lines.Max(line => line.Width);
        var blockX = CalculateHorizontalOffset(blockWidth);

        foreach (var line in lines)
        {
            var lineX = blockX + CalculateLineOffset(line.Width, blockWidth);
            var x = lineX;
            foreach (var run in line.Runs)
            {
                DrawRun(context, run, new Point(x, y));
                x += run.Width;
            }

            y += line.Height + lineSpacing;
        }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var document = GetDocument();
        if (document is null || document.Runs.Count == 0)
            return new Size(0, 0);

        var lines = LayoutLines(document, availableSize.Width);
        if (lines.Count == 0)
            return new Size(0, 0);

        var height = lines.Sum(line => line.Height)
                     + Math.Max(0, lines.Count - 1) * Math.Max(0, LineSpacing);

        return new Size(lines.Max(line => line.Width), height);
    }

    private SubtitleDocument? GetDocument()
    {
        var text = Text;
        if (string.IsNullOrEmpty(text))
            return null;

        return _document ??= _parser.Parse(text);
    }

    private List<SubtitleLine> LayoutLines(
        SubtitleDocument document,
        double availableWidth)
    {
        var lines = new List<SubtitleLine>();
        var currentLine = new SubtitleLine(FontSize);
        var endsWithLineBreak = false;

        foreach (var run in document.Runs)
        {
            if (run.IsLineBreak)
            {
                lines.Add(currentLine);
                currentLine = new SubtitleLine(FontSize);
                endsWithLineBreak = true;
                continue;
            }

            foreach (var part in SplitForWrapping(run.Text))
            {
                var measuredRun = CreateMeasuredRun(run, part);
                var fitsLine = double.IsInfinity(availableWidth) ||
                               currentLine.Runs.Count == 0 ||
                               currentLine.Width + measuredRun.Width <= availableWidth;

                if (fitsLine)
                {
                    currentLine.Add(measuredRun);
                    endsWithLineBreak = false;
                    continue;
                }

                lines.Add(currentLine);
                currentLine = new SubtitleLine(FontSize);

                if (string.IsNullOrWhiteSpace(part))
                    continue;

                currentLine.Add(measuredRun);
                endsWithLineBreak = false;
            }
        }

        if (currentLine.Runs.Count > 0 || endsWithLineBreak)
            lines.Add(currentLine);

        return lines;
    }

    private MeasuredRun CreateMeasuredRun(SubtitleRun run, string text)
    {
        var fontFamily = GetFontFamily(run.FontFamily);
        var fontSize = run.FontSize ?? FontSize;
        var typeface = new Typeface(
            fontFamily,
            run.Italic ? FontStyle.Italic : FontStyle.Normal,
            run.Bold ? FontWeight.Bold : FontWeight.Normal);
        var foreground = GetForeground(run.Foreground);
        var formattedText = new FormattedText(
            text,
            CultureInfo.CurrentCulture,
            FlowDirection,
            typeface,
            fontSize,
            foreground);

        return new MeasuredRun(run, formattedText, foreground, fontSize);
    }

    private FontFamily GetFontFamily(string? fontFamilyName)
    {
        if (string.IsNullOrWhiteSpace(fontFamilyName))
            return FontFamily;

        try
        {
            return new FontFamily(fontFamilyName);
        }
        catch (Exception exception) when (exception is ArgumentException or FormatException)
        {
            return FontFamily;
        }
    }

    private IBrush GetForeground(string? color)
    {
        if (string.IsNullOrWhiteSpace(color))
            return Foreground ?? Brushes.White;

        try
        {
            return new SolidColorBrush(Color.Parse(color));
        }
        catch (FormatException)
        {
            return Foreground ?? Brushes.White;
        }
    }

    private void DrawRun(
        DrawingContext context,
        MeasuredRun run,
        Point position)
    {
        if (OutlineBrush is not null && OutlineThickness > 0)
        {
            DrawOutline(
                context,
                run.FormattedText,
                position,
                run.Foreground,
                OutlineBrush,
                OutlineThickness);
        }

        context.DrawText(run.FormattedText, position);

        if (run.Source.Underline)
        {
            var underlineY = position.Y + run.Height - 1;
            var pen = new Pen(run.Foreground, Math.Max(1, run.FontSize / 16));

            context.DrawLine(
                pen,
                new Point(position.X, underlineY),
                new Point(position.X + run.Width, underlineY));
        }
    }

    private static void DrawOutline(
        DrawingContext context,
        FormattedText text,
        Point position,
        IBrush foreground,
        IBrush brush,
        double thickness)
    {
        text.SetForegroundBrush(brush);
        try
        {
            foreach (var offset in OutlineOffsets)
                context.DrawText(text, position + offset * thickness);
        }
        finally
        {
            text.SetForegroundBrush(foreground);
        }
    }

    private double CalculateHorizontalOffset(double blockWidth)
    {
        return HorizontalTextAlignment switch
        {
            HorizontalAlignment.Left => 0,
            HorizontalAlignment.Right => Bounds.Width - blockWidth,
            _ => (Bounds.Width - blockWidth) / 2
        };
    }

    private double CalculateLineOffset(double lineWidth, double blockWidth)
    {
        return TextAlignment switch
        {
            TextAlignment.Left => 0,
            TextAlignment.Right => blockWidth - lineWidth,
            _ => (blockWidth - lineWidth) / 2
        };
    }

    private double CalculateVerticalOffset(double height)
    {
        return VerticalTextAlignment switch
        {
            VerticalAlignment.Top => 0,
            VerticalAlignment.Bottom => Math.Max(0, Bounds.Height - height),
            _ => Math.Max(0, (Bounds.Height - height) / 2)
        };
    }

    private static IEnumerable<string> SplitForWrapping(string text)
    {
        foreach (Match match in WrappingPartsRegex.Matches(text))
            yield return match.Value;
    }

    private sealed class SubtitleLine(double minimumHeight)
    {
        public List<MeasuredRun> Runs { get; } = [];

        public double Width { get; private set; }

        public double Height { get; private set; } = minimumHeight;

        public void Add(MeasuredRun run)
        {
            Runs.Add(run);
            Width += run.Width;
            Height = Math.Max(Height, run.Height);
        }
    }

    private sealed record MeasuredRun(
        SubtitleRun Source,
        FormattedText FormattedText,
        IBrush Foreground,
        double FontSize)
    {
        public double Width => FormattedText.WidthIncludingTrailingWhitespace;

        public double Height => FormattedText.Height;
    }
}
