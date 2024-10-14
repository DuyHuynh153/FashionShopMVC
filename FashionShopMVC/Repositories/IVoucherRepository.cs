using FashionShopMVC.Data;
using FashionShopMVC.Models.Domain;
using FashionShopMVC.Models.DTO.VoucherDTO;
using Microsoft.EntityFrameworkCore;

namespace FashionShopMVC.Repositories
{
    public interface IVoucherRepository
    {
        public Task<List<GetVoucherDTO>> GetAll();
        public Task<GetVoucherDTO> GetById(int id);
        public Task<GetVoucherDTO> GetByDiscountCode(string discountCode);
        public Task<CreateVoucherDTO> Create(CreateVoucherDTO createVoucherDTO);
        public Task<UpdateVoucherDTO> Update(UpdateVoucherDTO updateVoucherDTO, int id);
        public Task<bool> Delete(int id);
        public Task<bool> ReduceQuantityVoucher(int idVoucher);
        public Task<bool> IncreaseQuantityVoucher(int idVoucher);
        public Task<int> Count();
    }
    public class VoucherRepository : IVoucherRepository
    {
        private readonly FashionShopDBContext _fashionShopDBContext;

        public VoucherRepository(FashionShopDBContext fashionShopDBContext)
        {
            _fashionShopDBContext = fashionShopDBContext;
        }

        public async Task<List<GetVoucherDTO>> GetAll()
        {
            var listVoucherDTO = await _fashionShopDBContext.Vouchers.Select(voucher => new GetVoucherDTO
            {
                id = voucher.ID,
                discountCode = voucher.DiscountCode,
                discountAmount = voucher.DiscountAmount,
                discountPercentage = voucher.DiscountPercentage,
                discountValue = voucher.DiscountValue,
                minimumValue = voucher.MinimumValue,
                quantity = voucher.Quantity,
                startDate = voucher.StartDate,
                endDate = voucher.EndDate,
                describe = voucher.Describe,
                status = voucher.Status,
                createdDate = voucher.CreatedDate,
                createdBy = voucher.CreatedBy,
                updatedDate = voucher.UpdatedDate,
                updatedBy = voucher.UpdatedBy,
            }).OrderByDescending(v => v.id).ToListAsync();

            return listVoucherDTO;
        }

        public async Task<GetVoucherDTO> GetById(int id)
        {
            var voucherDTO = await _fashionShopDBContext.Vouchers.Select(voucher => new GetVoucherDTO
            {
                id = voucher.ID,
                discountCode = voucher.DiscountCode,
                discountAmount = voucher.DiscountAmount,
                discountPercentage = voucher.DiscountPercentage,
                discountValue = voucher.DiscountValue,
                minimumValue = voucher.MinimumValue,
                quantity = voucher.Quantity,
                startDate = voucher.StartDate,
                endDate = voucher.EndDate,
                describe = voucher.Describe,
                status = voucher.Status,
                createdDate = voucher.CreatedDate,
                createdBy = voucher.CreatedBy,
                updatedDate = voucher.UpdatedDate,
                updatedBy = voucher.UpdatedBy,
            }).FirstOrDefaultAsync(v => v.id == id);

            return voucherDTO;
        }

        public async Task<GetVoucherDTO> GetByDiscountCode(string discountCode)
        {
            var voucherDTO = await _fashionShopDBContext.Vouchers.Select(voucher => new GetVoucherDTO
            {
                id = voucher.ID,
                discountCode = voucher.DiscountCode,
                discountAmount = voucher.DiscountAmount,
                discountPercentage = voucher.DiscountPercentage,
                discountValue = voucher.DiscountValue,
                minimumValue = voucher.MinimumValue,
                quantity = voucher.Quantity,
                startDate = voucher.StartDate,
                endDate = voucher.EndDate,
                describe = voucher.Describe,
                status = voucher.Status,
                createdDate = voucher.CreatedDate,
                createdBy = voucher.CreatedBy,
                updatedDate = voucher.UpdatedDate,
                updatedBy = voucher.UpdatedBy,
            }).FirstOrDefaultAsync(v => v.discountCode == discountCode && v.status == true);

            return voucherDTO;
        }

