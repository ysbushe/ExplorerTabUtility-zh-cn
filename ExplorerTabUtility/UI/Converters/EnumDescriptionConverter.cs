using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using System.ComponentModel;
using System.Resources;

namespace ExplorerTabUtility.UI.Converters;

public class EnumDescriptionConverter : IValueConverter
{
    private static readonly ResourceManager ResourceMgr = new(
        "ExplorerTabUtility.Strings.Strings",
        typeof(EnumDescriptionConverter).Assembly);

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null) return DependencyProperty.UnsetValue;

        var valueStr = value.ToString()!;

        // Try to get localized description from resources first
        var resourceKey = $"Action_{valueStr}";
        var localized = ResourceMgr.GetString(resourceKey, culture);
        if (!string.IsNullOrEmpty(localized))
            return localized;

        // Fall back to DescriptionAttribute
        var fieldInfo = value.GetType().GetField(valueStr);
        if (fieldInfo == null) return valueStr;

        var descriptionAttribute = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false)
            .FirstOrDefault() as DescriptionAttribute;

        return descriptionAttribute?.Description ?? valueStr;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value;
    }
}