using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Profiles.Entities
{
    public class EditProfileEntity
    {
        public IFormFile? File { get; set; }
        public string? DisplayName { get; set; }
        public string? Bio { get; set; }
    }
}
