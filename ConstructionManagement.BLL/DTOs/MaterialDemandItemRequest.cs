using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstructionManagement.BLL.DTOs
{
    public class MaterialDemandItemRequest
    {
        public string MaterialName { get; set; } = string.Empty;

        public decimal RequestedQuantity { get; set; }

        public string Unit { get; set; } = string.Empty;
        public decimal? ApprovedQuantity { get; set; }

        public decimal? UnitRate { get; set; }
    }
}
