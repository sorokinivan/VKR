namespace VKR.Repositories.News
{
    public interface INewsRepository
    {
        Task<List<VKR.Models.NewsModels.News>> GetNewsAsync();
        Task<VKR.Models.NewsModels.News> GetNewsDetailsAsync(int id);
        Task SaveNews(VKR.Models.NewsModels.NewsDataModel model);
    }
}
