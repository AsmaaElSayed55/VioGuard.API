using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos.User
{
    public record AuthResponseDto(
        string Token,
        DateTime Expiration,
        UserDto User
    );
}
