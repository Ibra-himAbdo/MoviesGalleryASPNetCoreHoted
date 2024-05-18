using MudBlazor;
using System.Net.Http;
using static MudBlazor.Colors;

namespace MoviesGalleryASPNetCoreHoted.Client.Pages
{
    public partial class EditMovie
    {
        private List<GetMovieDto> movies = new();
        private string[]? moviesTiltes;
        private MudForm? form;
        private string value = string.Empty;
        private UpdateMovieDto UpdateMovie = new();
        private IBrowserFile? posterFile;
        private string? genre = string.Empty;
        private string? actors = string.Empty;
        private bool IsDisabled = false;

        protected async Task Submit()
        {
            IsDisabled = true;
            await form!.Validate();
            if (form.IsValid)
            {
                UpdateMovie.Genre = genre!.Split(',');
                UpdateMovie.Actors = actors!.Split(',');
                await OnValidSubmit();
            }
            IsDisabled = false;
        }

        private async Task OnValidSubmit()
        {
            try
            {
                using var content = new MultipartFormDataContent();
                content.Add(new StringContent(UpdateMovie.Id.ToString()), "Id");
                content.Add(new StringContent(UpdateMovie.Title ?? string.Empty), "Title");
                content.Add(new StringContent(UpdateMovie.Year.ToString()), "Year");
                content.Add(new StringContent(UpdateMovie.Rating.ToString()), "Rating");
                content.Add(new StringContent(UpdateMovie.Director ?? string.Empty), "Director");
                content.Add(new StringContent(UpdateMovie.Plot ?? string.Empty), "Plot");
                content.Add(new StringContent(UpdateMovie.Trailer ?? string.Empty), "Trailer");

                if (UpdateMovie.Genre != null)
                    foreach (var genre in UpdateMovie.Genre)
                        content.Add(new StringContent(genre), "Genre");

                if (UpdateMovie.Actors != null)
                    foreach (var actor in UpdateMovie.Actors)
                        content.Add(new StringContent(actor), "Actors");

                if (posterFile != null)
                {
                    var stream = posterFile.OpenReadStream();
                    var streamContent = new StreamContent(stream);
                    streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(posterFile.ContentType);
                    content.Add(streamContent, "PosterFile", posterFile.Name);
                }

                HttpResponseMessage response = await _httpClient.PutAsync("api/Movies", content);
                var serverResponse = await response.Content.ReadAsStringAsync();
                var result = serverResponse.GetServiceResponse();

                if (response.IsSuccessStatusCode && result.Success)
                {
                    UpdateMovie = new();
                    genre = string.Empty;
                    actors = string.Empty;
                    Snackbar!.Add("Updated!", MudBlazor.Severity.Success);
                    StateHasChanged();
                }
                else
                {
                    Snackbar!.Add($"Failed to Update: {result.Message}", Severity.Error);
                }

            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"Error: {ex.Message}");
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

        private void MapUpdateMovieDto()
        {
            if (!string.IsNullOrEmpty(value) && movies.Any(m => m.Title!.ToLower() == value.ToLower()))
            {
                var movie = movies.FirstOrDefault(m => m.Title!.ToLower() == value.ToLower());
                UpdateMovie.Id = movie!.Id;
                UpdateMovie.Title = movie!.Title;
                UpdateMovie.Year = movie!.Year;
                UpdateMovie.Rating = movie!.Rating;
                UpdateMovie.Director = movie!.Director;
                UpdateMovie.Plot = movie!.Plot;
                UpdateMovie.Trailer = movie!.Trailer;
                genre = string.Join(",", movie!.Genre!);
                actors = string.Join(",", movie!.Actors!);
                Console.WriteLine(UpdateMovie.ToString());
            }
        }
        private async Task OnInputFileChange(InputFileChangeEventArgs e)
        {
            await Task.Run(() =>
            posterFile = e.File);
        }
    }
}