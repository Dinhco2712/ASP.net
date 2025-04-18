
namespace _2122110336_phandinhco.Dto
{
    public class ProductDto
    {
        
        public int Id { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string Description { get; set; }
        public int quantity { get; set; }


        public DateTime createAt { get; set; }
        public DateTime updateAt { get; set; }
        public string avatar { get; set; }

        public decimal Price { get; internal set; }
    }

}
