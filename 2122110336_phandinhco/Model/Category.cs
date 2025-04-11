using System.ComponentModel.DataAnnotations;

namespace _2122110336_phandinhco.Model
{
    public class Category
    {
        [Key]
        public int id { get; set; }
        public string name { get; set; }
        public string avatar { get; set; }

        public DateTime craeteAt { get; set; }
        public DateTime updateAt { get; set; }

        public List<Product> products { get; set; } = new List<Product>();
    }

}
