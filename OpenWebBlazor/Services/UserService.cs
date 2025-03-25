using Microsoft.EntityFrameworkCore;
using OpenWebBlazor.Models;
using OpenWebBlazor.ViewModels;

namespace OpenWebBlazor.Services;

public class UserService
{
    private readonly WebDbContext _dbContext;

    public UserService(WebDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<LoginResult> Login(string username, string password)
    {
        var result = new LoginResult();
        var userData = await _dbContext.WebUsers.FirstOrDefaultAsync(a => a.Name == username);
        if (userData == null)
        {
            result.Success = false;
            result.Message = "用户不存在";
        }
        else if (userData.Password != password)
        {
            result.Success = false;
            result.Message = "密码错误";
        }
        else
        {
            result.Success = true;
            result.Name = userData.Name;
        }

        return result;
    }

    public async Task<List<UserViewModel>> GetUsers()
    {
        var users = await _dbContext.WebUsers.ToListAsync();
        var user_ids = users.Select(a => a.Id).ToList();
        var user_roles = _dbContext.WebUserRoles.Where(a => user_ids.Contains(a.UserId))
            .Join(_dbContext.WebRoles, a => a.RoleId, b => b.Id, (a, b) => new { a, b })
            .ToLookup((a => a.a.UserId))
            .Select((a => new
            {
                user_id = a.Key,
                roles = a.Select((b => b.b)).ToList()
            })).ToList();
        return users.Select(a => new UserViewModel()
        {
            Id = a.Id,
            Name = a.Name,
            State = a.State,
            Roles = user_roles.FirstOrDefault(b => b.user_id == a.Id)?.roles.Select(b =>
                new UserViewModel.RolesViewModel()
                {
                    Id = b.Id,
                    Name = b.Name
                }).ToList()
        }).ToList();
    }
}