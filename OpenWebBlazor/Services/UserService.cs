using Microsoft.EntityFrameworkCore;
using OpenWebBlazor.Models;

namespace OpenWebBlazor.Services;

public class UserService
{
    private readonly WebDbContext _dbContext;

    public UserService(WebDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> Login(string username, string password)
    {
        var user_data = await _dbContext.WebUsers.FirstOrDefaultAsync(a => a.Name == username);
        if(user_data == null) return false;
        else if(user_data.Password != password) return false;
        return true;
    }
}