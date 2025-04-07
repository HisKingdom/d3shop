using MediatR;
using Microsoft.AspNetCore.Mvc;
using NetCorePal.D3Shop.Admin.Shared.Permission;
using NetCorePal.D3Shop.Admin.Shared.Requests;
using NetCorePal.D3Shop.Admin.Shared.Responses;
using NetCorePal.D3Shop.Web.Application.Queries.Identity.Admin;
using NetCorePal.D3Shop.Web.Auth;
using NetCorePal.D3Shop.Web.Blazor;
using NetCorePal.D3Shop.Web.Controllers.Identity.VueAdmin.Models;
using NetCorePal.D3Shop.Web.Controllers.Identity.VueAdmin.Requests;
using NetCorePal.D3Shop.Web.Controllers.Identity.VueAdmin.Responses;
using NetCorePal.Extensions.Dto;

namespace PlaygroundApi.Controllers
{
    [ApiController]
    [Route("api/system/[controller]")]
   // [VueAuthorize(PermissionCodes.RoleManagement)]
    public class RoleController(IMediator mediator, RoleQuery roleQuery) : ControllerBase
    {

        private CancellationToken CancellationToken => HttpContext?.RequestAborted ?? CancellationToken.None;

        private static readonly List<Role> _roles = new List<Role>
        {
            new Role
            {
                Id = 1,
                Name = "超级管理员",
                //Code = "SUPER_ADMIN",
                //Description = "系统超级管理员",
                CreateTime = DateTime.Now,
               // IsActive = true,
                Status = 1,
                Remark = "系统超级管理员，拥有所有权限",
                //Permissions = new List<NetCorePal.D3Shop.Web.Controllers.Identity.VueAdmin.Models.Permission>
                //{
                //    new NetCorePal.D3Shop.Web.Controllers.Identity.VueAdmin.Models.Permission
                //    {
                //        Id = 1,
                //        Name = "系统管理",
                //        Code = "1",
                //        Type = "Menu",
                //        Path = "/system",
                //        Component = "Layout",
                //        Icon = "setting",
                //        Sort = 1
                //    },
                //    new NetCorePal.D3Shop.Web.Controllers.Identity.VueAdmin.Models.Permission
                //    {
                //        Id = 2,
                //        Name = "用户管理",
                //        Code = "1-1",
                //        Type = "Menu",
                //        Path = "/user",
                //        Component = "UserManagement",
                //        Icon = "user",
                //        Sort = 2
                //    },
                //}
            },
            new Role
            {
                Id = 2,
                Name = "普通用户",
                //Code = "USER",
               // Description = "普通用户",
                CreateTime = DateTime.Now,
               // IsActive = true,
                Status = 1,
                Remark = "普通用户，仅有基本权限",
                //Permissions = new List<NetCorePal.D3Shop.Web.Controllers.Identity.VueAdmin.Models.Permission>
                //{
                //    new NetCorePal.D3Shop.Web.Controllers.Identity.VueAdmin.Models.Permission
                //    {
                //        Id = 2,
                //        Name = "首页",
                //        Code = "HOME",
                //        Type = "Menu",
                //        Path = "/home",
                //        Component = "Layout",
                //        Icon = "home",
                //        Sort = 0
                //    }
                //}
            }
        };

        [HttpGet]
        public ActionResult<List<Role>> GetRoles()
        {
            return Ok(_roles);
        }

        [HttpGet("{id}")]
        public ActionResult<Role> GetRole(int id)
        {
            var role = _roles.FirstOrDefault(r => r.Id == id);
            if (role == null)
            {
                return NotFound(new { message = "角色不存在" });
            }

            return Ok(role);
        }

        [HttpPost]
        public ActionResult<Role> CreateRole([FromBody] Role role)
        {
            role.Id = _roles.Max(r => r.Id) + 1;
            role.CreateTime = DateTime.Now;
            //  role.IsActive = true;
            _roles.Add(role);
            return CreatedAtAction(nameof(GetRole), new { id = role.Id }, role);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateRole([FromQuery] int id, [FromBody] Role role)
        {
            var existingRole = _roles.FirstOrDefault(r => r.Id == id);
            if (existingRole == null)
            {
                return NotFound(new { message = "角色不存在" });
            }

            existingRole.Name = role.Name;
            // existingRole.Code = role.Code;
            //existingRole.Description = role.Description;
            existingRole.Permissions = role.Permissions;

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteRole(int id)
        {
            var role = _roles.FirstOrDefault(r => r.Id == id);
            if (role == null)
            {
                return NotFound(new { message = "角色不存在" });
            }

            _roles.Remove(role);
            return Ok(new
            {
                code = 0,
                data = "",
                error = "",
                message = "ok"
            });
        }


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
