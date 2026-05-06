using SaigonRide.Models;
using SaigonRide.Repositories;

namespace SaigonRide.Services
{
    public class AuthService
    {
        private readonly UserRepository _repo;

        public AuthService(UserRepository repo)
        {
            _repo = repo;
        }

        public User Login(string email, string password)
        {
            return _repo.GetUser(email, password);
        }

        public void Register(User user)
        {
            _repo.AddUser(user);
        }
    }
}