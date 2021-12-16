using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using MoviesAPI.Helpers;

namespace MoviesAPI.Controllers
{
    [Route("api/movies")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class MoviesController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IFileStorageService fileStorageService;
        private readonly UserManager<IdentityUser> userManager;
        private string containerName = "movies";

        public MoviesController(ApplicationDbContext context, IMapper mapper,
            IFileStorageService fileStorageService,
            UserManager<IdentityUser> userManager)
        {
            this.context = context;
            this.mapper = mapper;
            this.fileStorageService = fileStorageService;
            this.userManager = userManager;
        }
        [HttpPost]
        public async Task<ActionResult<int>> Post([FromForm] MovieCreationDTO movieCreationDTO)
        {
            var movie = mapper.Map<Movie>(movieCreationDTO);
            if(movieCreationDTO.Poster != null)
            {
                movie.Poster = await fileStorageService.SaveFile(containerName, movieCreationDTO.Poster);
            }
            AnotateActorsOrder(movie);
            context.Add(movie);
            await context.SaveChangesAsync();
            return movie.Id;
        }
        [HttpGet("filter")]
        public async Task<ActionResult<List<MovieDTO>>> Filter([FromQuery] FilterMoviesDTO filterMoviesDTO)
        {
            var moviesQueryable = this.context.Movies.AsQueryable();
            if (!string.IsNullOrEmpty(filterMoviesDTO.Title))
            {
                moviesQueryable = moviesQueryable.Where(x =>
                    x.Title.Contains(filterMoviesDTO.Title));
            }
            if(filterMoviesDTO.InTheaters)
            {
                moviesQueryable = moviesQueryable.Where(x => x.InTheaters == filterMoviesDTO.InTheaters);
            }
            if (filterMoviesDTO.UpcomingReleases)
            {
                var today = DateTime.Today;
                moviesQueryable = moviesQueryable.Where(x => x.ReleaseDate > today);
            }

            if(filterMoviesDTO.GenreId != 0)
            {
                moviesQueryable = moviesQueryable.Where(x => x.MoviesGenres.Select(y => y.GenreId).Contains(
                    filterMoviesDTO.GenreId));
            }
            await HttpContext.addPaginationParameterInHeader(moviesQueryable);
            var movies = await moviesQueryable.OrderBy(x => x.Title).Paginate(filterMoviesDTO.PaginationDTO)
                .ToListAsync();
            return mapper.Map<List<MovieDTO>>(movies);
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<HomeDTO>> getHomePage()
        {
            var top = 6;
            var date = DateTime.Today;

            var upcomingReleases = await this.context.Movies
                .Where(x => x.ReleaseDate > date)
                .Take(top)
                .OrderBy(x=>x.ReleaseDate)
                .ToListAsync();

            var inTheaters = await this.context.Movies
                .Where(x=>x.InTheaters)
                .Take(top)
                .OrderBy(x=>x.ReleaseDate)
                .ToListAsync();

            var homeDTO = new HomeDTO();

            homeDTO.upcomingReleases = mapper.Map<List<MovieDTO>>(upcomingReleases);
            homeDTO.inTheaters = mapper.Map<List<MovieDTO>>(inTheaters);

            return homeDTO;
        }
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<MovieDTO>> Get(int id)
        {
            var movie = await this.context.Movies
                .Include(x => x.MoviesGenres).ThenInclude(x => x.Genre)
                .Include(x => x.MovieTheatersMovies).ThenInclude(x => x.MovieTheater)
                .Include(x => x.MoviesActors).ThenInclude(x => x.Actor)
                .FirstOrDefaultAsync(m=>m.Id == id);
            if(movie == null)
            {
                return NotFound();
            }

             var averageVote = 0.0;
            var userVote = 0;

            if(await context.Ratings.AnyAsync(x=>x.MovieId==id))
            {
                averageVote = await context.Ratings.Where(x => x.MovieId == id)
                    .AverageAsync(x => x.Rate);
            }
            // jeśli user jest zalogowany
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                var email = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "email").Value;
                var user = await userManager.FindByEmailAsync(email);
                var userId = Convert.ToInt32(user.Id);

                var ratingDb = await context.Ratings.FirstOrDefaultAsync(x => x.MovieId == id &&
                userId == x.UserId);

                if(ratingDb != null)
                {
                    userVote = ratingDb.Rate;
                }
            }

            var dto = mapper.Map<MovieDTO>(movie);

            dto.AverageVote = averageVote;
            dto.UserVote = userVote;
            dto.Actors = dto.Actors.OrderBy(x => x.Order).ToList();

            return dto;
            
        }
        [HttpGet("PostGet")]
        public async Task<ActionResult<MoviePostGetDTO>> PostGet()
        {
            var genres = await context.Genres.OrderBy(x=>x.Name).ToListAsync();
            var movieTheaters = await context.MovieTheaters.OrderBy(x => x.Name).ToListAsync();
            var genresDTO = mapper.Map<List<GenreDTO>>(genres);
            var movieTheatersDTO = mapper.Map<List<MovieTheaterDTO>>(movieTheaters);
            return new MoviePostGetDTO() { Genres = genresDTO, MovieTheaters = movieTheatersDTO };
        }
        [HttpGet("putget/{id:int}")]
        public async Task<ActionResult<MoviePutGetDTO>> PutGet(int id)
        {
            var movieActionResult = await Get(id);
            if(movieActionResult.Result is NotFoundResult) { return NotFound(); }

            var movie = movieActionResult.Value;
            var SelectedGenresIds = movie.Genres.Select(x => x.Id).ToList();
            var nonSelectedGenres = await context.Genres.Where(g => !SelectedGenresIds.Contains(g.Id)).ToListAsync();

            var SelectedMovieTheatersIds = movie.MovieTheaters.Select(x => x.Id).ToList();
            var nonSelectedMovieTheaters = await context.MovieTheaters.Where(mt => !SelectedMovieTheatersIds.Contains(mt.Id)).ToListAsync();

            var nonSelectedGenresDTO = mapper.Map<List<GenreDTO>>(nonSelectedGenres);
            var nonSelectedMovieTheatersDTO = mapper.Map<List<MovieTheaterDTO>>(nonSelectedMovieTheaters);

            var response = new MoviePutGetDTO();
            response.Movie = movie;
            response.NonSelectedGenres = nonSelectedGenresDTO;
            response.SelectedGenres = movie.Genres;
            response.NonSelectedMovieTheaters = nonSelectedMovieTheatersDTO;
            response.SelectedMovieTheaters = movie.MovieTheaters;
            response.Actors = movie.Actors;

            return response;
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var movie = await context.Movies.FirstOrDefaultAsync(x => x.Id == id);
            if(movie == null)
            {
                return NotFound();
            }
            context.Remove(movie);
            await context.SaveChangesAsync();
            if(movie.Poster != null)
            {
                await fileStorageService.DeleteFile(movie.Poster, containerName);
            }
            return NoContent();
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromForm] MovieCreationDTO movieCreationDTO)
        {
            var movie = await this.context.Movies
                .Include(x => x.MoviesActors)
                .Include(x => x.MoviesGenres)
                .Include(x => x.MovieTheatersMovies)
                .FirstOrDefaultAsync(x=>x.Id==id);

            if(movie == null)
            {
                return NotFound();
            }

            movie = mapper.Map(movieCreationDTO, movie);
            if (movieCreationDTO.Poster != null)
            {
                movie.Poster = await fileStorageService.EditFile(containerName, movieCreationDTO.Poster, movie.Poster);
            }
            await this.context.SaveChangesAsync();
            return NoContent();
        }
        private void AnotateActorsOrder(Movie movie)
        {
            if(movie.MoviesActors != null)
            {
                for (int i = 0; i < movie.MoviesActors.Count; i++)
                {
                    movie.MoviesActors[i].Order = i;
                }
            }
        }
    }
}
