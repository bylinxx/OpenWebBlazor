using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OpenWebBlazor.Models;

[Index(nameof(UserName), IsUnique = true)]
public class WebUsers
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [DisplayName("用户标识")]
    public string Id { get; set; }
    [DisplayName("用户名")]
    [Required]
    public string? UserName { get; set; }
    [DisplayName("用户密码")]
    public string? Password { get; set; }
    [DisplayName("用户状态")]
    public int State { get; set; } = 0;
}
