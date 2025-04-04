using System.Security.Cryptography;
using AntDesign.TableModels;
using Microsoft.EntityFrameworkCore;
using OpenWebBlazor.Models;
using OpenWebBlazor.ViewModels;

namespace OpenWebBlazor.Services;

public class UserService
{
    private readonly IDbContextFactory<WebDbContext> _dbContextFactory;

    public UserService(IDbContextFactory<WebDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<LoginResult> Login(string username, string password)
    {
        await using var _dbContext = _dbContextFactory.CreateDbContext();
        var result = new LoginResult();
        var userData = await _dbContext.WebUsers.FirstOrDefaultAsync(a => a.UserName == username && a.State == 1);
        if (userData == null)
        {
            result.Success = false;
            result.Message = "用户不存在或禁止登录";
        }
        else if (userData.Password != Common.Md5Helper.ComputeMd5(password))
        {
            result.Success = false;
            result.Message = "密码错误";
        }
        else
        {
            result.Success = true;
            result.UserName = userData.UserName;
            result.UserId = userData.Id;
        }

        return result;
    }

    public async Task<ListResult<UserListItem>> GetList(QueryModel? query)
    {
        await using var _dbContext = _dbContextFactory.CreateDbContext();
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
        var data = users.Select(a => new UserListItem()
        {
            Id = a.Id,
            Name = a.UserName,
            State = a.State,
            Roles = user_roles.FirstOrDefault(b => b.user_id == a.Id)?.roles.Select(b =>
                new UserListItem.RolesViewModel()
                {
                    Id = b.Id,
                    Name = b.Name
                }).ToList() ?? new List<UserListItem.RolesViewModel>()
        }).ToList();

        return new ListResult<UserListItem>()
        {
            Data = data.Skip((query.PageIndex - 1) * query.PageSize).Take(query.PageSize).ToList(),
            Total = data.Count
        };
    }

    public async Task<BaseResult> Edit(WebUsers model)
    {
        await using var _dbContext = _dbContextFactory.CreateDbContext();
        try
        {
            var _model = await _dbContext.WebUsers.FirstOrDefaultAsync(a => a.Id == model.Id);
            if (_model == null)
            {
                _dbContext.WebUsers.Add(model);
            }
            else
            {
                _model.UserName = model.UserName;
                _model.State = model.State;
                if (!String.IsNullOrEmpty(model.Password))
                {
                    _model.Password = model.Password;
                }
            }

            await _dbContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            return new BaseResult() { Success = false, Message = e.Message };
        }

        return new BaseResult() { Success = true };
    }

    public async Task<BaseResult> Delete(string id)
    {
        await using var _dbContext = _dbContextFactory.CreateDbContext();
        var data = await _dbContext.WebUsers.FirstOrDefaultAsync(a => a.Id == id);
        if (data == null)
        {
            return new BaseResult() { Success = false, Message = "用户不存在" };
        }
        else
        {
            _dbContext.WebUsers.Remove(data);

            var user_roles = _dbContext.WebUserRoles.Where(a => a.UserId == data.Id).ToList();
            _dbContext.WebUserRoles.RemoveRange(user_roles);

            await _dbContext.SaveChangesAsync();
            return new BaseResult() { Success = true };
        }
    }

    public async Task<BaseResult> SetPassword(UserPassword model)
    {
        await using var _dbContext = _dbContextFactory.CreateDbContext();
        var data = await _dbContext.WebUsers.FirstOrDefaultAsync(a => a.Id == model.UserId);
        if (data == null)
        {
            return new BaseResult() { Success = false, Message = "用户不存在" };
        }
        else
        {
            data.Password = Common.Md5Helper.ComputeMd5(model.Password);
            await _dbContext.SaveChangesAsync();
            return new BaseResult() { Success = true };
        }
    }
    public async Task<BaseResult> SetRoles(UserRole model)
    {
        await using var _dbContext = _dbContextFactory.CreateDbContext();
        var user = await _dbContext.WebUsers.FirstOrDefaultAsync(a => a.Id == model.UserId);
        if (user == null)
        {
            return new BaseResult() { Success = false, Message = "用户不存在" };
        }
        else
        {
            var user_roles = _dbContext.WebUserRoles.Where(a => a.UserId == model.UserId).ToList();
            _dbContext.WebUserRoles.RemoveRange(user_roles);
            foreach (var role_id in model.RoleIds)
            {
                _dbContext.WebUserRoles.Add(new WebUserRoles()
                {
                    UserId = model.UserId,
                    RoleId = role_id
                });
            }
            await _dbContext.SaveChangesAsync();
            return new BaseResult() { Success = true };
        }
    }
}