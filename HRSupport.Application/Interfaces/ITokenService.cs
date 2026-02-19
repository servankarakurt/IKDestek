using HRSupport.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Text;

namespace HRSupport.Application.Interfaces
{
    public interface ITokenService
    {
      string GenerateToken(User user);
    }
}
