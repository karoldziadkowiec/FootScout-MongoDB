using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.Repositories.Interfaces;
using System.Text;
using FootScout_MongoDB.WebAPI.DbManager;
using MongoDB.Driver;

namespace FootScout_MongoDB.WebAPI.Repositories.Classes
{
    public class ProblemRepository : IProblemRepository
    {
        private readonly MongoDBContext _dbContext;

        public ProblemRepository(MongoDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Problem> GetProblem(int problemId)
        {
            return await _dbContext.ProblemsCollection
                .Find(p => p.Id == problemId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Problem>> GetAllProblems()
        {
            return await _dbContext.ProblemsCollection
                .Find(_ => true)
                .ToListAsync();
        }

        public async Task<IEnumerable<Problem>> GetSolvedProblems()
        {
            return await _dbContext.ProblemsCollection
                .Find(p => p.IsSolved == true)
                .SortByDescending(p => p.CreationDate)
                .ToListAsync();
        }

        public async Task<int> GetSolvedProblemCount()
        {
            return (int)await _dbContext.ProblemsCollection
                .CountDocumentsAsync(p => p.IsSolved == true);
        }

        public async Task<IEnumerable<Problem>> GetUnsolvedProblems()
        {
            return await _dbContext.ProblemsCollection
                .Find(p => p.IsSolved == false)
                .SortByDescending(p => p.CreationDate)
                .ToListAsync();
        }

        public async Task<int> GetUnsolvedProblemCount()
        {
            return (int)await _dbContext.ProblemsCollection
                .CountDocumentsAsync(p => p.IsSolved == false);
        }

        public async Task CreateProblem(Problem problem)
        {
            problem.CreationDate = DateTime.Now;
            problem.IsSolved = false;

            await _dbContext.ProblemsCollection.InsertOneAsync(problem);
        }

        public async Task CheckProblemSolved(Problem problem)
        {
            problem.IsSolved = true;

            var problemFilter = Builders<Problem>.Filter.Eq(p => p.Id, problem.Id);
            await _dbContext.ProblemsCollection.ReplaceOneAsync(problemFilter, problem);
        }

        public async Task<MemoryStream> ExportProblemsToCsv()
        {
            var problems = await GetAllProblems();
            var csv = new StringBuilder();
            csv.AppendLine("Problem Id,Is Solved,Requester E-mail,Requester First Name,Requester Last Name,Title,Description,Creation Date");

            foreach (var problem in problems)
            {
                csv.AppendLine($"{problem.Id},{problem.IsSolved},{problem.Requester.Email},{problem.Requester.FirstName},{problem.Requester.LastName},{problem.Title},{problem.Description},{problem.CreationDate:yyyy-MM-dd}");
            }

            var byteArray = Encoding.UTF8.GetBytes(csv.ToString());
            var csvStream = new MemoryStream(byteArray);

            return csvStream;
        }
    }
}