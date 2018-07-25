using Colin.Lottery.Models;
using System;
using System.Reflection;

namespace Colin.Lottery.Utils
{
    /// <summary>
    /// 枚举扩展函数
    /// </summary>
    public static class EnumSugarExtenions
    {
        /// <summary>
        /// 获得枚举字段的特性(Attribute)，该属性不允许多次定义。
        /// </summary>
        public static string GetAttributeValue(this Enum thisValue)
        {
            FieldInfo field = thisValue.GetType().GetField(thisValue.ToString());
            var attr = (Attribute.GetCustomAttribute(field, typeof(Desc)) as Desc);
            if (attr == null) return string.Empty;
            return (Attribute.GetCustomAttribute(field, typeof(Desc)) as Desc).Value;
        }
        /// <summary>
        /// 获得枚举字段的特性(Attribute)，该属性不允许多次定义。
        /// </summary>
        public static T GetAttribute<T>(this Enum thisValue) where T : class
        {
            FieldInfo field = thisValue.GetType().GetField(thisValue.ToString());
            var attr = (Attribute.GetCustomAttribute(field, typeof(T)) as T);
            return attr;
        }
        /// <summary>
        /// 获得枚举字段的名称。
        /// </summary>
        /// <returns></returns>
        public static string GetName(this Enum thisValue)
        {
            return Enum.GetName(thisValue.GetType(), thisValue);
        }
        /// <summary>
        /// 获得枚举字段的值。
        /// </summary>
        /// <returns></returns>
        public static T GetValue<T>(this Enum thisValue)
        {
            return (T)Enum.Parse(thisValue.GetType(), thisValue.ToString());
        }
    }
}
