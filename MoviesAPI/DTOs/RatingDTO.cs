using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI.DTOs
{
    public class RatingDTO
    {
        public int MovieId { get; set; }
        [Range(1,5)]
        public int Rating { get; set; }
    }
}
