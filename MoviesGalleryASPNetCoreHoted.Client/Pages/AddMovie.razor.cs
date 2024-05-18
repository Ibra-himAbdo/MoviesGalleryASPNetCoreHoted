using MudBlazor;
using System.Runtime.InteropServices;

namespace MoviesGalleryASPNetCoreHoted.Client.Pages
{
    public partial class AddMovie
    {
        private MudForm? form;
        private AddMovieDto? NewMovie = new();
        private string? genre = string.Empty;
        private string? actors = string.Empty;
        private IBrowserFile? posterFile;
        private bool IsDisabled = false;

        protected async Task Submit()
        {
            IsDisabled = true;
            await form!.Validate();
            if (form.IsValid)
            {
                NewMovie!.Genre = genre!.Split(',');
                NewMovie!.Actors = actors!.Split(',');
                await OnValidSubmit();

            }
            IsDisabled = false;
        }
        private async Task OnValidSubmit()
        {
            try
            {
                using var content = new MultipartFormDataContent();
                content.Add(new StringContent(NewMovie!.Title ?? string.Empty), "Title");
                content.Add(new StringContent(NewMovie.Year.ToString()), "Year");
                content.Add(new StringContent(NewMovie.Rating.ToString()), "Rating");
                content.Add(new StringContent(NewMovie.Director ?? string.Empty), "Director");
                content.Add(new StringContent(NewMovie.Plot ?? string.Empty), "Plot");
                content.Add(new StringContent(NewMovie.Trailer ?? string.Empty), "Trailer");

                if (NewMovie.Genre != null)
                {
                    foreach (var genre in NewMovie.Genre)
                    {
                        content.Add(new StringContent(genre), "Genre");
                    }
                }

                if (NewMovie.Actors != null)
                {
                    foreach (var actor in NewMovie.Actors)
                    {
                        content.Add(new StringContent(actor), "Actors");
                    }
                }

                if (posterFile != null)
                {
                    var stream = posterFile.OpenReadStream();
                    var streamContent = new StreamContent(stream);
                    streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(posterFile.ContentType);
                    content.Add(streamContent, "PosterFile", posterFile.Name);
                }

                HttpResponseMessage response = await _httpClient.PostAsync("api/Movies", content);
                var serverResponse = await response.Content.ReadAsStringAsync();
                var result = serverResponse.GetServiceResponse();
                if (response.IsSuccessStatusCode && result.Success)
                {
                    NewMovie = new();
                    genre = string.Empty;
                    actors = string.Empty;
                    StateHasChanged();
                    Snackbar!.Add("Added!", MudBlazor.Severity.Success);
                }
                else
                {
                    Snackbar!.Add($"Failed to add movie: {result.Message}", Severity.Error);
                }
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"Error: {ex.Message}");
            }
        }
        private async Task OnInputFileChange(InputFileChangeEventArgs e)
        {
            await Task.Run(() => 
            posterFile = e.File);
        }

    }
}