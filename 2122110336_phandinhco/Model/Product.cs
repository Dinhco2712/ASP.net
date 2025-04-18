using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace _2122110336_phandinhco.Model
{
    public class Product
    {
        [Key]
        public int id { get; set; }
        public string name { get; set; }
        public string avatar { get; set; }
        public string description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal price { get; set; }

        public int CategoryId { get; set; }
        [JsonIgnore]
        public Category? Category { get; set; }

        public int quantity { get; set; } 

        public DateTime createAt { get; set; }
        public DateTime updateAt { get; set; }
    }


}
