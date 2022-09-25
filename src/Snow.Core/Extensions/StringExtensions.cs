using System.Globalization;

namespace Snow.Core.Extensions;

public static class StringExtensions
{
    public static bool TryParseDouble(this string numberString, out double number) =>
        double.TryParse(numberString, NumberStyles.Any, CultureInfo.InvariantCulture, out number);
}
