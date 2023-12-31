using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SecondOfficer.Lambda.Contracts;

namespace SecondOfficer.Tests.Generator.Models
{
    public abstract class LambdaModel : ILambdaModel
    {
        public virtual long Id { get; set; }
    }
}
