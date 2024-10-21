using FashionShop.Models.DTO.UserDTO;


namespace Service
{
    public interface IUserRepository
    {
        public  Task<IEnumerable<GetUserDTO>> GetAllUserAsync(string? searchByName, string? filterRole);
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
