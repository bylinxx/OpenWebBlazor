using System.ComponentModel;

namespace OpenWebBlazor.ViewModels;

public class UserList
{
    [DisplayName("用户标识")]
    public int Id { get; set; }
    [DisplayName("用户名")]
    public string Name { get; set; }
    [DisplayName("用户状态")]
    public int State { get; set; }
    [DisplayName("用户角色")]
    public List<RolesViewModel> Roles { get; set; }

    public class RolesViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}