using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Battles.Cdn.FileServices;
using Battles.Cdn.Infrastructure;
using TrickingRoyal.Database;

namespace Battles.Cdn.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class VideoController : BaseController
    {
        private readonly AppDbContext _ctx;
        private readonly VideoManager _videoManager;
        private readonly ILogger<VideoController> _logger;

        public VideoController(
            AppDbContext ctx,
            VideoManager videoManager,
            ILogger<VideoController> logger)
        {
            _ctx = ctx;
            _videoManager = videoManager;
            _logger = logger;
        }

        [HttpPost("{matchId}/{task}")]
        public async Task<IActionResult> SaveVideo(
            int matchId,
            string task,
            IFormFile video)
        {
            try
            {
                if (task != "update" && task != "upload")
                {
                    return BadRequest("Invalid Action");
                }

                switch (task)
                {
                    case "upload" when !await _ctx.CanGo(matchId, UserId):
                        return BadRequest("Not allowed to go");
                    case "update" when !await _ctx.CanUpdate(matchId, UserId):
                        return BadRequest("Not allowed to update");
                    default:
                    {
                        var videoName = await _videoManager.SaveAsync(matchId.ToString(), video);
                        return Ok(videoName);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to save video");
            }

            return BadRequest("Failed To Save Initial Video");
        }
    }
}