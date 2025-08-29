using BrainWave.Api.DTOs;
using BrainWave.Api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;

namespace BrainWave.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CollaborationController : ControllerBase
    {
        private readonly CollaborationRepository _repo;
        private readonly UserRepository _userRepo;

        public CollaborationController(CollaborationRepository repo, UserRepository userRepo)
        {
            _repo = repo;
            _userRepo = userRepo;
        }

        [HttpGet("{taskId}")]
        public async Task<ActionResult<IEnumerable<CollaborationDtos>>> GetCollaborations(int taskId)
        {
            var collabs = await _repo.GetCollaborationsByTaskIdAsync(taskId);
            var collaborationDtos = new List<CollaborationDtos>();

            foreach (var c in collabs)
            {
                var collaborators = await _repo.GetCollaboratorNamesAsync(c.CollaborationID);
                collaborationDtos.Add(new CollaborationDtos
                {
                    CollaborationID = c.CollaborationID,
                    TaskID = c.TaskID,
                    Collaboration_Title = c.Collaboration_Title,
                    Collaboration_Description = c.Collaboration_Description,
                    InvitePin = c.InvitePin,
                    Created_Date = c.Created_Date,
                    Pin_Expiry = c.Pin_Expiry,
                    IsActive = c.IsActive,
                    CollaboratorNames = collaborators
                });
            }

            return Ok(collaborationDtos);
        }

        [HttpPost]
        public async Task<ActionResult<CollaborationDtos>> CreateCollaboration([FromBody] CreateCollaborationDto dto)
        {
            // Generate a 6-digit PIN
            var random = new Random();
            var pin = random.Next(100000, 999999).ToString();

            var collab = new Collaboration
            {
                TaskID = dto.TaskID,
                Collaboration_Title = dto.Collaboration_Title,
                Collaboration_Description = dto.Collaboration_Description,
                InvitePin = pin,
                Created_Date = DateTime.UtcNow,
                Pin_Expiry = DateTime.UtcNow.AddHours(24), // PIN expires in 24 hours
                IsActive = true
            };

            await _repo.AddCollaborationAsync(collab);

            return Ok(new CollaborationDtos
            {
                CollaborationID = collab.CollaborationID,
                TaskID = collab.TaskID,
                Collaboration_Title = collab.Collaboration_Title,
                Collaboration_Description = collab.Collaboration_Description,
                InvitePin = collab.InvitePin,
                Created_Date = collab.Created_Date,
                Pin_Expiry = collab.Pin_Expiry,
                IsActive = collab.IsActive,
                CollaboratorNames = new List<string>()
            });
        }

        [HttpPost("join")]
        public async Task<IActionResult> JoinCollaboration([FromBody] JoinCollaborationDto dto)
        {
            var collaboration = await _repo.GetCollaborationByPinAsync(dto.InvitePin);
            if (collaboration == null)
            {
                return BadRequest("Invalid PIN");
            }

            if (collaboration.Pin_Expiry < DateTime.UtcNow)
            {
                return BadRequest("PIN has expired");
            }

            if (!collaboration.IsActive)
            {
                return BadRequest("Collaboration is not active");
            }

            // Check if user is already a collaborator
            var isAlreadyCollaborator = await _repo.IsUserCollaboratorAsync(collaboration.CollaborationID, dto.UserID);
            if (isAlreadyCollaborator)
            {
                return BadRequest("User is already a collaborator");
            }

            await _repo.AddUserToCollaborationAsync(collaboration.CollaborationID, dto.UserID);
            return Ok(new { Message = "Successfully joined collaboration" });
        }

        [HttpPost("{id}/regenerate-pin")]
        public async Task<ActionResult<string>> RegeneratePin(int id)
        {
            var collaboration = await _repo.GetCollaborationByIdAsync(id);
            if (collaboration == null) return NotFound();

            var random = new Random();
            var newPin = random.Next(100000, 999999).ToString();

            collaboration.InvitePin = newPin;
            collaboration.Pin_Expiry = DateTime.UtcNow.AddHours(24);

            await _repo.UpdateCollaborationAsync(collaboration);
            return Ok(new { InvitePin = newPin, Pin_Expiry = collaboration.Pin_Expiry });
        }

        [HttpDelete("{collaborationId}/users/{userId}")]
        public async Task<IActionResult> RemoveUserFromCollaboration(int collaborationId, int userId)
        {
            await _repo.RemoveUserFromCollaborationAsync(collaborationId, userId);
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCollaboration(int id, [FromBody] CreateCollaborationDto dto)
        {
            var collaboration = await _repo.GetCollaborationByIdAsync(id);
            if (collaboration == null) return NotFound();

            collaboration.Collaboration_Title = dto.Collaboration_Title;
            collaboration.Collaboration_Description = dto.Collaboration_Description;

            await _repo.UpdateCollaborationAsync(collaboration);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCollaboration(int id)
        {
            var collaboration = await _repo.GetCollaborationByIdAsync(id);
            if (collaboration == null) return NotFound();

            await _repo.DeleteCollaborationAsync(collaboration);
            return NoContent();
        }
    }
}
