using System.Text.Json.Nodes;
using System.Text.Json;
using OpenWebBlazor.ViewModels;
using Microsoft.EntityFrameworkCore;
using OpenWebBlazor.Models;
using OpenWebBlazor.Common;

namespace OpenWebBlazor.Services
{
    public class InitialService
    {
        private readonly IConfiguration _config;
        private readonly IHostEnvironment _env;
        private static IServiceProvider _serviceProvider;

        public InitialService(IConfiguration config, IHostEnvironment env, IServiceProvider serviceProvider)
        {
            _config = config;
            _env = env;
            _serviceProvider = serviceProvider;
        }

        public Task<bool> IsInitialedAsync()
        {
            var db_config = _config.GetConnectionString("DefaultConnection");
            return Task.FromResult(!string.IsNullOrEmpty(db_config));
        }

        public async Task<bool> InitializeAsync(DatabaseConfig config)
        {
            var connectionString = $@"Server={config.Server};Database={config.Database};User Id={config.UserId};Password={config.Password};MultipleActiveResultSets=True;TrustServerCertificate=True;";

            await SaveConnectionConfig(connectionString);

            await SeedDataInitializeAsync(connectionString);

            return true;
        }
        private async Task<bool> SaveConnectionConfig(string connectionString)
        {
            try
            {
                var secretsDir = Path.Combine(_env.ContentRootPath, "Secrets");
                var filePath = Path.Combine(secretsDir, "appsettings.json");

                Directory.CreateDirectory(secretsDir);

                JsonNode json;

                if (!File.Exists(filePath))
                {
                    json = new JsonObject
                    {
                        ["ConnectionStrings"] = new JsonObject
                        {
                            ["DefaultConnection"] = connectionString
                        }
                    };
                }
                else
                {
                    var jsonText = File.ReadAllText(filePath);
                    json = JsonNode.Parse(jsonText) ?? new JsonObject();

                    var connectionStrings = json["ConnectionStrings"] as JsonObject ?? new JsonObject();
                    json["ConnectionStrings"] = connectionStrings;

                    connectionStrings["DefaultConnection"] = connectionString;
                }

                var options = new JsonSerializerOptions { WriteIndented = true };
                File.WriteAllText(filePath, json.ToJsonString(options));

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        private async Task SeedDataInitializeAsync(string connectionString)
        {
            var options = new DbContextOptionsBuilder<WebDbContext>()
                .UseSqlServer(connectionString)
                .Options;
            await using var _dbContext = new WebDbContext(options);

            await _dbContext.Database.MigrateAsync();

            {
                var user = new WebUsers
                {
                    Id = Guid.NewGuid().ToString("N"),
                    UserName = "admin",
                    Password = Md5Helper.ComputeMd5("123123"),
                    State = 1
                };
                _dbContext.WebUsers.Add(user);

                var role = new WebRoles
                {
                    Id = Guid.NewGuid().ToString("N"),
                    Name = "超级管理员",
                    IsSuper = true
                };
                _dbContext.WebRoles.Add(role);

                var user_role = new WebUserRoles()
                {
                    RoleId = role.Id,
                    UserId = user.Id
                };
                _dbContext.WebUserRoles.Add(user_role);

                var home_root = new WebMenus
                {
                    Id = Guid.NewGuid().ToString("N"),
                    Name = "首页",
                    Path = String.Empty,
                    ParentId = String.Empty,
                    IsShow = false,
                    IsAuth = false,
                    Sort = 0
                };
                _dbContext.WebMenus.Add(home_root);

                var home = new WebMenus
                {
                    Id = Guid.NewGuid().ToString("N"),
                    Name = "首页",
                    ParentId = home_root.Id,
                    Path = "/",
                    IsShow = false,
                    IsAuth = true,
                    Sort = 1
                };
                _dbContext.WebMenus.Add(home);

                var system_root = new WebMenus
                {
                    Id = Guid.NewGuid().ToString("N"),
                    Name = "系统设置",
                    Path = String.Empty,
                    ParentId = String.Empty,
                    IsShow = true,
                    IsAuth = true,
                    Sort = 1
                };
                _dbContext.WebMenus.Add(system_root);

                var menu_list = new WebMenus
                {
                    Id = Guid.NewGuid().ToString("N"),
                    Name = "菜单管理",
                    Path = "/menu/list",
                    ParentId = system_root.Id,
                    IsShow = true,
                    IsAuth = true,
                    Sort = 0
                };
                _dbContext.WebMenus.Add(menu_list);

                var user_root = new WebMenus
                {
                    Id = Guid.NewGuid().ToString("N"),
                    Name = "用户管理",
                    Path = String.Empty,
                    ParentId = String.Empty,
                    IsShow = true,
                    IsAuth = true,
                    Sort = 2
                };
                _dbContext.WebMenus.Add(user_root);

                var user_list = new WebMenus
                {
                    Id = Guid.NewGuid().ToString("N"),
                    Name = "用户列表",
                    Path = "/account/list",
                    ParentId = user_root.Id,
                    IsShow = true,
                    IsAuth = true,
                    Sort = 0
                };
                _dbContext.WebMenus.Add(user_list);

                var role_list = new WebMenus
                {
                    Id = Guid.NewGuid().ToString("N"),
                    Name = "角色管理",
                    Path = "/role/list",
                    ParentId = user_root.Id,
                    IsShow = true,
                    IsAuth = true,
                    Sort = 1
                };
                _dbContext.WebMenus.Add(role_list);

                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
