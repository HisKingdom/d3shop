using MediatR;
using Microsoft.AspNetCore.Mvc;
using NetCorePal.D3Shop.Admin.Shared.Permission;
using NetCorePal.D3Shop.Admin.Shared.Requests;
using NetCorePal.D3Shop.Admin.Shared.Responses;
using NetCorePal.D3Shop.Domain.AggregatesModel.Identity.MenuAggregate;
using NetCorePal.D3Shop.Domain.AggregatesModel.Identity.RoleAggregate;
using NetCorePal.D3Shop.Web.Application.Commands.Identity.Admin;
using NetCorePal.D3Shop.Web.Application.Commands.Identity.VueAdmin;
using NetCorePal.D3Shop.Web.Application.Queries;
using NetCorePal.D3Shop.Web.Application.Queries.Identity.Admin;
using NetCorePal.D3Shop.Web.Auth;
using NetCorePal.D3Shop.Web.Blazor;
using NetCorePal.D3Shop.Web.Controllers.Identity.VueAdmin.Models;
using NetCorePal.D3Shop.Web.Controllers.Identity.VueAdmin.Requests;
using NetCorePal.D3Shop.Web.Controllers.Identity.VueAdmin.Responses;
using NetCorePal.Extensions.Dto;
using NetCorePal.Extensions.Primitives;

namespace PlaygroundApi.Controllers
{
    [ApiController]
    [Route("api/system/[controller]")]
    // [VueAuthorize(PermissionCodes.RoleManagement)]
    public class RoleController(IMediator mediator, RoleQuery roleQuery, MenuQuery menuQuery) : ControllerBase
    {

        private CancellationToken CancellationToken => HttpContext?.RequestAborted ?? CancellationToken.None;




        [HttpPost]
        public async Task<ResponseData<RoleId>> CreateRole([FromBody] VueCreateRoleRequest request)
        {
            var menus = await menuQuery.GetAllMenusAsync(CancellationToken);

            var permissions = request.Permissions
                .Select(item =>
                {
                    var menu = menus.FirstOrDefault(m => m.Id == item);
                    if (menu == null || string.IsNullOrEmpty(menu.AuthCode))
                    {
                        throw new KnownException("无效的菜单", -1);
                    }
                    return (menu.Id, menu.AuthCode);
                })
                .ToList();

            var roleId = await mediator.Send(new VueCreateRoleCommand(request.Name, request.Remark, request.Status, permissions), CancellationToken);
            return roleId.AsResponseData();
        }

        [HttpPut("{id}")]
        public async Task<ResponseData> UpdateRole([FromRoute] RoleId id, [FromBody] VueUpdateRoleRequest request, CancellationToken cancellationToken)
        {
            var menus = await menuQuery.GetAllMenusAsync(cancellationToken);
            var permissions = request.Permissions
                .Select(item =>
                {
                    var menu = menus.FirstOrDefault(m => m.Id == item);
                    if (menu == null || string.IsNullOrEmpty(menu.AuthCode))
                    {
                        throw new KnownException("无效的菜单", -1);
                    }
                    return (menu.Id, menu.AuthCode);
                })
                .ToList();

            await mediator.Send(new VueUpdateRoleCommand(
                id,
                request.Name,
                request.Remark,
                request.Status,
                permissions),
                cancellationToken);

            return new ResponseData();
        }

        //[HttpDelete("{id}")]
        //public IActionResult DeleteRole(int id)
        //{
        //    var role = _roles.FirstOrDefault(r => r.Id == id);
        //    if (role == null)
        //    {
        //        return NotFound(new { message = "角色不存在" });
        //    }

        //    _roles.Remove(role);
        //    return Ok(new
        //    {
        //        code = 0,
        //        data = "",
        //        error = "",
        //        message = "ok"
        //    });
        //}


        [HttpGet("list")]
        //[VueAuthorize(PermissionCodes.RoleView)]
        public async Task<ResponseData<PagedData<VueRoleResponse>>> GetAllRoles([FromQuery] VueRoleQueryRequest request)
        {
            var roles = await roleQuery.GetVueAllRolesAsync(request, CancellationToken);
            return roles.AsResponseData();
        }

        //[HttpGet("list2")]
        //public ActionResult<object> GetRoleList([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        //{
        //    try
        //    {
        //        var totalCount = _roles.Count;
        //        var items = _roles
        //            .Skip((page - 1) * pageSize)
        //            .Take(pageSize)
        //            .Select(r => new
        //            {
        //                id = r.Id,
        //                name = r.Name,
        //                //  code = r.Code,
        //                status = r.Status,
        //                remark = r.Remark,
        //                //description = r.Description,
        //                createTime = r.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
        //                permissions = r.Permissions?.Select(p => p.Code).ToArray() ?? Array.Empty<string>()
        //            })
        //            .ToList();

        //        var result = new
        //        {
        //            items = items,
        //            total = totalCount,
        //            page = page,
        //            pageSize = pageSize
        //        };

        //        return Ok(new
        //        {
        //            code = 0,
        //            data = result,
        //            error = "",
        //            message = "ok"
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { code = -1, message = "服务器内部错误", error = ex.Message });
        //    }
        //}
    }
}
