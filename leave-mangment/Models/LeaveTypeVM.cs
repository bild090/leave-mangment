using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace leave_mangment.Models
{
    public class LeaveTypeMV
    {
        
        public int Id { get; set; }
        [Required]
        public String Name { get; set; }
        [Display(Name = ("Date Created"))]
        public DateTime? DateCreated { get; set; }
    }
}
