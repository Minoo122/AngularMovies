using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MoviesAPI.Validations;

namespace MoviesAPI.Entities
{
    public class Genre
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="You must fill the Name field!!!")]
        [StringLength(50)]
        [FirstLetterUppercaseAttribute]
        public string Name { get; set; }
    }
}
