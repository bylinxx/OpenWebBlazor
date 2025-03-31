using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OpenWebBlazor.Models;

public class WebRoles
{
    [Key]
    public string Id { get; set; }
    [Required]
    [DisplayName("��ɫ����")]
    public string Name { get; set; }
    [Required]
    [DisplayName("�Ƿ񳬹�")]
    public bool IsSuper { get; set; }
}