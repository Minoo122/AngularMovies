using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using NetTopologySuite.Geometries;

namespace MoviesAPI.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles(GeometryFactory geometryFactory)
        {
            CreateMap<GenreDTO, Genre>().ReverseMap();
            CreateMap<GenreCreationDTO, Genre>();

            CreateMap<ActorDTO, Actor>().ReverseMap();
            CreateMap<ActorCreationDTO, Actor>().ForMember(x=>x.Picture, options=>options.Ignore());

            CreateMap<MovieTheater, MovieTheaterDTO>()
                .ForMember(x => x.Latitude, dto => dto.MapFrom(prop => prop.Location.Y)) 
                .ForMember(x => x.Longitude, dto => dto.MapFrom(prop => prop.Location.X));

            CreateMap<MovieTheaterCreationDTO, MovieTheater>()
                .ForMember(x => x.Location, x => x.MapFrom(dto => geometryFactory.CreatePoint(new Coordinate(dto.Longitude, dto.Latitude))));

            CreateMap<MovieCreationDTO, Movie>()
                .ForMember(x => x.Poster, options => options.Ignore())
                .ForMember(x => x.MoviesGenres, options => options.MapFrom(MapMoviesGenres))
                .ForMember(x => x.MovieTheatersMovies, options => options.MapFrom(MapMovieTheatersMovies))
                .ForMember(x => x.MoviesActors, options => options.MapFrom(MapMoviesActors));

            CreateMap<Movie, MovieDTO>()
                .ForMember(x => x.Genres, x => x.MapFrom(MapMoviesGenres))
                .ForMember(x => x.MovieTheaters, x => x.MapFrom(MapMovieTheatersMovies))
                .ForMember(x => x.Actors, x => x.MapFrom(MapMoviesActors));
        }
        private List<GenreDTO> MapMoviesGenres(Movie movie, MovieDTO movieDto)
        {
            List<GenreDTO> result = new List<GenreDTO>();
            if(movie.MoviesGenres != null)
            {
                foreach (var genre in movie.MoviesGenres)
                {
                    result.Add(new GenreDTO() { Id = genre.GenreId, Name = genre.Genre.Name });
                }
            }
            return result;
        }
        private List<MoviesGenres> MapMoviesGenres(MovieCreationDTO movieCreationDTO, Movie movie)
        {
            List<MoviesGenres> result = new List<MoviesGenres>(); //!!!
            if (movieCreationDTO.GenresIds == null) { return result; }
            foreach (var id in movieCreationDTO.GenresIds)
            {
                result.Add(new MoviesGenres() { GenreId = id });
            }
            return result;
        }
        private List<MovieTheaterDTO> MapMovieTheatersMovies(Movie movie, MovieDTO movieDTO)
        {
            List<MovieTheaterDTO> result = new List<MovieTheaterDTO>();
            if(movie.MovieTheatersMovies != null)
            {
                foreach (var theater in movie.MovieTheatersMovies)
                {
                    result.Add(new MovieTheaterDTO()
                    {
                        Id = theater.MovieTheaterId,
                        Name = theater.MovieTheater.Name,
                        Latitude = theater.MovieTheater.Location.Y,
                        Longitude = theater.MovieTheater.Location.X,
                    });
                }
            }
            return result;
        }
        private List<MovieTheatersMovies> MapMovieTheatersMovies(MovieCreationDTO movieCreationDTO, Movie movie)
        {
            List<MovieTheatersMovies> result = new List<MovieTheatersMovies>();
            if (movieCreationDTO.MovieTheatersIds == null) { return result; }
            foreach (var id in movieCreationDTO.MovieTheatersIds)
            {
                result.Add(new MovieTheatersMovies() { MovieTheaterId = id });
            }
            return result;
        }
        private List<ActorsMovieDTO> MapMoviesActors(Movie movie, MovieDTO movieDTO)
        {
            var result = new List<ActorsMovieDTO>();
            if(movie.MoviesActors != null)
            {
                foreach (var actor in movie.MoviesActors)
                {
                    result.Add(new ActorsMovieDTO()
                    {
                        Id = actor.ActorId,
                        Character = actor.Character,
                        Name = actor.Actor.Name,
                        Order = actor.Order,
                        Picture = actor.Actor.Picture
                    });
                }
            }
            return result;
        }
        private List<MoviesActors> MapMoviesActors(MovieCreationDTO movieCreationDTO, Movie movie)
        {
            List<MoviesActors> result = new List<MoviesActors>();
            if (movieCreationDTO.Actors == null) { return result; }
            foreach (var actor in movieCreationDTO.Actors)
            {
                result.Add(new MoviesActors() { Character = actor.Character, ActorId = actor.Id });
            }
            return result;
        }
    }
}
//creationdto
//[Range(-90, 90)]
//public double Latitude { get; set; }
//[Range(-180, 180)]
//public double Longitude { get; set; }


//[StringLength(maximumLength: 75)]
//public string Name { get; set; }
//public Point Location { get; set; }
