using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using NetCorePal.D3Shop.Domain.AggregatesModel.Identity.MenuAggregate;
using NetCorePal.D3Shop.Web.Application.Queries;
using NetCorePal.D3Shop.Web.Application.Commands.Identity.Menus;
using NetCorePal.D3Shop.Admin.Shared.Permission;
using NetCorePal.D3Shop.Web.Auth;

namespace NetCorePal.D3Shop.Web.Controllers.Identity.VueAdmin
{

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
    [VueAuthorize(PermissionCodes.RoleManagement)]
    public class MenuController(IMediator _mediator, MenuQuery _menuQuery) : ControllerBase
    {
       
        private CancellationToken CancellationToken => HttpContext?.RequestAborted ?? default;

       

        /// <summary>
        /// 获取所有菜单列表
        /// </summary>
        /// <returns>菜单列表</returns>
        [HttpGet("list")]
        public async Task<ActionResult<List<MenuDto>>> GetMenuList()
        {
            var menus = await _menuQuery.GetAllMenusAsync(CancellationToken);
            var menuDtos = menus.Select(m => MapToDto(m)).ToList();
            return Ok(new ApiResponse<List<MenuDto>>
            {
                Code = 0,
                Data = menuDtos,
                Message = "success"
            });
        }

        /// <summary>
        /// 根据ID获取菜单详情
        /// </summary>
        /// <param name="id">菜单ID</param>
        /// <returns>菜单详情</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<MenuDto>> GetMenu(long id)
        {
            var menu = await _menuQuery.GetMenuByIdAsync(new MenuId(id), CancellationToken);
            if (menu is null)
            {
                return NotFound();
            }

            return Ok(new ApiResponse<MenuDto>
            {
                Code = 0,
                Data = MapToDto(menu),
                Message = "success"
            });
        }

