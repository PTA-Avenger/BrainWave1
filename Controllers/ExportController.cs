using BrainWave.Api.DTOs;
using BrainWave.Api.Entities;
using BrainWave.API.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BrainWave.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExportController : ControllerBase
    {
        private readonly ExportRepository _repo;
        public ExportController(ExportRepository repo) => _repo = repo;

        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<ExportDtos>>> GetExports(int userId)
        {
            var exports = await _repo.GetExportsByUserIdAsync(userId);
            return Ok(exports.Select(e => new ExportDtos
            {
                ExportID = e.ExportID,
                Export_Format = e.Export_Format,
                Date_Requested = e.Date_Requested
            }));
        }

        [HttpPost]
        public async Task<ActionResult<ExportDtos>> CreateExport([FromBody] ExportDtos dto)
        {
            var export = new Export
            {
                UserID = dto.UserID,  // in real app: replace with logged-in UserID from JWT
                TaskID = dto.TaskID,  // make sure client passes TaskID
                Export_Format = dto.Export_Format,
                Date_Requested = DateTime.UtcNow
            };

            await _repo.AddExportAsync(export);

            dto.ExportID = export.ExportID;
            dto.Date_Requested = export.Date_Requested;
            return Ok(dto);
        }
    }
}
