using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OpenWebBlazor.Models;

[Index(nameof(UserName), IsUnique = true)]
public class WebUsers
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public int State { get; set; } = 0;
}
