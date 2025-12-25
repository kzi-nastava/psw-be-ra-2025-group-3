using Explorer.Blog.API.Public;
using Explorer.Blog.Core.Domain.Newsletter;
using Explorer.Blog.Core.Domain.RepositoryInterfaces;
using System;
using System.Net.Mail;

namespace Explorer.Blog.Core.UseCases
{
    public class NewsletterService : INewsletterService
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

            // osnovna validacija formata
            try
            {
                _ = new MailAddress(email);
            }
            catch
            {
                throw new ArgumentException("Invalid email format.");
            }

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
