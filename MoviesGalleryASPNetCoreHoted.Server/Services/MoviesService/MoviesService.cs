using Microsoft.AspNetCore.Components.Forms;

namespace MoviesGalleryASPNetCoreHoted.Server.Services.MoviesService;

public class MoviesService : IMoviesService
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public MoviesService(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ServiceResponse<List<GetMovieDto>>> AddMovie(AddMovieDto newMovie)
    {
        var serviceResponse = new ServiceResponse<List<GetMovieDto>>();
        var movie = _mapper.Map<Movie>(newMovie);

        if (await MovieExists(movie.Title!))
        {
            serviceResponse.Success = false;
            serviceResponse.Message = $"Movie with title: '{movie.Title}' already exists";
            return serviceResponse;
        }

        if (newMovie.PosterFile is not null)
            movie.PosterURL = await ConvertIntoBase64(newMovie.PosterFile);

        //saving new movie to db
        await _context.Movies.AddAsync(movie);
        await _context.SaveChangesAsync();

        serviceResponse.Data = await _context.Movies.Select(movie => _mapper.Map<GetMovieDto>(movie)).ToListAsync();
        serviceResponse.Message = "Movie added successfully";
        return serviceResponse;
    }

    public async Task<ServiceResponse<List<GetMovieDto>>> DeleteMovie(int id)
    {
        var serviceResponse = new ServiceResponse<List<GetMovieDto>>();
        try
        {
            var movie = 
                await _context.Movies.FirstOrDefaultAsync(movie => movie.Id == id);

            if (movie is null)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"Movie with Id '{id}' is not found";
                return serviceResponse;
            }

            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();

            serviceResponse.Data =
                await _context.Movies.Select(movie => _mapper.Map<GetMovieDto>(movie)).ToListAsync();
            serviceResponse.Message = "Movie deleted successfully";
        }
        catch (Exception ex)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = $"An error occurred while deleting the movie: {ex.Message}";
        }
        return serviceResponse;
    }

    public async Task<ServiceResponse<List<GetMovieDto>>> GetAllMovies()
    {
        var serviceResponse = new ServiceResponse<List<GetMovieDto>>();
        var dbMovies = await _context.Movies.ToListAsync();
        serviceResponse.Data = dbMovies.Select(movie => _mapper.Map<GetMovieDto>(movie)).ToList();
        return serviceResponse;
    }

    public async Task<ServiceResponse<GetMovieDto>> GetMovieById(int id)
    {
        var serviceResponse = new ServiceResponse<GetMovieDto>();
        var movie = await _context.Movies.FirstOrDefaultAsync(movie => movie.Id == id);
        serviceResponse.Data = _mapper.Map<GetMovieDto>(movie);
        return serviceResponse;
    }

    public async Task<ServiceResponse<List<GetMovieDto>>> GetMovieByPartialName(string partialName)
    {
        var serviceResponse = new ServiceResponse<List<GetMovieDto>>();
        try
        {
            var dbMovies = await _context.Movies
                .Where(movie => movie.Title!.ToLower().Contains(partialName.ToLower()))
                .ToListAsync();

            serviceResponse.Data = dbMovies.Select(movie => _mapper.Map<GetMovieDto>(movie)).ToList();
            serviceResponse.Message = "Movies found successfully";
        }
        catch (Exception ex)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = $"An error occurred while retrieving movies: {ex.Message}";
        }
        return serviceResponse;
    }

    public async Task<ServiceResponse<GetMovieDto>> UpdateMovie(UpdateMovieDto updatedMovie)
    {
        var serviceResponse = new ServiceResponse<GetMovieDto>();
        try
        {
            var movie = 
                await _context.Movies.FirstOrDefaultAsync(movie => movie.Id == updatedMovie.Id);

            if (movie is null)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"Movie with Id '{updatedMovie.Id}' is not found";
                return serviceResponse;
            }

            if (updatedMovie.PosterFile is not null)
                movie.PosterURL = await ConvertIntoBase64(updatedMovie.PosterFile);

            _mapper.Map(updatedMovie, movie);
            await _context.SaveChangesAsync();
            serviceResponse.Data = _mapper.Map<GetMovieDto>(movie);
            serviceResponse.Message = "Movie updated successfully";
        }
        catch (Exception ex)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = $"An error occurred while updating the movie: {ex.Message}";
        }
        return serviceResponse;
    }

    private async Task<bool> MovieExists(string title)
        => await _context.Movies.AnyAsync(movie => movie.Title!.ToLower() == title.ToLower());

    private async Task<string> ConvertIntoBase64(IFormFile file)
    {
        using (var converted = new MemoryStream())
        {
            await file.OpenReadStream().CopyToAsync(converted);
            return Convert.ToBase64String(converted.ToArray());
        }
    }

}