        public async Task<CreateVoucherDTO> Create(CreateVoucherDTO createVoucherDTO)
        {
            var voucherDomain = new Voucher
            {
                DiscountCode = createVoucherDTO.DiscountCode,
                DiscountAmount = createVoucherDTO.DiscountAmount,
                DiscountPercentage = createVoucherDTO.DiscountPercentage,
                DiscountValue = createVoucherDTO.DiscountValue,
                MinimumValue = createVoucherDTO.MinimumValue,
                Quantity = createVoucherDTO.Quantity,
                StartDate = createVoucherDTO.StartDate,
                EndDate = createVoucherDTO.EndDate,
                Describe = createVoucherDTO.Describe,
                Status = createVoucherDTO.Status,
                CreatedDate = DateTime.Now,
                CreatedBy = createVoucherDTO.CreatedBy
            };

            await _fashionShopDBContext.Vouchers.AddAsync(voucherDomain);
            await _fashionShopDBContext.SaveChangesAsync();

            return createVoucherDTO;
        }

        public async Task<UpdateVoucherDTO> Update(UpdateVoucherDTO updateVoucherDTO, int id)
        {
            var voucherDomain = await _fashionShopDBContext.Vouchers.FindAsync(id);

            if (voucherDomain != null)
            {
                voucherDomain.DiscountCode = updateVoucherDTO.DiscountCode;
                voucherDomain.DiscountAmount = updateVoucherDTO.DiscountAmount;
                voucherDomain.DiscountPercentage = updateVoucherDTO.DiscountPercentage;
                voucherDomain.DiscountValue = updateVoucherDTO.DiscountValue;
                voucherDomain.MinimumValue = updateVoucherDTO.MinimumValue;
                voucherDomain.Quantity = updateVoucherDTO.Quantity;
                voucherDomain.StartDate = updateVoucherDTO.StartDate;
                voucherDomain.EndDate = updateVoucherDTO.EndDate;
                voucherDomain.Describe = updateVoucherDTO.Describe;
                voucherDomain.Status = updateVoucherDTO.Status;

                voucherDomain.UpdatedBy = updateVoucherDTO.UpdatedBy;
                voucherDomain.UpdatedDate = DateTime.Now;

                await _fashionShopDBContext.SaveChangesAsync();

                return updateVoucherDTO;
            }

            return null;
        }

        public async Task<bool> Delete(int id)
        {
            var voucherDomain = await _fashionShopDBContext.Vouchers.FindAsync(id);

            if (voucherDomain != null)
            {
                _fashionShopDBContext.Vouchers.Remove(voucherDomain);
                await _fashionShopDBContext.SaveChangesAsync();

                return true;
            }

            return false;
        }

        public async Task<bool> ReduceQuantityVoucher(int idVoucher)
        {
            var voucherById = await _fashionShopDBContext.Vouchers.SingleOrDefaultAsync(v => v.ID == idVoucher);

            if (voucherById != null)
            {
                voucherById.Quantity -= 1;
                await _fashionShopDBContext.SaveChangesAsync();

                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> IncreaseQuantityVoucher(int idVoucher)
        {
            var voucherById = await _fashionShopDBContext.Vouchers.SingleOrDefaultAsync(v => v.ID == idVoucher);

            if (voucherById != null)
            {
                voucherById.Quantity += 1;
                await _fashionShopDBContext.SaveChangesAsync();

                return true;
            }
            else
            {
                return false;
            }

        }

        public async Task<int> Count()
        {
            var countVoucher = await _fashionShopDBContext.Vouchers.CountAsync();
            return countVoucher;
        }
    }
}
