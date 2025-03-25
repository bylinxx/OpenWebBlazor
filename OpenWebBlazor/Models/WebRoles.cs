using System.ComponentModel.DataAnnotations;

namespace OpenWebBlazor.Models;

public class WebRoles
{
    [Key]
    public string Id { get; set; }
    public string Name { get; set; }
    public bool IsSuper { get; set; }
}