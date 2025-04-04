using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OpenWebBlazor.ViewModels;

public class UserPassword
{
    public string UserId { get; set; }
    [DisplayName("密码")]
    [Required(ErrorMessage = "密码是必填项")]
    [MinLength(6, ErrorMessage = "密码至少需要6个字符")]
    public string Password { get; set; }

    [DisplayName("确认密码")]
    [Required(ErrorMessage = "请再次输入密码")]
    [Compare("Password", ErrorMessage = "两次输入的密码不一致")]
    public string ConfirmPassword { get; set; }
}