using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace leave_mangment.Models
{
    public class DetailsLeaveTypeMV
    {
        
        public int Id { get; set; }
        [Required]
        public String Name { get; set; }
        public DateTime DateCreated { get; set; }
    }

    public class CreateLeaveTypeMV
    {

        public int Id { get; set; }
        [Required]
        public String Name { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
