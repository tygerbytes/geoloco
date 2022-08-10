using GeoLoco.Core.Model;
using System;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace GeoLoco.Core.Services
{
    public static class Enums
    {
        public static T? TryParse<T>(string text) where T : struct
        {
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }

            if (Enum.TryParse(text, ignoreCase: true, out T result))
            {
                return result;
            }

            return null;
        }

        public static string GetDescriptionAttribute<T>(this T source)
        {
            var fieldInfo = source.GetType().GetField(source.ToString());

            var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
            {
                return attributes[0].Description;
            }

            return source.ToString();
        }
    }
}
