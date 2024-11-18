using FashionShopMVC.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionShopMVC.Service.Service
{
    public interface IEmailAuthService
    {
        void SendAuthEmail (Message message); 
    }
}
