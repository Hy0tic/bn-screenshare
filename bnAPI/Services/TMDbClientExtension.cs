using System.Text.Json;
using bnAPI.Models;
using RestSharp;

namespace BnScreenshareAPI.Services;

public class TMDbClientExtension
{
    private string ApiKey { get; set; }

    public TMDbClientExtension(string apiKey)
    {
        this.ApiKey = apiKey;
    }
    public async Task<DiscoverMovieResponse> GetAnimatedMovieOnly(int pageNumber)
    {
        var options = new RestClientOptions($"https://api.themoviedb.org/3/discover/movie?include_adult=true&include_video=true&language=en-US&page={pageNumber}&sort_by=popularity.desc&with_genres=16");
        var client = new RestClient(options);
        var request = new RestRequest("");
        
        request.AddQueryParameter("api_key", ApiKey);
        request.AddHeader("accept", "application/json");
        
        var response = await client.GetAsync(request);

        var jsonString = response.Content;
        var result = JsonSerializer.Deserialize<DiscoverMovieResponse>(jsonString);

        return result;

    }

}