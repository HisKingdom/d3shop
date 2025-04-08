using MediatR;
using Microsoft.AspNetCore.Mvc;
using NetCorePal.D3Shop.Domain.AggregatesModel.Identity.DepartmentAggregate;
using NetCorePal.D3Shop.Web.Application.Commands.Identity.VueAdmin;
using NetCorePal.D3Shop.Web.Application.Queries.Identity.Admin;
using NetCorePal.D3Shop.Web.Controllers.Identity.VueAdmin.Requests;
using NetCorePal.D3Shop.Web.Controllers.Identity.VueAdmin.Responses;
using NetCorePal.Extensions.Dto;
using NetCorePal.Extensions.Primitives;
using NetCorePal.D3Shop.Web.Auth;
using NetCorePal.D3Shop.Web.Blazor;
using NetCorePal.D3Shop.Admin.Shared.Permission;
using NetCorePal.D3Shop.Admin.Shared.Requests;
using NetCorePal.D3Shop.Web.Application.Commands.Identity.Admin;

namespace PlaygroundApi.Controllers
{
    [Route("api/system/dept")]
    [ApiController]
    [VueAuthorize(PermissionCodes.DepartmentManagement)]
    public class DepartmentController(IMediator mediator, DepartmentQuery departmentQuery) : ControllerBase
    {


        [HttpPost]
        [VueAuthorize(PermissionCodes.DepartmentCreate)]
        public async Task<ResponseData<DeptId>> CreateDepartment([FromBody] CreateDepartmentRequest request, CancellationToken cancellationToken)
        {
            var departmentId = await mediator.Send(new CreateDepartmentCommand(
                request.Name,
                request.Remark,
                request.Users,
                request.Pid,
                request.Status
                ),
                cancellationToken);

            return new ResponseData<DeptId>(departmentId);
        }

        [HttpGet("list")]
        public async Task<ResponseData<List<VueDepartmentResponse>>> GetDepartments(CancellationToken cancellationToken)
        {
            var departments = await departmentQuery.GetAllDepartmentsAsync(cancellationToken);
            return new ResponseData<List<VueDepartmentResponse>>(departments);
        }

        [HttpGet("{id}")]
        public async Task<ResponseData<VueDepartmentResponse>> GetDepartment(string id, CancellationToken cancellationToken)
        {
            var departments = await departmentQuery.GetAllDepartmentsAsync(cancellationToken);
            var department = departments.FirstOrDefault(d => d.Id == id);

            if (department == null)
            {
                throw new KnownException("部门不存在", -1);
            }

            return new ResponseData<VueDepartmentResponse>(department);
        }

      

        [HttpPut("{id}")]
        public async Task<ResponseData<object>> UpdateDepartment(string id, [FromBody] VueUpdateDepartmentRequest request, CancellationToken cancellationToken)
        {
            if (!long.TryParse(id, out long departmentIdValue))
            {
                throw new KnownException("无效的部门ID", -1);
            }

            var deptId = new DeptId(departmentIdValue);

            await mediator.Send(new VueUpdateDepartmentCommand(
                deptId,
                request.Name,
                request.Code,
                request.ParentId,
                request.Status,
                request.Remark),
                cancellationToken);

            return new ResponseData<object>(new object());
        }

        [HttpPut("{id}/status")]
        public async Task<ResponseData<DeptId>> UpdateDepartmentStatus(DeptId id, [FromBody] VueUpdateDepartmentStatusRequest request)
        {
            var command = new VueUpdateDepartmentStatusCommand(
                id,
                request.Status);

            await mediator.Send(command);
            return id.AsResponseData();
        }

        [HttpDelete("{id}")]
        public async Task<ResponseData<object>> DeleteDepartment(string id, CancellationToken cancellationToken)
        {
            if (!long.TryParse(id, out long departmentIdValue))
            {
                throw new KnownException("无效的部门ID", -1);
            }

            var deptId = new DeptId(departmentIdValue);

            await mediator.Send(new VueDeleteDepartmentCommand(deptId), cancellationToken);

            return new ResponseData<object>(new object());
        }
    }
}

