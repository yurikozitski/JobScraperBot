using JobScraperBot.DAL.Entities;

namespace JobScraperBot.DAL.Interfaces
{
    public interface IRepository<TEntity> where TEntity : BaseEntity
    {
        Task AddAsync(TEntity entity);

        Task DeleteByChatIdAsync(long chatId);
    }
}
