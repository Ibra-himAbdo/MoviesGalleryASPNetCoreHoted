namespace MoviesGalleryASPNetCoreHoted.Shared.Models;

public class ServiceResponse<T>
{
    [JsonPropertyName("data")]
    public T? Data { get; set; }
    [JsonPropertyName("success")]
    public bool Success { get; set; } = true;
    [JsonPropertyName("message")]
    public string? Message { get; set; } = string.Empty;
}
