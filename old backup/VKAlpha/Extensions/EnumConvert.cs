using System;

namespace VKAlpha.Extensions
{
    public static class EnumConvertExtension
    {
        public static int EnumConvert(this Enum @enum)
        {
            return Convert.ToInt32(@enum);
        }
    }
}
