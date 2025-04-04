using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2122110336_phandinhco.Model
{
    public class Product
    {
        [Key]
        public int id { get; set; }
        public string name { get; set; }
        public string avatar { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal price { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public DateTime createAt { get; set; }
        public DateTime updateAt { get; set; }
    }
}
