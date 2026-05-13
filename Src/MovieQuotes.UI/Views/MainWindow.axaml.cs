namespace MovieQuotes.UI.Views;

using Avalonia.Controls;
using Avalonia.Controls.Notifications;

public partial class MainWindow : Window
{
    private WindowNotificationManager _notificationManager;
    public MainWindow()
    {
        InitializeComponent();
         _notificationManager = new WindowNotificationManager(this)
        {
            Position = NotificationPosition.TopRight,
            MaxItems = 3
        };
    }

    public WindowNotificationManager manger => _notificationManager;
}
