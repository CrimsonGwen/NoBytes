
namespace Restaurant.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly RestaurantContext _context;
        private IGenericRepository<CustomUser>? customUserRepository;
        private IGenericRepository<Land>? landenRepository;

        public UnitOfWork(RestaurantContext context)
        {
            _context = context;
        }

        public IGenericRepository<CustomUser> CustomUserRepository
        {
            get
            {
                if (this.customUserRepository == null)
                    this.customUserRepository = new GenericRepository<CustomUser>(_context);

                return customUserRepository;
            }
        }

        public IGenericRepository<Land> LandenRepository
        {
            get
            {
                if (this.landenRepository == null)
                    this.landenRepository = new GenericRepository<Land>(_context);

                return landenRepository;
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
