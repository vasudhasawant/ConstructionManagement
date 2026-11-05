using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstructionManagement.BLL.DTOs
{
    public class RegisterRequest
    {
        public string FullName { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string? Email { get; set; }

        public string Password { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;
    }
}
