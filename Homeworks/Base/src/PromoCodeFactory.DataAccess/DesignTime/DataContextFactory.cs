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
        // Текущая папка при выполнении: обычно DataAccess (если -p DataAccess)
        var dataAccessDir = Directory.GetCurrentDirectory();
        var webHostDir = Path.GetFullPath(Path.Combine(dataAccessDir, "..", "PromoCodeFactory.WebHost"));

        var configBuilder = new ConfigurationBuilder();

        // Подхватываем appsettings из WebHost (основной источник)
        var webHostAppSettings = Path.Combine(webHostDir, "appsettings.json");
        if (File.Exists(webHostAppSettings))
            configBuilder.AddJsonFile(webHostAppSettings, optional: true);

        // Локальный appsettings DataAccess (если вдруг есть)
        var localSettings = Path.Combine(dataAccessDir, "appsettings.json");
        if (File.Exists(localSettings))
            configBuilder.AddJsonFile(localSettings, optional: true);

        // ENV-специфичный файл из WebHost
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

        // Если указан только файл или относительный путь – привязываем к каталогу WebHost
        var conn = NormalizeSqlitePath(rawConn, webHostDir);

        var options = new DbContextOptionsBuilder<DataContext>()
            .UseSqlite(conn)
            .Options;

        return new DataContext(options);
    }

    private static string NormalizeSqlitePath(string connectionString, string webHostDir)
    {
        // Ищем сегмент "Data Source="
        // Поддержим варианты "Data Source=" и "DataSource=" (без пробела)
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
                    // Если это не абсолютный путь – делаем абсолютным в WebHost
                    if (!Path.IsPathRooted(pathPart))
                    {
                        // Если указан подкаталог — сохраним относительную структуру
                        var absolute = Path.GetFullPath(Path.Combine(webHostDir, pathPart));
                        parts[i] = $"{kv[0]}={absolute}";
                    }
                }
            }
        }
        return string.Join(';', parts);
    }
}