        /// <summary>
        /// 创建新菜单
        /// </summary>
        /// <param name="request">创建菜单请求</param>
        /// <returns>创建结果</returns>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<long>>> CreateMenu([FromBody] CreateMenuRequest request)
        {
            try
            {
                var menuId = await _mediator.Send(new CreateMenuCommand(
                    request.Name,
                    request.Path,
                    request.Type,
                    request.ParentId != null ? new MenuId(request.ParentId.Value) : null,
                    request.AuthCode,
                    request.Component,
                    request.Redirect,
                    request.Order,
                    request.Icon
                ), CancellationToken);

                return Ok(new ApiResponse<long>
                {
                    Code = 0,
                    Data = menuId.Id,
                    Message = "Menu created successfully"
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Code = 400,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// 更新菜单信息
        /// </summary>
        /// <param name="id">菜单ID</param>
        /// <param name="request">更新菜单请求</param>
        /// <returns>更新结果</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateMenu(long id, [FromBody] UpdateMenuRequest request)
        {
            try
            {
                await _mediator.Send(new UpdateMenuCommand(
                    new MenuId(id),
                    request.Name,
                    request.Path,
                    request.Type,
                    request.ParentId != null ? new MenuId(request.ParentId.Value) : null,
                    request.AuthCode,
                    request.Component,
                    request.Redirect,
                    request.Order,
                    request.Icon
                ), CancellationToken);

                return Ok(new ApiResponse<object>
                {
                    Code = 0,
                    Message = "Menu updated successfully"
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Code = 400,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <param name="id">菜单ID</param>
        /// <returns>删除结果</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteMenu(long id)
        {
            try
            {
                await _mediator.Send(new DeleteMenuCommand(new MenuId(id)), CancellationToken);
                return Ok(new ApiResponse<object>
                {
                    Code = 0,
                    Message = "Menu deleted successfully"
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Code = 400,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// 设置菜单可见性
        /// </summary>
        /// <param name="id">菜单ID</param>
        /// <param name="request">可见性设置请求</param>
        /// <returns>设置结果</returns>
        [HttpPut("{id}/visibility")]
        public async Task<ActionResult<ApiResponse<object>>> SetMenuVisibility(long id, [FromBody] SetVisibilityRequest request)
        {
            try
            {
                await _mediator.Send(new SetMenuVisibilityCommand(new MenuId(id), request.IsVisible), CancellationToken);
                return Ok(new ApiResponse<object>
                {
                    Code = 0,
                    Message = "Menu visibility updated successfully"
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Code = 400,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// 设置菜单启用状态
        /// </summary>
        /// <param name="id">菜单ID</param>
        /// <param name="request">启用状态设置请求</param>
        /// <returns>设置结果</returns>
        [HttpPut("{id}/enabled")]
        public async Task<ActionResult<ApiResponse<object>>> SetMenuEnabled(long id, [FromBody] SetEnabledRequest request)
        {
            try
            {
                await _mediator.Send(new SetMenuEnabledCommand(new MenuId(id), request.IsEnabled), CancellationToken);
                return Ok(new ApiResponse<object>
                {
                    Code = 0,
                    Message = "Menu enabled status updated successfully"
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Code = 400,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// 将Menu实体转换为MenuDto
        /// </summary>
        /// <param name="menu">菜单实体</param>
        /// <returns>菜单DTO</returns>
        private static MenuDto MapToDto(Menu menu)
        {
            return new MenuDto
            {
                Id = menu.Id.ToString(),
                Pid = menu.ParentId?.ToString(),
                Name = menu.Name,
                Path = menu.Path,
                Component = menu.Component,
                Redirect = menu.Redirect,
                Type = (MenuType)menu.Type,
                Meta = new MenuMeta
                {
                    Title = menu.Name,
                    Icon = menu.Icon,
                    Order = menu.Order,
                    HideInMenu = !menu.IsVisible,
                    HideInTab = !menu.IsEnabled,
                    KeepAlive = true,
                    AffixTab = false,
                    HideInBreadcrumb = false,
                    HideChildrenInMenu = false,
                    OpenInNewWindow = false,
                    NoBasicLayout = false,
                    MaxNumOfOpenTab = 10
                }
            };
        }
    }

    /// <summary>
    /// 菜单数据传输对象
    /// </summary>
    public class MenuDto
    {
        /// <summary>
        /// 菜单ID
        /// </summary>
        public string Id { get; set; } = string.Empty;
        /// <summary>
        /// 父菜单ID
        /// </summary>
        public string? Pid { get; set; }
        /// <summary>
        /// 菜单名称
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// 菜单路径
        /// </summary>
        public string Path { get; set; } = string.Empty;
        /// <summary>
        /// 组件路径
        /// </summary>
        public string? Component { get; set; }
        /// <summary>
        /// 重定向路径
        /// </summary>
        public string? Redirect { get; set; }
        /// <summary>
        /// 菜单类型
        /// </summary>
        public MenuType Type { get; set; }
        /// <summary>
        /// 菜单元数据
        /// </summary>
        public MenuMeta Meta { get; set; } = new();
    }

    /// <summary>
    /// 创建菜单请求
    /// </summary>
    public class CreateMenuRequest
    {
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
        public long? ParentId { get; set; }
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
        /// 排序顺序
        /// </summary>
        public int Order { get; set; }
        /// <summary>
        /// 菜单图标
        /// </summary>
        public string? Icon { get; set; }
    }

    /// <summary>
    /// 更新菜单请求
    /// </summary>
    public class UpdateMenuRequest
    {
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
        public long? ParentId { get; set; }
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
        /// 排序顺序
        /// </summary>
        public int Order { get; set; }
        /// <summary>
        /// 菜单图标
        /// </summary>
        public string? Icon { get; set; }
    }

    /// <summary>
    /// 设置可见性请求
    /// </summary>
    public class SetVisibilityRequest
    {
        /// <summary>
        /// 是否可见
        /// </summary>
        public bool IsVisible { get; set; }
    }

    /// <summary>
    /// 设置启用状态请求
    /// </summary>
    public class SetEnabledRequest
    {
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; }
    }

    /// <summary>
    /// API响应类
    /// </summary>
    /// <typeparam name="T">响应数据类型</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// 响应状态码
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 响应数据
        /// </summary>
        public T? Data { get; set; }
        /// <summary>
        /// 响应消息
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }
}

