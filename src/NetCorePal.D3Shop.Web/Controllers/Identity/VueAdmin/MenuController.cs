using AntDesign;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetCorePal.D3Shop.Admin.Shared.Permission;
using NetCorePal.D3Shop.Web.Auth;
using Refit;

namespace PlaygroundApi.Controllers
{
    /// <summary>
    /// 菜单类型枚举，定义了系统中不同类型的菜单项
    /// </summary>
    public enum MenuType
    {
        /// <summary>
        /// 目录类型，用于组织其他菜单项的容器
        /// </summary>
        Catalog,   
        /// <summary>
        /// 菜单类型，用于导航到具体功能页面
        /// </summary>
        Menu,       
        /// <summary>
        /// 内嵌类型，用于在页面中嵌入其他内容
        /// </summary>
        Embedded,   
        /// <summary>
        /// 链接类型，用于跳转到外部URL
        /// </summary>
        Link,     
        /// <summary>
        /// 按钮类型，用于触发特定操作
        /// </summary>
        Button    
    }

    /// <summary>
    /// 徽标类型枚举，定义了菜单项上徽标的显示样式
    /// </summary>
    public enum BadgeType
    {
        /// <summary>
        /// 点状徽标，显示为一个小圆点
        /// </summary>
        Dot,    
        /// <summary>
        /// 普通徽标，显示为文字或数字
        /// </summary>
        Normal 
    }

    /// <summary>
    /// 徽标颜色枚举，定义了徽标的颜色变体
    /// </summary>
    public enum BadgeVariant
    {
        /// <summary>
        /// 默认颜色
        /// </summary>
        Default,
        /// <summary>
        /// 危险/错误颜色
        /// </summary>
        Destructive,
        /// <summary>
        /// 主要/强调颜色
        /// </summary>
        Primary,
        /// <summary>
        /// 成功颜色
        /// </summary>
        Success,
        /// <summary>
        /// 警告颜色
        /// </summary>
        Warning
    }

    /// <summary>
    /// 菜单元数据类，定义了菜单项的显示和交互属性
    /// </summary>
    public class MenuMeta
    {
        /// <summary>
        /// 激活状态下的图标
        /// </summary>
        public string? ActiveIcon { get; set; }
        /// <summary>
        /// 激活状态下的路径
        /// </summary>
        public string? ActivePath { get; set; }
        /// <summary>
        /// 是否固定标签页
        /// </summary>
        public bool? AffixTab { get; set; }
        /// <summary>
        /// 固定标签页的顺序
        /// </summary>
        public int? AffixTabOrder { get; set; }
        /// <summary>
        /// 徽标内容
        /// </summary>
        public string? Badge { get; set; }
        /// <summary>
        /// 徽标类型
        /// </summary>
        public BadgeType? BadgeType { get; set; }
        /// <summary>
        /// 徽标颜色变体
        /// </summary>
        public BadgeVariant? BadgeVariants { get; set; }
        /// <summary>
        /// 是否在菜单中隐藏子项
        /// </summary>
        public bool? HideChildrenInMenu { get; set; }
        /// <summary>
        /// 是否在面包屑中隐藏
        /// </summary>
        public bool? HideInBreadcrumb { get; set; }
        /// <summary>
        /// 是否在菜单中隐藏
        /// </summary>
        public bool? HideInMenu { get; set; }
        /// <summary>
        /// 是否在标签页中隐藏
        /// </summary>
        public bool? HideInTab { get; set; }
        /// <summary>
        /// 菜单图标
        /// </summary>
        public string? Icon { get; set; }
        /// <summary>
        /// iframe源地址
        /// </summary>
        public string? IframeSrc { get; set; }
        /// <summary>
        /// 是否保持页面状态
        /// </summary>
        public bool? KeepAlive { get; set; }
        /// <summary>
        /// 外部链接地址
        /// </summary>
        public string? Link { get; set; }
        /// <summary>
        /// 最大打开的标签页数量
        /// </summary>
        public int? MaxNumOfOpenTab { get; set; }
        /// <summary>
        /// 是否不使用基础布局
        /// </summary>
        public bool? NoBasicLayout { get; set; }
        /// <summary>
        /// 是否在新窗口打开
        /// </summary>
        public bool? OpenInNewWindow { get; set; }
        /// <summary>
        /// 排序顺序
        /// </summary>
        public int? Order { get; set; }
        /// <summary>
        /// 查询参数
        /// </summary>
        public Dictionary<string, object>? Query { get; set; }
        /// <summary>
        /// 菜单标题
        /// </summary>
        public string? Title { get; set; }
    }

