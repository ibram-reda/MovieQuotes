namespace MovieQuotes.UI.Converters;

using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Globalization;
using System.IO;

public class BitmapValueConverter : IValueConverter
{
    public static BitmapValueConverter Instance = new BitmapValueConverter();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is null)
            return _defaultImage;
         
        if (value is string sValue )
        {
            if(string.IsNullOrWhiteSpace(sValue))
                return _defaultImage;
            var uri = new Uri(sValue, UriKind.RelativeOrAbsolute);
            var scheme = uri.IsAbsoluteUri ? uri.Scheme : "file";

            switch (scheme)
            {
                case "file":
                    if(!File.Exists(sValue))
                        return _defaultImage;
                    return new Bitmap(sValue);

                default:
                    throw new NotSupportedException();

            }
        }

        throw new NotSupportedException();
    }

    Bitmap _defaultImage = new Bitmap(AssetLoader.Open(new Uri("avares://MovieQuotes.UI/Assets/PhotoNotFound.jpg")));

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}