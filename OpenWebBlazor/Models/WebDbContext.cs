using Microsoft.EntityFrameworkCore;

namespace OpenWebBlazor.Models;

public class WebDbContext: DbContext
{
    public WebDbContext(DbContextOptions<WebDbContext> options) : base(options)
    {
        
    }
    public DbSet<WebUsers> WebUsers { get; set; }
    public DbSet<WebRoles> WebRoles { get; set; }
    public DbSet<WebUserRoles> WebUserRoles { get; set; }
    public DbSet<WebMenus> WebMenus { get; set; }
    public DbSet<WebMenuRoles> WebMenuRoles { get; set; }
}
