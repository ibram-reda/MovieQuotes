namespace MovieQuotes.UI.Views.Controls;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Interactivity;
using LibVLCSharp.Shared;
using System.Windows.Input;

public partial class MediaPlayerControls : UserControl
{
    public static readonly StyledProperty<MediaPlayer> PlayerProperty =
       AvaloniaProperty.Register<MediaPlayerControls, MediaPlayer>(
           nameof(Player),
           defaultBindingMode: BindingMode.OneWay);

    public static readonly StyledProperty<ICommand> NextPhraseProperty =
       AvaloniaProperty.Register<MediaPlayerControls, ICommand>(
           nameof(NextPhrase),
           defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<ICommand> PreviousPhraseProperty =
       AvaloniaProperty.Register<MediaPlayerControls, ICommand>(
           nameof(PreviousPhrase),
           defaultBindingMode: BindingMode.TwoWay);

    public ICommand PreviousPhrase
    {
        get => GetValue(PreviousPhraseProperty);
        set => SetValue(PreviousPhraseProperty, value);
    }

    public ICommand NextPhrase
    {
        get => GetValue(NextPhraseProperty);
        set => SetValue(NextPhraseProperty, value);
    }

    public MediaPlayer Player
    {
        get => GetValue(PlayerProperty);
        set
        {
            SetValue(PlayerProperty, value);
        }
    }


    public MediaPlayerControls()
    {
        InitializeComponent();
    }

    private void Play(object? sender, RoutedEventArgs e)
    {
        Player.Play();
    }

    private void Pause(object? sender, RoutedEventArgs e)
    {
        Player.Pause();
    }

    private void Forword(object? sender, RoutedEventArgs e)
    {
        Player.Time += 10000;
    }

    private void Stop(object? sender, RoutedEventArgs e)
    {
        Player.Stop();
    }
    private void Backword(object? sender, RoutedEventArgs e)
    {
        Player.Time -= 10000;
    }

    private void Next(object? sender, RoutedEventArgs e)
    {
        if (NextPhrase.CanExecute(null))
            NextPhrase.Execute(null);
    }

    private void Previous(object? sender, RoutedEventArgs e)
    {
        if (PreviousPhrase.CanExecute(null))
            PreviousPhrase.Execute(null);
    }

    private void ToggleSound(object? sender, RoutedEventArgs e)
    {
        Player.Mute ^= true;
        SoundBtn.Content  = Player.Mute ? "🔉" : "🔇" ;
    }
     

    private void Volume_ValueChanged(object? sender,  RangeBaseValueChangedEventArgs e)
    {
        Player.Volume = (int)e.NewValue;
    }
}