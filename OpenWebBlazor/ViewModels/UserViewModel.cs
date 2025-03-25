namespace OpenWebBlazor.ViewModels;

public class UserViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int State { get; set; }
    public List<RolesViewModel> Roles { get; set; }
    
    public class RolesViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}