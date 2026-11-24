namespace Restaurant.Data.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly RestaurantContext _context;
        public ProductRepository(RestaurantContext context)
        {
            _context = context;
        }
        public Task AddAsync(Product product)
        {
            throw new NotImplementedException();
        }   

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Producten.OrderBy(p => p.Id).ToListAsync();
        }

        public Task UpdateAsync(Product product)
        {
            throw new NotImplementedException();
        }
    }
}
