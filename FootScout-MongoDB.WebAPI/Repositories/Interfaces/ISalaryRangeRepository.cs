using FootScout_MongoDB.WebAPI.Entities;

namespace FootScout_MongoDB.WebAPI.Repositories.Interfaces
{
    public interface ISalaryRangeRepository
    {
        Task CreateSalaryRange(SalaryRange salaryRange);
    }
}