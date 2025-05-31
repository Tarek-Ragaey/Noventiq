using System;
using System.Collections.Generic;

namespace Noventiq.Application.IServices.Models
{
    public class AuthResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        public Dictionary<string, string> Errors { get; set; }

        public AuthResponse()
        {
            Roles = new List<string>();
            Errors = new Dictionary<string, string>();
        }
    }
} 