using System.Linq;
using System.Threading.Tasks;
using Battles.Models;
using Microsoft.EntityFrameworkCore;

namespace TrickingRoyal.Database.Queries
{
    public static class VideoQueries
    {
        public static Task<Video> GetVideo(this AppDbContext @this, int matchId, string userId)
        {
            return @this.Videos
                .Where(x => x.MatchId == matchId && x.UserId == userId)
                .OrderByDescending(x => x.VideoIndex)
                .FirstOrDefaultAsync();
        }
        
        public static Task<Video> GetVideo(this AppDbContext @this, int matchId, string userId, int index)
        {
            return @this.Videos
                .Where(x => x.MatchId == matchId && x.UserId == userId)
                .OrderByDescending(x => x.VideoIndex)
                .FirstOrDefaultAsync();
        }
    }
}