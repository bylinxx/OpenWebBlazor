using AntDesign.TableModels;
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

    public async Task<BaseResult> Edit(WebRoles role)
    {
        try
        {
            var data = await _dbContext.WebRoles.FirstOrDefaultAsync(a => a.Id == role.Id);
            if (data == null)
            {
                role.Id = Guid.NewGuid().ToString("N");
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

    public async Task<BaseResult> Delete(string id)
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

    public async Task<ListResult<WebRoles>> GetList(QueryModel<WebRoles> query)
    {
        var data = await _dbContext.WebRoles.ExecuteTableQuery(query).CurrentPagedRecords(query).ToListAsync();
        var total = await _dbContext.WebRoles.ExecuteTableQuery(query).CountAsync();
        return new ListResult<WebRoles>()
        {
            Data = data,
            Total = total
        };
    }
    public async Task<List<WebRoles>> GetRolesByUserId(int user_id)
    {
        var role_ids = await _dbContext.WebUserRoles.Where(a => a.UserId == user_id).Select(a => a.RoleId).ToListAsync();
        return await _dbContext.WebRoles.Where(a => role_ids.Contains(a.Id)).ToListAsync();
    }
    public async Task<List<WebRoles>> GetRoleSelectList()
    {
        return await _dbContext.WebRoles.AsNoTracking().ToListAsync();
    }
    public async Task<BaseResult> CheckAuth(int user_id, string path)
    {
        var userData = await _dbContext.WebUsers.FirstOrDefaultAsync(a => a.Id == user_id);
        if (userData != null && userData.State != 0)
        {
            var userRoles = await GetRolesByUserId(userData.Id);
            if (userRoles.Any(a => a.IsSuper))
            {
                return new BaseResult { Success = true };
            }

            var menu = await _dbContext.WebMenus.FirstOrDefaultAsync(a => a.Path == path);
            if (menu == null)
            {
                return new BaseResult { Success = false, Message = "Path not found" };
            }

            if (!menu.IsAuth)
            {
                return new BaseResult { Success = true };
            }

            var role_ids = await _dbContext.WebMenuRoles.Where(a => a.MenuId == menu.Id).Select(a => a.RoleId).ToListAsync();
            if (role_ids.Any(a => userRoles.Any(b => b.Id == a)))
            {
                return new BaseResult { Success = true };
            }
        }
        return new BaseResult { Success = false, Message = "No permission" };
    }
}