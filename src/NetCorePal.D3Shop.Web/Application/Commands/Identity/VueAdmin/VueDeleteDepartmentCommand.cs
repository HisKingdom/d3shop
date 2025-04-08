using NetCorePal.D3Shop.Domain.AggregatesModel.Identity.DepartmentAggregate;
using NetCorePal.D3Shop.Infrastructure.Repositories.Identity.Admin;
using NetCorePal.Extensions.Primitives;

namespace NetCorePal.D3Shop.Web.Application.Commands.Identity.VueAdmin
{
    public record VueDeleteDepartmentCommand(DeptId DepartmentId) : ICommand;

    public class VueDeleteDepartmentCommandHandler : ICommandHandler<VueDeleteDepartmentCommand>
    {
        private readonly IDepartmentRepository _departmentRepository;

        public VueDeleteDepartmentCommandHandler(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        public async Task Handle(VueDeleteDepartmentCommand request, CancellationToken cancellationToken)
        {
            var department = await _departmentRepository.GetAsync(request.DepartmentId, cancellationToken) ??
                             throw new KnownException($"未找到部门，DepartmentId = {request.DepartmentId}");

            department.Delete();
        }
    }
}