using System.ComponentModel.DataAnnotations;

namespace NetCorePal.D3Shop.Web.Controllers.Identity.VueAdmin.Requests
{
    public class VueUpdateDepartmentRequest
    {
        [Required(ErrorMessage = "部门名称不能为空")]
        public string Name { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;

        public string? ParentId { get; set; }

        public int Status { get; set; }

        public string Remark { get; set; } = string.Empty;
    }
}