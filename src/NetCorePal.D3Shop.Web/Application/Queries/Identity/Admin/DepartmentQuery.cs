using Microsoft.EntityFrameworkCore;
using NetCorePal.D3Shop.Admin.Shared.Requests;
using NetCorePal.D3Shop.Admin.Shared.Responses;
using NetCorePal.D3Shop.Domain.AggregatesModel.Identity.DepartmentAggregate;
using NetCorePal.D3Shop.Domain.AggregatesModel.Identity.MenuAggregate;
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


    //public async Task<PagedData<DepartmentResponse>> GetAllDepartmentsAsync(DepartmentQueryRequest queryRequest,
    //    CancellationToken cancellationToken)
    //{
    //    var departments = await DepartmentSet.AsNoTracking()
    //        .WhereIf(!queryRequest.Name.IsNullOrWhiteSpace(), dt => dt.Name.Contains(queryRequest.Name!))
    //        .OrderBy(dt => dt.Id)
    //        .Select(dt => new DepartmentResponse(dt.Id, dt.Name, dt.Description))
    //        .ToPagedDataAsync(queryRequest, cancellationToken);
    //    return departments;
    //}

    /// <summary>
    /// 获取所有部门
    /// </summary>
    /// <param name="queryRequest"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<List<DepartmentResponse>> GetAllDepartmentsAsync(DepartmentQueryRequest queryRequest, CancellationToken cancellationToken)
    {
        var departments = await DepartmentSet.AsNoTracking()
            .WhereIf(!queryRequest.Name.IsNullOrWhiteSpace(), dt => dt.Name.Contains(queryRequest.Name!))
            .OrderBy(dt => dt.Id)
            .Select(d => new DepartmentResponse(
                d.Id,
                d.Name,
                d.Description,
                d.Code,
                d.ParentId,
                d.Status,
                d.CreatedAt,
                new List<DepartmentResponse>()))
            .ToListAsync(cancellationToken);

        // 构建部门树
        var departmentMap = new Dictionary<DeptId, DepartmentResponse>();
        var topLevelDepts = new List<DepartmentResponse>();

        // 首先将所有部门添加到字典中
        foreach (var dept in departments)
        {
            departmentMap[dept.Id] = dept;
        }

        // 然后建立父子关系
        foreach (var dept in departments)
        {
            if (dept.ParentId == new DeptId(0))
            {
                topLevelDepts.Add(dept);
            }
            else if (departmentMap.TryGetValue(dept.ParentId, out var parent))
            {
                parent.Children.Add(dept);
            }
        }

        return topLevelDepts;
    }

    private DepartmentResponse MapToDto(DepartmentResponse dept)
    {

        // 递归转换子菜单，避免循环引用
        if (dept.Children != null && dept.Children.Any())
        {
            dept.Children = dept.Children.Select(child => MapToDto(child)).ToList();
        }
        else
        {
            dept.Children = new List<DepartmentResponse>();
        }
        return dept;
    }

    //public async Task<PagedData<DepartmentResponse>> GetAllDepartmentsAsync(DepartmentQueryRequest queryRequest, CancellationToken cancellationToken)
    //{
    //    var departments = await DepartmentSet
    //        .AsNoTracking()
    //        .Where(d => !d.IsDeleted)
    //        .OrderBy(d => d.Id)
    //        .Select(d => new DepartmentResponse(
    //            d.Id,
    //            d.Name,
    //            d.Description,
    //            d.Code,
    //            d.ParentId,
    //            d.Status,
    //            d.CreatedAt,
    //            new List<DepartmentResponse>()))
    //        .ToPagedDataAsync(queryRequest, cancellationToken);

    //    var departmentResponses = new List<DepartmentResponse>();
    //    var departmentMap = new Dictionary<DeptId, DepartmentResponse>();

    //    // 创建所有部门的响应对象
    //    foreach (var department in departments.Items)
    //    {
    //        var response = new DepartmentResponse(

    //            id: department.Id,
    //            name: department.Name,
    //            remark: department.Description,
    //            code: department.Code,
    //            parentId: department.ParentId,
    //            status: department.Status,
    //            createTime: department.CreatedAt,
    //            children: new List<DepartmentResponse>());


    //        departmentResponses.Add(response);
    //        departmentMap[department.Id] = response;
    //    }

    //    // 构建部门树
    //    foreach (var department in departments.Items)
    //    {
    //        if (department.ParentId != new DeptId(0) && departmentMap.TryGetValue(department.ParentId, out var parentResponse))
    //        {
    //            var childResponse = departmentMap[department.Id];
    //            parentResponse.Children ??= new List<DepartmentResponse>();
    //            parentResponse.Children.Add(childResponse);
    //            departmentResponses.Remove(childResponse);
    //        }
    //    }

    //    return departmentResponses;
    //}

}