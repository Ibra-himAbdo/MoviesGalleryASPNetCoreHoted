using MoviesGalleryASPNetCoreHoted.Shared.Models;
using System.Net.Http;

namespace MoviesGalleryASPNetCoreHoted.Client.Pages;

public partial class DeleteMovie
{
    private List<GetMovieDto> movies = new();
    private string[]? moviesTiltes;
    private string value = string.Empty;
    GetMovieDto? selectedMovie = null;

    private async Task OnDelete()
    {
       var response = await _httpClient.DeleteAsync($"api/Movies/{selectedMovie!.Id}");
        if (response.IsSuccessStatusCode)
        {
            await OnInitializedAsync();
            selectedMovie = null;
            value = string.Empty;
            Snackbar.Add("Deleted!", MudBlazor.Severity.Info);
        }
    }

    private async Task<IEnumerable<string>> Search(string value)
    {
        // In real life use an asynchronous function for fetching data from an api.
        await Task.Delay(5);

        // if text is null or empty, show complete list
        if (string.IsNullOrEmpty(value))
            return moviesTiltes!;
        await Console.Out.WriteLineAsync(value);
        return moviesTiltes!.Where(x => x.Contains(value, StringComparison.InvariantCultureIgnoreCase));
    }

    private void GetSelectedMovie()
    {
        if (!string.IsNullOrEmpty(value) && movies.Any(m => m.Title!.ToLower() == value.ToLower()))
            selectedMovie = movies.FirstOrDefault(m => m.Title == value); 
    }
    protected override async Task OnInitializedAsync()
    {
        try
        {
            var json = await _httpClient.GetStringAsync("api/Movies/GetAll");

            var response = json.GetServiceResponse();
            if (response!.Success || response is not null)
            {
                movies = response.Data!;
                moviesTiltes = movies.Select(m => m.Title).ToArray()!;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching movies: {ex.Message}");
        }
    }

}
