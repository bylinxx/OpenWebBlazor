using AntDesign;
using AntDesign.TableModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using OpenWebBlazor.Models;
using OpenWebBlazor.ViewModels;

namespace OpenWebBlazor.Services
{
    public class MenuService
    {
        private readonly IDbContextFactory<WebDbContext> _dbContextFactory;
        private readonly RoleService _roleService;

        public MenuService(IDbContextFactory<WebDbContext> dbContextFactory, RoleService roleService)
        {
            _dbContextFactory = dbContextFactory;
            _roleService = roleService;
        }

        public async Task<BaseResult> EditMenu(WebMenus menu)
        {
            await using var _dbContext = _dbContextFactory.CreateDbContext();
            try
            {
                var data = await _dbContext.WebMenus.FirstOrDefaultAsync(a => a.Id == menu.Id);
                if (data == null)
                {
                    menu.Path = menu.Path ?? String.Empty;
                    _dbContext.WebMenus.Add(menu);
                }
                else
                {
                    data.Name = menu.Name;
                    data.ParentId = menu.ParentId;
                    data.Path = menu.Path;
                    data.IsShow = menu.IsShow;
                    data.IsAuth = menu.IsAuth;
                    data.Sort = menu.Sort;
                }
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return new BaseResult() { Success = false, Message = e.Message };
            }
            return new BaseResult { Success = true };
        }
        // 删除菜单
        public async Task<BaseResult> DeleteMenu(string id)
        {
            await using var _dbContext = _dbContextFactory.CreateDbContext();
            var data = await _dbContext.WebMenus.FirstOrDefaultAsync(a => a.Id == id);
            if (data == null)
            {
                return new BaseResult() { Success = false, Message = "找不到记录" };
            }
            else
            {
                if (_dbContext.WebMenus.Any(a => a.ParentId == id))
                {
                    return new BaseResult() { Success = false, Message = "请先删除下属子菜单" };
                }
            }
            _dbContext.WebMenus.Remove(data);
            await _dbContext.SaveChangesAsync();
            return new BaseResult { Success = true };
        }

        // 菜单列表
        public async Task<ListResult<WebMenus>> GetList(QueryModel<WebMenus> query)
        {
            await using var _dbContext = _dbContextFactory.CreateDbContext();
            var total = await _dbContext.WebMenus.ExecuteTableQuery<WebMenus>(query).CountAsync();
            var data = await _dbContext.WebMenus.ExecuteTableQuery(query).CurrentPagedRecords(query).ToListAsync();
            return new ListResult<WebMenus>()
            {
                Data = data,
                Total = total
            };
        }

