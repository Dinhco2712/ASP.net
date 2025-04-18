namespace _2122110336_phandinhco.Dto
{
    public class OrderDTO
    {
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderDetailDTO> OrderDetails { get; set; }
    }

}
