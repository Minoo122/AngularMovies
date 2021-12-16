using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI.DTOs
{
    public class HomeDTO
    {
        public List<MovieDTO> upcomingReleases { get; set; }
        public List<MovieDTO> inTheaters { get; set; }
    }
}
