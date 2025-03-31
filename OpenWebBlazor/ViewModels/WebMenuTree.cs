using System.ComponentModel;

namespace OpenWebBlazor.ViewModels
{
    public class WebMenuTree
    {
        public int Id { get; set; }
        [DisplayName("菜单名称")]
        public string Name { get; set; }
        [DisplayName("菜单路径")]
        public string Path { get; set; }
        [DisplayName("菜单排序")]
        public int Sort { get; set; }
        [DisplayName("父类Id")]
        public int ParentId { get; set; }
        public List<WebMenuTree> Items { get; set; }
    }
}
