namespace MovieQuotes.UI.Views.Controls;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Interactivity;
using LibVLCSharp.Shared;

public partial class MediaPlayerControls : UserControl
{
    public static readonly StyledProperty<MediaPlayer> PlayerProperty =
       AvaloniaProperty.Register<MediaPlayerControls, MediaPlayer>(
           nameof(Player),
           defaultBindingMode: BindingMode.OneWay);

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