using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ConstructionManagement.BLL.DTOs.Menu
{
    public class MenuItemResponse
    {
        public int MenuId { get; set; }

        public string MenuName { get; set; } = string.Empty;

        public string? Icon { get; set; }

        public string? Route { get; set; }

        public int? ParentMenuId { get; set; }

        public int SortOrder { get; set; }

        public bool CanCreate { get; set; }

        public bool CanEdit { get; set; }

        public bool CanDelete { get; set; }

        public List<MenuItemResponse> Children { get; set; }
            = new List<MenuItemResponse>();
    }
}
