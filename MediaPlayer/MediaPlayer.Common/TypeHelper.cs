using System.Collections;

namespace MediaPlayer.Common;

/// <summary>
/// 
/// </summary>
public static partial class TypeHelper
{
    #region Functions

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static IEnumerable? CreateSequence(object? value)
    {
        var container = Array.CreateInstance(typeof(object), 1);

        container.Initialize();

        if (value == null) return container;

        if ((value != null) && IsSequenceType(value) && value.GetType().IsSZArray)
        {
            return value as Array;
        }

        if ((value != null) && IsSequenceType(value))
        {
            var sequence_type = GetSequenceType(value.GetType());

            if ((sequence_type != null) && sequence_type.IsSZArray) 
            {
                var arrlist = value as ArrayList;

                if (arrlist != null) 
                {
                    return arrlist;
                }
            }
            else if ((sequence_type != null) && !sequence_type.IsSZArray && sequence_type.IsVariableBoundArray)
            {
                /* Multi-dimensional array */

                var arrlists = value as IEnumerable<ArrayList>;

                if (arrlists != null) 
                {
                    /* Flatten the multidimensional array to a single dumensional array */

                    var arrlist = new ArrayList();

                    foreach (var arr in arrlists)
                    {
                        foreach (var entry in arr)
                        {
                            arrlist.Add(entry);
                        }
                    }

                    return arrlist;
                }
            }
            else if ((sequence_type != null) && IsSequenceType(sequence_type))
            {
                return value as IEnumerable;
            }
        }

        return container;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <returns></returns>
    public static TAttribute? GetAttribute<TAttribute>(this Type? type, bool inherited = false) where TAttribute : Attribute
    {
        TAttribute? attribute = default;

        try
        {
            if (type is not null)
            {
                attribute = type.GetCustomAttribute<TAttribute>(inherited);
            }
        }
        catch (Exception)
        {
            attribute = default;
        }

        return attribute;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TAttribute"></typeparam>
    /// <param name="property"></param>
    /// <returns></returns>
    public static TAttribute? GetAttribute<TAttribute>(PropertyInfo? property) where TAttribute : Attribute
    {
        if (property == null) return default;

        if (!IsAttribute<TAttribute>(property)) return default;

        try
        {
            return property.GetCustomAttribute<TAttribute>();
        }
        catch (Exception)
        {
        }

        return default;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TAttribute"></typeparam>
    /// <param name="instance"></param>
    /// <returns></returns>
    public static TAttribute? GetEnumAttribute<TAttribute>(this Enum value, bool inherit = false) where TAttribute : Attribute
    {
        TAttribute? attribute = default;

        try
        {
            var information = value.GetType().GetField(value.ToString());

            if (information is not null)
            {
                var attributes = information.GetCustomAttributes<TAttribute>(inherit);

                if (attributes is not null && attributes.Any())
                {
                    attribute = attributes.FirstOrDefault();
                }
            }
        }
        catch
        {
            attribute = default;
        }


        return attribute;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TAttribute"></typeparam>
    /// <param name="assembly"></param>
    /// <returns></returns>
    public static IEnumerable<TAttribute> GetAttributes<TAttribute>(this Assembly? assembly) where TAttribute : Attribute
    {
        IEnumerable<TAttribute> attributes = Enumerable.Empty<TAttribute>();

        try
        {
            if (assembly is not null)
            {
                attributes = assembly.GetCustomAttributes<TAttribute>();
            }
        }
        catch (Exception) 
        { 
            attributes = Enumerable.Empty<TAttribute>();
        }

        return attributes;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="expression"></param>
    /// <returns></returns>
    public static Type? GetExpressionType(Expression? expression)
    {
        if (expression == null) return default;

        return GetType(expression?.Type ?? default);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int GetSequenceLength(object? value)
    {
        if ((value != null) && IsSequenceType(value))
        {
            var sequence_type = GetSequenceType(value.GetType());

            if ((sequence_type != null) && sequence_type.IsSZArray)
            {
                var sequence_list = value as ArrayList;

                if (sequence_list != null) return sequence_list.Count;
            }
            else if ((sequence_type != null) && !sequence_type.IsSZArray && sequence_type.IsVariableBoundArray)
            {
                /* Multi-dimensional array */

                var sequence_list = value as IEnumerable<ArrayList>;

                return sequence_list?.Min(arrlist => arrlist.Count) ?? 0;
            }
            else if ((sequence_type != null) && IsEnumerableType(sequence_type))
            {
                return (value as IEnumerable)?.Cast<object>()?.Count() ?? 0;
            }
        }
        else if ((value != null) && (value is Type))
        {
            var sequence_type = value as Type;

            if ((sequence_type != null) && sequence_type.IsSZArray)
            {
                var sequence_list =  sequence_type as IEnumerable<Type>;

                if (sequence_list != null) return sequence_list.ToArray().Length;
            }
            else if ((sequence_type != null) && !sequence_type.IsSZArray && sequence_type.IsVariableBoundArray)
            {
                /* Multi-dimensional array */

                var sequence_list = sequence_type as IEnumerable<ArrayList>;

                if (sequence_list != null) return sequence_list.Min(x => x.Count);
            }
            else if ((sequence_type != null) && typeof(IEnumerable<>).IsAssignableFrom(sequence_type))
            {
                return (sequence_type as IEnumerable<Type>)?.Count() ?? 0;
            }
        }

        return 0;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sequence_type"></param>
    /// <returns></returns>
    public static Type? GetSequenceType(Type sequence_type)
    {
        if (sequence_type == null) return default;

        if (sequence_type.IsSZArray)
        {
            return sequence_type.GetElementType();
        }
        else if (IsArrayType(sequence_type) && !sequence_type.IsSZArray && sequence_type.IsVariableBoundArray)
        {
            /* Multi-dimensional jagged arrays */

            return sequence_type.GetElementType();
        }
        else if (typeof(IEnumerable<>).IsAssignableFrom(sequence_type))
        {
            return sequence_type.GetElementType();
        }

        if (sequence_type.IsGenericType)
        {
            foreach (var arg in sequence_type.GetGenericArguments()) 
            {
                var arg_type = typeof(IEnumerable<>).MakeGenericType(arg);

                if (arg_type.IsAssignableFrom(sequence_type))
                {
                    return arg_type;
                }
            }
        }

        if (sequence_type.IsInterface)
        {
            var interface_types = sequence_type.GetInterfaces();

            foreach(var interface_type in interface_types) 
            { 
                var arg = GetSequenceType(interface_type);

                if (arg != null) return arg;
            }
        }

        if ((sequence_type.BaseType != null) && !ReferenceEquals(typeof(object), sequence_type.BaseType))
        { 
            return GetSequenceType(sequence_type.BaseType);
        }

        return null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="generic_type"></param>
    /// <returns></returns>
    public static Type? GetType(Type? generic_type)
    {
        if (generic_type == null) return default;

        if (generic_type.IsValueType) return generic_type;

        if (generic_type.IsEnum) return generic_type;

        if (generic_type.IsClass) return generic_type;

        return GetSequenceType(generic_type) ?? generic_type;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TAttribute"></typeparam>
    /// <param name="assembly"></param>
    /// <returns></returns>
    public static IEnumerable<Type>? GetTypesFromAttribute<TAttribute>(Assembly assembly) where TAttribute : Attribute
    {
        IEnumerable<Type> types = Enumerable.Empty<Type>();

        try
        {
            if (assembly is not null)
            {
                types = assembly.GetTypes().Where(x => x.HasAttribute<TAttribute>() && x.IsClass);
            }
        }
        catch (Exception)
        {
            types = Enumerable.Empty<Type>();
        }

        return types;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entry"></param>
    /// <returns></returns>
    public static bool HasAttribute<T>(this Type? entry) where T : Attribute => GetAttribute<T>(entry) is not null;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsArrayType(object? value)
    {
        if ((value != null) && (value is Type))
        {
            return (value as Type)?.IsArray ?? false;
        }
        else if (value != null)
        {
            return value.GetType().IsArray;
        }

        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TAttribute"></typeparam>
    /// <param name="property"></param>
    /// <returns></returns>
    public static bool IsAttribute<TAttribute>(PropertyInfo? property) where TAttribute : Attribute
    {
        if (property == null) return false;

        try
        {
            var attribute = property.GetCustomAttribute<TAttribute>();

            return attribute != null;
        }
        catch (Exception)
        {
        }

        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="destination"></param>
    /// <returns></returns>
    public static bool IsConvertibleTo(Type? source, Type? destination)
    {
        if ((source == null) ^ (destination == null)) return false;

        if ((source != null) && (destination != null))
        {
            if (ReferenceEquals(source, destination)) return true;

            if (source.IsValueType && destination.IsValueType)
            {
                if (ReferenceEquals(source, typeof(bool))) return true;

                if (ReferenceEquals(source, typeof(byte))) return true;

                if (ReferenceEquals(source, typeof(sbyte))) return true;

                if (ReferenceEquals(source, typeof(char))) return true;

                if (ReferenceEquals(source, typeof(ushort)))
                {
                    if (ReferenceEquals(destination, typeof(bool))) return false;

                    if (ReferenceEquals(destination, typeof(byte))) return false;

                    if (ReferenceEquals(destination, typeof(sbyte))) return false;

                    if (ReferenceEquals(destination, typeof(char))) return false;

                    return true;
                }

                if (ReferenceEquals(source, typeof(ushort)) || ReferenceEquals(source, typeof(short)))
                {
                    if (ReferenceEquals(destination, typeof(bool))) return false;

                    if (ReferenceEquals(destination, typeof(byte))) return false;

                    if (ReferenceEquals(destination, typeof(sbyte))) return false;

                    if (ReferenceEquals(destination, typeof(char))) return false;

                    return true;
                }

                if (ReferenceEquals(source, typeof(uint)) || ReferenceEquals(source, typeof(int)))
                {
                    if (ReferenceEquals(destination, typeof(bool))) return false;

                    if (ReferenceEquals(destination, typeof(byte))) return false;

                    if (ReferenceEquals(destination, typeof(sbyte))) return false;

                    if (ReferenceEquals(destination, typeof(char))) return false;

                    if (ReferenceEquals(destination, typeof(ushort))) return false;

                    if (ReferenceEquals(destination, typeof(short))) return false;

                    return true;
                }

                if (ReferenceEquals(source, typeof(ulong)) || ReferenceEquals(source, typeof(long)))
                {
                    if (ReferenceEquals(destination, typeof(bool))) return false;

                    if (ReferenceEquals(destination, typeof(byte))) return false;

                    if (ReferenceEquals(destination, typeof(sbyte))) return false;

                    if (ReferenceEquals(destination, typeof(char))) return false;

                    if (ReferenceEquals(destination, typeof(ushort))) return false;

                    if (ReferenceEquals(destination, typeof(short))) return false;

                    if (ReferenceEquals(destination, typeof(uint))) return false;

                    if (ReferenceEquals(destination, typeof(int))) return false;

                    return true;
                }

                if (ReferenceEquals(source, typeof(float)) ||
                    ReferenceEquals(source, typeof(decimal)) ||
                    ReferenceEquals(source, typeof(double)))
                {
                    return true;
                }

            }
            else if (source.IsValueType && ReferenceEquals(destination, typeof(BigInteger)))
            {
                return true;
            }
            else if (source.IsValueType && ReferenceEquals(destination, typeof(string)))
            {
                return true;
            }
            else if (!source.IsValueType && !destination.IsValueType)
            {
                if ((source.GetInterface("IConvertible", true) != null) &&
                    (destination.GetInterface("IConvertible", true) != null))
                {
                    return true;
                }
                else if (source.IsSZArray && 
                    ReferenceEquals(source.GetElementType(),typeof(char)) &&
                    ReferenceEquals(destination, typeof(string)))
                {
                    return true;
                }
                else if (destination.IsSZArray &&
                    ReferenceEquals(destination.GetElementType(), typeof(char)) &&
                    ReferenceEquals(source, typeof(string)))
                {
                    return true;
                }
                else if (source.IsSZArray && 
                    IsEnumerableType(destination) && IsGenericType(destination))
                {
                    var array_type = source.GetElementType();

                    var destination_args = destination.GetGenericArguments();

                    var destination_args_length = destination_args?.Length ?? 0;

                    if ((destination_args != null) && 
                        (destination_args_length == 1) && (array_type != null))
                    {
                        return ReferenceEquals(destination_args[0], array_type) ||
                            IsConvertibleTo(destination_args[0], array_type);
                    }
                }
                else if (source.IsVariableBoundArray && IsEnumerableType(destination))
                {
                    var array_type = source.GetElementType();

                    var destination_args = destination.GetGenericArguments();

                    var destination_args_length = destination_args?.Length ?? 0;

                    if ((destination_args != null) &&
                        (destination_args_length == 1) && (array_type != null))
                    {
                        return ReferenceEquals(destination_args[0], array_type)  ||
                            IsConvertibleTo(destination_args[0], array_type);
                    }
                }
                else if (IsEnumerableType(source) && IsEnumerableType(destination))
                {
                    if (IsArrayType(source) && IsArrayType(destination))
                    {
                        var source_type = source.GetElementType();

                        var destination_type = destination.GetElementType();

                        if ((source_type != null) && (destination_type != null))
                        {
                            return ReferenceEquals(source_type, destination_type) ||
                                IsConvertibleTo(source_type, destination_type);
                        }
                    }
                    else if (IsGenericType(source) && IsGenericType(destination))
                    {
                        var source_args = source.GetGenericArguments();

                        var destination_args = destination.GetGenericArguments();

                        if ((source_args?.Length ?? 0) == (destination_args?.Length ?? 0) && 
                            (source_args?.Any() ?? false))
                        {
                            var length = source_args?.Length ?? 0;

                            for (var index = 0; 
                                (source_args != null) && (destination_args != null) && (index < length); 
                                index++)
                            {
                                if (ReferenceEquals(source_args[index], destination_args[index]) ||
                                    IsConvertibleTo(source_args[index], destination_args[index]))
                                {
                                    continue;
                                }

                                return false;
                            }

                            return true;
                        }
                    }
                    else if (IsGenericType(source) && !IsGenericType(destination) && IsArrayType(destination))
                    {
                        var source_args = source.GetGenericArguments();

                        var array_type = destination.GetElementType();

                        var source_args_length = source_args?.Length ?? 0;

                        if ((source_args != null) && 
                            (array_type != null) && (source_args_length == 1))
                        {
                            return ReferenceEquals(source_args[0], array_type) ||
                                IsConvertibleTo(source_args[0], array_type);
                        }
                    }
                    else if (!IsGenericType(source) && IsArrayType(source) && IsGenericType(destination))
                    {
                        var destination_args = destination.GetGenericArguments();

                        var array_type = source.GetElementType();

                        var destination_args_length = destination_args?.Length ?? 0;

                        if ((destination_args != null) &&
                            (array_type != null) && (destination_args_length == 1))
                        {
                            return ReferenceEquals(destination_args[0], array_type) ||
                                IsConvertibleTo(destination_args[0], array_type);
                        }
                    }
                }
                else if (source.IsSZArray && 
                    ReferenceEquals(source, typeof(char[])) && ReferenceEquals(destination, typeof(string)))
                {
                    return true;
                }
                else if (source.IsSZArray && destination.IsSZArray)
                {
                    return IsConvertibleTo(source.GetElementType(), destination.GetElementType());  
                }
            }
            else if (source.IsValueType && ReferenceEquals(destination, typeof(string)))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsEnumerableType(object? value)
    {
        if (value != null)
        {
            if (value is Type)
            {
                return IsArrayType(value as Type) ||
                    typeof(IEnumerable).IsAssignableFrom(value as Type);
            }

            return IsArrayType(value.GetType()) || 
                typeof(IEnumerable).IsAssignableFrom(value.GetType());
        }

        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsGenericType(object? value)
    {
        if (value != null)
        {
            if (value is Type)
            {
                return (value as Type)?.IsGenericType ?? false;
            }

            return value.GetType().IsGenericType;
        }

        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsSequenceType(object? value)
    {
        var response = false;

        if (value != null)
        {
            if (value is Type)
            {
                var sequence_type = GetSequenceType(value as Type ?? (Type)Type.Missing);

                response = (sequence_type != null);
            }
            else
            {
                var sequence_type = value.GetType();

                response = (sequence_type != null) &&
                    (IsArrayType(sequence_type) || IsEnumerableType(sequence_type));

                if (!response && (sequence_type != null))
                {
                    foreach (var it in sequence_type.GetInterfaces())
                    {
                        if (it.IsGenericType && typeof(IEnumerable<>).IsAssignableFrom(it.GetType()))
                        {
                            response = true;
                            break;
                        }
                    }
                }
            }
        }

        return response;
    }

    #endregion
}
