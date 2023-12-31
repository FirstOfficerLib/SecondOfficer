using System;
using System.Collections.Generic;
using System.Text;

namespace SecondOfficer.Lambda.Contracts
{
    public interface IUser: ILambdaModel
    {
        public string[] Roles { get; set; }
    }
}
