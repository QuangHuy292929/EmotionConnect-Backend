using System;
using System.Collections.Generic;
using System.Text;

namespace Infracstructure.Extensions;

public static class EnumExtensions
{
    public static T ToEnum<T>(this string value, T defaultValue = default) where T : struct, Enum
    {
        if (Enum.TryParse<T>(value, true, out var result))
        {
            return result;
        }

        return defaultValue;
    }

    public static bool TryToEnum<T>(this string value, out T result) where T : struct, Enum
    {
        return Enum.TryParse<T>(value, true, out result);
    }
}
