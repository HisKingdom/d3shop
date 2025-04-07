using System;

namespace NetCorePal.D3Shop.Web.Controllers.Identity.VueAdmin.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }= string.Empty;
        public string Code { get; set; } = string.Empty;
        public int? ParentId { get; set; }
        public Department? Parent { get; set; } 
        public List<Department>? Children { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime CreateTime { get; set; }
        public bool IsActive { get; set; }
    }

    public class DepartmentInfo
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public int? ParentId { get; set; } = new int?();
        public string Description { get; set; } = string.Empty;
    }
}
