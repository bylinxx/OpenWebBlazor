using Microsoft.EntityFrameworkCore;
using OpenWebBlazor.Models;
using OpenWebBlazor.ViewModels;

namespace OpenWebBlazor.Services;

public class RoleService
{
    private readonly IDbContextFactory<WebDbContext> _dbContextFactory;

    public RoleService(IDbContextFactory<WebDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<BaseResult> EditRole(WebRoles role)
    {
        using (var _dbContext = _dbContextFactory.CreateDbContext())
        {
            try
            {
                var data = await _dbContext.WebRoles.FirstOrDefaultAsync(a => a.Id == role.Id);
                if (data == null)
                {
                    _dbContext.WebRoles.Add(role);
                }
                else
                {
                    data.Name = role.Name;
                    data.IsSuper = role.IsSuper;
                }

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return new BaseResult() { Success = false, Message = e.Message };
            }

            return new BaseResult { Success = true };
        }
    }

    public async Task<BaseResult> DeleteRole(string id)
    {
        using (var _dbContext = _dbContextFactory.CreateDbContext())
        {
            var data = await _dbContext.WebRoles.FirstOrDefaultAsync(a => a.Id == id);
            if (data == null)
            {
                return new BaseResult() { Success = false, Message = "Role not found" };
            }

            _dbContext.WebRoles.Remove(data);
            await _dbContext.SaveChangesAsync();
            return new BaseResult { Success = true };
        }
    }

    public async Task<List<WebRoles>> GetRoles()
    {
        using (var _dbContext = _dbContextFactory.CreateDbContext())
        {
            return await _dbContext.WebRoles.ToListAsync();
        }
    }
    public async Task<List<WebRoles>> GetRolesByUserId(int user_id)
    {
        using (var _dbContext = _dbContextFactory.CreateDbContext())
        {
            var role_ids = await _dbContext.WebUserRoles.Where(a => a.UserId == user_id).Select(a => a.RoleId).ToListAsync();
            return await _dbContext.WebRoles.Where(a => role_ids.Contains(a.Id)).ToListAsync();
        }
    }
}