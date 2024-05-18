namespace MoviesGalleryASPNetCoreHoted.Client.Pages;

public partial class Home
{
    private List<GetMovieDto>? movies;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var json = await _httpClient.GetStringAsync("api/Movies/GetAll");

            var response = json.GetServiceResponse();
            if (response!.Success || response is not null)
                movies = response.Data;
        }

        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching movies: {ex.Message}");
        }
    }
}
