using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace leave_mangment.Models
{
    public class LeaveHistoryVM
    {
        
        public int Id { get; set; }
        public EmployeeVM RequestingEmplyee { get; set; }
        public String RequestingEmplyeeId { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        public LeaveTypeVM LeaveType { get; set; }
        public int LeaveTypeId { get; set; }
        public DateTime RequestedDate { get; set; }
        public DateTime RequestedActioned { get; set; }
        public bool? Approved { get; set; }
        public EmployeeVM ApprovedBy { get; set; }
        public String ApprovedById { get; set; }
        public IEnumerable<SelectListItem> LeaveTypes { get; set; }
    }
}
