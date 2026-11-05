
using ConstructionManagement.BLL.DTOs;
using ConstructionManagement.BLL.DTOs.Menu;
using ConstructionManagement.DAL.Context;
using Microsoft.EntityFrameworkCore;

namespace ConstructionManagement.BLL.Services
{
    public class MenuService
    {
        private readonly ConstructionDbContext _context;

        public MenuService(ConstructionDbContext context)
        {
            _context = context;
        }

        public async Task<MenuResponse> GetMenuAsync(string role)
        {
            if (string.IsNullOrWhiteSpace(role))
            {
                throw new ArgumentException("User role is required.");
            }

            var records = await _context.MenuRolePermissions
                .AsNoTracking()
                .Include(x => x.Menu)
                .Where(x =>
                    x.RoleName == role &&
                    x.CanView &&
                    x.Menu.IsActive)
                .OrderBy(x => x.Menu.SortOrder)
                .Select(x => new MenuItemResponse
                {
                    MenuId = x.Menu.MenuId,
                    MenuName = x.Menu.MenuName,
                    Icon = x.Menu.Icon,
                    Route = x.Menu.Route,
                    ParentMenuId = x.Menu.ParentMenuId,
                    SortOrder = x.Menu.SortOrder,
                    CanCreate = x.CanCreate,
                    CanEdit = x.CanEdit,
                    CanDelete = x.CanDelete
                })
                .ToListAsync();

            // Create lookup for all permitted menu items
            var menuLookup = records.ToDictionary(x => x.MenuId);

            var rootMenus = new List<MenuItemResponse>();

            foreach (var menu in records)
            {
                if (menu.ParentMenuId.HasValue &&
                    menuLookup.TryGetValue(
                        menu.ParentMenuId.Value,
                        out var parent))
                {
                    parent.Children.Add(menu);
                }
                else
                {
                    rootMenus.Add(menu);
                }
            }

            // Sort child menus
            SortChildren(rootMenus);

            return new MenuResponse
            {
                Role = role,
                Menu = rootMenus
            };
        }

        private static void SortChildren(
            List<MenuItemResponse> menus)
        {
            menus.Sort((a, b) =>
                a.SortOrder.CompareTo(b.SortOrder));

            foreach (var menu in menus)
            {
                SortChildren(menu.Children);
            }
        }
    }
}
