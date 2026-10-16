using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ByteVault.Apresentacao.Conversores;

public class ConversorContadorZeroVisibilidade : IValueConverter
{
    public bool Inverter { get; set; }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var contagem = 0;
        if (value is int i) contagem = i;

        var ehZero = contagem == 0;
        if (Inverter) ehZero = !ehZero;

        return ehZero ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
