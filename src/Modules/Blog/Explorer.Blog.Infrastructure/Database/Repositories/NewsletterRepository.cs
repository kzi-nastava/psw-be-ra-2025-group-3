using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Explorer.Blog.Core.Domain.Newsletter;
using Explorer.Blog.Core.Domain.RepositoryInterfaces;

namespace Explorer.Blog.Infrastructure.Database.Repositories
{
    public class NewsletterRepository : INewsletterRepository
    {
        private readonly BlogContext _context;

        public NewsletterRepository(BlogContext context)
        {
            _context = context;
        }

        public bool Exists(string email)
        {
            return _context.NewsletterSubscribers.Any(n => n.Email == email);
        }

        public void Add(NewsletterSubscriber subscriber)
        {
            _context.NewsletterSubscribers.Add(subscriber);
            _context.SaveChanges();
        }
    }
}
