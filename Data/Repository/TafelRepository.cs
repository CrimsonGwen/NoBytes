namespace Restaurant.Data.Repository
{
    public class TafelRepository:ITafelRepository
    {
        private readonly RestaurantContext _context;

        public TafelRepository(RestaurantContext context)
        {
            _context = context;
        }

        public async Task<bool> CheckAvailabilityAsync(DateTime date, int tijdSlotId, int people, int? excludeReservatieId = null)
        {
            int totalCapacity = await _context.Tafels.CountAsync();

            int reservedPeople = await _context.Reservaties
                .Where(r => r.Datum.HasValue
                            && r.Datum.Value.Date == date.Date
                            && r.TijdSlotId == tijdSlotId
                            && (!excludeReservatieId.HasValue || r.Id != excludeReservatieId.Value))
                .SumAsync(r => r.AantalPersonen);

            return (reservedPeople + people) <= totalCapacity;
        }

        public async Task<List<string>> GetAlternativeSlotsAsync(DateTime date, int people, int? excludeReservatieId = null)
        {
            var availableSlots = new List<string>();
            var allTimeSlots = await _context.Tijdslots.ToListAsync();

            foreach (var slot in allTimeSlots)
            {
                if (await CheckAvailabilityAsync(date, slot.Id, people, excludeReservatieId))
                {
                    availableSlots.Add(slot.Naam);
                }
            }

            return availableSlots;
        }
        public async Task<IEnumerable<SelectListItem>> GetTijdSlotSelectListAsync()
        {
            return await _context.Tijdslots
                .Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = t.Naam
                })
                .ToListAsync();
        }
    }
}
