using System.Security.Cryptography;
using AntDesign.TableModels;
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
        var userData = await _dbContext.WebUsers.FirstOrDefaultAsync(a => a.UserName == username && a.State == 1);
        if (userData == null)
        {
            result.Success = false;
            result.Message = "用户不存在或禁止登录";
        }
        else if (userData.Password != password)
        {
            result.Success = false;
            result.Message = "密码错误";
        }
        else
        {
            result.Success = true;
            result.Name = userData.UserName;
            result.Id = userData.Id;
        }

        return result;
    }

    public async Task<ListResult<UserList>> GetList(QueryModel? query)
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
        var data = users.Select(a => new UserList()
        {
            Id = a.Id,
            Name = a.UserName,
            State = a.State,
            Roles = user_roles.FirstOrDefault(b => b.user_id == a.Id)?.roles.Select(b =>
                new UserList.RolesViewModel()
                {
                    Id = b.Id,
                    Name = b.Name
                }).ToList()
        }).ToList();

        return new ListResult<UserList>()
        {
            Data = data.Skip((query.PageIndex - 1) * query.PageSize).Take(query.PageSize).ToList(),
            Total = data.Count
        };
    }
    public async Task<BaseResult> Edit(WebUsers model)
    {
        try
        {
            var _model = await _dbContext.WebUsers.FirstOrDefaultAsync(a => a.Id == model.Id);
            if (_model == null)
            {
                _dbContext.WebUsers.Add(_model);
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
}