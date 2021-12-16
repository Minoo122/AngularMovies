using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using MoviesAPI.Helpers;

namespace MoviesAPI.Controllers
{
    [Route("api/actors")]
    [ApiController]
    public class ActorsController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IFileStorageService fileStorageService;
        private readonly string containerName = "actors";

        public ActorsController(ApplicationDbContext context, IMapper mapper,
            IFileStorageService fileStorageService)
        {
            this.context = context;
            this.mapper = mapper;
            this.fileStorageService = fileStorageService;
        }
        [HttpGet]
        public async Task<ActionResult<List<ActorDTO>>> Get([FromQuery] PaginationDTO paginationDTO)
        {
            var queryable = context.Actors.AsQueryable();
            await HttpContext.addPaginationParameterInHeader(queryable); // TUTAJ AWAIT TRZEBA DAĆ NIE ZAPOMNIEĆ
            var actors = await queryable.OrderBy(x => x.Name).Paginate(paginationDTO).ToListAsync();
            if (actors == null)
            {
                return NotFound();
            }

            return mapper.Map<List<ActorDTO>>(actors);
        }
        [HttpPost("searchByName")]
        // ZMIENIONE STRING NAME NA OBJECT NAME
        public async Task<ActionResult<List<ActorsMovieDTO>>> SearchByName([FromBody] object name)
        {
            if(string.IsNullOrWhiteSpace(name.ToString())) { return new List<ActorsMovieDTO>(); }
            return await context.Actors
                .Where(x => x.Name.Contains(name.ToString()))
                .OrderBy(x => x.Name)
                .Select(x => new ActorsMovieDTO() { Id = x.Id, Name = x.Name, Picture = x.Picture })
                .Take(5)
                .ToListAsync();

        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ActorDTO>> Get(int id) {
            var actor = await this.context.Actors.FirstOrDefaultAsync(x => x.Id == id);
            if(actor == null)
            {
                return NotFound();
            }
            return mapper.Map<ActorDTO>(actor);
        }
        [HttpPost]
        public async Task<ActionResult> Post([FromForm] ActorCreationDTO actorCreationDTO)
        {
            var actor = mapper.Map<Actor>(actorCreationDTO);
            if (actorCreationDTO != null)
            {
                actor.Picture = await fileStorageService.SaveFile(containerName, actorCreationDTO.Picture); // używanie blob
            }

            context.Add(actor);
            await context.SaveChangesAsync();
            return NoContent();
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id,[FromForm] ActorCreationDTO actorCreationDTO)
        {
            var actor = await context.Actors.FirstOrDefaultAsync(x=>x.Id==id);
            if(actor == null)
            {
                return NotFound();
            }
            actor = mapper.Map(actorCreationDTO, actor); //1 w 2 zmieniamy
            //actor = mapper.Map<Actor>(actorCreationDTO); // czy to to samo ??? 
            if(actorCreationDTO.Picture != null)
            {
                actor.Picture = await fileStorageService.EditFile(containerName, actorCreationDTO.Picture, actor.Picture);
            }
            await context.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var actor = await this.context.Actors.FirstOrDefaultAsync(x => x.Id == id);
            if (actor == null)
            {
                return NotFound();
            }
            context.Remove(actor);
            await context.SaveChangesAsync();
            await fileStorageService.DeleteFile(actor.Picture, containerName);
            return NoContent();
        }
    }
}
