using MinimalJwt.Models;
using MinimalJwt.Repositories;

namespace MinimalJwt.Services;

public class MovieService : IMovieService {
    public Movie Create(Movie movie) {
        movie.Id = MovieRepository.Movies.Count + 1;
        MovieRepository.Movies.Add(movie);
        return movie;
    }

    public Movie? Get(int id) {
        return MovieRepository.Movies.FirstOrDefault(t => t.Id.Equals(id));
    }

    public List<Movie> List() {
        return MovieRepository.Movies;
    }

    public Movie? Update(Movie movie) {
        var updMovie = MovieRepository.Movies.FirstOrDefault(t => t.Id.Equals(movie.Id));
        if (updMovie is null)
            return null;
        updMovie.Title = movie.Title;
        updMovie.Description = movie.Description;
        updMovie.Rating = movie.Rating;
        return updMovie;
    }

    public bool Delete(int id) {
        return MovieRepository.Movies.RemoveAll(t => t.Id.Equals(id)) > 0;
    }
}