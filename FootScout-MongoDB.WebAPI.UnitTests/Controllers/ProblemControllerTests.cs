using AutoMapper;
using FootScout_MongoDB.WebAPI.Controllers;
using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.Models.DTOs;
using FootScout_MongoDB.WebAPI.Repositories.Interfaces;
using FootScout_MongoDB.WebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Text;

namespace FootScout_MongoDB.WebAPI.UnitTests.Controllers
{
    public class ProblemControllerTests
    {
        private readonly Mock<IProblemRepository> _mockProblemRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<INewIdGeneratorService> _mockNewIdGeneratorService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ProblemController _problemController;

        public ProblemControllerTests()
        {
            _mockProblemRepository = new Mock<IProblemRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockNewIdGeneratorService = new Mock<INewIdGeneratorService>();
            _mockMapper = new Mock<IMapper>();
            _problemController = new ProblemController(_mockProblemRepository.Object, _mockUserRepository.Object, _mockNewIdGeneratorService.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetProblem_ExistingProblem_ReturnsOkResultWithProblem()
        {
            // Arrange
            var problemId = 1;
            var problem = new Problem { Id = problemId, Title = "Problem", Description = "Problem DESC" };

            _mockProblemRepository.Setup(repo => repo.GetProblem(problemId)).ReturnsAsync(problem);

            // Act
            var result = await _problemController.GetProblem(problemId);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Problem>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedProblem = Assert.IsType<Problem>(okResult.Value);
            Assert.Equal(problemId, returnedProblem.Id);
            Assert.Equal("Problem", returnedProblem.Title);
        }

        [Fact]
        public async Task GetProblem_NonExistingProblem_ReturnsNotFound()
        {
            // Arrange
            var problemId = 1;

            _mockProblemRepository.Setup(repo => repo.GetProblem(problemId)).ReturnsAsync((Problem)null);

            // Act
            var result = await _problemController.GetProblem(problemId);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Problem>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task GetAllProblems_ReturnsOkResultWithProblemsList()
        {
            // Arrange
            var problems = new List<Problem>
            {
                new Problem { Id = 1, Title = "Problem 1", Description = "Problem DESC 1" },
                new Problem { Id = 2, Title = "Problem 2", Description = "Problem DESC 2" }
            };

            _mockProblemRepository.Setup(repo => repo.GetAllProblems()).ReturnsAsync(problems);

            // Act
            var result = await _problemController.GetAllProblems();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Problem>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedProblems = Assert.IsType<List<Problem>>(okResult.Value);
            Assert.Equal(problems.Count, returnedProblems.Count);
        }

        [Fact]
        public async Task GetSolvedProblems_ReturnsOkResultWithSolvedProblemsList()
        {
            // Arrange
            var solvedProblems = new List<Problem>
            {
                new Problem { Id = 1,  Title = "Problem", Description = "Problem DESC" }
            };

            _mockProblemRepository.Setup(repo => repo.GetSolvedProblems()).ReturnsAsync(solvedProblems);

            // Act
            var result = await _problemController.GetSolvedProblems();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Problem>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedProblems = Assert.IsType<List<Problem>>(okResult.Value);
            Assert.Equal(solvedProblems.Count, returnedProblems.Count);
        }

        [Fact]
        public async Task GetSolvedProblemCount_ReturnsOkResultWithCount()
        {
            // Arrange
            var count = 5;

            _mockProblemRepository.Setup(repo => repo.GetSolvedProblemCount()).ReturnsAsync(count);

            // Act
            var result = await _problemController.GetSolvedProblemCount();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedCount = Assert.IsType<int>(okResult.Value);
            Assert.Equal(count, returnedCount);
        }

        [Fact]
        public async Task GetUnsolvedProblems_ReturnsOkResultWithUnsolvedProblemsList()
        {
            // Arrange
            var unsolvedProblems = new List<Problem>
            {
                new Problem { Id = 1, Title = "Problem", Description = "Problem DESC" }
            };

            _mockProblemRepository.Setup(repo => repo.GetUnsolvedProblems()).ReturnsAsync(unsolvedProblems);

            // Act
            var result = await _problemController.GetUnsolvedProblems();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Problem>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedProblems = Assert.IsType<List<Problem>>(okResult.Value);
            Assert.Equal(unsolvedProblems.Count, returnedProblems.Count);
        }

        [Fact]
        public async Task GetUnsolvedProblemCount_ReturnsOkResultWithCount()
        {
            // Arrange
            var count = 10;

            _mockProblemRepository.Setup(repo => repo.GetUnsolvedProblemCount()).ReturnsAsync(count);

            // Act
            var result = await _problemController.GetUnsolvedProblemCount();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedCount = Assert.IsType<int>(okResult.Value);
            Assert.Equal(count, returnedCount);
        }

        [Fact]
        public async Task CreateProblem_ValidDto_ReturnsOkResultWithProblem()
        {
            // Arrange
            var dto = new ProblemCreateDTO { Title = "Problem", Description = "Problem DESC" };
            var problem = new Problem { Id = 1, Title = "Problem", Description = "Problem DESC" };

            _mockMapper.Setup(m => m.Map<Problem>(dto)).Returns(problem);
            _mockProblemRepository.Setup(repo => repo.CreateProblem(problem)).Returns(Task.CompletedTask);

            // Act
            var result = await _problemController.CreateProblem(dto);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            var returnedProblem = Assert.IsType<Problem>(actionResult.Value);
            Assert.Equal(problem.Description, returnedProblem.Description);
        }

        [Fact]
        public async Task CreateProblem_InvalidDto_ReturnsBadRequest()
        {
            // Arrange
            ProblemCreateDTO dto = null;

            // Act
            var result = await _problemController.CreateProblem(dto);

            // Assert
            var actionResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid dto data.", actionResult.Value);
        }

        [Fact]
        public async Task CheckProblemSolved_ValidProblem_ReturnsNoContent()
        {
            // Arrange
            var problemId = 1;
            var problem = new Problem { Id = 1, Title = "Problem", Description = "Problem DESC" };

            _mockProblemRepository.Setup(repo => repo.CheckProblemSolved(problem)).Returns(Task.CompletedTask);

            // Act
            var result = await _problemController.CheckProblemSolved(problemId, problem);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task CheckProblemSolved_InvalidProblemId_ReturnsBadRequest()
        {
            // Arrange
            var problemId = 1;
            var problem = new Problem { Id = 2, Title = "Problem", Description = "Problem DESC" };

            // Act
            var result = await _problemController.CheckProblemSolved(problemId, problem);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task ExportProblemsToCsv_ReturnsFileResult()
        {
            // Arrange
            var csvData = "Problem Id,Is Solved,Requester E-mail,Requester First Name,Requester Last Name,Title,Description,Creation Date";
            var csvStream = new MemoryStream(Encoding.UTF8.GetBytes(csvData));

            _mockProblemRepository.Setup(repo => repo.ExportProblemsToCsv()).ReturnsAsync(csvStream);

            // Act
            var result = await _problemController.ExportProblemsToCsv();

            // Assert
            var fileResult = Assert.IsType<FileStreamResult>(result);
            Assert.Equal("text/csv", fileResult.ContentType);
            Assert.Equal("problems.csv", fileResult.FileDownloadName);

            using var reader = new StreamReader(fileResult.FileStream);
            var resultCsvData = await reader.ReadToEndAsync();
            Assert.Equal(csvData, resultCsvData);
        }
    }
}