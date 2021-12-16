using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MoviesAPI.DTOs
{
    public class ActorCreationDTO
    {
        [Required(ErrorMessage = "You must fill the Name field!!!")]
        [StringLength(120)]
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public IFormFile Picture { get; set; }
        public string Biography { get; set; }
    }
}
