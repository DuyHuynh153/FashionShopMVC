using FashionShopMVC.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FashionShopMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]")]
    [Authorize]
    public class StatisticsController : Controller
    {
        private readonly IStatisticRepository _statisticRepository;

        public StatisticsController(IStatisticRepository statisticRepository)
        {
            _statisticRepository = statisticRepository;
        }
        [HttpGet]
        
        public async Task<IActionResult> Index(DateTime? fromDate, DateTime? toDate, string revenueType) {
            var revenueStatistics = await _statisticRepository.GetStatisticView(fromDate, toDate, revenueType);
            return View(revenueStatistics);
        }

        [HttpGet]
        [Route("GetRevenueStatistic")]
        public async Task<IActionResult> GetRevenueStatistic(DateTime? fromDate, DateTime? toDate, string revenueType)
        {
            try
            {
                var revenueStatistics = await _statisticRepository.GetStatisticView(fromDate, toDate, revenueType);
                return PartialView("_RevenueStatistics", revenueStatistics);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("GetRevenueStatisticList")]
        public async Task<IActionResult> GetRevenueStatisticList(DateTime? fromDate, DateTime? toDate, string revenueType)
        {
            try
            {
                var revenueStatistics = await _statisticRepository.GetStatisticView(fromDate, toDate, revenueType);

                return Json(revenueStatistics);
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
