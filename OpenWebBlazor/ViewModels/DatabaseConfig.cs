using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace OpenWebBlazor.ViewModels
{
    public class DatabaseConfig
    {
        [Required]
        [DisplayName("服务器地址")]
        public string Server { get; set; }
        [Required]
        [DisplayName("数据库名称")]
        public string Database { get; set; }
        [Required]
        [DisplayName("数据库用户")]
        public string UserId { get; set; }
        [Required]
        [DisplayName("数据库密码")]
        public string Password { get; set; }
    }
}
