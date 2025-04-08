using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.D3Shop.Web.Controllers.Identity.VueAdmin.Models;
using NetCorePal.D3Shop.Admin.Shared.Permission;
using NetCorePal.D3Shop.Web.Auth;
using NetCorePal.D3Shop.Web.Application.Queries.Identity.Admin;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using NetCorePal.D3Shop.Web.Helper;
using OpenIddict.Client;
using NetCorePal.D3Shop.Admin.Shared.Requests;
using NetCorePal.D3Shop.Admin.Shared.Responses;
using NetCorePal.Extensions.Dto;
using NetCorePal.D3Shop.Web.Controllers.Identity.VueAdmin.Responses;
using NetCorePal.D3Shop.Web.Controllers.Identity.VueAdmin.Requests;

namespace PlaygroundApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [VueAuthorize(PermissionCodes.AdminUserManagement)]
    public class UserController(IMediator mediator, AdminUserQuery adminUserQuery, RoleQuery roleQuery, ICurrentVueAdminUser currentUser) : ControllerBase
    {
        private CancellationToken CancellationToken => HttpContext?.RequestAborted ?? default;

        //private static readonly List<User> _users = new List<User>
        //{
        //    new User
        //    {
        //        Id = 1,
        //        Username = "admin",
        //        Password = "123456", // 实际应用中应该使用加密密码
        //        RealName = "管理员",
        //        Email = "admin@example.com",
        //        Phone = "13800138000",
        //        DepartmentId = 1,
        //        Department = new Department
        //        {
        //            Id = 1,
        //            Name = "技术部",
        //            Code = "TECH",
        //            Description = "技术部门"
        //        },
        //        Roles = new List<VueCreateRoleRequest>
        //        {
        //            new VueCreateRoleRequest
        //            {
        //                Id = 1,
        //                Name = "超级管理员",
        //                //Code = "SUPER_ADMIN",
        //                //Description = "系统超级管理员",
        //                //Permissions = new List<NetCorePal.D3Shop.Web.Controllers.Identity.VueAdmin.Models.Permission>
        //                //{
        //                //    new NetCorePal.D3Shop.Web.Controllers.Identity.VueAdmin.Models.Permission
        //                //    {
        //                //        Id = 1,
        //                //        Name = "系统管理",
        //                //        Code = "SYSTEM_MANAGE",
        //                //        Type = "Menu",
        //                //        Path = "/system",
        //                //        Component = "Layout",
        //                //        Icon = "setting",
        //                //        Sort = 1
        //                //    }
        //                //}
        //            }
        //        },
        //        CreateTime = DateTime.Now,
        //        IsActive = true
        //    }
        //};


        [HttpGet("info")]
        public async Task<ResponseData<VueAdminUserResponse?>> GetUserInfo()
        {
            var userId = currentUser.UserId;
            var adminUsers = await adminUserQuery.GetAdminUserByIdAsync(userId, CancellationToken);
            return adminUsers.AsResponseData();
        }


        //[HttpGet("info")]
        //public ActionResult<object> GetUserInfo()
        //{
        //    try
        //    {
        //        var userId = currentUser.UserId;

        //        var user = _users.FirstOrDefault(u => u.Id == userId);
        //        if (user == null)
        //        {
        //            return NotFound(new { code = -1, message = "用户不存在", error = "NotFoundException" });
        //        }

        //        return new JsonResult(new
        //        {
        //            code = 0,
        //            data = new
        //            {
        //                id = user.Id,
        //                username = user.Username,
        //                realName = user.RealName,
        //                roles = user.Roles.Select(r => r.Code).ToArray(),
        //                homePath = "/dashboard"
        //            },
        //            error = "",
        //            message = "ok"
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { code = -1, message = "服务器内部错误", error = ex.Message });
        //    }
        //}


        [HttpGet("/api/auth/codes")]
        [VueAuthorize(PermissionCodes.AdminUserCreate)]
        public async Task<ActionResult<object>> GetAccessCodes()
        {
            try
            {
                //从请求中获取用户ID
                var userId = currentUser.UserId;

                var codes = await adminUserQuery.GetAdminUserPermissionCodes(userId);

                return Ok(new { code = 0, data = codes, error = "", message = "ok" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { code = -1, message = "服务器内部错误", error = ex.Message });
            }
        }

        [HttpPost("/api/auth/logout")]
        public ActionResult<object> Logout()
        {
            try
            {
                // 清除Refresh Token Cookie
                Response.Cookies.Delete("refreshToken", new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None
                });

                return Ok(new { code = 0, data = new { }, error = "", message = "ok" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { code = -1, message = "服务器内部错误", error = ex.Message });
            }
        }
    }
}
