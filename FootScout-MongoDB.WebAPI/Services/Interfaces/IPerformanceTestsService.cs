namespace FootScout_MongoDB.WebAPI.Services.Interfaces
{
    public interface IPerformanceTestsService
    {
        Task SeedComponents(int testCounter);
        Task ClearDatabaseOfSeededComponents();
    }
}