using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ConstructionManagement.DAL.Entities
{
    public class MenuRolePermission
    {
        public int MenuId { get; set; }

        public string RoleName { get; set; } = null!;

        public bool CanView { get; set; }

        public bool CanCreate { get; set; }

        public bool CanEdit { get; set; }

        public bool CanDelete { get; set; }

        public virtual MenuItem Menu { get; set; } = null!;
    }
}
