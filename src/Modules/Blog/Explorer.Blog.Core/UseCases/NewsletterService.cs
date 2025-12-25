using Explorer.Blog.Core.Domain.Newsletter;
using Explorer.Blog.Core.Domain.RepositoryInterfaces;

namespace Explorer.Blog.Core.UseCases
{
    public class NewsletterService
    {
        private readonly INewsletterRepository _repository;

        public NewsletterService(INewsletterRepository repository)
        {
            _repository = repository;
        }

        public void Subscribe(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.");

            if (_repository.Exists(email))
                throw new InvalidOperationException("Email already subscribed.");

            var subscriber = new NewsletterSubscriber
            {
                Email = email,
                SubscribedAt = DateTime.UtcNow
            };

            _repository.Add(subscriber);
        }
    }
}
