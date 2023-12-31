using System;
using System.Collections.Generic;
using System.Text;

namespace SecondOfficer.Lambda.Contracts
{
    public interface IUserService
    {
        bool IsInRoles(string[] roles);
    }
}
