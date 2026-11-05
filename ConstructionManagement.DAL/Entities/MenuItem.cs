using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ConstructionManagement.DAL.Entities
{
    public class MenuItem
    {
        public int MenuId { get; set; }

        public string MenuName { get; set; } = null!;

        public string? Icon { get; set; }

        public string? Route { get; set; }

        public int? ParentMenuId { get; set; }

        public int SortOrder { get; set; }

        public bool IsActive { get; set; }

        public virtual MenuItem? ParentMenu { get; set; }

        public virtual ICollection<MenuItem> ChildMenus { get; set; }
            = new List<MenuItem>();

        public virtual ICollection<MenuRolePermission> RolePermissions { get; set; }
            = new List<MenuRolePermission>();
    }
}
