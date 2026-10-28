namespace Restaurant.Data.Repository
{
    public interface ITafelRepository
    {
        Task<bool> CheckAvailabilityAsync(DateTime date, int tijdSlotId, int persons);
        Task<List<string>> GetAlternativeSlotsAsync(DateTime date, int persons);
        Task<IEnumerable<SelectListItem>> GetTijdSlotSelectListAsync();
    }
}
