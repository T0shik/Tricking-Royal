using Battles.Cdn.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Battles.Cdn.FileServices;
using Battles.Cdn.Infrastructure;
using Battles.Enums;
using TrickingRoyal.Database;

namespace Battles.Cdn.Controllers
{
    [Authorize]
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

        [HttpPost("[controller]/init/{matchId}")]
        public async Task<IActionResult> SaveInitialVideo(int matchId, IFormFile video)
        {
            try
            {
                if (!await _ctx.CanGo(matchId, UserId))
                {
                    return BadRequest("Not allowed to go");
                }

                var (complete, response) = await _videoManager.SaveInitVideoAsync(matchId.ToString(), video);
                if (complete)
                    return Ok(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }

            return BadRequest("Failed To Save Initial Video");
        }

        [HttpPost("[controller]/trim/{id}")]
        public async Task<IActionResult> TrimInitialVideo(string id, [FromBody] TrimOptions options)
        {
            try
            {
                var (complete, filePaths) =
                    await _videoManager.TrimVideoAsync(id, options.Video, options.Start, options.End);
                if (complete)
                    return Ok(filePaths);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }

            return BadRequest("Failed To Trim Video");
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