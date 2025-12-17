using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Blog.Core.Domain.Blogs
{
    public class BlogRating : Entity
    {
        public long BlogId { get; private set; }
        public int UserId { get; private set; }
        public VoteType VoteType { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private BlogRating() { }

        public BlogRating(long blogId, int userId, VoteType voteType, DateTime createdAt)
        {
            BlogId = blogId;
            UserId = userId;

            if (!Enum.IsDefined(typeof(VoteType), voteType))
                throw new ArgumentException("Invalid vote type.", nameof(voteType));

            VoteType = voteType;
            CreatedAt = createdAt;
        }

        public void ChangeVote(VoteType newVoteType, DateTime now)
        {
            if (!Enum.IsDefined(typeof(VoteType), newVoteType))
                throw new ArgumentException("Invalid vote type.", nameof(newVoteType));

            VoteType = newVoteType;
            CreatedAt = now;
        }
    }
}
