using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noventiq.Domain.Entites
{
    public class RoleTranslation
    {
        public int Id { get; set; }
        public string RoleId { get; set; }
        public string LanguageKey { get; set; }
        public string TranslatedName { get; set; }

        // Navigation property
        public virtual IdentityRole Role { get; set; }
    }
}
