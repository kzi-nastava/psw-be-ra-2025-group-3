using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Blog.API.Dtos
{
    public class BlogVoteStateDto
    {
        public long BlogId { get; set; }
        public bool? IsUpvote { get; set; }
        public int Score { get; set;}
        public int UpvoteCount { get; set; }
        public int DownvoteCount { get; set; }
        public int BlogStatus { get; set; }
    }
}
