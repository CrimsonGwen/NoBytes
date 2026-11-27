namespace Restaurant.Data.Repository
{
    public interface ITafelRepository
    {
        Task<bool> CheckAvailabilityAsync(DateTime date, int tijdSlotId, int persons, int? excludeReservatieId = null);
        Task<List<string>> GetAlternativeSlotsAsync(DateTime date, int persons, int? excludeReservatieId = null);
        Task<IEnumerable<SelectListItem>> GetTijdSlotSelectListAsync();
    }
}
