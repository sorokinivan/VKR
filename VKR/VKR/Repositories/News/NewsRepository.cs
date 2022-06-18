using Microsoft.EntityFrameworkCore;
using VKR.Models;

namespace VKR.Repositories.News
{
    public class NewsRepository : INewsRepository
    {
        private readonly DBContext _context;

        public NewsRepository(DBContext context)
        {
            _context = context;
        }


        public async Task<List<VKR.Models.NewsModels.News>> GetNewsAsync()
        {
            return await _context.News.ToListAsync();
        }

        public async Task<VKR.Models.NewsModels.News> GetNewsDetailsAsync(int id)
        {
            return await _context.News.Where(x=>x.Id == id).FirstOrDefaultAsync();
        }

        public async Task SaveNews(VKR.Models.NewsModels.NewsDataModel model)
        {
            var news = new VKR.Models.NewsModels.News
            {
                NewsDate = DateTime.Now,
                NewsName = model.NewsName,
                NewsText = model.NewsText
            };
            _context.News.Add(news);
            await _context.SaveChangesAsync();
        }
    }
}
