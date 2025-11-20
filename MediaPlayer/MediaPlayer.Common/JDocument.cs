using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Threading;
using Newtonsoft.Json;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace MediaPlayer.Common;

/// <summary>
/// Representative JSON document
/// </summary>
public static partial class JDocument
{
    #region Constants

    public readonly static JsonDocumentOptions DocumentOptions
        = new()
        {
            AllowTrailingCommas = true,
            CommentHandling = JsonCommentHandling.Disallow,
            MaxDepth = int.MaxValue,
        };

    /// <summary>
    /// 
    /// </summary>
    public readonly static JsonNodeOptions NodeOptions
        = new()
        {
            PropertyNameCaseInsensitive = true
        };

    /// <summary>
    /// 
    /// </summary>
    public readonly static JsonSerializerOptions SerializerOptions
        = new ()
        {
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Disallow,
            MaxDepth = int.MaxValue,
            PropertyNameCaseInsensitive = true,
        };

    #endregion

    #region Functions

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="document"></param>
    /// <returns></returns>
    public static (T?, Exception?) Get<T>(string? key, JsonDocument? document)
    {
        T? entry = default;

        Exception? error = default;

        if (document == null || string.IsNullOrEmpty(key))
        {
            return (entry, error ?? new ArgumentException("The provided key or document was NULL"));
        }

        if (document.RootElement.TryGetProperty(key.Trim(), out var property))
        {
            try
            {
                entry = property.Deserialize<T>(SerializerOptions);
            }
            catch (Exception violation)
            {
                error = violation;

                entry = default;
            }
        }

        return (entry, error);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="node"></param>
    /// <returns></returns>
    public static (T?, Exception?) Get<T>(JsonNode? node)
    {
        T? entry = default;

        Exception? error = default;

        if (node == null)
        {
            return (entry, error ?? new ArgumentNullException(nameof(node)));
        }

        try
        {
            if (node != null)
            {
                entry = node.GetValue<T>();
            }
        }
        catch (Exception violation)
        {
            error = violation;
        }

        return (entry, error);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="node"></param>
    /// <returns></returns>
    public static async Task<(T?, Exception?)> GetAsync<T>(JsonNode? node)
    {
        using JoinableTaskContext ctx = new();

        JoinableTaskFactory factory = new(ctx);

        return await factory.RunAsync(async () =>
        {
            return await Task.FromResult(Get<T>(node));
        });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="document"></param>
    /// <returns></returns>
    public static async Task<(T?, Exception?)?> GetAsync<T>(string? key, JsonDocument? document) where T : class, new()
    {
        using JoinableTaskContext ctx = new();

        JoinableTaskFactory factory = new(ctx);

        return await factory.RunAsync(async () =>
        {
             return await Task.FromResult(Get<T>(key, document));
        });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="document"></param>
    /// <param name="services"></param>
    /// <returns></returns>
    public static JResponse<T> GetList<T>(string? key, JsonNode? document, IServiceProvider? provider = null) 
        where T : class, new()
    {
        var response = new JResponse<T>();

        if (document == null) return response;

        if ((document.Parent == null) && !string.IsNullOrEmpty(key))
        {
            key = key.Trim();

            var nodes = document[key];

            if (nodes != null)
            {
                return GetList<T>(key, nodes.AsObject(), provider);
            }
        }

        var document_nodes = document.AsObject();

        T? instance = default;

        var options = BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.SetProperty;

        if ((document_nodes != null) && (document_nodes.GetValueKind() == JsonValueKind.Object))
        {
            foreach (var document_node in document_nodes)
            {
                if ((document_node.Value != null) && (document_node.Value.GetValueKind() == JsonValueKind.Object))
                {
                    if (instance == null)
                    {
                        instance = new T();
                    }

                    var properties = instance.GetType().GetProperties(options);

                    var node = document_node.Value;

                    if ((node != null) && (node.GetValueKind() == JsonValueKind.Object))
                    {
                        foreach(var kv in node.AsObject())
                        {
                            foreach (var property in properties)
                            {
                                var attribute = TypeHelper.GetAttribute<JsonPropertyAttribute>(property);

                                if ((attribute != null) && !string.IsNullOrEmpty(attribute.PropertyName))
                                {
                                    if (typeof(ICollection<>).IsAssignableFrom(property.PropertyType) && 
                                        (kv.Value != null) && (kv.Value.GetValueKind() == JsonValueKind.Object))
                                    {
                                        object[]? components = default;

                                        try
                                        {
                                            components = (object[]?)Activator.CreateInstance(property.PropertyType);
                                        }
                                        catch (Exception violation)
                                            when (violation is TypeLoadException || 
                                                violation is System.Runtime.InteropServices.COMException ||
                                                violation is MissingMethodException ||
                                                violation is MemberAccessException || 
                                                violation is MethodAccessException ||
                                                violation is TargetInvocationException ||
                                                violation is NotSupportedException ||
                                                violation is ArgumentNullException ||
                                                violation is ArgumentException)
                                        {
                                            components = default;

                                            if (response.Errors == null)
                                            {
                                                response.Errors = new AggregateException(violation);
                                            }
                                            else
                                            {
                                                response.Errors = new AggregateException(new Exception[] { response.Errors, violation });
                                            }
                                        }

                                        if (components != null)
                                        {
                                            var registrations = new List<object>();

                                            foreach (var component in components)
                                            {
                                                var vproperties = component.GetType().GetProperties();

                                                foreach (var kvv in kv.Value.AsObject())
                                                {
                                                    foreach (var vproperty in vproperties)
                                                    {
                                                        var vattribute = TypeHelper.GetAttribute<JsonPropertyAttribute>(vproperty);

                                                        if ((vattribute != null) && (vattribute.PropertyName != null) &&
                                                            vattribute.PropertyName.Equals(kvv.Key, StringComparison.OrdinalIgnoreCase))
                                                        {
                                                            SetNodeProperty(kvv.Value, component, vproperty);
                                                        }
                                                    }
                                                }

                                                registrations.Add(component);
                                            }

                                            if (registrations.Count == 1)
                                            {
                                                try
                                                {
                                                    property.SetValue(instance, registrations[0]);
                                                }
                                                catch (Exception violation)
                                                    when (violation is TargetInvocationException ||
                                                        violation is TargetException ||
                                                        violation is MethodAccessException ||
                                                        violation is ArgumentException)
                                                {
                                                    if (response.Errors == null)
                                                    {
                                                        response.Errors = new AggregateException(violation);
                                                    }
                                                    else
                                                    {
                                                        response.Errors = new AggregateException(new Exception[] { response.Errors, violation });
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else if ((kv.Value != null) && kv.Value.GetPropertyName().Equals(attribute.PropertyName))
                                    {
                                        if (property.PropertyType.IsValueType || (property.PropertyType.IsClass && ReferenceEquals(property.PropertyType, typeof(string))))
                                        {
                                            SetNodeProperty<T>(kv.Value, instance, property);
                                        }
                                        else if (property.PropertyType.IsInterface && (provider != null))
                                        {
                                            var component = provider.GetRequiredService(property.PropertyType);

                                            if (component != null)
                                            {
                                                var vproperties = component.GetType().GetProperties(options);

                                                foreach (var kvv in kv.Value.AsObject())
                                                {
                                                    foreach (var vproperty in vproperties)
                                                    {
                                                        var vattribute = TypeHelper.GetAttribute<JsonPropertyAttribute>(vproperty);

                                                        if ((vattribute != null) && (vattribute.PropertyName != null) &&
                                                            vattribute.PropertyName.Equals(kvv.Key, StringComparison.OrdinalIgnoreCase))
                                                        {
                                                            SetNodeProperty(kvv.Value, component, vproperty);
                                                        }
                                                    }
                                                }

                                                try
                                                {
                                                    property.SetValue(instance, component);
                                                }
                                                catch (Exception violation)
                                                    when (violation is TargetInvocationException ||
                                                        violation is TargetException ||
                                                        violation is MethodAccessException ||
                                                        violation is ArgumentException)
                                                {
                                                    if (response.Errors == null)
                                                    {
                                                        response.Errors = new AggregateException(violation);
                                                    }
                                                    else
                                                    {
                                                        response.Errors = new AggregateException(new Exception[] { response.Errors, violation });
                                                    }
                                                }
                                            }
                                        }
                                        else if (property.PropertyType.IsClass && !ReferenceEquals(property.PropertyType, typeof(string)))
                                        {
                                            object? component = default;

                                            try
                                            {
                                                component = Activator.CreateInstance(property.PropertyType, false);
                                            }
                                            catch (Exception violation)
                                                when (violation is TargetInvocationException ||
                                                    violation is TargetException ||
                                                    violation is MethodAccessException ||
                                                    violation is ArgumentException)
                                            {
                                                component = default;

                                                if (response.Errors == null)
                                                {
                                                    response.Errors = new AggregateException(violation);
                                                }
                                                else
                                                {
                                                    response.Errors = new AggregateException(new Exception[] { response.Errors, violation });
                                                }
                                            }

                                            if ((component != null) &&
                                                !(component.GetType().IsArray || component.GetType().IsSZArray) && !component.GetType().IsGenericType)
                                            {
                                                var vproperties = component.GetType().GetProperties(options);

                                                foreach (var kvv in kv.Value.AsObject())
                                                {
                                                    foreach (var vproperty in vproperties)
                                                    {
                                                        var vattribute = TypeHelper.GetAttribute<JsonPropertyAttribute>(vproperty);

                                                        if ((vattribute != null) && !string.IsNullOrEmpty(vattribute.PropertyName) &&
                                                            vattribute.PropertyName.Equals(kvv.Key, StringComparison.OrdinalIgnoreCase))
                                                        {
                                                            SetNodeProperty(kvv.Value, component, vproperty);
                                                        }
                                                    }
                                                }

                                                try
                                                {
                                                    property.SetValue(instance, component);
                                                }
                                                catch (Exception violation)
                                                    when (violation is TargetInvocationException ||
                                                        violation is TargetException ||
                                                        violation is MethodAccessException ||
                                                        violation is ArgumentException)
                                                {
                                                    if (response.Errors == null)
                                                    {
                                                        response.Errors = new AggregateException(violation);
                                                    }
                                                    else
                                                    {
                                                        response.Errors = new AggregateException(new Exception[] { response.Errors, violation });
                                                    }
                                                }
                                            }
                                            else if ((component != null) && (component.GetType().IsArray || component.GetType().IsSZArray))
                                            {
                                                
                                            }
                                            else if ((component != null) &&
                                                component.GetType().IsGenericType &&
                                                (component.GetType().GetGenericArguments().Length == 1))
                                            {
                                                var argType = component.GetType().GetGenericArguments()[0];

                                                List<object?> container = [];

                                                JsonArray? jarr = default;

                                                try
                                                {
                                                    jarr = kv.Value.AsArray();
                                                }
                                                catch (Exception violation)
                                                    when (violation is InvalidOperationException)
                                                {
                                                    jarr = default;

                                                    if (response.Errors == null)
                                                    {
                                                        response.Errors = new AggregateException(violation);
                                                    }
                                                    else
                                                    {
                                                        response.Errors = new AggregateException(new Exception[] { response.Errors, violation });
                                                    }
                                                }

                                                if (jarr != null)
                                                {
                                                    foreach (var kvr in jarr)
                                                    {
                                                        object? containerEntry = default;

                                                        try
                                                        {
                                                            if (argType.IsClass)
                                                            {
                                                                containerEntry = Activator.CreateInstance(argType, false);
                                                            }
                                                            else if (argType.IsInterface && (provider != null))
                                                            {
                                                                containerEntry = provider.GetRequiredService(argType);
                                                            }
                                                        }
                                                        catch (Exception violation)
                                                            when (violation is TargetInvocationException ||
                                                                violation is TargetException ||
                                                                violation is MethodAccessException ||
                                                                violation is ArgumentException)
                                                        {
                                                            containerEntry = default;

                                                            if (response.Errors == null)
                                                            {
                                                                response.Errors = new AggregateException(violation);
                                                            }
                                                            else
                                                            {
                                                                response.Errors = new AggregateException(new Exception[] { response.Errors, violation });
                                                            }
                                                        }

                                                        if ((containerEntry != null) &&
                                                            !(containerEntry.GetType().IsArray || containerEntry.GetType().IsSZArray) &&
                                                            !containerEntry.GetType().IsGenericType &&
                                                            (kvr != null))
                                                        {
                                                            var entryProperties = containerEntry.GetType().GetProperties();

                                                            foreach (var kvn in kvr.AsObject())
                                                            {
                                                                foreach (var entryProperty in entryProperties)
                                                                {
                                                                    var vattribute = TypeHelper.GetAttribute<JsonPropertyAttribute>(entryProperty);

                                                                    if ((vattribute != null) && !string.IsNullOrEmpty(vattribute.PropertyName))
                                                                    {
                                                                        if (kvn.Key.Trim().Equals(vattribute.PropertyName.Trim(), StringComparison.OrdinalIgnoreCase))
                                                                        {
                                                                            SetNodeProperty(kvn.Value, containerEntry, entryProperty);
                                                                        }
                                                                    }
                                                                }
                                                            }

                                                            container.Add(containerEntry);
                                                        }
                                                    }
                                                }

                                                if (container.Count > 0)
                                                {
                                                    MethodInfo[] actions = [];

                                                    try
                                                    {
                                                        actions = component.GetType().GetRuntimeMethods().ToArray();
                                                    }
                                                    catch (Exception violation)
                                                        when (violation is ArgumentNullException)
                                                    {
                                                        if (response.Errors == null)
                                                        {
                                                            response.Errors = new AggregateException(violation);
                                                        }
                                                        else
                                                        {
                                                            response.Errors = new AggregateException(new Exception[] { response.Errors, violation });
                                                        }

                                                        actions = [];
                                                    }

                                                    if (actions != null)
                                                    {
                                                        var clearAction = actions.FirstOrDefault(x => x.Name == "Clear" && x.IsPublic);

                                                        if (clearAction != null)
                                                        {
                                                            try
                                                            {
                                                                clearAction.Invoke(component, null);
                                                            }
                                                            catch (Exception violation)
                                                                when (violation is TypeLoadException ||
                                                                    violation is System.Runtime.InteropServices.COMException ||
                                                                    violation is MissingMethodException ||
                                                                    violation is MemberAccessException ||
                                                                    violation is MethodAccessException ||
                                                                    violation is TargetInvocationException ||
                                                                    violation is NotSupportedException ||
                                                                    violation is ArgumentNullException ||
                                                                    violation is ArgumentException)
                                                            {
                                                                if (response.Errors == null)
                                                                {
                                                                    response.Errors = new AggregateException(violation);
                                                                }
                                                                else
                                                                {
                                                                    response.Errors = new AggregateException(new Exception[] { response.Errors, violation });
                                                                }
                                                            }
                                                        }

                                                        var addAction = actions.FirstOrDefault(x => x.Name == "Add" && x.IsPublic);

                                                        if (addAction != null)
                                                        {
                                                            foreach (var entry in container)
                                                            {
                                                                if (entry != null)
                                                                {
                                                                    try
                                                                    {
                                                                        addAction.Invoke(component, new[] { entry });
                                                                    }
                                                                    catch (Exception violation)
                                                                        when (violation is TypeLoadException ||
                                                                            violation is System.Runtime.InteropServices.COMException ||
                                                                            violation is MissingMethodException ||
                                                                            violation is MemberAccessException ||
                                                                            violation is MethodAccessException ||
                                                                            violation is TargetInvocationException ||
                                                                            violation is NotSupportedException ||
                                                                            violation is ArgumentNullException ||
                                                                            violation is ArgumentException)
                                                                    {
                                                                        if (response.Errors == null)
                                                                        {
                                                                            response.Errors = new AggregateException(violation);
                                                                        }
                                                                        else
                                                                        {
                                                                            response.Errors = new AggregateException(new Exception[] { response.Errors, violation });
                                                                        }
                                                                    }
                                                                }
                                                            }

                                                            try
                                                            {
                                                                property.SetValue(instance, component);
                                                            }
                                                            catch (Exception violation)
                                                                when (violation is TargetInvocationException ||
                                                                    violation is TargetException ||
                                                                    violation is MethodAccessException ||
                                                                    violation is ArgumentException)
                                                            {
                                                                if (response.Errors == null)
                                                                {
                                                                    response.Errors = new AggregateException(violation);
                                                                }
                                                                else
                                                                {
                                                                    response.Errors = new AggregateException(new Exception[] { response.Errors, violation });
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else if ((component != null) &&
                                                component.GetType().IsGenericType &&
                                                (component.GetType().GetGenericArguments().Length > 1))
                                            {
                                                //TODO:
                                            }
                                        }
                                    }
                                }
                                else if ((attribute == null) && !string.IsNullOrEmpty(document_node.Key))
                                {
                                    if (property.PropertyType.IsValueType || (property.PropertyType.IsClass && ReferenceEquals(property.PropertyType, typeof(string))))
                                    {
                                        try
                                        {
                                            property.SetValue(instance, document_node.Key.Trim());
                                        }
                                        catch (Exception violation)
                                            when (violation is TargetInvocationException ||
                                                violation is TargetException ||
                                                violation is MethodAccessException ||
                                                violation is ArgumentException)
                                        {
                                            if (response.Errors == null)
                                            {
                                                response.Errors = new AggregateException(violation);
                                            }
                                            else
                                            {
                                                response.Errors = new AggregateException(new Exception[] { response.Errors, violation });
                                            }
                                        }
                                    }
                                    else if (property.PropertyType.IsInterface && (provider != null))
                                    {
                                        var component = provider.GetRequiredService(property.PropertyType);

                                        if (component != null && 
                                            (kv.Value != null) && (kv.Value.GetValueKind() == JsonValueKind.Object))
                                        {
                                            var vproperties = component.GetType().GetProperties(options);

                                            foreach (var kvv in kv.Value.AsObject())
                                            {
                                                foreach (var vproperty in vproperties)
                                                {
                                                    var vattribute = TypeHelper.GetAttribute<JsonPropertyAttribute>(vproperty);

                                                    if ((vattribute != null) && (vattribute.PropertyName != null) &&
                                                        vattribute.PropertyName.Equals(kvv.Key, StringComparison.OrdinalIgnoreCase))
                                                    {
                                                        SetNodeProperty<object>(kvv.Value, component, vproperty);
                                                    }
                                                }
                                            }

                                            try
                                            {
                                                property.SetValue(instance, component);
                                            }
                                            catch (Exception violation)
                                                when (violation is TargetInvocationException ||
                                                    violation is TargetException ||
                                                    violation is MethodAccessException ||
                                                    violation is ArgumentException)
                                            {
                                                if (response.Errors == null)
                                                {
                                                    response.Errors = new AggregateException(violation);
                                                }
                                                else
                                                {
                                                    response.Errors = new AggregateException(new Exception[] { response.Errors, violation });
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if ((response.Errors == null) && (instance != null))
                    {
                        response.Container.Add(instance);
                    }
                }
                else if ((document_node.Value != null) && (document_node.Value.GetValueKind() == JsonValueKind.Number))
                {
                    if (instance == null)
                    {
                        instance = new T();
                    }

                    var properties = instance.GetType().GetProperties();

                    if (properties != null && properties.Length > 0)
                    {
                        foreach (var property in properties)
                        {
                            var attribute = TypeHelper.GetAttribute<JsonPropertyAttribute>(property);

                            if ((attribute != null) && !string.IsNullOrEmpty(attribute.PropertyName))
                            {
                                if (document_node.Value.GetPropertyName().Equals(attribute.PropertyName))
                                {
                                    SetNodeProperty<T>(document_node.Value, instance, property);
                                }
                            }
                            else if ((attribute == null) && !string.IsNullOrEmpty(document_node.Key))
                            {
                                try
                                {
                                    property.SetValue(instance, document_node.Key.Trim());
                                }
                                catch (Exception violation)
                                    when (violation is TargetInvocationException ||
                                        violation is TargetException ||
                                        violation is MethodAccessException ||
                                        violation is ArgumentException)
                                {
                                    if (response.Errors == null)
                                    {
                                        response.Errors = new AggregateException(violation);
                                    }
                                    else
                                    {
                                        response.Errors = new AggregateException(new Exception[] { response.Errors, violation });
                                    }
                                }
                            }
                        }
                    }

                }
                else if ((document_node.Value != null) && 
                    (document_node.Value.GetValueKind() == JsonValueKind.True || document_node.Value.GetValueKind() == JsonValueKind.False))
                {
                    if (instance == null)
                    {
                        instance = new T();
                    }

                    var properties = instance.GetType().GetProperties();

                    if (properties != null && properties.Length > 0)
                    {
                        foreach (var property in properties)
                        {
                            var attribute = TypeHelper.GetAttribute<JsonPropertyAttribute>(property);

                            if ((attribute != null) && !string.IsNullOrEmpty(attribute.PropertyName))
                            {
                                if (document_node.Value.GetPropertyName().Equals(attribute.PropertyName))
                                {
                                    SetNodeProperty<T>(document_node.Value, instance, property);
                                }
                            }
                            else if ((attribute == null) && !string.IsNullOrEmpty(document_node.Key))
                            {
                                try
                                {
                                    property.SetValue(instance, document_node.Key.Trim());
                                }
                                catch (Exception violation)
                                    when (violation is TargetInvocationException ||
                                        violation is TargetException ||
                                        violation is MethodAccessException ||
                                        violation is ArgumentException)
                                {
                                    if (response.Errors == null)
                                    {
                                        response.Errors = new AggregateException(violation);
                                    }
                                    else
                                    {
                                        response.Errors = new AggregateException(new Exception[] { response.Errors, violation });
                                    }
                                }
                            }
                        }
                    }
                }
                else if ((document_node.Value != null) && (document_node.Value.GetValueKind() == JsonValueKind.Array))
                {
                    if (instance == null)
                    {
                        instance = new T();
                    }

                    var properties = instance.GetType().GetProperties();

                    // The instance contains numerous interface based / class based visible properties

                    var arr = document_node.Value.AsArray(); /* JsonNode collection */

                    if (arr != null)
                    {
                        foreach (var node in arr)
                        {
                            if ((node != null) && (node.AsObject() != null) && (properties != null)) 
                            {
                                foreach (var kvv in node.AsObject())
                                {
                                    foreach (var property in properties)
                                    {
                                        JsonPropertyAttribute? attribute = default;

                                        if (property != null)
                                        {
                                            attribute = TypeHelper.GetAttribute<JsonPropertyAttribute>(property);
                                        }

                                        if ((attribute != null) && 
                                            (property != null) && (attribute.PropertyName?.Equals(kvv.Key) ?? false))
                                        {
                                            SetNodeProperty<T>(kvv.Value, instance, property);
                                        }
                                        else if ((attribute == null) && (property != null) && !string.IsNullOrEmpty(kvv.Key))
                                        {
                                            try
                                            {
                                                property.SetValue(instance, kvv.Key.Trim());
                                            }
                                            catch (Exception violation)
                                                when (violation is TargetInvocationException ||
                                                    violation is TargetException ||
                                                    violation is MethodAccessException ||
                                                    violation is ArgumentException)
                                            {
                                                if (response.Errors == null)
                                                {
                                                    response.Errors = new AggregateException(violation);
                                                }
                                                else
                                                {
                                                    response.Errors = new AggregateException(new Exception[] { response.Errors, violation });
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                }
                else if ((document_node.Value != null) && (document_node.Value.GetValueKind() == JsonValueKind.String))
                {
                    if (instance == null)
                    {
                        instance = new T();
                    }

                    var properties = instance.GetType().GetProperties();

                    if (properties != null && properties.Length > 0)
                    {
                        foreach (var property in properties)
                        {
                            var attribute = TypeHelper.GetAttribute<JsonPropertyAttribute>(property);

                            if ((attribute != null) && !string.IsNullOrEmpty(attribute.PropertyName))
                            {
                                if (document_node.Value.GetPropertyName().Equals(attribute.PropertyName))
                                {
                                    SetNodeProperty<T>(document_node.Value, instance, property);
                                }
                            }
                            else if ((attribute == null) && !string.IsNullOrEmpty(document_node.Key))
                            {
                                try
                                {
                                    property.SetValue(instance, document_node.Key.Trim());
                                }
                                catch (Exception violation)
                                    when (violation is TargetInvocationException ||
                                        violation is TargetException ||
                                        violation is MethodAccessException ||
                                        violation is ArgumentException)
                                {
                                    if (response.Errors == null)
                                    {
                                        response.Errors = new AggregateException(violation);
                                    }
                                    else
                                    {
                                        response.Errors = new AggregateException(new Exception[] { response.Errors, violation });
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if ((response.Errors == null) && (instance != null))
            {
                response.Container.Add(instance);
            }
        }
        else if ((document_nodes != null) && (document_nodes.GetValueKind() == JsonValueKind.String))
        {
            if (instance == null)
            {
                instance = new T();
            }

            var properties = instance.GetType().GetProperties(options);

            var count = document_nodes.Count;

            for (int pos = 0; pos < count; ++pos)
            {
                var document_node = document_nodes.GetAt(pos);

                if (!string.IsNullOrEmpty(document_node.Key))
                {
                    foreach (var property in properties)
                    {
                        if (ReferenceEquals(property.PropertyType, typeof(string)) ||
                            ReferenceEquals(property.PropertyType, typeof(char)))
                        {
                            var vattribute = TypeHelper.GetAttribute<JsonPropertyAttribute>(property);

                            if ((vattribute != null) && !string.IsNullOrEmpty(vattribute.PropertyName))
                            {
                                if (vattribute.PropertyName.Equals(document_node.Key, StringComparison.OrdinalIgnoreCase))
                                {
                                    SetNodeProperty(document_node.Value, instance, property);
                                }
                            }
                        }
                    }
                }
            }

            if (instance != null && response.Errors == null)
            {
                response.Container.Add(instance);
            }
        }
        else if ((document_nodes != null) && (document_nodes.GetValueKind() == JsonValueKind.Array))
        {
            if (instance == null)
            {
                instance = new T();
            }

            var jarr = document_nodes.ToArray();

            if (jarr != null && jarr.Length > 0)
            {
                var properties = instance.GetType().GetProperties(options);

                foreach (var jnode in jarr)
                {
                    if (string.IsNullOrEmpty(jnode.Key)) continue;

                    foreach (var property in properties)
                    {
                        var vattribute = TypeHelper.GetAttribute<JsonPropertyAttribute>(property);

                        if ((vattribute != null) && !string.IsNullOrEmpty(vattribute.PropertyName))
                        {
                            if (vattribute.PropertyName.Equals(jnode.Key, StringComparison.OrdinalIgnoreCase))
                            {
                                SetNodeProperty(jnode.Value, instance, property);
                            }
                        }
                    }
                }

                if (instance != null && response.Errors == null)
                {
                    response.Container.Add(instance);
                }
            }
        }
        else if ((document_nodes != null) && (document_nodes.GetValueKind() == JsonValueKind.Number))
        {
            if (instance == null)
            {
                instance = new T();
            }

            var properties = instance.GetType().GetProperties(options);

            var count = document_nodes.Count;

            for (int pos = 0; pos < count; ++pos)
            {
                var document_node = document_nodes.GetAt(pos);

                if (!string.IsNullOrEmpty(document_node.Key))
                {
                    foreach (var property in properties)
                    {
                        if (ReferenceEquals(property.PropertyType, typeof(byte)) ||
                            ReferenceEquals(property.PropertyType, typeof(short)) ||
                            ReferenceEquals(property.PropertyType, typeof(ushort)) ||
                            ReferenceEquals(property.PropertyType, typeof(int)) ||
                            ReferenceEquals(property.PropertyType, typeof(uint)) ||
                            ReferenceEquals(property.PropertyType, typeof(long)) ||
                            ReferenceEquals(property.PropertyType, typeof(ulong)) ||
                            ReferenceEquals(property.PropertyType, typeof(BigInteger)) ||
                            ReferenceEquals(property.PropertyType, typeof(float)) ||
                            ReferenceEquals(property.PropertyType, typeof(double)) ||
                            ReferenceEquals(property.PropertyType, typeof(decimal)))
                        {
                            var vattribute = TypeHelper.GetAttribute<JsonPropertyAttribute>(property);

                            if ((vattribute != null) && !string.IsNullOrEmpty(vattribute.PropertyName))
                            {
                                if (vattribute.PropertyName.Equals(document_node.Key, StringComparison.OrdinalIgnoreCase))
                                {
                                    SetNodeProperty(document_node.Value, instance, property);
                                }
                            }
                        }
                    }
                }
            }

            if (instance != null && response.Errors == null)
            {
                response.Container.Add(instance);
            }
        }
        else if ((document_nodes != null) && 
            (document_nodes.GetValueKind() == JsonValueKind.True || document_nodes.GetValueKind() == JsonValueKind.False))
        {
            if (instance == null)
            {
                instance = new T();
            }

            var properties = instance.GetType().GetProperties(options);

            var count = document_nodes.Count;

            for (int pos = 0; pos < count; ++pos)
            {
                var document_node = document_nodes.GetAt(pos);

                if (!string.IsNullOrEmpty(document_node.Key))
                {
                    foreach (var property in properties)
                    {
                        if (ReferenceEquals(property.PropertyType, typeof(bool)))
                        {
                            var vattribute = TypeHelper.GetAttribute<JsonPropertyAttribute>(property);

                            if ((vattribute != null) && !string.IsNullOrEmpty(vattribute.PropertyName))
                            {
                                if (vattribute.PropertyName.Equals(document_node.Key, StringComparison.OrdinalIgnoreCase))
                                {
                                    SetNodeProperty(document_node.Value, instance, property);
                                }
                            }
                        }
                    }
                }
            }

            if (instance != null && response.Errors == null)
            {
                response.Container.Add(instance);
            }
        }

        return response;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="document"></param>
    /// <param name="services"></param>
    /// <returns></returns>
    public static async Task<JResponse<T>> GetListAsync<T>(string? key, JsonNode? document, IServiceProvider? provider = null) 
        where T: class, new()
    {
        using JoinableTaskContext ctx = new();

        JoinableTaskFactory factory = new(ctx);

        return await factory.RunAsync(async () => 
        {
            var response = GetList<T>(key, document, provider);

            return await Task.FromResult(response);
        });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public static (JsonDocument?, Exception?) ParseContent(string? content)
    {
        JsonDocument? document = default;

        Exception? error = default;

        if (!string.IsNullOrEmpty(content))
        {
            try
            {
                var resource = Path.GetTempFileName();

                using var stream = File.Open(resource, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);

                stream.Write(Encoding.UTF8.GetBytes(content));

                stream.Flush();

                stream.Close();

                using FileStream channel = File.Open(resource, FileMode.Open, FileAccess.Read, FileShare.Read);

                JsonDocumentOptions options = new()
                {
                    AllowTrailingCommas = false,
                    CommentHandling = JsonCommentHandling.Skip,
                    MaxDepth = int.MaxValue,
                };

                document = JsonDocument.Parse(channel, options);

                channel.Close();

                File.Delete(resource);
            }
            catch (Exception violation)
            {
                error = violation;  
            }
        }

        return (document, error);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public static async Task<(JsonDocument?, Exception?)> ParseContentAsync(string? content)
    {
        JsonDocument? document = default;

        Exception? error = default;

        if (!string.IsNullOrEmpty(content))
        {
            try
            {
                var resource = Path.GetTempFileName();

                using var stream = File.Open(resource, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);

                await stream.WriteAsync(Encoding.UTF8.GetBytes(content));

                await stream.FlushAsync();

                stream.Close();

                using FileStream channel = File.Open(resource, FileMode.Open, FileAccess.Read, FileShare.Read);

                JsonDocumentOptions options = new()
                {
                    AllowTrailingCommas = false,
                    CommentHandling = JsonCommentHandling.Skip,
                    MaxDepth = int.MaxValue,
                };

                document = JsonDocument.Parse(channel, options);

                channel.Close();

                File.Delete(resource);
            }
            catch (Exception violation)
            {
                error = violation;
            }
        }

        return await Task.FromResult((document, error));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static (JsonDocument?, Exception?) ParseDocument(string? path)
    {
        JsonDocument? document = default;

        Exception? error = default;

        if (!string.IsNullOrEmpty(path) && File.Exists(path.Trim()))
        {
            try
            {
                using var stream = File.Open(path.Trim(), FileMode.Open, FileAccess.Read, FileShare.Read);

                document = JsonDocument.Parse(stream);
            }
            catch (Exception violation)
            {
                document = default;

                error = violation;
            }
        }
        return (document, error);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns> 
    public static async Task<(JsonDocument?, Exception?)> ParseDocumentAsync(string? path)
    {
        using JoinableTaskContext ctx = new();

        JoinableTaskFactory factory = new (ctx);

        return await factory.RunAsync(async () =>
        {
            return await Task.FromResult(ParseDocument(path));
        });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static (JsonDocument?, Exception?) ParseDocument(string? directory, string? filename)
    {
        if (!string.IsNullOrEmpty(directory) && Directory.Exists(directory.Trim()) && !string.IsNullOrEmpty(filename))
        {
            return ParseDocument(Path.Combine(directory.Trim(), filename.Trim()));
        }
        return (default, default);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static (JsonDocument?, Exception?) ParseDocument(FileInfo? locator)
    {
        JsonDocument? document = default;

        Exception? error = default;

        if (locator != null && locator.Exists)
        {
            try
            {
                using var stream = File.Open(locator.FullName.Trim(), FileMode.Open, FileAccess.Read, FileShare.Read);

                JsonDocument.Parse(stream, new JsonDocumentOptions { MaxDepth = int.MaxValue });
            }
            catch (Exception violation)
            {
                error = violation;

                document = null;
            }
        }
        return (document, error);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="directory"></param>
    /// <param name="filename"></param>
    /// <returns></returns>
    public static async Task<(JsonDocument?, Exception?)> ParseDocumentAsync(string? directory, string? filename)
    {
        using JoinableTaskContext ctx = new();

        JoinableTaskFactory factory = new(ctx);

        return await factory.RunAsync(async () =>
        {
            return await Task.FromResult(ParseDocument(directory, filename));
        });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static async Task<(JsonDocument?, Exception?)> ParseDocumentAsync(FileInfo? locator)
    {
        using JoinableTaskContext ctx = new();

        JoinableTaskFactory factory = new(ctx);

        return await factory.RunAsync(async () => 
        {
            return await Task.FromResult(ParseDocument(locator));
        });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="directory"></param>
    /// <param name="filename"></param>
    /// <returns></returns>
    public static (JsonNode?, Exception?) ParseNode(string? directory, string? filename)
    {
        if ((directory != null) && Directory.Exists(directory))
        {
            if (filename != null && filename.Trim().Length > 0)
            {
                var resource = Path.Combine(directory, filename);

                if (File.Exists(resource)) return ParseNode(new FileInfo(resource));
            }
        }

        return (default, default);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static (JsonNode?, Exception?) ParseNode(string? path)
    {
        JsonNode? node = default;

        Exception? error = default;

        if (path != null && File.Exists(path.Trim()))
        {
            try
            {
                using var stream = File.Open(path.Trim(), FileMode.Open, FileAccess.Read, FileShare.Read);

                node = JsonNode.Parse(stream, NodeOptions, DocumentOptions);
            }
            catch (Exception violation)
            {
                error = violation;

                node = null;
            }
        }
        return (node, error);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="locator"></param>
    /// <returns></returns>
    public static (JsonNode?, Exception?) ParseNode(FileInfo? locator)
    {
        JsonNode? node = default;

        Exception? error = default;

        if (locator != null && locator.Exists)
        {
            try
            {
                using var stream = File.Open(locator.FullName.Trim(), FileMode.Open, FileAccess.Read, FileShare.Read);

                node = JsonNode.Parse(stream, NodeOptions, DocumentOptions);
            }
            catch (Exception violation)
            {
                error = violation;

                node = null;
            }
        }
        return (node, error);
    }

    ////
    ///

    /// <summary>
    /// 
    /// </summary>
    /// <param name="directory"></param>
    /// <param name="filename"></param>
    /// <returns></returns>
    public async static Task<(JsonNode?, Exception?)> ParseNodeAsync(string? directory, string? filename)
    {
        using JoinableTaskContext ctx = new();

        JoinableTaskFactory factory = new(ctx);

        return await factory.RunAsync(async () =>
        {
            return await Task.FromResult(ParseNode(directory, filename));
        });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public async static Task<(JsonNode?, Exception?)> ParseNodeAsync(string? path)
    {
        using JoinableTaskContext ctx = new();

        JoinableTaskFactory factory = new(ctx);

        return await factory.RunAsync(async () =>
        {
            return await Task.FromResult(ParseNode(path));
        });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="locator"></param>
    /// <returns></returns>
    public static async Task<(JsonNode?, Exception?)> ParseNodeAsync(FileInfo? locator)
    {
        using JoinableTaskContext ctx = new();

        JoinableTaskFactory factory = new(ctx);

        return await factory.RunAsync(async () =>
        {
            return await Task.FromResult(ParseNode(locator));
        });
    }

    #endregion

    #region Internal Functions

    /// <summary>
    /// 
    /// </summary>
    /// <param name="property"></param>
    /// <param name="node"></param>
    /// <returns></returns>
    private static object? GetEnumValue(PropertyInfo? property, JsonNode? node)
    {
        if (node != null && property != null && property.PropertyType.IsValueType && property.PropertyType.IsEnum)
        {
            var valueType = property.PropertyType.UnderlyingSystemType;

            if ((valueType != null) && (node.GetValueKind() == JsonValueKind.String))
            {
                var content = node.GetValue<string>();

                if (content != null && Enum.TryParse(valueType, content.Trim(), out var outcome))
                {
                    return outcome;
                }
            }
            else if ((valueType != null) && (node.GetValueKind() == JsonValueKind.Number))
            {
                var valueBaseType = Enum.GetUnderlyingType(valueType);

                if (valueBaseType != null && valueBaseType.Equals(typeof(bool)))
                {
                    if (node.GetValueKind() == JsonValueKind.String)
                    {
                        var content = node.GetValue<string>();

                        if (content != null && bool.TryParse(content.Trim(), out var outcome)) return outcome;
                    }
                    else if (node.GetValueKind() == JsonValueKind.True || node.GetValueKind() == JsonValueKind.False)
                    {
                        return node.GetValue<bool>();
                    }
                }

                if (valueBaseType != null && valueBaseType.Equals(typeof(byte)))
                {
                    if (node.GetValueKind() == JsonValueKind.String)
                    {
                        var content = node.GetValue<string>();

                        if (content != null && byte.TryParse(content.Trim(), out var outcome)) return outcome;
                    }
                    else if (node.GetValueKind() == JsonValueKind.Number)
                    {
                        return node.GetValue<byte>();
                    }
                }

                if (valueBaseType != null && valueBaseType.Equals(typeof(sbyte)))
                {
                    if (node.GetValueKind() == JsonValueKind.String)
                    {
                        var content = node.GetValue<string>();

                        if (content != null && sbyte.TryParse(content.Trim(), out var outcome)) return outcome;
                    }
                    else if (node.GetValueKind() == JsonValueKind.Number)
                    {
                        return node.GetValue<sbyte>();
                    }
                }

                if (valueBaseType != null && valueBaseType.Equals(typeof(char)))
                {
                    if (node.GetValueKind() == JsonValueKind.String)
                    {
                        var content = node.GetValue<string>();

                        if (content != null && char.TryParse(content.Trim(), out var outcome)) return outcome;
                    }
                    else if (node.GetValueKind() == JsonValueKind.Number)
                    {
                        return node.GetValue<char>();
                    }
                }

                if (valueBaseType != null && valueBaseType.Equals(typeof(short)))
                {
                    if (node.GetValueKind() == JsonValueKind.String)
                    {
                        var content = node.GetValue<string>();

                        if (content != null && short.TryParse(content.Trim(), out var outcome)) return outcome;
                    }
                    else if (node.GetValueKind() == JsonValueKind.Number)
                    {
                        return node.GetValue<short>();
                    }
                }

                if (valueBaseType != null && valueBaseType.Equals(typeof(ushort)))
                {
                    if (node.GetValueKind() == JsonValueKind.String)
                    {
                        var content = node.GetValue<string>();

                        if (content != null && ushort.TryParse(content.Trim(), out var outcome)) return outcome;
                    }
                    else if (node.GetValueKind() == JsonValueKind.Number)
                    {
                        return node.GetValue<ushort>();
                    }
                }

                if (valueBaseType != null && valueBaseType.Equals(typeof(int)))
                {
                    if (node.GetValueKind() == JsonValueKind.String)
                    {
                        var content = node.GetValue<string>();

                        if (content != null && int.TryParse(content.Trim(), out var outcome)) return outcome;
                    }
                    else if (node.GetValueKind() == JsonValueKind.Number)
                    {
                        return node.GetValue<int>();
                    }
                }

                if (valueBaseType != null && valueBaseType.Equals(typeof(uint)))
                {
                    if (node.GetValueKind() == JsonValueKind.String)
                    {
                        var content = node.GetValue<string>();

                        if (content != null && uint.TryParse(content.Trim(), out var outcome)) return outcome;
                    }
                    else if (node.GetValueKind() == JsonValueKind.Number)
                    {
                        return node.GetValue<uint>();
                    }
                }

                if (valueBaseType != null && valueBaseType.Equals(typeof(long)))
                {
                    if (node.GetValueKind() == JsonValueKind.String)
                    {
                        var content = node.GetValue<string>();

                        if (content != null && long.TryParse(content.Trim(), out var outcome)) return outcome;
                    }
                    else if (node.GetValueKind() == JsonValueKind.Number)
                    {
                        return node.GetValue<long>();
                    }
                }

                if (valueBaseType != null && valueBaseType.Equals(typeof(ulong)))
                {
                    if (node.GetValueKind() == JsonValueKind.String)
                    {
                        var content = node.GetValue<string>();

                        if (content != null && ulong.TryParse(content.Trim(), out var outcome)) return outcome;
                    }
                    else if (node.GetValueKind() == JsonValueKind.Number)
                    {
                        return node.GetValue<ulong>();
                    }
                }

                if (valueBaseType != null && valueBaseType.Equals(typeof(float)))
                {
                    if (node.GetValueKind() == JsonValueKind.String)
                    {
                        var content = node.GetValue<string>();

                        if (content != null && float.TryParse(content.Trim(), out var outcome)) return outcome;
                    }
                    else if (node.GetValueKind() == JsonValueKind.Number)
                    {
                        return node.GetValue<float>();
                    }
                }

                if (valueBaseType != null && valueBaseType.Equals(typeof(double)))
                {
                    if (node.GetValueKind() == JsonValueKind.String)
                    {
                        var content = node.GetValue<string>();

                        if (content != null && double.TryParse(content.Trim(), out var outcome)) return outcome;
                    }
                    else if (node.GetValueKind() == JsonValueKind.Number)
                    {
                        return node.GetValue<double>();
                    }
                }

                if (valueBaseType != null && valueBaseType.Equals(typeof(BigInteger)))
                {
                    if (node.GetValueKind() == JsonValueKind.String)
                    {
                        var content = node.GetValue<string>();

                        if (content != null && BigInteger.TryParse(content.Trim(), out var outcome)) return outcome;
                    }
                    else if (node.GetValueKind() == JsonValueKind.Number)
                    {
                        return node.GetValue<BigInteger>();
                    }
                }

                if (valueBaseType != null && valueBaseType.Equals(typeof(TimeSpan)))
                {
                    if (node.GetValueKind() == JsonValueKind.String)
                    {
                        var content = node.GetValue<string>();

                        if (content != null && TimeSpan.TryParse(content.Trim(), out var outcome)) return outcome;
                    }
                    else
                    {
                        return node.GetValue<TimeSpan>();
                    }
                }

                if (valueBaseType != null && valueBaseType.Equals(typeof(DateTime)))
                {
                    if (node.GetValueKind() == JsonValueKind.String)
                    {
                        var content = node.GetValue<string>();

                        if (content != null && DateTime.TryParse(content.Trim(), out var outcome)) return outcome;
                    }
                    else
                    {
                        return node.GetValue<DateTime>();
                    }
                }

                if (valueType.Equals(typeof(Guid)))
                {
                    if (node.GetValueKind() == JsonValueKind.String)
                    {
                        var content = node.GetValue<string>();

                        if (content != null && Guid.TryParse(content.Trim(), out var outcome)) return outcome;
                    }
                    else
                    {
                        return node.GetValue<Guid>();
                    }
                }

                if (valueBaseType != null && valueBaseType.Equals(typeof(string)))
                {
                    if (node.GetValueKind() == JsonValueKind.String)
                    {
                        return node.GetValue<string>();
                    }
                    else
                    {
                        return node.GetValue<object>().ToString();
                    }
                }

            }
        }

        return null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="node"></param>
    /// <param name="property"></param>
    private static void SetNodeProperty<T>(JsonNode? node, T? instance, PropertyInfo? property) where T : class, new()
    {
        if ((node != null) && (instance != null) && (property != null))
        {
            if (property.PropertyType.Equals(typeof(bool?)))
            {
                property.SetValue(instance, node.GetValue<bool?>());
            }
            else if (property.PropertyType.IsValueType && property.PropertyType.Equals(typeof(bool)))
            {
                property.SetValue(instance, node.GetValue<bool>());
            }
            else if (property.PropertyType.Equals(typeof(byte?)))
            {
                property.SetValue(instance, node.GetValue<byte?>());
            }
            else if (property.PropertyType.IsValueType && property.PropertyType.Equals(typeof(byte)))
            {
                property.SetValue(instance, node.GetValue<byte>());
            }
            else if (property.PropertyType.Equals(typeof(sbyte?)))
            {
                property.SetValue(instance, node.GetValue<sbyte?>());
            }
            else if (property.PropertyType.IsValueType && property.PropertyType.Equals(typeof(sbyte)))
            {
                property.SetValue(instance, node.GetValue<sbyte>());
            }
            else if (property.PropertyType.Equals(typeof(char?)))
            {
                property.SetValue(instance, node.GetValue<char?>());
            }
            else if (property.PropertyType.IsValueType && property.PropertyType.Equals(typeof(char)))
            {
                property.SetValue(instance, node.GetValue<char>());
            }
            else if (property.PropertyType.Equals(typeof(short?)))
            {
                property.SetValue(instance, node.GetValue<short?>());
            }
            else if (property.PropertyType.IsValueType && property.PropertyType.Equals(typeof(short)))
            {
                property.SetValue(instance, node.GetValue<short>());
            }
            else if (property.PropertyType.Equals(typeof(ushort?)))
            {
                property.SetValue(instance, node.GetValue<ushort?>());
            }
            else if (property.PropertyType.IsValueType && property.PropertyType.Equals(typeof(ushort)))
            {
                property.SetValue(instance, node.GetValue<ushort>());
            }
            else if (property.PropertyType.Equals(typeof(int?)))
            {
                property.SetValue(instance, node.GetValue<int?>());
            }
            else if (property.PropertyType.IsValueType && property.PropertyType.Equals(typeof(int)))
            {
                property.SetValue(instance, node.GetValue<int>());
            }
            else if (property.PropertyType.Equals(typeof(uint?)))
            {
                property.SetValue(instance, node.GetValue<uint?>());
            }
            else if (property.PropertyType.IsValueType && property.PropertyType.Equals(typeof(uint)))
            {
                property.SetValue(instance, node.GetValue<uint>());
            }
            else if (property.PropertyType.Equals(typeof(long?)))
            {
                property.SetValue(instance, node.GetValue<long?>());
            }
            else if (property.PropertyType.IsValueType && property.PropertyType.Equals(typeof(long)))
            {
                property.SetValue(instance, node.GetValue<long>());
            }
            else if (property.PropertyType.Equals(typeof(ulong?)))
            {
                property.SetValue(instance, node.GetValue<ulong?>());
            }
            else if (property.PropertyType.IsValueType && property.PropertyType.Equals(typeof(ulong)))
            {
                property.SetValue(instance, node.GetValue<ulong>());
            }
            else if (property.PropertyType.Equals(typeof(float?)))
            {
                property.SetValue(instance, node.GetValue<float?>());
            }
            else if (property.PropertyType.IsValueType && property.PropertyType.Equals(typeof(float)))
            {
                property.SetValue(instance, node.GetValue<float>());
            }
            else if (property.PropertyType.Equals(typeof(double?)))
            {
                property.SetValue(instance, node.GetValue<double?>());
            }
            else if (property.PropertyType.IsValueType && property.PropertyType.Equals(typeof(double)))
            {
                property.SetValue(instance, node.GetValue<double>());
            }
            else if (property.PropertyType.IsValueType && property.PropertyType.IsEnum)
            {
                property.SetValue(instance, GetEnumValue(property, node));
            }
            else if (property.PropertyType.Equals(typeof(BigInteger?)))
            {
                property.SetValue(instance, node.GetValue<BigInteger?>());
            }
            else if (property.PropertyType.Equals(typeof(BigInteger)))
            {
                property.SetValue(instance, node.GetValue<BigInteger>());
            }
            else if (property.PropertyType.Equals(typeof(TimeSpan?)))
            {
                property.SetValue(instance, node.GetValue<TimeSpan?>());
            }
            else if (property.PropertyType.Equals(typeof(TimeSpan)))
            {
                property.SetValue(instance, node.GetValue<TimeSpan>());
            }
            else if (property.PropertyType.Equals(typeof(DateTime?)))
            {
                property.SetValue(instance, node.GetValue<DateTime?>());
            }
            else if (property.PropertyType.Equals(typeof(DateTime)))
            {
                property.SetValue(instance, node.GetValue<DateTime>());
            }
            else if (property.PropertyType.Equals(typeof(Guid?)))
            {
                property.SetValue(instance, node.GetValue<Guid?>());
            }
            else if (property.PropertyType.Equals(typeof(Guid)))
            {
                property.SetValue(instance, node.GetValue<Guid>());
            }
            else if (property.PropertyType.Equals(typeof(string)))
            {
                property.SetValue(instance, node.GetValue<string>());
            }
        }

    }

    #endregion
}
