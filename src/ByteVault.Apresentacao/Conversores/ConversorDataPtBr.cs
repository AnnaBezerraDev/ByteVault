using System;
using System.Globalization;
using System.Windows.Data;

namespace ByteVault.Apresentacao.Conversores;

public class ConversorDataPtBr : IValueConverter
{
    private static readonly CultureInfo CulturaPtBr = new("pt-BR");

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is DateTime data)
        {
            var dataLocal = data.Kind == DateTimeKind.Utc ? data.ToLocalTime() : data;
            return dataLocal.ToString("dd/MM/yyyy HH:mm", CulturaPtBr);
        }
        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
