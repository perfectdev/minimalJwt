using MinimalJwt.Models;
using MinimalJwt.Repositories;

namespace MinimalJwt.Services;

public class UserService : IUserService {
    public User? Get(UserLogin userLogin) {
        var user = UserRepository.Users.FirstOrDefault(t =>
                                                           t.UserName.Equals(userLogin.UserName, StringComparison.OrdinalIgnoreCase) &&
                                                           t.Password.Equals(userLogin.Password));
        return user;
    }
}