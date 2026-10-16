using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ByteVault.Apresentacao.Conversores;

public class ConversorBooleanoVisibilidade : IValueConverter
{
    public bool Inverter { get; set; }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool b)
        {
            if (Inverter) b = !b;
            return b ? Visibility.Visible : Visibility.Collapsed;
        }
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Visibility v)
        {
            var res = v == Visibility.Visible;
            return Inverter ? !res : res;
        }
        return false;
    }
}
