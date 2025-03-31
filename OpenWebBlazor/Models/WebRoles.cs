using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OpenWebBlazor.Models;

public class WebRoles
{
    [Key]
    public string Id { get; set; }
    [Required]
    [DisplayName("角色名称")]
    public string Name { get; set; }
    [Required]
    [DisplayName("是否超管")]
    public bool IsSuper { get; set; }
}