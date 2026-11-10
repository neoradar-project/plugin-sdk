using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace NeoRadar.PluginSdk.Tests.PublicApi;

// Every type and member a plugin can see, one line each, sorted ordinally. A member whose line
// changes reads as one removed and one added.
internal static class PublicApiSurface
{
    private const BindingFlags AllDeclared =
        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;

    private static readonly Dictionary<Type, string> s_keywords = new()
    {
        [typeof(void)] = "void",
        [typeof(bool)] = "bool",
        [typeof(byte)] = "byte",
        [typeof(sbyte)] = "sbyte",
        [typeof(char)] = "char",
        [typeof(short)] = "short",
        [typeof(ushort)] = "ushort",
        [typeof(int)] = "int",
        [typeof(uint)] = "uint",
        [typeof(long)] = "long",
        [typeof(ulong)] = "ulong",
        [typeof(float)] = "float",
        [typeof(double)] = "double",
        [typeof(decimal)] = "decimal",
        [typeof(string)] = "string",
        [typeof(object)] = "object",
    };

    internal static List<string> Describe(Assembly assembly) => Describe(assembly.GetExportedTypes());

    internal static List<string> Describe(IEnumerable<Type> types)
    {
        NullabilityInfoContext nullability = new();
        List<string> lines = new();
        foreach (Type type in types)
        {
            lines.Add(DescribeType(type, nullability));
            if (IsDelegate(type)) continue;
            string owner = Qualified(type) + " :: ";
            foreach (MemberInfo member in type.GetMembers(AllDeclared))
            {
                string? line = DescribeMember(member, type, nullability);
                if (line is not null) lines.Add(owner + line);
            }
        }

        lines.Sort(StringComparer.Ordinal);
        return lines;
    }

    private static string DescribeType(Type type, NullabilityInfoContext nullability)
    {
        string name = Qualified(type) + GenericParameters(type.GetGenericArguments());
        if (type.IsEnum) return $"enum {name} : {TypeName(Enum.GetUnderlyingType(type), null)}";
        if (IsDelegate(type))
        {
            MethodInfo invoke = type.GetMethod("Invoke")!;
            return $"delegate {TypeName(invoke.ReturnType, nullability.Create(invoke.ReturnParameter))} {name}({Parameters(invoke, nullability)})";
        }

        string kind;
        if (type.IsInterface) kind = "interface";
        else if (type.IsValueType)
        {
            bool isReadOnly = type.CustomAttributes.Any(a => a.AttributeType.FullName == "System.Runtime.CompilerServices.IsReadOnlyAttribute");
            bool isRecord = type.GetMethod("PrintMembers", AllDeclared) is not null;
            kind = (isReadOnly ? "readonly " : "") + (isRecord ? "record struct" : "struct");
        }
        else
        {
            string modifiers = type.IsAbstract && type.IsSealed ? "static "
                : type.IsAbstract ? "abstract "
                : type.IsSealed ? "sealed "
                : "";
            bool isRecord = type.GetMethod("<Clone>$", AllDeclared) is not null;
            kind = modifiers + (isRecord ? "record" : "class");
        }

        List<string> bases = new();
        if (type.BaseType is not null && type.BaseType != typeof(object) && type.BaseType != typeof(ValueType))
            bases.Add(TypeName(type.BaseType, null));
        bases.AddRange(type.GetInterfaces().Select(i => TypeName(i, null)).OrderBy(n => n, StringComparer.Ordinal));
        return bases.Count == 0 ? $"{kind} {name}" : $"{kind} {name} : {string.Join(", ", bases)}";
    }

    private static string? DescribeMember(MemberInfo member, Type owner, NullabilityInfoContext nullability)
    {
        if (member.Name.StartsWith('<')) return null;
        switch (member)
        {
            case FieldInfo field when IsVisible(field) && !field.IsSpecialName:
                return DescribeField(field, owner, nullability);
            case ConstructorInfo constructor when IsVisible(constructor):
                return $"{Modifiers(constructor, owner)}{(constructor.IsStatic ? ".cctor" : ".ctor")}({Parameters(constructor, nullability)})";
            case MethodInfo method when IsVisible(method) && (!method.IsSpecialName || method.Name.StartsWith("op_", StringComparison.Ordinal)):
                return $"{Modifiers(method, owner)}{TypeName(method.ReturnType, nullability.Create(method.ReturnParameter))} " +
                       $"{method.Name}{GenericParameters(method.GetGenericArguments())}({Parameters(method, nullability)})";
            case PropertyInfo property:
                return DescribeProperty(property, owner, nullability);
            case EventInfo evt when evt.AddMethod is not null && IsVisible(evt.AddMethod):
                return $"{Modifiers(evt.AddMethod, owner)}event {TypeName(evt.EventHandlerType!, nullability.Create(evt))} {evt.Name}";
            default:
                return null;
        }
    }

    private static string DescribeField(FieldInfo field, Type owner, NullabilityInfoContext nullability)
    {
        if (owner.IsEnum)
            return $"{field.Name} = {Convert.ToString(field.GetRawConstantValue(), CultureInfo.InvariantCulture)}";

        string type = TypeName(field.FieldType, nullability.Create(field));
        string visibility = field.IsFamily || field.IsFamilyOrAssembly ? "protected " : "";
        if (field.IsLiteral)
            return $"{visibility}const {type} {field.Name} = {Literal(field.GetRawConstantValue(), field.FieldType)}";

        string modifiers = (field.IsStatic ? "static " : "") + (field.IsInitOnly ? "readonly " : "");
        return $"{visibility}{modifiers}{type} {field.Name}";
    }

