using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace leave_mangment.Data
{
    public class LeaveType
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public String Name { get; set; }
        public int DefualtDays { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
