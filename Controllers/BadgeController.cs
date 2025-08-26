using BrainWave.Api.DTOs;
using BrainWave.Api.Entities;
using BrainWave.API.DTOs;
using BrainWave.API.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BrainWave.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BadgeController : ControllerBase
    {
        private readonly BadgeRepository _repo;
        public BadgeController(BadgeRepository repo) => _repo = repo;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BadgeDtos>>> GetBadges()
        {
            var badges = await _repo.GetAllBadgesAsync();
            return Ok(badges.Select(b => new BadgeDtos
            {
                BadgeID = b.BadgeID,
                Badge_Type = b.Badge_Type,
                Badge_Description = b.Badge_Description
            }));
        }

        [HttpPost]
        public async Task<ActionResult<BadgeDtos>> CreateBadge([FromBody] BadgeDtos dto)
        {
            var badge = new Badge
            {
                Badge_Type = dto.Badge_Type,
                Badge_Description = dto.Badge_Description
            };

            await _repo.AddBadgeAsync(badge);

            dto.BadgeID = badge.BadgeID;
            return Ok(dto);
        }

        [HttpPost("assign/{userId}/{badgeId}")]
        public async Task<IActionResult> AssignBadge(int userId, int badgeId)
        {
            var success = await _repo.AssignBadgeAsync(userId, badgeId);
            if (!success) return BadRequest("Failed to assign badge.");

            return Ok();
        }
    }
}
