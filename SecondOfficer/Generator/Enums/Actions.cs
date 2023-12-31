using System;
using System.Collections.Generic;
using System.Text;

namespace SecondOfficer.Generator.Enums
{
    [Flags]
    public enum Actions
    {
        Read = 1,
        Write = 2,
        Delete = 4,
        All = 7
    }   
}
