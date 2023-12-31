


using SecondOfficer.Generator.Attributes;
using SecondOfficer.Generator.Enums;

namespace SecondOfficer.Tests.Generator.Models
{
    [RestConfig(Actions.All, "Admin")]
    [RestConfig(Actions.Read, "*")]
    public class Page : LambdaModel
    {
        public long BookId { get; set; }    
        public int? PageNumber { get; set; }
        public string Content { get; set; } = string.Empty;
        public Book? Book { get; set; }
      
    }
}
