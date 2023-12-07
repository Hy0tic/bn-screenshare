using Microsoft.AspNetCore.Mvc;
using TMDbLib.Client;

namespace bnAPI.Controllers;

[ApiController]
public class MovieController : Controller
{
    public MovieController(TMDbClient tmdbClient)
    {
        this.tmdbClient = tmdbClient;
    }

    private TMDbClient tmdbClient { get;set; }

    [HttpGet("/searchMovie")]
    public async Task<IActionResult> SearchMovie(string searchInput)
    {
        var movieSearch = await tmdbClient.SearchMovieAsync(searchInput);
        return Ok(movieSearch.Results);
    }

    [HttpGet("/popularMovies")]
    public async Task<IActionResult> GetPopularMovies()
    {
        var response = await tmdbClient.GetMoviePopularListAsync();
        return Ok(response.Results);
    }

    [HttpGet("/nowPlayingMovies")]
    public async Task<IActionResult> GetNowPlayingMovies()
    {
        var response = await tmdbClient.GetMovieNowPlayingListAsync();
        return Ok(response.Results);
    }

    [HttpGet("/TopRated")]
    public async Task<IActionResult> GetTopRatedMovies()
    {
        var response = await tmdbClient.GetMovieTopRatedListAsync();
        return Ok(response.Results);
    }
}