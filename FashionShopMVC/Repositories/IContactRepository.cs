using FashionShopMVC.Data;
using FashionShopMVC.Helper;
using FashionShopMVC.Models.DTO.ContactDTO;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;

namespace FashionShopMVC.Repositories
{
    public interface IContactRepository
    {
        AdminPaginationSet<ContactDTO> GetAllContact(int page, int pageSize, string? searchByPhoneNumber);
        bool Confirm(int id);
    }

    public class ContactRepository : IContactRepository
    {
        private readonly FashionShopDBContext _identityDbContext;
        public ContactRepository(FashionShopDBContext identityDbContext)
        {
            _identityDbContext = identityDbContext;
        }
        public AdminPaginationSet<ContactDTO> GetAllContact(int page, int pageSize, string? searchByPhoneNumber)
        {
            var allContact = _identityDbContext.Contacts.AsQueryable();

            if (!searchByPhoneNumber.IsNullOrEmpty())
            {
                allContact = allContact.Where(c => c.PhoneNumber.Contains(searchByPhoneNumber));
            }

            var allContactDomain = allContact.Select(c => new ContactDTO
            {
                ID = c.ID,
                FullName = c.FullName,
                Email = c.Email,
                PhoneNumber = c.PhoneNumber,
                Content = c.Content,
                Status = c.Status,

            }).OrderByDescending(c => c.ID).ToList();

            var totalCount = allContactDomain.Count();
            var listContactPagination = allContactDomain.Skip(page * pageSize).Take(pageSize);

            AdminPaginationSet<ContactDTO> contactPaginationSet = new AdminPaginationSet<ContactDTO>()
            {
                List = listContactPagination,
                Page = page,
                TotalCount = totalCount,
                PagesCount = (int)Math.Ceiling((decimal)totalCount / pageSize),
            };

            return contactPaginationSet;
        }

        public bool Confirm(int id)
        {
            var cofirmContact = _identityDbContext.Contacts.FirstOrDefault(c => c.ID == id);

            if (cofirmContact != null)
            {
                cofirmContact.Status = true;

                _identityDbContext.SaveChanges();
                return true;
            }

            return false;
        }
    }
}