    /// <summary>
    /// 系统菜单实体类，定义了菜单的基本结构和层级关系
    /// </summary>
    public class SystemMenu
    {
        /// <summary>
        /// 菜单ID
        /// </summary>
        public string Id { get; set; } = string.Empty;
        /// <summary>
        /// 菜单名称
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// 菜单路径
        /// </summary>
        public string Path { get; set; } = string.Empty;
        /// <summary>
        /// 父菜单ID
        /// </summary>
        public string Pid { get; set; } = string.Empty;
        /// <summary>
        /// 菜单类型
        /// </summary>
        public MenuType Type { get; set; }
        /// <summary>
        /// 权限代码
        /// </summary>
        public string? AuthCode { get; set; }
        /// <summary>
        /// 组件路径
        /// </summary>
        public string? Component { get; set; }
        /// <summary>
        /// 重定向路径
        /// </summary>
        public string? Redirect { get; set; }
        /// <summary>
        /// 菜单元数据
        /// </summary>
        public MenuMeta? Meta { get; set; }
        /// <summary>
        /// 子菜单列表
        /// </summary>
        public List<SystemMenu>? Children { get; set; }
    }

    /// <summary>
    /// 菜单响应类，用于API响应的标准格式
    /// </summary>
    public class MenuResponse
    {
        /// <summary>
        /// 响应状态码
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 响应数据
        /// </summary>
        public object Data { get; set; } = new();
        /// <summary>
        /// 响应消息
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }

    /// <summary>
    /// 菜单控制器，提供菜单管理的API接口
    /// </summary>
    [ApiController]
    [Route("api/system/[controller]")]
    public class MenuController : ControllerBase
    {

        private static readonly List<SystemMenu> _menuList = new()
    {
        new SystemMenu
        {
            Id = "1",
            Name = "系统管理",
            Path = "/system",
            AuthCode="SYSTEM_MANAGE",
            Pid = "0",
            Type = MenuType.Catalog,
            Meta = new MenuMeta
            {
                Icon = "carbon:settings",
                Title = "系统管理",
                Order = 1
            },
            Children = new List<SystemMenu>
            {
                new SystemMenu
                {
                    Id = "1-1",
                    Name = "用户管理",
                    Path = "/system/user",
                    AuthCode="USER_MANAGE",
                    Pid = "1",
                    Type = MenuType.Menu,
                    Component = "system/user/index",
                    Meta = new MenuMeta
                    {
                        Icon = "carbon:user",
                        Title = "用户管理",
                        Order = 1
                    }
                },
                new SystemMenu
                {
                    Id = "1-2",
                    Name = "角色管理",
                    Path = "/system/role",
                    Pid = "1",
                    Type = MenuType.Menu,
                    Component = "system/role/index",
                    Meta = new MenuMeta
                    {
                        Icon = "carbon:user-role",
                        Title = "角色管理",
                        Order = 2
                    }
                },
                new SystemMenu
                {
                    Id = "1-3",
                    Name = "菜单管理",
                    Path = "/system/menu",
                    Pid = "1",
                    Type = MenuType.Menu,
                    Component = "system/menu/index",
                    Meta = new MenuMeta
                    {
                        Icon = "carbon:menu",
                        Title = "菜单管理",
                        Order = 3
                    }
                }
            }
        },
        new SystemMenu
        {
            Id = "2",
            Name = "工作台",
            Path = "/dashboard",
            Pid = "0",
            Type = MenuType.Menu,
            Component = "dashboard/index",
            Meta = new MenuMeta
            {
                Icon = "carbon:dashboard",
                Title = "工作台",
                Order = 0,
                AffixTab = true
            }
        },
        new SystemMenu
        {
            Id = "3",
            Name = "外部链接",
            Path = "/external",
            Pid = "0",
            Type = MenuType.Catalog,
            Meta = new MenuMeta
            {
                Icon = "carbon:link",
                Title = "外部链接",
                Order = 2
            },
            Children = new List<SystemMenu>
            {
                new SystemMenu
                {
                    Id = "3-1",
                    Name = "GitHub",
                    Path = "https://github.com",
                    Pid = "3",
                    Type = MenuType.Link,
                    Meta = new MenuMeta
                    {
                        Icon = "carbon:logo-github",
                        Title = "GitHub",
                        OpenInNewWindow = true
                    }
                },
                new SystemMenu
                {
                    Id = "3-2",
                    Name = "文档",
                    Path = "https://docs.example.com",
                    Pid = "3",
                    Type = MenuType.Link,
                    Meta = new MenuMeta
                    {
                        Icon = "carbon:document",
                        Title = "文档",
                        OpenInNewWindow = true
                    }
                }
            }
        },
        new SystemMenu
        {
            Id = "4",
            Name = "功能演示",
            Path = "/demo",
            Pid = "0",
            Type = MenuType.Catalog,
            Meta = new MenuMeta
            {
                Icon = "carbon:apps",
                Title = "功能演示",
                Order = 3
            },
            Children = new List<SystemMenu>
            {
                new SystemMenu
                {
                    Id = "4-1",
                    Name = "表格",
                    Path = "/demo/table",
                    Pid = "4",
                    Type = MenuType.Menu,
                    Component = "demo/table/index",
                    Meta = new MenuMeta
                    {
                        Icon = "carbon:table",
                        Title = "表格",
                        BadgeType = BadgeType.Dot,
                        BadgeVariants = BadgeVariant.Primary
                    }
                },
                new SystemMenu
                {
                    Id = "4-2",
                    Name = "表单",
                    Path = "/demo/form",
                    Pid = "4",
                    Type = MenuType.Menu,
                    Component = "demo/form/index",
                    AuthCode = "System:Dept:Edit",
                    Meta = new MenuMeta
                    {
                        Icon = "carbon:form",
                        Title = "表单",
                        BadgeType = BadgeType.Normal,
                        Badge = "New",
                        BadgeVariants = BadgeVariant.Success
                    }
                }
            }
        }
    };

