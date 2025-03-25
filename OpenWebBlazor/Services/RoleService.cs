using Microsoft.EntityFrameworkCore;
using OpenWebBlazor.Models;
using OpenWebBlazor.ViewModels;

namespace OpenWebBlazor.Services;

public class RoleService
{
    private readonly WebDbContext _dbContext;

    public RoleService(WebDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<BaseResult> EditRole(WebRoles role)
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

    public async Task<BaseResult> DeleteRole(string id)
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

    public async Task<List<WebRoles>> GetRoles()
    {
        return await _dbContext.WebRoles.ToListAsync();
    }
}