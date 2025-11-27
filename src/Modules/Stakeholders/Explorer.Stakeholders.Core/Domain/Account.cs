using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain
{
    public enum AccountRole
    {
        Admin,
        Author,
        Tourist
    }

    public enum AccountStatus
    {
        Active,
        Blocked
    }
    public class Account : Entity
    {

        public string Username { get; private set; }
        public string Password { get; private set; }
        public string Email { get; private set; }
        public AccountRole Role { get; private set; }
        public AccountStatus Status { get; private set; }

        protected Account() { }

        public Account(string username, string password, string email, AccountRole role)
        {
            Username = username;
            Password = password;
            Email = email;
            Role = role;
            Status = AccountStatus.Active;
            Validate();
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(Username))
                throw new ArgumentException("Username is required.");

            if (string.IsNullOrWhiteSpace(Password))
                throw new ArgumentException("Password is required.");

            if (string.IsNullOrWhiteSpace(Email))
                throw new ArgumentException("Email is required.");
        }
        public void Block()
        {
            Status = AccountStatus.Blocked;
        }

        public void Unblock()
        {
            Status = AccountStatus.Active;
        }
    }
}