        [HttpGet("list")]
        public ActionResult<List<SystemMenu>> GetMenuList()
        {
            return Ok(new MenuResponse
            {
                Code = 0,
                Data = _menuList,
                Message = "success"
            });
        }

        [HttpPost]
        public ActionResult CreateMenu([FromBody] SystemMenu menu)
        {
            // 模拟创建菜单
            menu.Id = Guid.NewGuid().ToString();
            _menuList.Add(menu);
            return Ok();
        }

        [HttpPut("{id}")]
        public ActionResult UpdateMenu(string id, [FromBody] SystemMenu menu)
        {
            // 模拟更新菜单
            var existingMenu = _menuList.FirstOrDefault(m => m.Id == id);
            if (existingMenu == null)
                return NotFound();

            // 更新属性
            existingMenu.Name = menu.Name;
            existingMenu.Path = menu.Path;
            existingMenu.Pid = menu.Pid;
            existingMenu.Type = menu.Type;
            existingMenu.Component = menu.Component;
            existingMenu.Redirect = menu.Redirect;
            existingMenu.AuthCode = menu.AuthCode;
            existingMenu.Meta = menu.Meta;

            return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteMenu(string id)
        {
            // 模拟删除菜单
            var menu = _menuList.FirstOrDefault(m => m.Id == id);
            if (menu == null)
                return NotFound();

            _menuList.Remove(menu);
            return Ok();
        }


        [HttpGet("name-exists")]
        public ActionResult<ApiResponse<bool>> IsMenuNameExists(
         [FromQuery] string? name = null,
         [FromQuery] string? id = null)
        {
            // 如果name为空，返回false
            if (string.IsNullOrEmpty(name))
            {
                return Ok(new MenuResponse
                {
                    Code = 0,
                    Data = false,
                    Message = "success"
                });
            }

            var exists = _menuList.Any(m =>
                m.Name == name && (id == null || m.Id != id));

            return Ok(new MenuResponse
            {
                Code = 0,
                Data = exists,
                Message = "success"
            });
        }

        [HttpGet("path-exists")]
        public ActionResult<ApiResponse<bool>> IsMenuPathExists(
            [FromQuery] string? path = null,
            [FromQuery] string? id = null)
        {
            // 如果path为空，返回false
            if (string.IsNullOrEmpty(path))
            {
                return Ok(new MenuResponse
                {
                    Code = 0,
                    Data = false,
                    Message = "success"
                });
            }

            var exists = _menuList.Any(m =>
                m.Path == path && (id == null || m.Id != id));

            return Ok(new MenuResponse
            {
                Code = 0,
                Data = exists,
                Message = "success"
            });
        }
    }
}

