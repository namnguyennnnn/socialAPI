
using DoAn4.DTOs.AuthenticationDTOs;
using DoAn4.DTOs.PostDTO;
using DoAn4.DTOs.UserDTO;
using DoAn4.Interfaces;
using DoAn4.Models;
using DoAn4.Services.AuthenticationService;
using DoAn4.Services.ImageService;
using DoAn4.Services.VideoService;

using Microsoft.EntityFrameworkCore;

using System.Security.Authentication;



namespace DoAn4.Services.PostService
{
    public class PostService : IPostService
    {
       
        private readonly IPostRepository _postRepository;
        private readonly IImageService _imageService;
        private readonly IVideoService _videoService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IFriendshipRepository _friendshipRepository;
        


        public PostService(IUserRepository userRepository, IFriendshipRepository friendshipRepository, IPostRepository postRepository, IVideoService videoService, IImageService imageService, IAuthenticationService authenticationService)
        {
            _postRepository = postRepository;
            _imageService = imageService;
            _videoService = videoService;
            _authenticationService = authenticationService;
            _friendshipRepository = friendshipRepository;
            
        }

        public async Task<List<InFoPostDto>> GetFriendPostsAsync(string token, int skip, int take)
        {
            var user = await _authenticationService.GetIdUserFromAccessToken(token);
            var allFriendship = await _friendshipRepository.GetAllFriendIdsAsync(user.UserId);
            var postsOfFriends = await _postRepository.GetNewFeeds(allFriendship,user.UserId, skip, take);
            if (user == null)
            {
                throw new AuthenticationException("Token đã hết hạn");
            }
            else if (allFriendship == null)
            {
                throw new ArgumentNullException(nameof(allFriendship),"Không có người bạn nào");
            }
            return postsOfFriends;

        }

        public async Task<InFoPostDto> CreatePostAsync(string token, PostDto? postDto = null)
        {
            var user = await _authenticationService.GetIdUserFromAccessToken(token);
            if (user == null)
            {
                throw new AuthenticationException("Token đã hết hạn");
            }

            if (postDto == null || (string.IsNullOrEmpty(postDto.Content) && (postDto.ImageFiles == null || !postDto.ImageFiles.Any()) && (postDto.VideoFiles == null || !postDto.VideoFiles.Any())))
            {
                throw new ArgumentException("Không có thông tin bài đăng được cung cấp.");
            }

            var localTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
            var newPost = new Post
            {
                PostId = Guid.NewGuid(),
                UserPostId = user.UserId,
                Content = postDto?.Content,
                PostTime = localTime,
                UpdateTime = localTime,
                TotalReact = 0,
                TotalComment = 0,
                IsDeleted = false
            };

            await _postRepository.CreatePostAsync(newPost);

            List<string>? imageLinks = null;
            List<string>? videoLinks = null;
            if (postDto.ImageFiles != null && postDto.ImageFiles.Any())
            {
                var images = await _imageService.UploadImages(newPost.PostId, postDto.ImageFiles);
                imageLinks = images.Select(img => img.ImageLink).ToList();
            }
            if (postDto.VideoFiles != null && postDto.VideoFiles.Any())
            {
                var video = await _videoService.UploadVideo(newPost.PostId, postDto.VideoFiles);
                videoLinks = video.Select(video => video.VideoLink).ToList();
            }

            var infoUserDto = new InfoUserDTO
            {
                UserId = newPost.UserPostId,
                Email = user.Email,
                Fullname = user.FullName,
                Avatar = user.Avatar,
                CoverPhoto = user.CoverPhoto
            };

            return new InFoPostDto
            {
                PostId = newPost.PostId,
                Content = newPost.Content,
                TotalReact = newPost.TotalReact,
                TotalComment = newPost.TotalComment,
                React = 7 ,
                PostTime = newPost.PostTime,
                UpdateTime = newPost.UpdateTime,
                User = infoUserDto,
                Images = imageLinks,
                Videos = videoLinks
            };
        }


