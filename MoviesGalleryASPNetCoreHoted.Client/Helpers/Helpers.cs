namespace MoviesGalleryASPNetCoreHoted.Client.Helpers;

public static class Helpers
{
    public static IFormFile ConvertToIFormFile(this IBrowserFile browserFile)
    {
        // Read the stream from the IBrowserFile
        var stream = browserFile.OpenReadStream(browserFile.Size);

        // Create an instance of FormFile with the stream, file name, and content type
        var formFile = new FormFile(stream, 0, browserFile.Size, browserFile.Name, browserFile.ContentType);

        return formFile;
    }

    public static ServiceResponse<List<GetMovieDto>> GetServiceResponse(this string json)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        return JsonSerializer.Deserialize<ServiceResponse<List<GetMovieDto>>>(json, options)!;
    }
}



