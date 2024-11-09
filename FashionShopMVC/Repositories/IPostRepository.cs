using FashionShopMVC.Data;
using FashionShopMVC.Helper;
using FashionShopMVC.Models.Domain;
using FashionShopMVC.Models.DTO.PostDTO;
using Microsoft.EntityFrameworkCore;

namespace FashionShopMVC.Repositories
{
    public interface IPostRepository
    {
        public  Task<AdminPaginationSet<PostDTO>> GetAll(int page, int pageSize, string? searchByName);
        public  Task<CreatePostDTO> Create(CreatePostDTO postDTO);
        public  Task<UpdatePostDTO> Update(UpdatePostDTO postDTO, int id);
        public Task<bool> Delete(int id);
        public Task<PostDTO> GetById(int idPost);
    }
    public class PostRepository : IPostRepository
    {
        private readonly FashionShopDBContext _fashionShopDBContext;
        public PostRepository(FashionShopDBContext fashionShopDBContext)
        {
            _fashionShopDBContext=fashionShopDBContext;
        }
        public async Task<AdminPaginationSet<PostDTO>> GetAll(int page, int pageSize, string? searchByName)
        {
            var listPostDomain = _fashionShopDBContext.Posts.AsQueryable();
            if (searchByName != null)
            {
                listPostDomain = listPostDomain.Where(p => p.Title.Contains(searchByName));
            }
            var listPostDTO = await listPostDomain.Select(post => new PostDTO
            {
                ID = post.ID,
                Title = post.Title,
                Content = post.Content,
                Image = post.Image,
                Status = post.Status,
            }).OrderByDescending(p => p.ID).ToListAsync();

            var totalCount = listPostDTO.Count();
            var listPostPagination = listPostDTO.Skip(page * pageSize).Take(pageSize);

            AdminPaginationSet<PostDTO> postPaginationSet = new AdminPaginationSet<PostDTO>()
            {
                List = listPostPagination,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                PagesCount = (int)Math.Ceiling((decimal)totalCount / pageSize),
            };

            return postPaginationSet;
        }
        public async Task<CreatePostDTO> Create(CreatePostDTO postDTO)
        {
            var postDomain = new Post
            {
                Title = postDTO.Title,
                Image = postDTO.Image,
                Content = postDTO.Content,
                Status = postDTO.Status,

            };
            await _fashionShopDBContext.Posts.AddAsync(postDomain);
            await _fashionShopDBContext.SaveChangesAsync();

            return postDTO;
        }
        public async Task<PostDTO> GetById(int idPost)
        {
            var postDomain = await _fashionShopDBContext.Posts.Select(post => new PostDTO
            {
                ID = post.ID,
                Title = post.Title,
                Content = post.Content,
                Image = post.Image,
                Status = post.Status,
            }).FirstOrDefaultAsync(p => p.ID == idPost);

            return postDomain;
        }
        public async Task<UpdatePostDTO> Update(UpdatePostDTO postDTO, int id)
        {
            var postDomain = await _fashionShopDBContext.Posts.FirstOrDefaultAsync(p => p.ID == id);

            if (postDomain != null)
            {
                postDomain.Title = postDTO.Title;
                postDomain.Content = postDTO.Content;
                postDomain.Image = postDTO.Image;
                postDomain.Status = postDTO.Status;
                
                await _fashionShopDBContext.SaveChangesAsync();
                return postDTO;
            }
            else
            {
                return null!;
            }
        }
        public async Task<bool> Delete(int id)
        {
            var postDomain = await _fashionShopDBContext.Posts.FirstOrDefaultAsync(p => p.ID == id);

            if (postDomain != null)
            {
                _fashionShopDBContext.Remove(postDomain);
                await _fashionShopDBContext.SaveChangesAsync();

                return true;
            }
            else
            {
                return false;
            }
        }  
    }
}
