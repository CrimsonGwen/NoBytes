
namespace Restaurant.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly RestaurantContext _context;
        private IGenericRepository<CustomUser>? customUserRepository;
        private IGenericRepository<Land>? landenRepository;
        private IGenericRepository<Reservatie>? reservatieRepository;
        private IGenericRepository<Tafel>? tafelRepository;

        private ITafelRepository customTafelRepository;

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

        public IGenericRepository<Reservatie> ReservatieRepository
        {
            get
            {
                if (this.reservatieRepository == null)
                    this.reservatieRepository = new GenericRepository<Reservatie>(_context);

                return reservatieRepository;
            }
        }

        public IGenericRepository<Tafel> TafelRepository
        {
            get
            {
                if (this.tafelRepository == null)
                    this.tafelRepository = new GenericRepository<Tafel>(_context);

                return tafelRepository;
            }
        }

        public ITafelRepository CustomTafelRepository
        {
            get
            {
                if (this.customTafelRepository == null)
                    this.customTafelRepository = new TafelRepository(_context);

                return customTafelRepository;
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