    private static string? DescribeProperty(PropertyInfo property, Type owner, NullabilityInfoContext nullability)
    {
        MethodInfo? getter = property.GetMethod is { } g && IsVisible(g) ? g : null;
        MethodInfo? setter = property.SetMethod is { } s && IsVisible(s) ? s : null;
        MethodInfo? first = getter ?? setter;
        if (first is null) return null;

        List<string> accessors = new();
        if (getter is not null) accessors.Add("get;");
        if (setter is not null)
        {
            bool isInit = setter.ReturnParameter.GetRequiredCustomModifiers().Contains(typeof(IsExternalInit));
            accessors.Add(isInit ? "init;" : "set;");
        }

        ParameterInfo[] index = property.GetIndexParameters();
        string name = index.Length == 0
            ? property.Name
            : $"this[{string.Join(", ", index.Select(p => Parameter(p, nullability)))}]";
        return $"{Modifiers(first, owner)}{TypeName(property.PropertyType, nullability.Create(property))} {name} {{ {string.Join(" ", accessors)} }}";
    }

    private static bool IsVisible(MethodBase method) => method.IsPublic || method.IsFamily || method.IsFamilyOrAssembly;

    private static bool IsVisible(FieldInfo field) => field.IsPublic || field.IsFamily || field.IsFamilyOrAssembly;

    private static bool IsDelegate(Type type) => type.BaseType == typeof(MulticastDelegate);

    private static string Modifiers(MethodBase method, Type owner)
    {
        string visibility = method.IsFamily || method.IsFamilyOrAssembly ? "protected " : "";
        if (method.IsStatic) return visibility + "static ";
        if (owner.IsInterface) return visibility + (method.IsAbstract ? "" : "default ");
        if (method.IsAbstract) return visibility + "abstract ";
        if (!method.IsVirtual || method is not MethodInfo info) return visibility;

        bool overrides = info.GetBaseDefinition() != info;
        if (overrides) return visibility + (method.IsFinal ? "sealed override " : "override ");
        return method.IsFinal ? visibility : visibility + "virtual ";
    }

    private static string Parameters(MethodBase method, NullabilityInfoContext nullability) =>
        string.Join(", ", method.GetParameters().Select(p => Parameter(p, nullability)));

    private static string Parameter(ParameterInfo parameter, NullabilityInfoContext nullability)
    {
        string modifier = parameter.IsOut ? "out "
            : parameter.ParameterType.IsByRef && parameter.IsIn ? "in "
            : parameter.ParameterType.IsByRef ? "ref "
            : parameter.GetCustomAttribute<ParamArrayAttribute>() is not null ? "params "
            : "";
        string text = $"{modifier}{TypeName(parameter.ParameterType, nullability.Create(parameter))} {parameter.Name}";
        return parameter.HasDefaultValue ? $"{text} = {Literal(parameter.DefaultValue, parameter.ParameterType)}" : text;
    }

    private static string Literal(object? value, Type type)
    {
        Type target = Nullable.GetUnderlyingType(type) ?? type;
        return value switch
        {
            null => "null",
            string text => $"\"{text}\"",
            bool flag => flag ? "true" : "false",
            _ when target.IsEnum => $"{TypeName(target, null)}.{Enum.ToObject(target, value)}",
            float single => single.ToString("R", CultureInfo.InvariantCulture),
            double number => number.ToString("R", CultureInfo.InvariantCulture),
            _ => Convert.ToString(value, CultureInfo.InvariantCulture) ?? "null",
        };
    }

    private static string GenericParameters(Type[] arguments) =>
        arguments.Length == 0 ? "" : $"<{string.Join(", ", arguments.Select(a => a.Name))}>";

    private static string TypeName(Type type, NullabilityInfo? nullability)
    {
        if (type.IsByRef) return TypeName(type.GetElementType()!, nullability);
        if (type.IsArray) return TypeName(type.GetElementType()!, nullability?.ElementType) + "[]" + NullableMark(type, nullability);
        if (type.IsGenericParameter) return type.Name + NullableMark(type, nullability);

        Type? underlying = Nullable.GetUnderlyingType(type);
        if (underlying is not null) return TypeName(underlying, null) + "?";
        if (s_keywords.TryGetValue(type, out string? keyword)) return keyword + NullableMark(type, nullability);

        string name = Qualified(type);
        if (type.IsGenericType)
        {
            Type[] arguments = type.GetGenericArguments();
            NullabilityInfo[]? argumentNullability = nullability?.GenericTypeArguments;
            name += "<" + string.Join(", ", arguments.Select((a, i) =>
                TypeName(a, argumentNullability is not null && i < argumentNullability.Length ? argumentNullability[i] : null))) + ">";
        }

        return name + NullableMark(type, nullability);
    }

    private static string NullableMark(Type type, NullabilityInfo? nullability) =>
        !type.IsValueType && nullability is not null && ReadOrWriteState(nullability) == NullabilityState.Nullable ? "?" : "";

    private static NullabilityState ReadOrWriteState(NullabilityInfo nullability) =>
        nullability.ReadState != NullabilityState.Unknown ? nullability.ReadState : nullability.WriteState;

    private static string Qualified(Type type)
    {
        string name = type.Name;
        int tick = name.IndexOf('`');
        if (tick >= 0) name = name[..tick];
        return type.IsNested ? $"{Qualified(type.DeclaringType!)}.{name}" : $"{type.Namespace}.{name}";
    }
}
