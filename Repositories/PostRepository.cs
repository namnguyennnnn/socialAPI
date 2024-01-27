using AutoMapper;
using DoAn4.Data;
using DoAn4.DTOs;
using DoAn4.DTOs.PostDTO;
using DoAn4.DTOs.UserDTO;
using DoAn4.Interfaces;
using DoAn4.Models;
using Microsoft.EntityFrameworkCore;


namespace DoAn4.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;


        public PostRepository(IMapper mapper, DataContext context ) 
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<InFoPostDto>> GetNewFeeds(List<Guid> friendIds, Guid curentUserId, int skip, int take)
        {
            var posts = await _context.Posts
                .Include(p => p.Images)
                .Include(p => p.Videos)
                .Where(p => (friendIds.Contains(p.UserPostId) || p.UserPostId == curentUserId) && !p.IsDeleted)
                .OrderByDescending(p => p.PostTime)
                .Skip(skip)
                .Take(take)
                .Select(p => new InFoPostDto
                {
                    PostId = p.PostId,
                    Content = p.Content,
                    TotalReact = p.TotalReact,
                    TotalComment = p.TotalComment,
                    PostTime = p.PostTime,
                    UpdateTime = p.UpdateTime,
                    User = new InfoUserDTO
                    {
                        UserId = p.User.UserId,
                        Email = p.User.Email,
                        Fullname = p.User.Fullname,
                        Gender = p.User.Gender,
                        Avatar = p.User.Avatar
                    },
                    Images = p.Images.Select(i => i.ImageLink).ToList(),
                    Videos = p.Videos.Select(v => v.VideoLink).ToList()
                })
                .ToListAsync();

            // Lấy danh sách PostId
            var postIds = posts.Select(p => p.PostId).ToList();

            // Lấy danh sách Like cho các bài đăng
            var likes = await _context.Likes
                .Where(l => postIds.Contains(l.PostId))
                .ToListAsync();

            // Gán React từ bảng Like vào InFoPostDto
            foreach (var post in posts)
            {
                var postLikes = likes.Where(l => l.PostId == post.PostId).ToList();
                post.React = postLikes.Count > 0 ? postLikes.Sum(l => l.React) : 7;
            }

            return posts;
        }




        public async Task<Guid> CreatePostAsync(Post post)
        {
            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();
            return post.PostId;
        }
        
        public async Task<Post> GetPostByIdAsync(Guid postId) {
            var post = await _context.Posts.FindAsync(postId);
            return post;
        }

        public async Task<InFoPostDto> GetInfoPostByIdAsync(Guid postId)
        {
            var post = await _context.Posts
                .Include(p => p.Images)
                .Include(p => p.Videos)
                .FirstOrDefaultAsync(p => p.PostId == postId);

            if (post == null)
                return null;

            var infoPostDto = new InFoPostDto
            {
                PostId = post.PostId,
                Content = post.Content,
                TotalReact = post.TotalReact,
                TotalComment = post.TotalComment,
                PostTime = post.PostTime,
                UpdateTime = post.UpdateTime,                
                Images = post.Images.Select(i => i.ImageLink).ToList(),
                Videos = post.Videos.Select(v => v.VideoLink).ToList()
            };

            return infoPostDto;
        }


        public async Task<List<InFoPostDto>> GetAllPostByIdUserAsync(Guid userId)
        {
            var listPost = await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Images)
                .Include(p => p.Videos)
                .Where(p => p.UserPostId == userId && !p.IsDeleted)
                .OrderByDescending(p => p.PostTime)
                .ToListAsync();

            var postIds = listPost.Select(p => p.PostId).ToList();

            var likes = await _context.Likes
                .Where(l => postIds.Contains(l.PostId))
                .ToListAsync();

            var result = listPost.Select(post =>
            {
                var infoPostDto = new InFoPostDto
                {
                    PostId = post.PostId,
                    Content = post.Content,
                    TotalReact = post.TotalReact,
                    TotalComment = post.TotalComment,
                    PostTime = post.PostTime,
                    UpdateTime = post.UpdateTime,
                    User = new InfoUserDTO
                    {
                        UserId = post.User.UserId,
                        Email = post.User.Email,
                        Fullname = post.User.Fullname,
                        Gender = post.User.Gender,
                        Avatar = post.User.Avatar
                    },
                    Images = post.Images.Select(i => i.ImageLink).ToList(),
                    Videos = post.Videos.Select(v => v.VideoLink).ToList()
                };

                var postLikes = likes.Where(l => l.PostId == post.PostId).ToList();
                infoPostDto.React = postLikes.Sum(l => l.React);

                
                if (infoPostDto.React == 0)
                {
                    infoPostDto.React = 7;
                }

                return infoPostDto;
            }).ToList();

            return result;
        }





        public async Task UpdatePostAsync(Post post)
        {
            _context.Entry(post).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeletePostAsync(Guid PostId)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(p => p.PostId == PostId);
            if (post != null)
            {
                post.IsDeleted = true;
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }

        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        
    }
}
