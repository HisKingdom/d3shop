namespace NetCorePal.D3Shop.Web.Controllers.Identity.VueAdmin.Models
{
    public class SystemUser
    {
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Nickname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
        public int Status { get; set; }
        public string DeptId { get; set; } = string.Empty;
        public List<string> RoleIds { get; set; } = new List<string>();
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
