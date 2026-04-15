using System.Text.RegularExpressions;
using Dapper;

namespace Letiao.ATS.Api.Infrastructure;

/// <summary>
/// Dapper 全局配置：将数据库下划线命名（snake_case）自动映射到 C# 属性（PascalCase）
/// 例：password_hash → PasswordHash, real_name → RealName
/// </summary>
public class SnakeCaseTypeMapper : SqlMapper.ITypeMap
{
    private readonly SqlMapper.ITypeMap _defaultMap;
    private readonly Type _type;

    public SnakeCaseTypeMapper(Type type)
    {
        _type = type;
        _defaultMap = new DefaultTypeMap(type);
    }

    public System.Reflection.ConstructorInfo? FindExplicitConstructor() =>
        _defaultMap.FindExplicitConstructor();

    public System.Reflection.ConstructorInfo? FindConstructor(string[] names, Type[] types) =>
        _defaultMap.FindConstructor(names, types);

    public SqlMapper.IMemberMap? GetConstructorParameter(
        System.Reflection.ConstructorInfo constructor, string columnName) =>
        _defaultMap.GetConstructorParameter(constructor, columnName);

    public SqlMapper.IMemberMap? GetMember(string columnName)
    {
        // snake_case → PascalCase 转换
        var pascalName = Regex.Replace(columnName, "_([a-z])", m => m.Groups[1].Value.ToUpper());
        pascalName = char.ToUpper(pascalName[0]) + pascalName[1..];

        // 先按转换后名称查找属性
        var prop = _type.GetProperty(pascalName,
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        if (prop != null)
            return new SimpleMemberMap(columnName, prop);

        // 回退到默认映射（不区分大小写）
        return _defaultMap.GetMember(columnName);
    }
}

public class SimpleMemberMap(string columnName, System.Reflection.PropertyInfo property) : SqlMapper.IMemberMap
{
    public string ColumnName => columnName;
    public Type MemberType => property.PropertyType;
    public System.Reflection.PropertyInfo? Property => property;
    public System.Reflection.FieldInfo? Field => null;
    public System.Reflection.ParameterInfo? Parameter => null;
}

/// <summary>在程序启动时调用，一次性注册类型映射</summary>
public static class DapperConfig
{
    public static void ConfigureSnakeCaseMapping()
    {
        // 注册需要 snake_case 映射的实体类型
        var entityTypes = typeof(DapperConfig).Assembly
            .GetTypes()
            .Where(t => t.Namespace?.StartsWith("Letiao.ATS.Api.Domain.Entities") == true);

        foreach (var type in entityTypes)
            SqlMapper.SetTypeMap(type, new SnakeCaseTypeMapper(type));
    }
}
