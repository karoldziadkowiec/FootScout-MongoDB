using AutoMapper;
using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.Models.DTOs;
using FootScout_MongoDB.WebAPI.Repositories.Interfaces;
using FootScout_MongoDB.WebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootScout_MongoDB.WebAPI.Controllers
{
    [Route("api/problems")]
    [Authorize(Policy = "AdminOrUserRights")]
    [ApiController]
    public class ProblemController : ControllerBase
    {
        private readonly IProblemRepository _problemRepository;
        private readonly IUserRepository _userRepository;
        private readonly INewIdGeneratorService _newIdGeneratorService;
        private readonly IMapper _mapper;

        public ProblemController(IProblemRepository problemRepository, IUserRepository userRepository, INewIdGeneratorService newIdGeneratorService, IMapper mapper)
        {
            _problemRepository = problemRepository;
            _userRepository = userRepository;
            _newIdGeneratorService = newIdGeneratorService;
            _mapper = mapper;
        }

        // GET: api/problems/:problemId
        [HttpGet("{problemId}")]
        public async Task<ActionResult<Problem>> GetProblem(int problemId)
        {
            var problem = await _problemRepository.GetProblem(problemId);
            if (problem == null)
                return NotFound();

            return Ok(problem);
        }

        // GET: api/problems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Problem>>> GetAllProblems()
        {
            var problems = await _problemRepository.GetAllProblems();
            return Ok(problems);
        }

        // GET: api/problems/solved
        [HttpGet("solved")]
        public async Task<ActionResult<IEnumerable<Problem>>> GetSolvedProblems()
        {
            var solvedProblemss = await _problemRepository.GetSolvedProblems();
            return Ok(solvedProblemss);
        }

        // GET: api/problems/solved/count
        [HttpGet("solved/count")]
        public async Task<IActionResult> GetSolvedProblemCount()
        {
            int count = await _problemRepository.GetSolvedProblemCount();
            return Ok(count);
        }

        // GET: api/problems/unsolved
        [HttpGet("unsolved")]
        public async Task<ActionResult<IEnumerable<Problem>>> GetUnsolvedProblems()
        {
            var unsolvedProblemss = await _problemRepository.GetUnsolvedProblems();
            return Ok(unsolvedProblemss);
        }

        // GET: api/problems/unsolved/count
        [HttpGet("unsolved/count")]
        public async Task<IActionResult> GetUnsolvedProblemCount()
        {
            int count = await _problemRepository.GetUnsolvedProblemCount();
            return Ok(count);
        }

        // POST: api/problems
        [HttpPost]
        public async Task<ActionResult> CreateProblem([FromBody] ProblemCreateDTO dto)
        {
            if (dto == null)
                return BadRequest("Invalid dto data.");

            var problem = _mapper.Map<Problem>(dto);
            problem.Id = await _newIdGeneratorService.GenerateNewProblemId();

            if (problem.RequesterId is not null)
            {
                var requester = await _userRepository.GetUser(problem.RequesterId);
                problem.Requester = _mapper.Map<User>(requester);
            }

            await _problemRepository.CreateProblem(problem);
            return Ok(problem);
        }

        // PUT: api/problems/:problemId
        [HttpPut("{problemId}")]
        public async Task<ActionResult> CheckProblemSolved(int problemId, [FromBody] Problem problem)
        {
            if (problemId != problem.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (problem.RequesterId is not null)
            {
                var requester = await _userRepository.GetUser(problem.RequesterId);
                problem.Requester = _mapper.Map<User>(requester);
            }

            await _problemRepository.CheckProblemSolved(problem);
            return NoContent();
        }

        // GET: api/problems/export
        [HttpGet("export")]
        public async Task<IActionResult> ExportProblemsToCsv()
        {
            var csvStream = await _problemRepository.ExportProblemsToCsv();
            return File(csvStream, "text/csv", "problems.csv");
        }
    }
}