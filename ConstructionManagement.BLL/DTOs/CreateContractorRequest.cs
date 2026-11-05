using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstructionManagement.BLL.DTOs
{
    public class CreateContractorRequest
    {
        public int UserId { get; set; }

        public string ContractorCompany { get; set; } = string.Empty;
    }
}
