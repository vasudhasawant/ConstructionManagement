using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstructionManagement.BLL.DTOs
{
    public class UpdateMaterialDemandStatusRequest
    {
        public string Status { get; set; } = string.Empty;

        public string? Remarks { get; set; }

        public List<ApproveMaterialDemandItemRequest> Items { get; set; }
            = new List<ApproveMaterialDemandItemRequest>();
    }

    public class ApproveMaterialDemandItemRequest
    {
        public long DemandItemId { get; set; }

        public decimal ApprovedQuantity { get; set; }
    }
}