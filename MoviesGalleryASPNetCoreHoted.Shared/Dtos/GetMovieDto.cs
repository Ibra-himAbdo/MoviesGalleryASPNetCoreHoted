namespace MoviesGalleryASPNetCoreHoted.Shared.Dtos;

public record GetMovieDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("title")]
    public string? Title { get; set; }
    [JsonPropertyName("year")]
    public int Year { get; set; }
    [JsonPropertyName("genre")]
    public string[]? Genre { get; set; }
    [JsonPropertyName("rating")]
    public float Rating { get; set; }
    [JsonPropertyName("director")]
    public string? Director { get; set; }
    [JsonPropertyName("actors")]
    public string[]? Actors { get; set; }
    [JsonPropertyName("plot")]
    public string? Plot { get; set; }
    [JsonPropertyName("posterURL")]
    public string? PosterURL { get; set; }
    [JsonPropertyName("trailer")]
    public string? Trailer { get; set; }
}
