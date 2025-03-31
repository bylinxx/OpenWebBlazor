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
        private readonly WebDbContext _dbContext;
        private readonly RoleService _roleService;

        public MenuService(WebDbContext dbContext, RoleService roleService)
        {
            _dbContext = dbContext;
            _roleService = roleService;
        }

        public async Task<BaseResult> EditMenu(WebMenus menu)
        {
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
        public async Task<BaseResult> DeleteMenu(int id)
        {
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
        public ListResult<WebMenus> GetList(QueryModel<WebMenus> query)
        {
            var total = _dbContext.WebMenus.ExecuteTableQuery<WebMenus>(query).Count();
            var data = _dbContext.WebMenus.ExecuteTableQuery(query).CurrentPagedRecords(query).ToList();
            return new ListResult<WebMenus>()
            {
                Data = data,
                Total = total
            };
        }

        // 菜单列表
        public async Task<List<WebMenuTree>> GetMenuTreeAsync(int user_id)
        {
            var userData = await _dbContext.WebUsers.FirstOrDefaultAsync(a => a.Id == user_id);
            if (userData != null && userData.State != 0)
            {
                var userRoles = await _roleService.GetRolesByUserId(userData.Id);
                if (userRoles.Any(a => a.IsSuper))
                {
                    var menus = await _dbContext.WebMenus.ToListAsync();
                    return menus.Where(a => a.ParentId == 0).Select(a => new WebMenuTree()
                    {
                        ParentId = a.ParentId,
                        Id = a.Id,
                        Name = a.Name,
                        Path = a.Path,
                        Sort = a.Sort,
                        Items = menus.Where(b => b.ParentId == a.Id).Select(b => new WebMenuTree()
                        {
                            Id = b.Id,
                            Name = b.Name,
                            Path = b.Path,
                            Sort = b.Sort
                        }).OrderBy(b => b.Sort).ToList()
                    }).OrderBy(a => a.Sort).ToList();
                }
                else
                {
                    var menus = await _dbContext.WebMenus.ToListAsync();
                    var userMenuIds = await _dbContext.WebRoleMenus.Where(a => userRoles.Select(b => b.Id).Contains(a.RoleId)).Select(a => a.MenuId).ToListAsync();
                    return menus.Where(a => a.ParentId == 0 && userMenuIds.Contains(a.Id)).Select(a => new WebMenuTree()
                    {
                        ParentId = a.ParentId,
                        Id = a.Id,
                        Name = a.Name,
                        Path = a.Path,
                        Sort = a.Sort,
                        Items = menus.Where(b => b.ParentId == a.Id && userMenuIds.Contains(b.Id)).Select(b => new WebMenuTree()
                        {
                            Id = b.Id,
                            Name = b.Name,
                            Path = b.Path,
                            Sort = b.Sort
                        }).OrderBy(b => b.Sort).ToList()
                    }).OrderBy(a => a.Sort).ToList();
                }
            }
            else
            {
                return new List<WebMenuTree>();
            }
        }
        public async Task<List<WebMenuTree>> GetMenuTreeAsync()
        {
            var menus = await _dbContext.WebMenus.ToListAsync();
            return menus.Where(a => a.ParentId == 0).Select(a => new WebMenuTree()
            {
                ParentId = a.ParentId,
                Id = a.Id,
                Name = a.Name,
                Path = a.Path,
                Sort = a.Sort,
                Items = menus.Where(b => b.ParentId == a.Id).Select(b => new WebMenuTree()
                {
                    Id = b.Id,
                    ParentId = b.ParentId,
                    Name = b.Name,
                    Path = b.Path,
                    Sort = b.Sort
                }).OrderBy(b => b.Sort).ToList()
            }).OrderBy(a => a.Sort).ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<List<WebMenus>> GetMenuSelectListAsync()
        {
            var list = new List<WebMenus>()
            {
                new WebMenus(){ Id = 0, Name = "根菜单"}
            };
            var data = await _dbContext.WebMenus.Where(a => a.ParentId == 0).AsNoTracking().ToListAsync();
            list.AddRange(data);
            return list;
        }
    }
}
