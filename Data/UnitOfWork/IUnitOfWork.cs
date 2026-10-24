

namespace Restaurant.Data.UnitOfWork
{
    public interface IUnitOfWork
    {
        //Generic Repositories
        IGenericRepository<CustomUser> CustomUserRepository { get; }
        IGenericRepository<Land> LandenRepository { get; }
        IGenericRepository<Reservatie> ReservatieRepository { get; }
        IGenericRepository<Tafel> TafelRepository { get; }

        //Custom repositories
        ITafelRepository CustomTafelRepository { get; }

        public Task SaveChangesAsync();
    }
}
