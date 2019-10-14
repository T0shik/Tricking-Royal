using System.Collections.Generic;

namespace Battles.Application.ViewModels.Matches
{
    public class MatchCommentsViewModel
    {
        public CommentViewModel MainComment { get; set; }
        public IEnumerable<CommentViewModel> SubComments { get; set; }
    }
}