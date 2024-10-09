namespace FashionShopMVC.Models.DTO
{
    public class CategoryDTO
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
    public class CategoryByIdDTO
    {
        public string Name { get; set; }
    }
    public class AddCategoryRequestDTO
    {
        public string Name { get; set; }
    }
}
