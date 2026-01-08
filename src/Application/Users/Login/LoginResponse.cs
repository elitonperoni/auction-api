using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.Login;

public sealed record LoginResponse
{
    public string Token { get; set; }
    public string UserName { get; set; }
}
