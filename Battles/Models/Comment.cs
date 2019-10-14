using System.Collections.Generic;
using Battles.Domain.Models;

namespace Battles.Models
{
    public class Comment
    {
        public Comment()
        {
            SubComments = new List<SubComment>();
        }

        public int Id { get; set; }

        public int MatchId { get; set; }
        public Match Match { get; set; }

        public string Message { get; set; }
        public string Picture { get; set; }
        public string DisplayName { get; set; }

        public virtual ICollection<SubComment> SubComments { get; set; }
    }
}