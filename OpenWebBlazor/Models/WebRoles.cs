using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpenWebBlazor.Models;

public class WebRoles
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public string Id { get; set; }
    [Required]
    [DisplayName("角色名称")]
    public string Name { get; set; }
    [Required]
    [DisplayName("是否超管")]
    public bool IsSuper { get; set; }
}