using System;
using System.Collections.Generic;

namespace Noventiq.Application.IServices.Models
{
    public class UserListDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }
} 