using FashionShopMVC.Helper;
using FashionShopMVC.Models.Domain;
using FashionShopMVC.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FashionShopMVC.Controllers
{
    public class OrderController : Controller
    {
        private readonly OrderRepository orderRepository;
        private User currUser;
        // GET: OrderController
        public IActionResult Index()
        {
            var userJson = HttpContext.Session.GetString(CommonConstants.SessionUser);
            string referrerUrl = HttpContext.Request.Headers["Referer"].ToString();
            if(userJson!=null)
            {
                this.currUser = JsonConvert.DeserializeObject<User>(userJson);
            }
            var listOrder = orderRepository.GetByUserID(this.currUser.Id);
            return View();
        }

        // GET: OrderController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: OrderController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: OrderController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: OrderController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: OrderController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: OrderController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: OrderController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
