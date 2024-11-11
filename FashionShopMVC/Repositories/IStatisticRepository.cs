using FashionShopMVC.Data;
using FashionShopMVC.Models.ViewModel;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace FashionShopMVC.Repositories
{
    public interface IStatisticRepository
    {
        public Task<StatisticViewModel> GetStatisticView(DateTime? fromDate, DateTime? toDate, string revenueType);

    }
    public class StatisticRepository : IStatisticRepository
    {
        private readonly FashionShopDBContext _fashionShopDBContext;

        public StatisticRepository(FashionShopDBContext fashionShopDBContext)
        {
            _fashionShopDBContext = fashionShopDBContext;
        }

        public async Task<StatisticViewModel> GetStatisticView(DateTime? fromDate, DateTime? toDate, string revenueType)
        {
            if (!fromDate.HasValue)
            {
                fromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            }
            if (!toDate.HasValue)
            {
                toDate = fromDate.Value.AddMonths(1).AddDays(-1);
            }
            if (revenueType.IsNullOrEmpty())
            {
                revenueType = "day";
            }
            var statistic = new StatisticViewModel
            {
                CountOrder = CountCustomer(),
                CountCustomer = CountCustomer(),
                CountProduct = CountProduct(),
                CountVoucher = CountVoucher(),
                listRevenueStatistic = await GetRevenueStatistic(fromDate, toDate, revenueType)
            };
            return statistic;
        }

        private async Task<IEnumerable<RevenueStatisticViewModel>> GetRevenueStatistic(DateTime? fromDate, DateTime? toDate, string revenueType) 
        {
            var query = await _fashionShopDBContext.Set<RevenueStatisticViewModel>()
                .FromSqlRaw("EXEC GetRevenueStatistic @fromDate, @toDate, @period",
                    new SqlParameter("@fromDate", fromDate),
                    new SqlParameter("@toDate", toDate),
                    new SqlParameter("@period", revenueType)).ToListAsync();
            return query;
        }

        private async Task<IEnumerable<RevenueStatisticViewYear>> GetRevenueSatisticYear(DateTime? fromDate, DateTime? toDate, string revenueType)
        {
            var query = await _fashionShopDBContext.Set<RevenueStatisticViewYear>()
                .FromSqlRaw("EXEC GetRevenueStatistic @fromDate, @toDate, @period",
                    new SqlParameter("@fromDate", fromDate),
                    new SqlParameter("@toDate", toDate),
                    new SqlParameter("@period", revenueType)).ToListAsync();
            return query;
        }

        private int CountCustomer()
        {
            var customerRoleId = _fashionShopDBContext.Roles
                                .Where(r => r.Name == "Khách Hàng")
                                .Select(r => r.Id)
                                .FirstOrDefault();
            var countCustomer = _fashionShopDBContext.UserRoles
                                .Count(ur => ur.RoleId == customerRoleId);
            return countCustomer;
        }

        private int CountProduct()
        {
            var countProduct = _fashionShopDBContext.Products.Count();
            return countProduct;
        }

        private int CountOrder() 
        { 
            var countOrder = _fashionShopDBContext.Orders.Count();
            return countOrder;
        }

        private int CountVoucher()
        {
            var countVoucher = _fashionShopDBContext.Vouchers.Count();
            return countVoucher;
        }
    }
}
