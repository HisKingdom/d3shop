using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using NetCorePal.D3Shop.Admin.Shared.Permission;
using NetCorePal.D3Shop.Domain.AggregatesModel.Identity.RoleAggregate;
using NetCorePal.D3Shop.Web.Admin.Client.Pages;
using NetCorePal.D3Shop.Web.Application.Commands.Identity.Client;
using NetCorePal.D3Shop.Web.Application.Queries.Identity.Admin;
using NetCorePal.D3Shop.Web.Application.Queries.Identity.Client;
using NetCorePal.D3Shop.Web.Auth;
using NetCorePal.D3Shop.Web.Controllers.Identity.Client.Requests;
using NetCorePal.D3Shop.Web.Controllers.Identity.Client.Responses;
using NetCorePal.D3Shop.Web.Controllers.Identity.VueAdmin.Models;
using NetCorePal.D3Shop.Web.Controllers.Identity.VueAdmin.Requests;
using NetCorePal.D3Shop.Web.Controllers.Identity.VueAdmin.Responses;
using NetCorePal.D3Shop.Web.Helper;
using NetCorePal.Extensions.Dto;
using NetCorePal.Extensions.Primitives;
using OpenIddict.Client;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NetCorePal.D3Shop.Web.Controllers.Identity.VueAdmin
{
    [Route("api/auth/[action]")]
    [ApiController]
    [AllowAnonymous]
    public class VueAuthController(
    IMediator mediator,
    AdminUserQuery adminUserQuery,
    TokenGenerator tokenGenerator,
    OpenIddictClientService openIddictClientService,
    IMemoryCache memoryCache) : ControllerBase
    {

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
        //        Roles = new List<NetCorePal.D3Shop.Web.Controllers.Identity.VueAdmin.Models.Role>
        //        {
        //            new NetCorePal.D3Shop.Web.Controllers.Identity.VueAdmin.Models.Role
        //            {
        //                Id = 1,
        //                Name = "超级管理员1",
        //                Code = "SUPER_ADMIN1",
        //                Description = "系统超级管理员1",
        //                Permissions = new List<NetCorePal.D3Shop.Web.Controllers.Identity.VueAdmin.Models.Permission>
        //                {
        //                    new NetCorePal.D3Shop.Web.Controllers.Identity.VueAdmin.Models.Permission
        //                    {
        //                        Id = 1,
        //                        Name = "系统管理2",
        //                        Code = "SYSTEM_MANAGE2",
        //                        Type = "Menu",
        //                        Path = "/system",
        //                        Component = "Layout",
        //                        Icon = "setting",
        //                        Sort = 1
        //                    }
        //                }
        //            }
        //        },
        //        CreateTime = DateTime.Now,
        //        IsActive = true
        //    }
        //};



        [HttpPost]
        [AllowAnonymous]
        public async Task<ResponseData<VueUserLoginResponse>> Login([FromBody] VueUserLoginRequest request)
        {
            //重定向地址
            var redirectUri = HttpContext.Request?.Query["redirect"].FirstOrDefault();
            if (string.IsNullOrEmpty(redirectUri))
            {
                redirectUri = "/analytics";
            }
            var authInfo =
                await adminUserQuery.GetUserInfoForLoginAsync(request.UserName, HttpContext.RequestAborted);
            if (authInfo is null)
                throw new KnownException("无效的用户", -1);

            if (!PasswordHasher.VerifyHashedPassword(request.Password, authInfo.Password))
                throw new KnownException("密码错误", -1);


            var refreshToken = TokenGenerator.GenerateRefreshToken();

            ICollection<RoleId> roles = await adminUserQuery.GetAssignedRoleIdsAsync(authInfo.Id, HttpContext.RequestAborted);

            var token = await tokenGenerator.GenerateJwtAsync([
                new Claim(ClaimTypes.NameIdentifier, authInfo.Id.ToString()),
                new Claim(ClaimTypes.Name, authInfo.Name)
            ]);
            return VueUserLoginResponse.Success(token, refreshToken, authInfo.Id, authInfo.Name, authInfo.Name, roles, redirectUri).AsResponseData();
        }


        //[HttpPost("/api/auth/refresh")]
        //public ActionResult<object> RefreshToken()
        //{
        //    try
        //    {
        //        var refreshToken = Request.Cookies["refreshToken"];
        //        if (string.IsNullOrEmpty(refreshToken))
        //        {
        //            return Unauthorized(new { code = -1, message = "刷新令牌不存在", error = "UnauthorizedException" });
        //        }

        //        var user = ValidateRefreshToken(refreshToken);
        //        if (user == null)
        //        {
        //            return Unauthorized(new { code = -1, message = "刷新令牌无效或已过期", error = "UnauthorizedException" });
        //        }

        //        var accessToken = GenerateAccessToken(user);
        //        var newRefreshToken = GenerateRefreshToken(user);

        //        // 更新Refresh Token Cookie
        //        Response.Cookies.Append("refreshToken", newRefreshToken, new CookieOptions
        //        {
        //            HttpOnly = true,
        //            Secure = true,
        //            SameSite = SameSiteMode.None,
        //            Expires = DateTimeOffset.UtcNow.AddDays(1)
        //        });

        //        return Ok(new
        //        {
        //            code = 0,
        //            data = new
        //            {
        //                accessToken,
        //                refreshToken = newRefreshToken
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


     

        //private string GenerateAccessToken(User user)
        //{
        //    try
        //    {
        //        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ThisIsMySecretKeyForTestingJWTTokenGenerationAndValidation"));
        //        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //        var claims = new[]
        //        {
        //            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        //            new Claim(ClaimTypes.Name, user.Username),
        //            new Claim(ClaimTypes.Email, user.Email),
        //            new Claim("RealName", user.RealName)
        //        };

        //        var token = new JwtSecurityToken(
        //            issuer: "playground-api",
        //            audience: "playground-client",
        //            claims: claims,
        //            expires: DateTime.Now.AddMinutes(20),
        //            signingCredentials: credentials
        //        );

        //        return new JwtSecurityTokenHandler().WriteToken(token);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

        private string GenerateRefreshToken(User user)
        {
            try
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ThisIsMySecretKeyForTestingJWTTokenGenerationAndValidation"));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim("type", "refresh")
                };

                var token = new JwtSecurityToken(
                  issuer: "playground-api",
                    audience: "playground-client",
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: credentials
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //private User ValidateRefreshToken(string refreshToken)
        //{
        //    try
        //    {
        //        var tokenHandler = new JwtSecurityTokenHandler();
        //        var key = Encoding.UTF8.GetBytes("ThisIsMySecretKeyForTestingJWTTokenGenerationAndValidation");

        //        tokenHandler.ValidateToken(refreshToken, new TokenValidationParameters
        //        {
        //            ValidateIssuerSigningKey = true,
        //            IssuerSigningKey = new SymmetricSecurityKey(key),
        //            ValidateIssuer = true,
        //            ValidIssuer = "playground-api",
        //            ValidateAudience = true,
        //            ValidAudience = "playground-client",
        //            ValidateLifetime = true,
        //            ClockSkew = TimeSpan.Zero
        //        }, out SecurityToken validatedToken);

        //        var jwtToken = (JwtSecurityToken)validatedToken;
        //        var userId = int.Parse(jwtToken.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);
        //        var user = _users.FirstOrDefault(u => u.Id == userId);

        //        if (user == null)
        //        {
        //            return new Models.User();
        //        }

        //        return user;
        //    }
        //    catch (Exception ex)
        //    {
        //        return new Models.User();
        //    }
        //}
    }
}
