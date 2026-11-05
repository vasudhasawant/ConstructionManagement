using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstructionManagement.BLL.DTOs
{
    public class AssignSiteUserRequest
    {
        public int SiteId { get; set; }

        public int UserId { get; set; }
    }
}
