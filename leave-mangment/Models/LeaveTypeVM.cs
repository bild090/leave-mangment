using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace leave_mangment.Models
{
    public class LeaveTypeVM
    {
        
        public int Id { get; set; }
        [Required]
        public String Name { get; set; }
        [Required]
        [Display(Name = "Defualt Days")]
        [Range(1, 25, ErrorMessage ="Plase Enter A Valid Number")]
        public int DefualtDays { get; set; }
        [Display(Name = ("Date Created"))]
        public DateTime? DateCreated { get; set; }
    }
}
