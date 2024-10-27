namespace FashionShopMVC.Models.DTO.VoucherDTO
{
    public class GetVoucherDTO
    {
        public int id { get; set; }
        public string discountCode { get; set; }
        public bool discountAmount { get; set; }
        public bool discountPercentage { get; set; }
        public double discountValue { get; set; }
        public double minimumValue { get; set; }
        public int quantity { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public string? describe { get; set; }
        public bool status { get; set; }
        public DateTime createdDate { get; set; }
        public string createdBy { get; set; }
        public DateTime? updatedDate { get; set; }
        public string? updatedBy { get; set; }
    }
}
