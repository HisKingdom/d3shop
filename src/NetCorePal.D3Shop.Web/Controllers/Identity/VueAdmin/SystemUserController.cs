using MediatR;
using Microsoft.AspNetCore.Mvc;
using NetCorePal.D3Shop.Admin.Shared.Permission;
using NetCorePal.D3Shop.Web.Application.Queries.Identity.Admin;
using NetCorePal.D3Shop.Web.Auth;
using NetCorePal.D3Shop.Web.Controllers.Identity.VueAdmin.Models;

namespace NetCorePal.D3Shop.Web.Controllers.Identity.VueAdmin
{
    [ApiController]
    [Route("api/system/user")]
    [VueAuthorize(PermissionCodes.AdminUserManagement)]
    public class SystemUserController(
   IMediator mediator,
   AdminUserQuery adminUserQuery,
   RoleQuery roleQuery,
   ICurrentVueAdminUser currentUser) : ControllerBase
    {
        private static readonly List<SystemUser> _users = new List<SystemUser>
        {
            new SystemUser
            {
                Id = "1",
                Username = "admin",
                Nickname = "管理员",
                Email = "admin@example.com",
                Phone = "13800138000",
                Avatar = "https://example.com/avatar1.jpg",
                Status = 1,
                DeptId = "1",
                RoleIds = new List<string> { "1", "2" },
                CreateTime = DateTime.Now.AddDays(-30),
                UpdateTime = DateTime.Now
            },
            new SystemUser
            {
                Id = "2",
                Username = "user1",
                Nickname = "用户1",
                Email = "user1@example.com",
                Phone = "13800138001",
                Avatar = "https://example.com/avatar2.jpg",
                Status = 1,
                DeptId = "2",
                RoleIds = new List<string> { "2" },
                CreateTime = DateTime.Now.AddDays(-20),
                UpdateTime = DateTime.Now
            },
            new SystemUser
            {
                Id = "3",
                Username = "user2",
                Nickname = "用户2",
                Email = "user2@example.com",
                Phone = "13800138002",
                Avatar = "https://example.com/avatar3.jpg",
                Status = 0,
                DeptId = "2",
                RoleIds = new List<string> { "2" },
                CreateTime = DateTime.Now.AddDays(-10),
                UpdateTime = DateTime.Now
            }
        };

        [HttpGet("list")]
        public IActionResult GetUserList([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string username = "",
            [FromQuery] string nickname = "",
            [FromQuery] string email = "",
            [FromQuery] string phone = "",
            [FromQuery] int? status = null,
            [FromQuery] string deptId = "",
            [FromQuery] string roleId = "",
            [FromQuery] DateTime? startTime = null,
            [FromQuery] DateTime? endTime = null)
        {
            var query = _users.AsQueryable();

            if (!string.IsNullOrEmpty(username))
            {
                query = query.Where(u => u.Username.Contains(username));
            }

            if (!string.IsNullOrEmpty(nickname))
            {
                query = query.Where(u => u.Nickname.Contains(nickname));
            }

            if (!string.IsNullOrEmpty(email))
            {
                query = query.Where(u => u.Email.Contains(email));
            }

            if (!string.IsNullOrEmpty(phone))
            {
                query = query.Where(u => u.Phone.Contains(phone));
            }

            if (status.HasValue)
            {
                query = query.Where(u => u.Status == status.Value);
            }

            if (!string.IsNullOrEmpty(deptId))
            {
                query = query.Where(u => u.DeptId == deptId);
            }

            if (!string.IsNullOrEmpty(roleId))
            {
                query = query.Where(u => u.RoleIds.Contains(roleId));
            }

            if (startTime.HasValue)
            {
                query = query.Where(u => u.CreateTime >= startTime.Value);
            }

            if (endTime.HasValue)
            {
                query = query.Where(u => u.CreateTime <= endTime.Value);
            }

            var total = query.Count();

            var list = query
                .OrderByDescending(u => u.CreateTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            //return Ok(new { list, total });

            var result = new
            {
                items = list,
                total = total,
                page = page,
                pageSize = pageSize
            };

            return Ok(new
            {
                code = 0,
                data = result,
                error = "",
                message = "ok"
            });
        }

        [HttpPost]
        public IActionResult CreateUser([FromBody] SystemUser user)
        {
            user.Id = Guid.NewGuid().ToString();
            user.CreateTime = DateTime.Now;
            user.UpdateTime = DateTime.Now;

            _users.Add(user);
            return Ok(user);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateUser(string id, [FromBody] SystemUser user)
        {
            var existingUser = _users.FirstOrDefault(u => u.Id == id);
            if (existingUser == null)
            {
                return NotFound(new { message = $"User with ID {id} not found" });
            }

            existingUser.Nickname = user.Nickname;
            existingUser.Email = user.Email;
            existingUser.Phone = user.Phone;
            existingUser.Avatar = user.Avatar;
            existingUser.Status = user.Status;
            existingUser.DeptId = user.DeptId;
            existingUser.RoleIds = user.RoleIds;
            existingUser.UpdateTime = DateTime.Now;

            return Ok(existingUser);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(string id)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound(new { message = $"User with ID {id} not found" });
            }

            _users.Remove(user);
            return Ok(new { message = "User deleted successfully" });
        }

        [HttpPatch("{id}/status")]
        public IActionResult UpdateUserStatus(string id, [FromBody] int status)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound(new { message = $"User with ID {id} not found" });
            }

            user.Status = status;
            user.UpdateTime = DateTime.Now;

            return Ok(new { message = "User status updated successfully" });
        }

        [HttpPatch("{id}/password")]
        public IActionResult ResetUserPassword(string id, [FromBody] string password)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound(new { message = $"User with ID {id} not found" });
            }

            // 这里只是模拟，实际应该加密密码
            return Ok(new { message = "Password reset successfully" });
        }
    }
}

