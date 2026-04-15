using Dapper;
using MySql.Data.MySqlClient;

namespace Letiao.ATS.Api.Infrastructure;

/// <summary>
/// Dapper 数据库连接工厂（通过 DI 注入，统一连接管理）
/// </summary>
public class DbConnectionFactory(IConfiguration configuration)
{
    private readonly string _connStr = configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("数据库连接串 'DefaultConnection' 未配置");

    /// <summary>创建并返回一个打开的 MySQL 连接</summary>
    public async Task<MySqlConnection> CreateOpenConnectionAsync()
    {
        var conn = new MySqlConnection(_connStr);
        await conn.OpenAsync();
        return conn;
    }

    /// <summary>数据库探活：执行 SELECT 1，验证连接正常</summary>
    public async Task<bool> PingAsync()
    {
        try
        {
            await using var conn = await CreateOpenConnectionAsync();
            var result = await conn.ExecuteScalarAsync<int>("SELECT 1");
            return result == 1;
        }
        catch
        {
            return false;
        }
    }
}
