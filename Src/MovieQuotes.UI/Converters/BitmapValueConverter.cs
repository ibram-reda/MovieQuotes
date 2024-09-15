namespace MovieQuotes.UI.Converters;

using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using System;
using System.Globalization;


public class BitmapValueConverter : IValueConverter
{
    public static BitmapValueConverter Instance = new BitmapValueConverter();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    { 
        if(value == null || value == "")
        {
            return null;

        } 
        if (value is string  )
        {
            var uri = new Uri((string)value, UriKind.RelativeOrAbsolute);
            var scheme = uri.IsAbsoluteUri ? uri.Scheme : "file";

            switch (scheme)
            {
                case "file":
                    return new Bitmap((string)value);

                default:
                    throw new NotSupportedException();

            }
        }

        throw new NotSupportedException();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}