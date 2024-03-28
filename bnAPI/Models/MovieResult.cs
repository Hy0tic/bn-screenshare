namespace bnAPI.Models;

public sealed class MovieResult
{
    public uint Id { get; set; }
    public string Title { get; set; }
    public uint[] GenreIds { get; set; }
    public string Overview { get; set; }
    public double Popularity { get; set; }
    public string PosterUrl { get; set; }
    public double VoteAverage { get; set; }
    public DateOnly ReleaseDate { get; set; }
    public int VoteCount { get; set; }
}