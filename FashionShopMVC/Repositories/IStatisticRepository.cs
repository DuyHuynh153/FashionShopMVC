using FashionShop.Models.ViewModel;
using FashionShopMVC.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace FashionShopMVC.Repositories
{
    public interface IStatisticRepository
    {
        public Task<IEnumerable<RevenueStatisticViewModel>> GetRevenueStatistic(DateTime fromDate, DateTime toDate);
    }
    public class StatisticRepository : IStatisticRepository
    {
        private readonly FashionShopDBContext _fashionShopDBContext;

        public StatisticRepository(FashionShopDBContext fashionShopDBContext)
        {
            _fashionShopDBContext = fashionShopDBContext;
        }
        public async Task<IEnumerable<RevenueStatisticViewModel>> GetRevenueStatistic(DateTime fromDate, DateTime toDate)
        {
            var query = _fashionShopDBContext.Set<RevenueStatisticViewModel>()
                .FromSqlRaw("EXEC GetRevenueStatistic @fromDate, @toDate",
                    new SqlParameter("@fromDate", fromDate),
                    new SqlParameter("@toDate", toDate));
            var statistic = await query.ToListAsync();
            return statistic;
        }
    }
}
