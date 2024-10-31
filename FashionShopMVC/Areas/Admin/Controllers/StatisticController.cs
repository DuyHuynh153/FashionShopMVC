using FashionShopMVC.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FashionShopMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class StatisticsController : Controller
    {
        private readonly IStatisticRepository _statisticRepository;

        public StatisticsController(IStatisticRepository statisticRepository)
        {
            _statisticRepository = statisticRepository;
        }

        [HttpGet]
        public IActionResult GetRevenueStatistic(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var revenueStatistics = _statisticRepository.GetRevenueStatistic(fromDate, toDate);

                return Ok(revenueStatistics);
            }
            catch
            {
                return BadRequest();
            }

        }
    }
}
