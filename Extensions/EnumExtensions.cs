using Analysis_Lab1.Enums;

namespace Analysis_Lab1.Extensions;

internal static class EnumExtensions
{
    internal static string GetDisplayName(this Enum value)
    {
        var fieldInfo = value.GetType().GetField(value.ToString());

        var enumCustomAttribute = fieldInfo?.GetCustomAttributes(typeof(DisplayNameAttribute), false)
            .FirstOrDefault();

        if (enumCustomAttribute is DisplayNameAttribute displayAttribute)
        {
            return displayAttribute.DisplayName;
        }

        return value.ToString();
    }
}