using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Explorer.Blog.Core.Domain.Newsletter;

namespace Explorer.Blog.Core.Domain.RepositoryInterfaces
{
    public interface INewsletterRepository
    {
        bool Exists(string email);
        void Add(NewsletterSubscriber subscriber);
    }
}

