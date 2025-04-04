using System.ComponentModel;

namespace OpenWebBlazor.ViewModels
{
    public class WebMenuTree
    {
        public string Id { get; set; }
        [DisplayName("菜单名称")]
        public string Name { get; set; }
        [DisplayName("菜单路径")]
        public string Path { get; set; }
        [DisplayName("菜单排序")]
        public int Sort { get; set; }
        [DisplayName("是否显示")]
        public bool IsShow { get; set; }
        [DisplayName("是否需要授权")]
        public bool IsAuth { get; set; }
        [DisplayName("父类Id")]
        public string ParentId { get; set; }
        public List<WebMenuTree> Items { get; set; }
        public List<string> RoleIds { get; set; }
    }
}
