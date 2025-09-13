using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PiViLityCore.Util
{
    public static class Member
    {
        /// <summary>
        /// オプションへの値設定
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="member"></param>
        /// <param name="value"></param>
        public static void SetValueObject(object instance, MemberInfo member, object value)
        {
            if (member is PropertyInfo prop)
            {
                prop.SetValue(instance, value);
            }
            else if (member is FieldInfo field)
            {
                field.SetValue(instance, value);
            }
        }

        /// <summary>
        /// オプションのメンバから値を取得
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        public static object? GetValueObject(object instance, MemberInfo member)
        {
            if (member is PropertyInfo prop)
            {
                return prop.GetValue(instance);
            }
            else if (member is FieldInfo field)
            {
                return field.GetValue(instance);
            }
            return null;
        }

        public static void SetValue(object instance, MemberInfo member, Size size) => SetValueObject(instance, member, size);

        public static Size GetValue(object instance, MemberInfo member)
        {
            object? obj = GetValueObject(instance, member);
            if (obj is Size size)
            {
                return size;
            }
            else
            {
                return new();
            }
        }


        public static void SetValue<T>(object instance, MemberInfo member, T value) where T : struct => SetValueObject(instance, member, value);
        public static void SetClassValue<T>(object instance, MemberInfo member, T value) where T : class => SetValueObject(instance, member, value);


        public static T GetValue<T>(object instance, MemberInfo member) where T : struct
        {
            object? obj = GetValueObject(instance, member);
            if (obj is T value)
            {
                return value;
            }
            else
            {
                return default;
            }
        }

        public static T GetClassValue<T>(object instance, MemberInfo member) where T : class
        {
            object? obj = GetValueObject(instance, member);
            if (obj is T value)
            {
                return value;
            }
            else
            {
                if (typeof(T) == typeof(string))
                {
                    return (T)(object)string.Empty;
                }
                return (T)Activator.CreateInstance(typeof(T))!;
            }
        }

    }
}
