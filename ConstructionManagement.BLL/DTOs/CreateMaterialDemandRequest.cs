using System;
using System.Collections.Generic;

namespace ConstructionManagement.BLL.DTOs
{
    public class CreateMaterialDemandRequest
    {
        public int SiteId { get; set; }

        public DateOnly? RequiredDate { get; set; }

        public string? Remarks { get; set; }

        public List<MaterialDemandItemRequest> Items { get; set; }
            = new List<MaterialDemandItemRequest>();
    }
}