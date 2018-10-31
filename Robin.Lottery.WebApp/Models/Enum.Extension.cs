using System;
using System.ComponentModel;

namespace Robin.Lottery.WebApp.Models
{
    public static class EnumExtensions
    {
        /// <summary>
        ///     获得枚举字段的特性(Attribute)，该属性不允许多次定义。
        /// </summary>
        public static string GetDescription(this Enum thisValue)
        {
            var field = thisValue.GetType().GetField(thisValue.ToString());
            var attr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
            if (attr == null) return string.Empty;

            return (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute)?
                .Description;
        }

        /// <summary>
        ///     获得枚举字段的特性(Attribute)，该属性不允许多次定义。
        /// </summary>
        public static T GetAttribute<T>(this Enum thisValue) where T : class
        {
            var field = thisValue.GetType().GetField(thisValue.ToString());
            var attr = Attribute.GetCustomAttribute(field, typeof(T)) as T;
            return attr;
        }

        /// <summary>
        ///     获得枚举字段的名称。
        /// </summary>
        /// <returns></returns>
        public static string GetName(this Enum thisValue)
        {
            return Enum.GetName(thisValue.GetType(), thisValue);
        }

        /// <summary>
        ///     获得枚举字段的值。
        /// </summary>
        /// <returns></returns>
        public static T GetValue<T>(this Enum thisValue)
        {
            return (T)Enum.Parse(thisValue.GetType(), thisValue.ToString());
        }

        public static string ToStringValue(this Enum thisValue)
        {
            return thisValue.GetValue<int>().ToString();
        }
    }
}