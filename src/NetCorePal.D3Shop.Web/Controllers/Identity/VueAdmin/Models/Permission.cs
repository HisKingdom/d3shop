using System;

namespace NetCorePal.D3Shop.Web.Controllers.Identity.VueAdmin.Models
{
    public class Permission
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // Menu, Button, API
        public string Path { get; set; } = string.Empty;
        public string Component { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public int? ParentId { get; set; } = new int?();
        public Permission? Parent { get; set; } 
        public List<Permission>? Children { get; set; }
        public int Sort { get; set; }
        public bool IsActive { get; set; }
    }

    public class PermissionInfo
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public string Component { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public int? ParentId { get; set; }
        public int Sort { get; set; }
    }
}
