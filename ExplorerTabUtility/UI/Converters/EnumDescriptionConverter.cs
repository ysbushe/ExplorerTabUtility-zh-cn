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
        if (value == null) return string.Empty;

        var valueStr = value.ToString()!;
        var enumType = value.GetType();

        // Determine the resource key prefix based on the enum type
        var prefix = enumType.Name switch
        {
            "HotKeyAction" => "Action_",
            "HotkeyScope" => "Scope_",
            _ => null
        };

        if (prefix != null)
        {
            var resourceKey = $"{prefix}{valueStr}";
            var localized = ResourceMgr.GetString(resourceKey, CultureInfo.CurrentUICulture);
            if (!string.IsNullOrEmpty(localized))
                return localized;
        }

        // Fall back to DescriptionAttribute
        var fieldInfo = enumType.GetField(valueStr);
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