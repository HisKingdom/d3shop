using FluentValidation;
using NetCorePal.D3Shop.Domain.AggregatesModel.Identity.DepartmentAggregate;
using NetCorePal.D3Shop.Infrastructure.Repositories.Identity.Admin;
using NetCorePal.Extensions.Primitives;

namespace NetCorePal.D3Shop.Web.Application.Commands.Identity.VueAdmin
{
    public record VueUpdateDepartmentCommand(
        DeptId DepartmentId,
        string Name,
        string Code,
        string? ParentId,
        int Status,
        string Remark) : ICommand;

    public class VueUpdateDepartmentCommandValidator : AbstractValidator<VueUpdateDepartmentCommand>
    {
        public VueUpdateDepartmentCommandValidator()
        {
            RuleFor(x => x.DepartmentId).NotEmpty().WithMessage("部门ID不能为空");
            RuleFor(x => x.Name).NotEmpty().WithMessage("部门名称不能为空");
        }
    }

    public class VueUpdateDepartmentCommandHandler : ICommandHandler<VueUpdateDepartmentCommand>
    {
        private readonly IDepartmentRepository _departmentRepository;

        public VueUpdateDepartmentCommandHandler(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        public async Task Handle(VueUpdateDepartmentCommand request, CancellationToken cancellationToken)
        {
            var department = await _departmentRepository.GetAsync(request.DepartmentId, cancellationToken) ??
                             throw new KnownException($"未找到部门，DepartmentId = {request.DepartmentId}");

            department.UpdateDepartInfo(
                request.Name,
                request.Code,
                request.Remark,
                request.Status,
                Array.Empty<DepartmentUser>());
        }
    }
}