using SecondOfficer.Generator.Attributes;
using SecondOfficer.Generator.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecondOfficer.Tests.Generator.Models
{
    [RestConfig(Actions.All, "*")]
    public class BookCover : LambdaModel
    {
        public int? TypeId { get; set; }
        public string? Summary { get; set; }
        public long BookId { get; set; }
        public Book? Book { get; set; } 


    }
}
