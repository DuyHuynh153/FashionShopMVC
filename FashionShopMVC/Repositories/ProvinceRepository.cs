﻿
using FashionShopMVC.Data;
using FashionShopMVC.Models.ViewModel;


namespace FashionShopMVC.Repositories
{
    public interface IProvinceRepository
    {
        public List<ProvinceViewModel> GetAll();
    }
    public class ProvinceRepository : IProvinceRepository
    {
        FashionShopDBContext _fashionShopDBContext;
        public ProvinceRepository(FashionShopDBContext fashionShopDBContext)
        {
            _fashionShopDBContext = fashionShopDBContext;
        }

        public List<ProvinceViewModel> GetAll()
        {
            var province = _fashionShopDBContext.Provinces.Select(p => new ProvinceViewModel()
            {
                ID = p.ID,
                Name = p.Name,
            }).ToList();

            return province;
        }
    }
}
