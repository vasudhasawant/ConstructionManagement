using ConstructionManagement.BLL.DTOs.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstructionManagement.BLL.DTOs
{
    public class MenuResponse
    {
        public string Role { get; set; } = string.Empty;

        public List<MenuItemResponse> Menu { get; set; }
            = new List<MenuItemResponse>();
    }
}
