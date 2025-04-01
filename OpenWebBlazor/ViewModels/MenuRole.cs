using System.ComponentModel;

namespace OpenWebBlazor.ViewModels
{
    public class MenuRole
    {
        public int MenuId { get; set; }
        [DisplayName("用户角色")]
        public string[] RoleIds { get; set; }
    }
}
