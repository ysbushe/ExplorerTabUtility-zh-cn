using System.Globalization;
using System.Threading;
using System.Windows;

namespace ExplorerTabUtility.Strings;

public static class LanguageManager
{
    private static readonly string[] SupportedLanguages = ["en-US", "zh-CN"];

    public static void Initialize(string languageSetting)
    {
        var culture = ResolveCulture(languageSetting);
        Thread.CurrentThread.CurrentUICulture = culture;
        Thread.CurrentThread.CurrentCulture = culture;

        if (Application.Current != null)
            Application.Current.Language = System.Windows.Markup.XmlLanguage.GetLanguage(culture.IetfLanguageTag);
    }

    private static CultureInfo ResolveCulture(string languageSetting)
    {
        if (languageSetting == "zh-CN")
            return new CultureInfo("zh-CN");

        if (languageSetting == "en-US")
            return new CultureInfo("en-US");

        // Auto: check system UI language
        var systemLang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        if (systemLang == "zh")
            return new CultureInfo("zh-CN");

        return new CultureInfo("en-US");
    }
}
