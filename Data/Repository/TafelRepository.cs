namespace Restaurant.Data.Repository
{
    public class TafelRepository:ITafelRepository
    {
        private readonly RestaurantContext _context; // Vervang door jouw DbContext

        public TafelRepository(RestaurantContext context)
        {
            _context = context;
        }

        public async Task<bool> CheckAvailabilityAsync(DateTime date, int tijdSlotId, int persons)
        {
            // Voorbeeldlogica: Tel het aantal reeds gereserveerde personen / tafels op die datum + tijdslot
            // en vergelijk dit met de totale capaciteit van je restaurant.
            int totalCapacity = await _context.Tafels.CountAsync(); // Of jouw logica

            int reservedPersons = await _context.Reservaties
                .Where(r => r.Datum.Value.Date == date.Date && r.TijdSlotId == tijdSlotId)
                .SumAsync(r => r.AantalPersonen);

            return (reservedPersons + persons) <= totalCapacity;
        }

        public async Task<List<string>> GetAlternativeSlotsAsync(DateTime date, int persons)
        {
            // Voorbeeld: Zoek tijdsloten op dezelfde dag waar nog wel plek is
            // Dit is een placeholder, pas aan naar jouw Tijdslot entiteit
            var availableSlots = new List<string>();

            var allTimeSlots = await _context.Tijdslots.ToListAsync();
            foreach (var slot in allTimeSlots)
            {
                if (await CheckAvailabilityAsync(date, slot.Id, persons))
                {
                    availableSlots.Add(slot.Naam); // Bijv. "Lunch: 11u30 - 12u30"
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
                    Text = t.Naam // Pas aan naar de eigenschap die de naam/tijd van het tijdslot bevat (bijv. "Lunch: 11u30 - 12u30")
                })
                .ToListAsync();
        }
    }
}
