using FashionShopMVC.Models.DTO.OrderDTO;
using FashionShopMVC.Data;
using FashionShopMVC.Helper;
using FashionShopMVC.Models.Domain;
using FashionShopMVC.Models.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace FashionShopMVC.Repositories
{
    public interface IOrderRepository
    {
        public AdminPaginationSet<AdminGetOrderDTO> GetAll(int page, int pageSize, int? typePayment, int? searchByID, string? searchByName, string? searchBySDT);
        public Task<GetOrderDetailDTO> GetOrderDetail(int id);
        public Task<GetOrderByIdDTO> GetOrderById(int id);
        public Task<IEnumerable<GetOrderByUserIdDTO>> GetByUserID(string userID);
        public Task<GetOrderByUserIdDTO> GetNewByUserID(string userID);
        public Task<GetOrderDTO> Create(CreateOrderDTO createOrderDTO);
        public Task<List<ShoppingCartViewModel>> Cancel(int id);
        public double TotalPayment(GetOrderByUserIdDTO getOrderByUserIdDTO);
        public double AdminTotalPayment(int id);
        public bool PayOnline(int id);
        public Task<int> Count();
    }
    public class OrderRepository : IOrderRepository
    {
        private readonly FashionShopDBContext _fashionShopDBContext;

        public OrderRepository(FashionShopDBContext fashionShopDBContext)
        {
            _fashionShopDBContext = fashionShopDBContext;
        }

        public AdminPaginationSet<AdminGetOrderDTO> GetAll(int page, int pageSize, int? typePayment, int? searchByID, string? searchByName, string? searchBySDT)
        {
            var listOrdersDomain = _fashionShopDBContext.Orders.AsQueryable();
            // Lọc theo hình thức thanh toán
            if (typePayment.HasValue)
            {
                listOrdersDomain = listOrdersDomain.Where(o => o.TypePayment == typePayment.Value);
            }

            // Lọc theo ID
            if (searchByID.HasValue)
            {
                listOrdersDomain = listOrdersDomain.Where(u => u.ID == searchByID.Value);
            }

            // Lọc theo tên
            if (!string.IsNullOrEmpty(searchByName))
            {
                listOrdersDomain = listOrdersDomain.Where(u => u.FullName.Contains(searchByName));
            }

            // Lọc theo số điện thoại
            if (!string.IsNullOrEmpty(searchBySDT))
            {
                listOrdersDomain = listOrdersDomain.Where(u => u.PhoneNumber.Contains(searchBySDT));
            }

            // Chuyển đổi kết quả thành DTO
            var listOrdersDTO = listOrdersDomain.Select(order => new AdminGetOrderDTO 
            {
                ID = order.ID,
                FullName = order.FullName,
                Email = order.Email,
                PhoneNumber = order.PhoneNumber,
                Address = order.Address,
                OrderDate = order.OrderDate,
                TypePayment = order.TypePayment,
                Status = order.Status,
                DeliveryFee = order.DeliveryFee,
                Voucher = order.Voucher,
                TotalPayment = order.OrderDetails.Sum(item => item.Quantity * item.Price)
            }).OrderByDescending(u => u.ID).ToList();


            var totalCount = listOrdersDTO.Count();
            var listOrderPagination = listOrdersDTO.Skip(page * pageSize).Take(pageSize).ToList();

            AdminPaginationSet<AdminGetOrderDTO> orderPaginationSet = new AdminPaginationSet<AdminGetOrderDTO>
            {
                List = listOrderPagination,
                Page = page,
                TotalCount = totalCount,
                PagesCount = (int)Math.Ceiling((decimal)totalCount / pageSize),
            };

            return orderPaginationSet;
        }

        public async Task<IEnumerable<GetOrderByUserIdDTO>> GetByUserID(string userID)
        {
            var listOrderByUserIdDTO = await _fashionShopDBContext.Orders
                .Where(order => order.UserID == userID)
                .Select(order => new GetOrderByUserIdDTO
                {
                    ID = order.ID,
                    FullName = order.FullName,
                    PhoneNumber = order.PhoneNumber,
                    ProvinceName = order.Province.Name,
                    DistrictName = order.District.Name,
                    WardName = order.Ward.Name,
                    Address = order.Address,
                    DeliveryFee = order.DeliveryFee,
                    OrderDate = order.OrderDate,
                    Note = order.Note,
                    Status = order.Status,
                    TypePayment = order.TypePayment,

                    Voucher = order.Voucher,
                    UserID = order.UserID,

                    //OrderDetails = order.OrderDetails.ToList()
                    OrderDetails = order.OrderDetails.Select(detail => new OrderDetail
                    {
                        ProductID = detail.ProductID,
                        //ProductName = detail.Product.Name,
                        Quantity = detail.Quantity,
                        Price = detail.Price
                    }).ToList()
                }).OrderByDescending(o => o.OrderDate).ToListAsync();
            return listOrderByUserIdDTO;
        }

        public async Task<GetOrderDetailDTO> GetOrderDetail(int id)
        {
            var orderByIdDTO = await _fashionShopDBContext.Orders.Select(order => new GetOrderDetailDTO
            {
                ID = order.ID,
                Email = order.Email,
                FullName = order.FullName,
                PhoneNumber = order.PhoneNumber,
                ProvinceName = order.Province.Name,
                DistrictName = order.District.Name,
                WardName = order.Ward.Name,
                Address = order.Address,
                DeliveryFee = order.DeliveryFee,
                OrderDate = order.OrderDate,
                Note = order.Note,
                Status = order.Status,
                TypePayment = order.TypePayment,

                Voucher = order.Voucher,
                UserID = order.UserID,

                OrderDetails = order.OrderDetails.Select(od => new OrderDetailDTO()
                {
                    ProductID = od.ProductID,
                    ProductName = od.Product.Name, // Truy cập trực tiếp từ navigation property
                    ImageUrl = od.Product.Image,
                    OrderID = od.OrderID,
                    Price = od.Price,
                    Quantity = od.Quantity
                }).ToList()
            }).SingleOrDefaultAsync(o => o.ID == id);

            return orderByIdDTO;
        }

        public async Task<GetOrderByUserIdDTO> GetNewByUserID(string id)
        {
            var orderByUserIdDTO = await _fashionShopDBContext.Orders.Select(order => new GetOrderByUserIdDTO
            {
                ID = order.ID,
                Email = order.Email,
                FullName = order.FullName,
                PhoneNumber = order.PhoneNumber,
                ProvinceName = order.Province.Name,
                DistrictName = order.District.Name,
                WardName = order.Ward.Name,
                Address = order.Address,
                DeliveryFee = order.DeliveryFee,
                OrderDate = order.OrderDate,
                Note = order.Note,
                Status = order.Status,
                TypePayment = order.TypePayment,

                Voucher = order.Voucher,
                UserID = order.UserID,

                OrderDetails = order.OrderDetails.Select(od => new OrderDetail()
                {
                    ProductID = od.ProductID,
                    Product = _fashionShopDBContext.Products.SingleOrDefault(p => p.ID == od.ProductID),
                    OrderID = od.OrderID,
                    Price = od.Price,
                    Quantity = od.Quantity

                }).ToList(),
            }).Where(o => o.UserID == id).OrderByDescending(o => o.OrderDate).FirstOrDefaultAsync();

            return orderByUserIdDTO;
        }

        public async Task<GetOrderDTO> Create(CreateOrderDTO createOrderDTO)
        {
            var orderDomain = new Order()
            {
                FullName = createOrderDTO.FullName,
                Email = createOrderDTO.Email,
                PhoneNumber = createOrderDTO.PhoneNumber,
                ProvinceID = createOrderDTO.ProvinceID,
                DistrictID = createOrderDTO.DistrictID,
                WardID = createOrderDTO.WardID,
                Address = createOrderDTO.Address,
                Note = createOrderDTO.Note,
                DeliveryFee = createOrderDTO.DeliveryFee,
                TypePayment = createOrderDTO.TypePayment,
                OrderDate = DateTime.Now,
                Status = 1,

                UserID = createOrderDTO.UserID,
                VoucherID = createOrderDTO.VoucherID,
            };
            await _fashionShopDBContext.Orders.AddAsync(orderDomain);
            await _fashionShopDBContext.SaveChangesAsync();

            foreach (var item in createOrderDTO.shoppingCarts)
            {
                var orderDetailDomain = new OrderDetail()
                {
                    OrderID = orderDomain.ID,
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                };

                if (item.Product.Discount > 0)
                {
                    orderDetailDomain.Price = item.Product.Price - (item.Product.Price * item.Product.Discount / 100);
                }
                else
                {
                    orderDetailDomain.Price = item.Product.Price;
                }

                await _fashionShopDBContext.OrderDetails.AddAsync(orderDetailDomain);
                await _fashionShopDBContext.SaveChangesAsync();
            }

            var orderNew = new GetOrderDTO()
            {
                ID = orderDomain.ID,
                FullName = createOrderDTO.FullName,
                Email = createOrderDTO.Email,
                PhoneNumber = createOrderDTO.PhoneNumber,
                ProvinceID = createOrderDTO.ProvinceID,
                DistrictID = createOrderDTO.DistrictID,
                WardID = createOrderDTO.WardID,
                Address = createOrderDTO.Address,
                Note = createOrderDTO.Note,
                DeliveryFee = createOrderDTO.DeliveryFee,
                OrderDate = DateTime.Now,
                TypePayment = createOrderDTO.TypePayment,
                TypePaymentVN = createOrderDTO.TypePaymentVN,
                shoppingCarts = createOrderDTO.shoppingCarts,

                UserID = createOrderDTO.UserID,
                VoucherID = createOrderDTO.VoucherID,
            };

            return orderNew;
        }

        public async Task<List<ShoppingCartViewModel>> Cancel(int id)
        {
            var orderById = await _fashionShopDBContext.Orders.SingleOrDefaultAsync(o => o.ID == id);

            if (orderById != null)
            {
                orderById.Status = 0;
                await _fashionShopDBContext.SaveChangesAsync();

                // Tăng số lượng voucher khi hủy hàng
                if (orderById.VoucherID != null)
                {
                    var voucher = await _fashionShopDBContext.Vouchers.SingleOrDefaultAsync(v => v.ID == orderById.VoucherID);

                    voucher!.Quantity += 1;
                    await _fashionShopDBContext.SaveChangesAsync();
                }

                var listOrder = await _fashionShopDBContext.OrderDetails.Where(od => od.OrderID == orderById.ID).Select(o => new ShoppingCartViewModel
                {
                    ProductID = o.ProductID,
                    Quantity = o.Quantity,
                }).ToListAsync();

                return listOrder;
            }

            return null;
        }
       
        public double TotalPayment(GetOrderByUserIdDTO getOrderByUserIdDTO)
        {
            double totalMoney = 0;

            foreach (var item in getOrderByUserIdDTO.OrderDetails)
            {
                totalMoney += item.Price * item.Quantity;
            }

            double voucherValue = 0;

            if (getOrderByUserIdDTO.Voucher != null)
            {
                if (getOrderByUserIdDTO.Voucher.DiscountAmount == true)
                {
                    voucherValue = getOrderByUserIdDTO.Voucher.DiscountValue;
                }
                else
                {
                    voucherValue = totalMoney * getOrderByUserIdDTO.Voucher.DiscountValue / 100;
                }
            }

            double totalPayment = totalMoney + getOrderByUserIdDTO.DeliveryFee - voucherValue;

            return totalPayment;
        }

        public double AdminTotalPayment(int id)
        {
            var order = _fashionShopDBContext.Orders.Select(order => new GetOrderByUserIdDTO
            {
                ID = order.ID,
                Voucher = order.Voucher,
                DeliveryFee = order.DeliveryFee,

                OrderDetails = order.OrderDetails.Select(od => new OrderDetail()
                {
                    ProductID = od.ProductID,
                    Product = _fashionShopDBContext.Products.SingleOrDefault(p => p.ID == od.ProductID),
                    OrderID = od.OrderID,
                    Price = od.Price,
                    Quantity = od.Quantity

                }).ToList(),
            }).SingleOrDefault(o => o.ID == id);

            double totalMoney = 0;
            foreach (var item in order.OrderDetails)
            {
                totalMoney += item.Price * item.Quantity;
            }

            double voucherValue = 0;

            if (order.Voucher != null)
            {
                if (order.Voucher.DiscountAmount == true)
                {
                    voucherValue = order.Voucher.DiscountValue;
                }
                else
                {
                    voucherValue = totalMoney * order.Voucher.DiscountValue / 100;
                }
            }

            double totalPayment = totalMoney + order.DeliveryFee - voucherValue;

            return totalPayment;
        }

        public bool PayOnline(int id)
        {
            var orderById = _fashionShopDBContext.Orders.SingleOrDefault(o => o.ID == id);

            if (orderById != null)
            {
                orderById.Status = 3;
                _fashionShopDBContext.SaveChanges();
                return true;
            }

            return false;
        }

        public async Task<int> Count()
        {
            var countOrder = await _fashionShopDBContext.Orders.CountAsync();
            return countOrder;
        }

        public async Task<GetOrderByIdDTO> GetOrderById(int id)
        {
            var orderByIdDTO = await _fashionShopDBContext.Orders.Select(order => new GetOrderByIdDTO
            {
                ID = order.ID,
                Email = order.Email,
                FullName = order.FullName,
                PhoneNumber = order.PhoneNumber,
                ProvinceName = order.Province.Name,
                DistrictName = order.District.Name,
                WardName = order.Ward.Name,
                Address = order.Address,
                DeliveryFee = order.DeliveryFee,
                OrderDate = order.OrderDate,
                Note = order.Note,
                Status = order.Status,
                TypePayment = order.TypePayment,

                Voucher = order.Voucher,
                UserID = order.UserID,

                OrderDetails = order.OrderDetails.Select(od => new OrderDetail()
                {
                    ProductID = od.ProductID,
                    Product = _fashionShopDBContext.Products.SingleOrDefault(p => p.ID == od.ProductID),
                    OrderID = od.OrderID,
                    Price = od.Price,
                    Quantity = od.Quantity

                }).ToList(),
            }).SingleOrDefaultAsync(o => o.ID == id);

            return orderByIdDTO;
        }
    }
}
