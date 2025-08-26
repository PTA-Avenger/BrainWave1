using BrainWave.Api.DTOs;
using BrainWave.Api.Entities;
using Microsoft.AspNetCore.Mvc;
using System;

namespace BrainWave.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CollaborationController : ControllerBase
    {
        private readonly CollaborationRepository _repo;
        public CollaborationController(CollaborationRepository repo) => _repo = repo;

        [HttpGet("{taskId}")]
        public async Task<ActionResult<IEnumerable<CollaborationDtos>>> GetCollaborations(int taskId)
        {
            var collabs = await _repo.GetCollaborationsByTaskIdAsync(taskId);
            return Ok(collabs.Select(c => new CollaborationDtos
            {
                CollaborationID = c.CollaborationID,
                Collaboration_Title = c.Collaboration_Title,
                Collaboration_Description = c.Collaboration_Description
            }));
        }

        [HttpPost]
        public async Task<ActionResult<CollaborationDtos>> CreateCollaboration([FromBody] CollaborationDtos dto)
        {
            var collab = new Collaboration
            {
                TaskID = 1, // TODO: pass TaskID
                Collaboration_Title = dto.Collaboration_Title,
                Collaboration_Description = dto.Collaboration_Description
            };

            await _repo.AddCollaborationAsync(collab);

            dto.CollaborationID = collab.CollaborationID;
            return Ok(dto);
        }

        [HttpPost("add-user/{collaborationId}/{userId}")]
        public async Task<IActionResult> AddUserToCollaboration(int collaborationId, int userId, [FromQuery] string role)
        {
            // This would require a method in your repository to add a user to a collaboration
            // For now, just return Ok();
            return Ok();
        }
    }
}
