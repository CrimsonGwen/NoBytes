

namespace Restaurant.Data.UnitOfWork
{
    public interface IUnitOfWork
    {
        IGenericRepository<CustomUser> CustomUserRepository { get; }
        IGenericRepository<Land> LandenRepository { get; }

        public Task SaveChangesAsync();
    }
}
