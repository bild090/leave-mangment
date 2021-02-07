using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace leave_mangment.Models
{
    public class LeaveAllocationVM
    {

        public int id { get; set; }
        public int NumberOfDays { get; set; }
        public DateTime DateCreated { get; set; }
        public EmployeeVM Employee { get; set; }
        public String EmployeeId { get; set; }
        public LeaveTypeVM LeaveType { get; set; }
        public int LeaveTypeId { get; set; }
        [Required]
        public int period { get; set; }
    }

    public class CreateLeaveAllocationVM
    {
        public List<LeaveTypeVM> LeaveTypes { get; set; }
        public int NumberUpdated { get; set; }
    }

    public class ViewAllocationVM
    {
        public EmployeeVM Employee { get; set; }
        public String EmployeeId { get; set; }
        public List<LeaveAllocationVM> LeaveAllocations { get; set; }
    }

    public class EditLeaveAllocation
        {
        public int id { get; set; }
        public EmployeeVM Employee { get; set; }
        [Display(Name ="Number Of Days")]
        public String EmployeeId { get; set; }
        public int NUmberOfDays { get; set; }
        public LeaveTypeVM LeaveType { get; set; }

    }
}
