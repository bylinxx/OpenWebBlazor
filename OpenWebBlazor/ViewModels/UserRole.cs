using System.ComponentModel;

namespace OpenWebBlazor.ViewModels
{
    public class UserRole
    {
        public int UserId { get; set; }
        [DisplayName("用户角色")]
        public string[] RoleIds { get; set; }
    }
}
