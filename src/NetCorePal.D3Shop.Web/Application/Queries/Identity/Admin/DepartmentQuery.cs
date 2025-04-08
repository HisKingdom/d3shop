using Microsoft.EntityFrameworkCore;
using NetCorePal.D3Shop.Admin.Shared.Requests;
using NetCorePal.D3Shop.Admin.Shared.Responses;
using NetCorePal.D3Shop.Domain.AggregatesModel.Identity.DepartmentAggregate;
using NetCorePal.D3Shop.Web.Controllers.Identity.VueAdmin.Responses;
using NetCorePal.D3Shop.Web.Extensions;
using NetCorePal.Extensions.Dto;
using NetCorePal.Extensions.Primitives;

namespace NetCorePal.D3Shop.Web.Application.Queries.Identity.Admin;

public class DepartmentQuery(ApplicationDbContext applicationDbContext) : IQuery
{
    private DbSet<Department> DepartmentSet { get; } = applicationDbContext.Departments;

    public async Task<bool> DoesDepartmentExist(string name, CancellationToken cancellationToken)
    {
        return await DepartmentSet.AsNoTracking()
            .AnyAsync(r => r.Name == name, cancellationToken: cancellationToken);
    }


    public async Task<PagedData<DepartmentResponse>> GetAllDepartmentsAsync(DepartmentQueryRequest queryRequest,
        CancellationToken cancellationToken)
    {
        var departments = await DepartmentSet.AsNoTracking()
            .WhereIf(!queryRequest.Name.IsNullOrWhiteSpace(), dt => dt.Name.Contains(queryRequest.Name!))
            .OrderBy(dt => dt.Id)
            .Select(dt => new DepartmentResponse(dt.Id, dt.Name, dt.Description))
            .ToPagedDataAsync(queryRequest, cancellationToken);
        return departments;
    }

    public async Task<List<VueDepartmentResponse>> GetAllDepartmentsAsync(CancellationToken cancellationToken)
    {
        var departments = await DepartmentSet
            .AsNoTracking()
            .Where(d => !d.IsDeleted)
            .OrderBy(d => d.Id)
            .ToListAsync(cancellationToken);

        var departmentResponses = new List<VueDepartmentResponse>();
        var departmentMap = new Dictionary<DeptId, VueDepartmentResponse>();

        // 创建所有部门的响应对象
        foreach (var department in departments)
        {
            var response = new VueDepartmentResponse
            {
                Id = department.Id.ToString(),
                Name = department.Name,
                Code = department.Code,
                ParentId = department.ParentId.ToString() == "0" ? null : department.ParentId.ToString(),
                Status = department.Status,
                Remark = department.Description,
                CreateTime = department.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                Children = new List<VueDepartmentResponse>()
            };

            departmentResponses.Add(response);
            departmentMap[department.Id] = response;
        }

        // 构建部门树
        foreach (var department in departments)
        {
            if (department.ParentId.ToString() != "0" && departmentMap.TryGetValue(department.ParentId, out var parentResponse))
            {
                var childResponse = departmentMap[department.Id];
                parentResponse.Children ??= new List<VueDepartmentResponse>();
                parentResponse.Children.Add(childResponse);
                departmentResponses.Remove(childResponse);
            }
        }

        return departmentResponses;
    }

}