using FashionShop.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionShop.Service.Service
{
    public interface IEmailAuthService
    {
        void SendAuthEmail (Message message); 
    }
}
