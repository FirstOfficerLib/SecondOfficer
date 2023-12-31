
using SecondOfficer.Generator.Attributes;
using SecondOfficer.Generator.Enums;

namespace SecondOfficer.Tests.Generator.Models
{
    [RestConfig(Actions.Read, "Admin,User")]
    public class Author : LambdaModel
    {
        
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }
        public List<Book> Books { get; set; } = new();
      
    }

    
}
