using MinimalJwt.Models;

namespace MinimalJwt.Repositories;

public abstract class MovieRepository {
    public static readonly List<Movie> Movies = new() {
        new Movie { Id = 1, Title = "Eternals", Description = "The saga of the Eternals, a race of immortal beings who lived on Earth and shaped its history and civilizations.", Rating = 6.8 },
        new Movie { Id = 2, Title = "Dune", Description = "Feature adaptation of Frank Herbert's science fiction novel, about the son of a noble family entrusted with the protection of the most valuable asset and most vital element in the galaxy.", Rating = 8.2 },
        new Movie { Id = 3, Title = "The Harder They Fall", Description = "When an outlaw discovers his enemy is being released from prison, he reunites his gang to seek revenge in this Western.", Rating = 6.6 },
        new Movie { Id = 4, Title = "Red Notice", Description = "An Interpol agent tracks the world's most wanted art thief.", Rating = 6.4 },
        new Movie { Id = 5, Title = "No Time to Die", Description = "James Bond has left active service. His peace is short-lived when Felix Leiter, an old friend from the CIA, turns up asking for help, leading Bond onto the trail of a mysterious villain armed with dangerous new technology.", Rating = 7.4 }
    };
}