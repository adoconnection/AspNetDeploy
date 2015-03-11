using System;

namespace Enums
{
    public static class EnumExtender
    {
        public static T Parse<T>(this Enum e, string value) where T : struct
        {
            return (T) Enum.Parse(typeof (T), value);
        }
    }
}