using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noventiq.Application.IServices.Models
{
    public class RoleModel
    {
        public string Name { get; set; } = string.Empty;
        public Dictionary<string, string> Translations { get; set; } = new Dictionary<string, string>();
    }

    public class RoleUpdateModel
    {
        public string NewName { get; set; } = string.Empty;
        public Dictionary<string, string> Translations { get; set; } = new Dictionary<string, string>();
    }
}
