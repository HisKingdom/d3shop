using NetCorePal.D3Shop.Domain.AggregatesModel.Identity.DepartmentAggregate;

namespace NetCorePal.D3Shop.Web.Controllers.Identity.VueAdmin.Responses
{
    public class VueDepartmentResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? ParentId { get; set; }
        public int Status { get; set; }
        public string Remark { get; set; } = string.Empty;
        public string CreateTime { get; set; } = string.Empty;
        public List<VueDepartmentResponse>? Children { get; set; }
    }
}