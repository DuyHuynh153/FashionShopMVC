namespace FashionShopMVC.Models.Domain
{
    public class Revenue
    {
        public int Id { get; set; }
        public DateTime date { get; set; }
        public int OrderID { get; set; }
        public int CustomerID {  get; set; }
        public float Revenues{  get; set; }

    }
}
