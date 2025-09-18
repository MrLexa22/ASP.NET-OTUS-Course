using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace PromoCodeFactory.DataAccess.DesignTime;

public class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
{
    public DataContext CreateDbContext(string[] args)
    {
        // ������� ����� ��� ����������: ������ DataAccess (���� -p DataAccess)
        var dataAccessDir = Directory.GetCurrentDirectory();
        var webHostDir = Path.GetFullPath(Path.Combine(dataAccessDir, "..", "PromoCodeFactory.WebHost"));

        var configBuilder = new ConfigurationBuilder();

        // ������������ appsettings �� WebHost (�������� ��������)
        var webHostAppSettings = Path.Combine(webHostDir, "appsettings.json");
        if (File.Exists(webHostAppSettings))
            configBuilder.AddJsonFile(webHostAppSettings, optional: true);

        // ��������� appsettings DataAccess (���� ����� ����)
        var localSettings = Path.Combine(dataAccessDir, "appsettings.json");
        if (File.Exists(localSettings))
            configBuilder.AddJsonFile(localSettings, optional: true);

        // ENV-����������� ���� �� WebHost
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (!string.IsNullOrWhiteSpace(env))
        {
            var envFile = Path.Combine(webHostDir, $"appsettings.{env}.json");
            if (File.Exists(envFile))
                configBuilder.AddJsonFile(envFile, optional: true);
        }

        configBuilder.AddEnvironmentVariables();

        var config = configBuilder.Build();
        var rawConn = config.GetConnectionString("Default") ?? "Data Source=promocode.db";

        // ���� ������ ������ ���� ��� ������������� ���� � ����������� � �������� WebHost
        var conn = NormalizeSqlitePath(rawConn, webHostDir);

        var options = new DbContextOptionsBuilder<DataContext>()
            .UseSqlite(conn)
            .Options;

        return new DataContext(options);
    }

    private static string NormalizeSqlitePath(string connectionString, string webHostDir)
    {
        // ���� ������� "Data Source="
        // ��������� �������� "Data Source=" � "DataSource=" (��� �������)
        var parts = connectionString.Split(';', StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < parts.Length; i++)
        {
            var p = parts[i].Trim();
            if (p.StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase) ||
                p.StartsWith("DataSource=", StringComparison.OrdinalIgnoreCase))
            {
                var kv = p.Split('=', 2);
                if (kv.Length == 2)
                {
                    var pathPart = kv[1].Trim();
                    // ���� ��� �� ���������� ���� � ������ ���������� � WebHost
                    if (!Path.IsPathRooted(pathPart))
                    {
                        // ���� ������ ���������� � �������� ������������� ���������
                        var absolute = Path.GetFullPath(Path.Combine(webHostDir, pathPart));
                        parts[i] = $"{kv[0]}={absolute}";
                    }
                }
            }
        }
        return string.Join(';', parts);
    }
}