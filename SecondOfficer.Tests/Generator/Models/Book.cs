using SecondOfficer.Generator.Attributes;
using SecondOfficer.Generator.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecondOfficer.Tests.Generator.Models
{
    [RestConfig(Actions.All, "Admin")]
    [RestConfig(Actions.Write | Actions.Read, "User")]
    public class Book : LambdaModel
    {
        public string? Title { get; set; }
        public int PageCount {get; set; }
        public DateTime Published { get; set; }
        public string? Isbn { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
 
        public long? BookCoverId { get; set; }

        //one to one
        public BookCover? BookCover { get; set; }
        //one to many
        public IList<Page> Pages { get; set; } = new List<Page>();
        //many to many
        public IList<Author> Authors { get; set; } = new List<Author>();
        //many to many to self
        public IList<Book> RelatedBooks { get; set; } = new List<Book>();

    }
}
