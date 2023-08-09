using MinimalJwt.Models;

namespace MinimalJwt.Repositories;

public abstract class UserRepository {
    public static readonly List<User> Users = new() {
        new User {
            UserName = "luke",
            Email = "luke.admin@email.com",
            Password = "123",
            GivenName = "Luke",
            Surname = "Rogers",
            Role = "Administrator"
        },
        new User {
            UserName = "andrew",
            Email = "andrew.standart@email.com",
            Password = "123",
            GivenName = "Andrew",
            Surname = "Who",
            Role = "Standart"
        }
    };
}