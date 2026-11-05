using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstructionManagement.BLL.DTOs
{
    public class WorkApprovalRequest
    {
        public string Decision { get; set; } = string.Empty;

        public string? Remarks { get; set; }
    }
}
