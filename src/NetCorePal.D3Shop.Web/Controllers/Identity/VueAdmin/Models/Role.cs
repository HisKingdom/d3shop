using System;

namespace NetCorePal.D3Shop.Web.Controllers.Identity.VueAdmin.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        //public string Code { get; set; } = string.Empty;
        //public string Description { get; set; } = string.Empty;
        public DateTime CreateTime { get; set; }
        //public bool IsActive { get; set; }
        public int Status { get; set; }
        public string Remark { get; set; } = string.Empty;

        public List<string> Permissions { get; set; } = new List<string>();
       // public List<Permission> Permissions { get; set; } = new List<Permission>();
    }

   
}
