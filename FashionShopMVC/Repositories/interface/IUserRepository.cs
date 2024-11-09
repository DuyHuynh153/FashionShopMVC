using FashionShopMVC.Helper;
using FashionShopMVC.Models.DTO.UserDTO;


namespace FashionShopMVC.Repositories.@interface
{
    public interface IUserRepository
    {
        public  Task<IEnumerable<GetUserDTO>> GetAllUserAsync(string? searchByName, string? filterRole);
        public Task<IEnumerable<GetUserDTO>> GetPagedUsersAdminAsync(string searchQuery,string role, int pageNumber, int pageSize);
        public Task<AdminPaginationSet<GetUserDTO>> GetAllCustomerAsync(int page, int pageSize, string? searchByName, string? filterRole, string? phoneNumber, string? email);
        public  Task<GetUserDTO> GetUserByIdAsync(string id);
        public Task<bool> RegisterAccountAdminAsync(RegisterRequestDTO registerRequestDTO);

        public Task<bool> RegisterAccountEmployeeAsync(RegisterRequestDTO registerRequestDTO);

        public Task<bool> RegisterAccountCustomer(RegisterRequestDTO registerRequestDTO);

        public Task<string> Login(LoginRequestDTO loginRequestDTO);

        public Task<bool> AccountLock(string idAccount);
        // public Task<bool> AccountUnlock(string idAccount);
        public Task<UpdateUserDTO> UpdateAsync(UpdateUserDTO updateUserDTO, string id);
        public Task<bool> Delete(string id);

        public Task<int> Count();
    }
}
