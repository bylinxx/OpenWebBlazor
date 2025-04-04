using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpenWebBlazor.Models;

public class WebMenus
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public string Id { get; set; }
    [DisplayName("上级菜单Id")]
    [Required]
    public string ParentId { get; set; }
    [Required]
    [DisplayName("菜单名称")]
    public string Name { get; set; }
    [DisplayName("菜单路径")]
    public string Path { get; set; }
    [DisplayName("是否显示")]
    public bool IsShow { get; set; }
    [DisplayName("是否需要授权")]
    public bool IsAuth { get; set; }
    [Required]
    [DisplayName("排序")]
    public int Sort { get; set; }
}