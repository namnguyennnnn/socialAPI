
using DoAn4.Data;
using DoAn4.Interfaces;
using DoAn4.Models;
using Microsoft.EntityFrameworkCore;


namespace DoAn4.Repositories
{
    public class ImageRepository : IImageRepository
    {
        private readonly DataContext _context;
        
        public ImageRepository(DataContext context)
        {
            _context = context;
        
        }
        public async Task CreateImageAsync(Images image)
        {
            await _context.Images.AddAsync(image);
            await _context.SaveChangesAsync();
        }

        public async Task<Images> GetImageByLinkAsync(string imageLink)
        {
            var image = await _context.Images.SingleOrDefaultAsync(img => img.ImageLink == imageLink, CancellationToken.None);
            return image;
        }

        public async Task<Images> GetImageByIdPostAsync(Guid postId)
        {
            var image = await _context.Images.FindAsync(postId);
            return image;
        }

        public async Task<List<Images>> GetRemovedImagesFromPost(Post post, List<string>? Images)
        {
            var removedImages = post.Images.Where(i => Images.Contains(i.ImageLink)).ToList();
            return removedImages;   
        }

        public async Task RemoveImageAsync(string imageLink)
        {
            var image = await _context.Images.SingleOrDefaultAsync(img => img.ImageLink == imageLink);

            if (image != null)
            {
                _context.Images.Remove(image);
                await _context.SaveChangesAsync();
            }
        }


        public async Task<int> SaveChangeAsync()
        {
            return await _context.SaveChangesAsync();
        }

       
    }
}
