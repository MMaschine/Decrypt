using System.ComponentModel;

namespace Decrypt.DataAccess.Helpers
{
    /// <summary>
    /// Helper to extract string representation of the enum members
    /// </summary>
    internal static class EnumHelper
    {
        public static string GetEnumDescription(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());

            var descriptionAttribute =
                field?.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault();

            return descriptionAttribute != null ? ((DescriptionAttribute)descriptionAttribute).Description
                : value.ToString();
        }
    }
}
