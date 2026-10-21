using System;
using System.Collections.Generic;
using System.Text;

namespace Authentication.Application.DTO
{
    public class RefreshTokenRequestDto
    {
        public string refresh_token { get; set; } = string.Empty;
    }
}
