using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos
{
    public record UserRegisterDto
    {
        public string Email { get; init; }  
        public string Password { get; init; }
        public string FullName { get; init; }
    }
}
