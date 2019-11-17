using Battles.Cdn.ViewModels;
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

        [HttpPost("{matchId}")]
        public async Task<IActionResult> SaveVideo(int matchId, IFormFile video)
        {
            try
            {
                if (!await _ctx.CanGo(matchId, UserId))
                {
                    return BadRequest("Not allowed to go");
                }

                var (complete, response) = await _videoManager.SaveInitVideoAsync(matchId.ToString(), video);

                if (complete)
                {
                    return Ok(response);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }

            return BadRequest("Failed To Save Initial Video");
        }

        [Authorize]
        [HttpPost("[controller]/update/{matchId}")]
        public async Task<IActionResult> UploadInitialVideoForReUploadIndex(
            int matchId,
            IFormFile video,
            [FromServices] AppDbContext ctx)
        {
            try
            {
                var canUpdate = await ctx.CanUpdate(matchId, UserId);
                if (!canUpdate)
                {
                    return BadRequest("Not allowed to update");
                }

                var (complete, response) = await _videoManager.SaveInitVideoAsync(matchId.ToString(), video);
                if (complete)
                {
                    return Ok(response);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }

            return BadRequest("Failed To Save Video");
        }
    }
}