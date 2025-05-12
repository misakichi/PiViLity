using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PiViLityCore.Util
{
    public static class Types
    {
        public static bool HasParentType(Type _type, Type parentType)
        {
            Type? type = _type;  
            while (type != null && type!=typeof(object))
            {
                if (type == parentType)
                {
                    return true;
                }
                type = type.BaseType;
            }
            if (type == parentType)
            {
                return true;
            }
            return false;
        }

        public static Array ConvertIListToTypedArray(IList list, Type elementType)
        {
            int count = list.Count;

            Array array = Array.CreateInstance(elementType, count);
            for (int i = 0; i < count; i++)
            {
                array.SetValue(list[i], i);
            }

            return array;
        }

        public static void SetMemberValue(object target, MemberInfo memberInfo, object value)
        {
            if (memberInfo is PropertyInfo propertyInfo)
            {
                propertyInfo.SetValue(target, value);
            }
            else if (memberInfo is FieldInfo fieldInfo)
            {
                fieldInfo.SetValue(target, value);
            }
            else
            {
                throw new ArgumentException("MemberInfo must be a PropertyInfo or FieldInfo");
            }
        }

        public static Type GetMemberType(MemberInfo memberInfo)
        {
            if (memberInfo is PropertyInfo propertyInfo)
            {
                return propertyInfo.PropertyType;
            }
            else if (memberInfo is FieldInfo fieldInfo)
            {
                return fieldInfo.FieldType;
            }
            else
            {
                throw new ArgumentException("MemberInfo must be a PropertyInfo or FieldInfo");
            }
        }

        public static object? ParseValue(string value, Type type)
        {
            if (type.IsEnum)
            {
                return Enum.Parse(type, value);
            }
            else
            {
                return Convert.ChangeType(value, type);
            }
        }
        public static void SetMemberValueUseParse(object target, MemberInfo memberInfo, string value)
        {
            Type type = GetMemberType(memberInfo);
            object? obj = ParseValue(value,type);
            SetMemberValue(target, memberInfo, obj);
        }
    }
}
