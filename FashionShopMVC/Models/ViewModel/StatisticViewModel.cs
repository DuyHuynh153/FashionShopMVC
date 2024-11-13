namespace FashionShopMVC.Models.ViewModel
{
    public class StatisticViewModel
    {
        public int CountOrder { get; set; }
        public int CountProduct { get; set; }
        public int CountCustomer { get; set; }
        public int CountVoucher { get; set; }
        public IEnumerable<RevenueStatisticViewModel> listRevenueStatistic { get; set; }
    }
}