        public async Task<InFoPostDto> UpdatePostAsync(string token, Guid postId, UpdatePostDto? updatePostDto = null)
        {
            var user = await _authenticationService.GetIdUserFromAccessToken(token) ?? throw new Exception("Token đã hết hạn");
            var infoPost = await _postRepository.GetInfoPostByIdAsync(postId) ?? throw new ArgumentNullException(nameof(postId), "Bài đăng không tồn tại");
            var post = await _postRepository.GetPostByIdAsync(postId) ?? throw new ArgumentNullException();

            // Kiểm tra xem user hiện tại có quyền chỉnh sửa bài đăng hay không
            if (post.UserPostId != user.UserId)
            {
                throw new Exception("Bạn không có quyền chỉnh sửa bài đăng này");
            }

            // Cập nhật nội dung và thời gian cập nhật
            if (updatePostDto?.Content != null)
            {
                post.Content = updatePostDto.Content;
                post.UpdateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
            }

            // Xử lý ảnh và video
            if (updatePostDto != null)
            {
                // Xóa ảnh từ post
                if (updatePostDto.ImagesLinkRemove != null && updatePostDto.ImagesLinkRemove.Any())
                {
                    await _imageService.RemoveImage(postId, updatePostDto);
                }

                // Check có ảnh nào mới được thêm vào post không
                if (updatePostDto.ImageFiles != null && updatePostDto.ImageFiles.Any())
                {
                    var addImage = await _imageService.UploadImages(post.PostId, updatePostDto.ImageFiles);
                    if (addImage == null)
                    {
                        throw new Exception("Thêm ảnh thất bại");
                    }
                }

                // Xóa video từ post
                if (updatePostDto.VideosLinkRemove != null && updatePostDto.VideosLinkRemove.Any())
                {
                    await _videoService.RemoveVideo(postId, updatePostDto);
                }

                // Check có video nào mới được thêm vào post không
                if (updatePostDto.VideoFiles != null && updatePostDto.VideoFiles.Any())
                {
                    var addVideo = await _videoService.UploadVideo(post.PostId, updatePostDto.VideoFiles);
                    if (addVideo == null)
                    {
                        throw new Exception("Thêm video thất bại");
                    }
                }
            }

            try
            {
                await _postRepository.UpdatePostAsync(post);
                infoPost = await _postRepository.GetInfoPostByIdAsync(postId);

                var infoUserDto = new InfoUserDTO
                {
                    UserId = user.UserId,
                    Email = user.Email,
                    Fullname = user.FullName,
                    Avatar = user.Avatar,
                    CoverPhoto = user.CoverPhoto
                };

                return new InFoPostDto
                {
                    PostId = infoPost.PostId,
                    Content = post.Content,
                    TotalReact = infoPost.TotalReact,
                    TotalComment = infoPost.TotalComment,
                    PostTime = infoPost.PostTime,
                    UpdateTime = infoPost.UpdateTime,
                    User = infoUserDto,
                    Images = infoPost.Images,
                    Videos = infoPost.Videos
                };
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Cập nhật không thành công: " + ex.Message);
            }
        }


        public async Task<ResultRespone> DeletePostAsync(string token, Guid posdId)
        {
            var user = await _authenticationService.GetIdUserFromAccessToken(token);
            var isDeletePost = await _postRepository.DeletePostAsync(posdId);

            if(user == null)
            {
                throw new Exception("Token hết hạn");
            }
            else if(isDeletePost != false)
            {
                return new ResultRespone { Status = 200 };
            }
            else
            {
                return new ResultRespone { Status = 400 };
            }
        }

        public async Task<string> UpdateAvatarAsync(string token, IFormFile imageFile)
        {
            var ext = Path.GetExtension(imageFile.FileName);
            var user = await _authenticationService.GetIdUserFromAccessToken(token);
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var utcNow = DateTime.UtcNow;
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, timeZone);
            if (user == null)
            {
                throw new AuthenticationException("Token đã hết hạn");
            }
            else if (imageFile == null)
            {
                throw new ArgumentException("Không có thông tin bài đăng được cung cấp");
            }          
            else if(imageFile == null)
            {
                throw new ArgumentException("File ảnh chưa được cung cấp");
            }
            else {
                var post = new Post
                {
                    PostId = Guid.NewGuid(),
                    UserPostId = user.UserId,
                    Content = "Đã cập nhật 1 ảnh đại diện",
                    TotalReact = 0 ,
                    TotalComment = 0,
                    PostTime = localTime,
                    UpdateTime = localTime,
                    IsDeleted = false
                };

                await _postRepository.CreatePostAsync(post);


                var img = await _imageService.UploadImage(post.PostId, imageFile);
                var imgPath = img.ImageLink;

                return imgPath;
            }
            
        }

        public async Task<List<InFoPostDto>> GetSelfPostsAsync(Guid userId)
        {
            var selfPosts = await _postRepository.GetAllPostByIdUserAsync(userId);
            if (selfPosts == null)
            {
                throw new AuthenticationException("Not Found");
            }
         
            return selfPosts;
        }

        public async Task<string> UpdateCoverPhotoAsync(string token, IFormFile imageFile)
        {
            var ext = Path.GetExtension(imageFile.FileName);
            var user = await _authenticationService.GetIdUserFromAccessToken(token);
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var utcNow = DateTime.UtcNow;
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, timeZone);
            if (user == null)
            {
                throw new AuthenticationException("Token đã hết hạn");
            }
            else if (imageFile == null)
            {
                throw new ArgumentException("Không có thông tin bài đăng được cung cấp");
            }
            else if (imageFile == null)
            {
                throw new ArgumentException("File ảnh chưa được cung cấp");
            }
            else
            {
                var post = new Post
                {
                    PostId = Guid.NewGuid(),
                    UserPostId = user.UserId,
                    Content = "Đã cập nhật ảnh bìa ",
                    TotalReact = 0,
                    TotalComment = 0,
                    PostTime = localTime,
                    UpdateTime = localTime,
                    IsDeleted = false
                };

                await _postRepository.CreatePostAsync(post);


                var img = await _imageService.UploadImage(post.PostId, imageFile);
                var imgPath = img.ImageLink;

                return imgPath;
            }
        }
    }

}