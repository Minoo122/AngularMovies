using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI.DTOs
{
    public class PaginationDTO
    {
        public int Page { get; set; } = 1;
        private int recordsPerPage = 10;
        private int maxAmount { get; set; } = 50;
        public int RecordsPerPage {
            get {
                return recordsPerPage;
            } 
            set {
                recordsPerPage = (value > maxAmount) ? maxAmount:value;
            } }
    }
}
