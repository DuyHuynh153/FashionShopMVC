using FashionShopMVC.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FashionShopMVC.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ICustomerRepository _customerRepository;


        public CustomersController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

    }
}
