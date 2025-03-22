using Microsoft.EntityFrameworkCore;

namespace OpenWebBlazor.Models;

public class WebDbContext: DbContext
{
    public WebDbContext(DbContextOptions<WebDbContext> options) : base(options)
    {
        
    }
    public DbSet<WebUsers> WebUsers { get; set; }
}
