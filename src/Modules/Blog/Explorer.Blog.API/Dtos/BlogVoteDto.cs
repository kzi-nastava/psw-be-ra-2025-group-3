using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Blog.API.Dtos
{
    public class BlogVoteDto
    {
        public long BlogId { get; set; }
        public bool IsUpvote { get; set; }
    }
}