        // 菜单列表
        public async Task<List<WebMenuTree>> GetMenuTreeAsync(string user_id)
        {
            await using var _dbContext = _dbContextFactory.CreateDbContext();
            var userData = await _dbContext.WebUsers.FirstOrDefaultAsync(a => a.Id == user_id);
            if (userData != null && userData.State != 0)
            {
                var userRoles = await _roleService.GetRolesByUserId(userData.Id);
                if (userRoles.Any(a => a.IsSuper))
                {
                    var menus = await _dbContext.WebMenus.ToListAsync();
                    return menus.Where(a => String.IsNullOrEmpty(a.ParentId)).Select(a => new WebMenuTree()
                    {
                        ParentId = a.ParentId,
                        Id = a.Id,
                        Name = a.Name,
                        Path = a.Path,
                        Sort = a.Sort,
                        IsAuth = a.IsAuth,
                        IsShow = a.IsShow,
                        Items = menus.Where(b => b.ParentId == a.Id).Select(b => new WebMenuTree()
                        {
                            Id = b.Id,
                            Name = b.Name,
                            Path = b.Path,
                            Sort = b.Sort,
                            IsAuth = b.IsAuth,
                            IsShow = b.IsShow
                        }).OrderBy(b => b.Sort).ToList()
                    }).OrderBy(a => a.Sort).ToList();
                }
                else
                {
                    var menus = await _dbContext.WebMenus.ToListAsync();
                    var userMenuIds = await _dbContext.WebMenuRoles.Where(a => userRoles.Select(b => b.Id).Contains(a.RoleId)).Select(a => a.MenuId).ToListAsync();
                    return menus.Where(a => String.IsNullOrEmpty(a.ParentId)).Select(a => new WebMenuTree()
                    {
                        ParentId = a.ParentId,
                        Id = a.Id,
                        Name = a.Name,
                        Path = a.Path,
                        Sort = a.Sort,
                        IsAuth = a.IsAuth,
                        IsShow = a.IsShow,
                        Items = menus.Where(b => b.ParentId == a.Id && (b.IsAuth == false || userMenuIds.Contains(b.Id))).Select(b => new WebMenuTree()
                        {
                            Id = b.Id,
                            Name = b.Name,
                            Path = b.Path,
                            Sort = b.Sort,
                            IsAuth = b.IsAuth,
                            IsShow = b.IsShow
                        }).OrderBy(b => b.Sort).ToList()
                    }).Where(a => a.Items.Any()).OrderBy(a => a.Sort).ToList();
                }
            }
            else
            {
                return new List<WebMenuTree>();
            }
        }
        public async Task<List<WebMenuTree>> GetMenuTreeAsync()
        {
            await using var _dbContext = _dbContextFactory.CreateDbContext();
            var menus = await _dbContext.WebMenus.ToListAsync();
            var menu_ids = menus.Select(a => a.Id).ToList();
            var menu_roles = await _dbContext.WebMenuRoles.Where(a => menu_ids.Contains(a.MenuId)).ToListAsync();
            return menus.Where(a => String.IsNullOrEmpty(a.ParentId)).Select(a => new WebMenuTree()
            {
                ParentId = a.ParentId,
                Id = a.Id,
                Name = a.Name,
                Path = a.Path,
                Sort = a.Sort,
                IsShow = a.IsShow,
                IsAuth = a.IsAuth,
                Items = menus.Where(b => b.ParentId == a.Id).Select(b => new WebMenuTree()
                {
                    Id = b.Id,
                    ParentId = b.ParentId,
                    Name = b.Name,
                    Path = b.Path,
                    Sort = b.Sort,
                    IsShow = b.IsShow,
                    IsAuth = b.IsAuth,
                    RoleIds = menu_roles.Where(c => c.MenuId == b.Id).Select(c => c.RoleId).ToList()
                }).OrderBy(b => b.Sort).ToList()
            }).OrderBy(a => a.Sort).ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<List<WebMenus>> GetMenuSelectListAsync()
        {
            await using var _dbContext = _dbContextFactory.CreateDbContext();
            var list = new List<WebMenus>()
            {
                new WebMenus(){ Id = String.Empty, Name = "根菜单"}
            };
            var data = await _dbContext.WebMenus.Where(a => String.IsNullOrEmpty(a.ParentId)).AsNoTracking().ToListAsync();
            list.AddRange(data);
            return list;
        }
        public async Task<BaseResult> SetRoles(MenuRole model)
        {
            await using var _dbContext = _dbContextFactory.CreateDbContext();
            var menu_data = await _dbContext.WebMenus.FirstOrDefaultAsync(a => a.Id == model.MenuId);
            if (menu_data == null)
            {
                return new BaseResult() { Success = false, Message = "菜单不存在" };
            }
            else
            {
                var menu_roles = await _dbContext.WebMenuRoles.Where(a => a.MenuId == menu_data.Id).ToListAsync();
                var delete_roles = menu_roles.Where(a => !model.RoleIds.Contains(a.RoleId)).ToList();
                foreach (var item in delete_roles)
                {
                    _dbContext.WebMenuRoles.Remove(item);
                }
                var add_roles = model.RoleIds.Where(a => !menu_roles.Select(b => b.RoleId).Contains(a)).ToList();
                foreach (var item in add_roles)
                {
                    _dbContext.WebMenuRoles.Add(new WebMenuRoles()
                    {
                        MenuId = menu_data.Id,
                        RoleId = item
                    });
                }
                await _dbContext.SaveChangesAsync();
                return new BaseResult() { Success = true };
            }
        }
    }
}
