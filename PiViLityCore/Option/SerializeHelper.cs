using PiViLityCore.Plugin;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PiViLityCore.Option
{
    public class SettingReadException : JsonException
    {
        public SettingReadException(string message) : base(message)
        {
        }
        public SettingReadException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
    public class SettingReadExceptionDuplicateTargetMember : SettingReadException
    {
        public SettingReadExceptionDuplicateTargetMember(string message) : base(message)
        {
        }
        public SettingReadExceptionDuplicateTargetMember(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public static class SerializeHelper
    {
        public delegate bool NotResolvedTypeWriteHandler(Utf8JsonWriter writer, MemberInfo? memberInfo, object? value);

        public static event NotResolvedTypeWriteHandler? NotResolvedTypeWrite;

        private static object readValue(ref Utf8JsonReader reader, Type readToType)
        {
            object v = "";
            if (readToType == typeof(long))
            {
                v = reader.GetInt64();
            }
            else if (readToType == typeof(int))
            {
                v = reader.GetInt32();
            }
            else if (readToType == typeof(short) || readToType == typeof(sbyte))
            {
                v = reader.GetInt16();
            }
            else if (readToType == typeof(ulong))
            {
                v = reader.GetUInt64();
            }
            else if (readToType == typeof(uint))
            {
                v = reader.GetUInt32();
            }
            else if (readToType == typeof(ushort) || readToType == typeof(byte))
            {
                v = reader.GetUInt16();
            }
            else if (readToType == typeof(double))
            {
                v = reader.GetDouble();
            }
            else if (readToType == typeof(float))
            {
                v = reader.GetSingle();
            }
            else
            {
                throw new JsonException($"Invalid setting JSON format.(token={reader.TokenType.ToString()}, position={reader.Position})");
            }
            return v;
        }

        private static object? readSettingValueArray(ref Utf8JsonReader reader, object? targetObj, MemberInfo? targetMember)
        {
            var memberType = targetMember is not null ? Util.Types.GetMemberType(targetMember) : null;
            Type? arrayType = null;
            if (memberType is not null)
            {
                if (memberType.IsArray)
                {
                    arrayType = memberType.GetElementType();
                }
                else if (memberType.IsGenericType)
                {
                    var genType = memberType.GetGenericTypeDefinition();
                    if (genType == typeof(List<>))
                    {
                        arrayType = memberType.GenericTypeArguments[0];
                    }
                }
            }
            if (arrayType is null)
            {
                //空読みモード
                int arratNest = 1;
                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndArray)
                    {
                        if (--arratNest == 0)
                            break;
                    }
                    else if (reader.TokenType == JsonTokenType.StartArray)
                    {
                        ++arratNest;
                    }
                }
                return null;
            }
            else
            {
                Type listType = typeof(List<>).MakeGenericType(arrayType);
                var listObj = Activator.CreateInstance(listType)!;
                var list = (IList)listObj;
                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndArray)
                    {
                        break;
                    }
                    else if (reader.TokenType == JsonTokenType.StartArray)
                    {
                        //さらに配列だった
                        var obj = readSettingValueArray(ref reader, targetObj, targetMember);
                        list.Add(obj);
                    }
                    else if (reader.TokenType == JsonTokenType.StartObject)
                    {
                        var elemObj = Activator.CreateInstance(arrayType)!;
                        readSettingValue(ref reader, [elemObj]);
                        if (elemObj is not null && elemObj.GetType() == arrayType)
                        {
                            list.Add(elemObj);
                        }
                    }
                    else if (reader.TokenType == JsonTokenType.String)
                    {
                        //文字列だった
                        var strValue = reader.GetString() ?? "";
                        var value = Util.Types.ParseValue(strValue, arrayType);
                        if (value is not null && value.GetType() == arrayType)
                        {
                            list.Add(value);
                        }
                    }
                    else if (reader.TokenType == JsonTokenType.Number)
                    {
                        if (targetMember is not null && targetObj is not null)
                        {
                            //数値だった
                            //var type = Util.Types.GetMemberType(targetMember);
                            var value = readValue(ref reader, arrayType);
                            if (value is not null && value.GetType() == arrayType)
                            {
                                list.Add(value);
                            }
                        }
                    }
                    else if (reader.TokenType == JsonTokenType.True)
                    {
                        if (typeof(bool) == arrayType)
                        {
                            list.Add(true);
                        }
                    }
                    else if (reader.TokenType == JsonTokenType.False)
                    {
                        if (typeof(bool) == arrayType)
                        {
                            list.Add(false);
                        }
                    }
                    else
                    {
                        throw new JsonException($"Invalid setting JSON format.(token={reader.TokenType.ToString()}, position={reader.Position})");
                    }
                }
                if (memberType?.IsArray ?? false)
                {
                    return Util.Types.ConvertIListToTypedArray(list, arrayType);
                }
                return listObj;
            }
        }

        /// <summary>
        /// 設定読み込み、値解析用
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="targetObj"></param>
        /// <param name="targetMember"></param>
        /// <exception cref="JsonException"></exception>
        private static void readSettingValue(ref Utf8JsonReader reader, object? targetObj, MemberInfo? targetMember)
        {
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    //データはクラスだった
                    if (targetMember is not null && targetObj is not null)
                    {
                        var objType = Util.Types.GetMemberType(targetMember);
                        var obj = Activator.CreateInstance(objType)!;
                        readSettingValue(ref reader, [obj]);
                        Util.Types.SetMemberValue(targetObj, targetMember, obj);
                    }
                    else
                    {
                        readSettingValue(ref reader, []);
                    }
                    break;
                }
                else if (reader.TokenType == JsonTokenType.StartArray)
                {
                    //配列だった
                    var list = readSettingValueArray(ref reader, targetObj, targetMember);
                    if (list is not null && targetMember is not null && targetObj is not null)
                    {
                        var memberType = Util.Types.GetMemberType(targetMember);
                        Util.Types.SetMemberValue(targetObj, targetMember, list);
                    }

                    break;
                }
                else if (reader.TokenType == JsonTokenType.EndArray)
                {
                    //配列おわり このメンバ終了
                    break;
                }
                else if (reader.TokenType == JsonTokenType.EndObject)
                {
                    break;
                }
                //else if (reader.TokenType == JsonTokenType.PropertyName)
                //{
                //    //プロパティ名=メンバ名
                //    memberName = reader.GetString() ?? "";
                //}
                else if (reader.TokenType == JsonTokenType.String)
                {
                    //文字列だった
                    var strValue = reader.GetString() ?? "";
                    if(targetMember is not null && targetObj is not null)
                        Util.Types.SetMemberValueUseParse(targetObj, targetMember, strValue);
                    break;
                }
                else if (reader.TokenType == JsonTokenType.Number)
                {
                    if (targetMember is not null && targetObj is not null)
                    {
                        //数値だった
                        var type = Util.Types.GetMemberType(targetMember);
                        var v = readValue(ref reader, type);
                        Util.Types.SetMemberValue(targetObj, targetMember, v);
                    }
                    break;
                }
                else if (reader.TokenType == JsonTokenType.True)
                {
                    if (targetMember is not null && targetObj is not null)
                    {
                        Util.Types.SetMemberValue(targetObj, targetMember, true);
                    }
                    break;
                }
                else if (reader.TokenType == JsonTokenType.False)
                {
                    if (targetMember is not null && targetObj is not null)
                    {
                        Util.Types.SetMemberValue(targetObj, targetMember, false);
                    }
                    break;
                }
                else if (reader.TokenType == JsonTokenType.Null)
                {
                    //null値だった
                    break;
                }
                else if (reader.TokenType == JsonTokenType.Comment)
                {
                    //コメントだった
                }
                else
                {
                    throw new JsonException($"Invalid setting JSON format.(token={reader.TokenType.ToString()}, position={reader.Position})");
                }
            }

        }

        /// <summary>
        /// 設定読み込み（クラスへの展開用）
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="targets"></param>
        /// <exception cref="SettingReadExceptionDuplicateTargetMember"></exception>
        /// <exception cref="JsonException"></exception>
        public static void readSettingValue(ref Utf8JsonReader reader, IEnumerable<object?> targets)
        {
            string memberName = "";
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    break;
                }
                else if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    //プロパティ名=メンバ名
                    memberName = reader.GetString() ?? "";
                    object? targetObj = null;
                    MemberInfo? targetMember = null;
                    foreach (var target in targets)
                    {
                        var members = target.GetType().GetMembers(BindingFlags.Public | BindingFlags.Instance).Where(m => m.MemberType == MemberTypes.Property || m.MemberType == MemberTypes.Field);
                        foreach (var member in members)
                        {
                            if (member.Name == memberName)
                            {
                                //メンバ名が一致した
                                if (OptionItemAttribute.MemberIsSave(member))
                                {
                                    if (targetObj != null)
                                    {
                                        // すでに見つかっているので、エラー
                                        throw new SettingReadExceptionDuplicateTargetMember($"Invalid setting JSON format. had duplicate target member({memberName})");
                                    }
                                    targetObj = target;
                                    targetMember = member;
                                }
                            }
                        }
                    }

                    readSettingValue(ref reader, targetObj, targetMember);
               }
                else
                {
                    throw new JsonException($"Invalid setting JSON format.(token={reader.TokenType.ToString()}, position={reader.Position})");
                }
            }

        }

        public static void writeSettingValue(Utf8JsonWriter writer, MemberInfo? memberInfo, object? value)
        {
            var type = value?.GetType();

            if (value is Array array)
            {
                writer.WriteStartArray();
                foreach (var obj in array)
                {
                    writeSettingValue(writer, memberInfo, obj);
                }
                writer.WriteEndArray();
            }
            else if (value is string strValue)
            {
                writer.WriteStringValue(strValue);
            }
            else if (value is int intValue)
            {
                writer.WriteNumberValue(intValue);
            }
            else if (value is bool boolValue)
            {
                writer.WriteBooleanValue(boolValue);
            }
            else if (value is double doubleValue)
            {
                writer.WriteNumberValue(doubleValue);
            }
            else if (value is float floatValue)
            {
                writer.WriteNumberValue(floatValue);
            }
            else if (value is long longValue)
            {
                writer.WriteNumberValue(longValue);
            }
            else if (value is DateTime dateTimeValue)
            {
                writer.WriteStringValue(dateTimeValue);
            }
            else if (value is DateTimeOffset dateTimeOffsetValue)
            {
                writer.WriteStringValue(dateTimeOffsetValue);
            }
            else if (value is Guid guidValue)
            {
                writer.WriteStringValue(guidValue.ToString());
            }
            else if (value is Enum enumValue)
            {
                writer.WriteStringValue(enumValue.ToString());
            }
            else if (memberInfo!=null && (NotResolvedTypeWrite?.Invoke(writer, memberInfo, value) ?? false))
            { 
                // カスタムシリアライズ処理OK
            }
            else if (type?.IsGenericType ?? false) 
            {
                var genType = type.GetGenericTypeDefinition();
                if (genType == typeof(List<>))
                {
                    // List<T> の場合
                    writer.WriteStartArray();
                    var list = value as System.Collections.IList;
                    if (list != null)
                    {
                        foreach (var item in list)
                        {
                            writeSettingValue(writer, memberInfo, item);
                        }
                    }
                    writer.WriteEndArray();
                }
                else if (genType == typeof(Dictionary<,>))
                {
                    // Dictionary<TKey,TValue> の場合
                    writer.WriteStartObject();
                    var dict = value as System.Collections.IDictionary;
                    if (dict != null)
                    {
                        foreach (var key in dict.Keys)
                        {
                            if (key != null)
                            {
                                var keyStr = key.ToString();
                                if (keyStr == null)
                                    continue;
                                writer.WritePropertyName(keyStr);
                                writeSettingValue(writer, memberInfo, dict[key]);
                            }
                        }
                    }
                    writer.WriteEndObject();
                }
            }
            else
            {
                if (value != null && type != null)
                {
                    writer.WriteStartObject();
                    //まずはvalueのメンバーを調べることを考える
                    var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty | BindingFlags.GetProperty);
                    var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetField | BindingFlags.GetField);
                    foreach (FieldInfo field in fields)
                    {
                        if (OptionItemAttribute.MemberIsSave(field) == false)
                            continue;
                        writer.WritePropertyName(field.Name);
                        writeSettingValue(writer, field, field.GetValue(value));
                    }
                    foreach (PropertyInfo prop in properties)
                    {
                        if (OptionItemAttribute.MemberIsSave(prop) == false)
                            continue;
                        if (prop.CanWrite)
                        {
                            writer.WritePropertyName(prop.Name);
                            writeSettingValue(writer, prop, prop.GetValue(value));
                        }
                    }
                    writer.WriteEndObject();
                }
                else
                {
                    // それ以外はnullとして扱う
                    writer.WriteNullValue();
                }
            }

        }
    }
}
