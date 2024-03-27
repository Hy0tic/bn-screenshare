using bnAPI.Models;
using BnScreenshareAPI.Services;
using Microsoft.AspNetCore.Mvc;
using TMDbLib.Client;

namespace bnAPI.Controllers;

[ApiController]
public class MovieController : Controller
{
    private readonly TMDbClient tmdbClient;
    private readonly TMDbClientExtension tmDbClientExtension;
    public MovieController(TMDbClient tmdbClient, TMDbClientExtension tmDbClientExtension)
    {
        this.tmdbClient = tmdbClient;
        this.tmDbClientExtension = tmDbClientExtension;
    }
    
    [HttpGet("/Recommend")]
    public async Task<IActionResult> Recommend()
    {
        var nowPlayingMovies = await tmdbClient.GetMovieNowPlayingListAsync();

        var filteredMovieList = nowPlayingMovies.Results.Where(m => m.VoteAverage > 5
                                                                    && m.Popularity > 500.0 
                                                                    && (m.GenreIds.Contains((int)MovieGenre.Mystery)
                                                                        || m.GenreIds.Contains((int)MovieGenre.Thriller)
                                                                        || m.GenreIds.Contains((int)MovieGenre.ScienceFiction)
                                                                        || m.GenreIds.Contains((int)MovieGenre.Horror))
                                                                        || m.GenreIds.Contains((int)MovieGenre.Animation)
                                                                        || m.GenreIds.Contains((int)MovieGenre.Comedy)
                                                                    );
        return Ok(filteredMovieList);
    }

    [HttpGet("/AnimatedMoviesRecommend")]
    public async Task<IActionResult> CasualWatchRecommend(int pageNumber = 1)
    {
        var response = await tmDbClientExtension.GetAnimatedMovieOnly(pageNumber);
        return Ok(response.results);
    }

}

