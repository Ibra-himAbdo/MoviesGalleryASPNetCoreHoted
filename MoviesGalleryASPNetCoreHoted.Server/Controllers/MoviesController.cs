namespace MoviesGalleryASPNetCoreHoted.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MoviesController : ControllerBase
{
    private readonly IMoviesService _moviesService;

    public MoviesController(IMoviesService moviesService)
    {
        _moviesService = moviesService;
    }

    [HttpGet("GetAll")]
    public async Task<ActionResult<ServiceResponse<IEnumerable<GetMovieDto>>>> GetMovies()
        => Ok(await _moviesService.GetAllMovies());

    [HttpGet("{id}")]
    public async Task<ActionResult<ServiceResponse<GetMovieDto>>> GetMoviebyId(int id)
        => Ok(await _moviesService.GetMovieById(id));

    [HttpGet("search")]
    public async Task<ActionResult<ServiceResponse<IEnumerable<GetMovieDto>>>> SearchMovies(string search)
    {
        var response = await _moviesService.GetMovieByPartialName(search);
        if(!response.Success)
            return BadRequest(response);
        return Ok(response);
    }

    [Consumes("multipart/form-data")]
    [HttpPost]
    public async Task<ActionResult<ServiceResponse<GetMovieDto>>> AddMovie(AddMovieDto newMovie) 
        => Ok(await _moviesService.AddMovie(newMovie));

    [Consumes("multipart/form-data")]
    [HttpPut]
    public async Task<ActionResult<ServiceResponse<GetMovieDto>>> UpdateMovie(UpdateMovieDto updatedMovie)
    {
        var response = await _moviesService.UpdateMovie(updatedMovie);
        if(!response.Success)
            return BadRequest(response);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ServiceResponse<IEnumerable<GetMovieDto>>>> DeleteMovie(int id)
    {
        var response = await _moviesService.DeleteMovie(id);
        if(!response.Success)
            return BadRequest(response);
        return Ok(response);
    }
}