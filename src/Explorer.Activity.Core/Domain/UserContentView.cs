using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.Domain;
using System;

namespace Explorer.Activity.Core.Domain
{
    public class UserContentView : Entity
    {
        public int UserId { get; private set; }
        public long ContentId { get; private set; }
        public ContentType ContentType { get; private set; }
        public DateTime ViewedAt { get; private set; }

        private UserContentView() { } // EF

        public UserContentView(int userId, long contentId, ContentType contentType)
        {
            UserId = userId;
            ContentId = contentId;
            ContentType = contentType;
            ViewedAt = DateTime.UtcNow;
        }
    }
}
