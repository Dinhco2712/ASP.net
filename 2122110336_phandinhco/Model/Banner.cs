using _2122110336_phandinhco.Model;

public class Banner
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string ImageUrl { get; set; }
    public string Link { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; } // Navigation property
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}