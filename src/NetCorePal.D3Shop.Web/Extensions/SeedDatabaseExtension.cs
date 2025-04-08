using NetCorePal.D3Shop.Domain.AggregatesModel.Identity.AdminUserAggregate;
using NetCorePal.D3Shop.Domain.AggregatesModel.Identity.MenuAggregate;
using NetCorePal.D3Shop.Web.Helper;

namespace NetCorePal.D3Shop.Web.Extensions
{
    public static class SeedDatabaseExtension
    {
        internal static IApplicationBuilder SeedDatabase(this IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();

            var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // 初始化管理员用户
            if (!dbContext.AdminUsers.Any(u => u.Name == AppDefaultCredentials.Name))
            {
                var adminUser = new AdminUser(AppDefaultCredentials.Name, "",
                    PasswordHasher.HashPassword(AppDefaultCredentials.Password), [], []);
                dbContext.AdminUsers.Add(adminUser);
                dbContext.SaveChanges();
            }

            // 初始化菜单数据
            if (!dbContext.Menus.Any())
            {
                // 创建系统管理菜单
                var systemMenu = new Menu(
                    "系统管理",
                    "/system",
                    MenuType.Catalog,
                    null,
                    "SYSTEM_MANAGE",
                    "Layout",
                    null,
                    1,
                    "carbon:settings",
                    new MenuMeta
                    {
                        Title = "系统管理",
                        Icon = "carbon:settings",
                        Order = 1
                    }
                );
                dbContext.Menus.Add(systemMenu);
                dbContext.SaveChanges();

                // 创建子菜单
                var menus = new List<Menu>
                {
                    //new Menu(
                    //    "用户管理",
                    //    "/system/user",
                    //    MenuType.Menu,
                    //    systemMenu.Id,
                    //    "USER_MANAGE",
                    //    "system/user/index",
                    //    null,
                    //    1,
                    //    "carbon:user",
                    //    new MenuMeta
                    //    {
                    //        Title = "用户管理",
                    //        Icon = "carbon:user",
                    //        Order = 1
                    //    }
                    //),
                    new Menu(
                        "角色管理",
                        "/system/role",
                        MenuType.Menu,
                        systemMenu.Id,
                        "ROLE_MANAGE",
                        "system/role/index",
                        null,
                        2,
                        "mdi:account-group",
                        new MenuMeta
                        {
                            Title = "角色管理",
                            Icon = "mdi:account-group",
                            Order = 2
                        }
                    ),
                    new Menu(
                        "菜单管理",
                        "/system/menu",
                        MenuType.Menu,
                        systemMenu.Id,
                        "MENU_MANAGE",
                        "system/menu/index",
                        null,
                        3,
                        "mdi:menu",
                        new MenuMeta
                        {
                            Title = "菜单管理",
                            Icon = "mdi:menu",
                            Order = 3
                        }
                    ),
                    new Menu(
                        "部门管理",
                        "/system/dept",
                        MenuType.Menu,
                        systemMenu.Id,
                        "DEPT_MANAGE",
                        "system/dept/index",
                        null,
                        4,
                        "charm:organisation",
                        new MenuMeta
                        {
                            Title = "部门管理",
                            Icon = "charm:organisation",
                            Order = 4
                        }
                    )
                };

                dbContext.Menus.AddRange(menus);
                dbContext.SaveChanges();
            }

            return app;
        }
    }
}