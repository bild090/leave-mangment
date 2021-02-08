using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace leave_mangment.Models
{
    public class LeaveRequestVM
    {
        
        public int Id { get; set; }
        public EmployeeVM RequestingEmplyee { get; set; }
        [Display(Name ="Employee Name")]
        public String RequestingEmplyeeId { get; set; }
        [Display(Name ="Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [Display(Name ="End Date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        public LeaveTypeVM LeaveType { get; set; }
        public int LeaveTypeId { get; set; }
        [Display(Name ="Date Requested")]
        public DateTime RequestedDate { get; set; }
        [Display(Name ="Date Actioned")]
        public DateTime RequestedActioned { get; set; }
        [Display(Name ="Approval State")]
        public bool? Approved { get; set; }
        public EmployeeVM ApprovedBy { get; set; }
        [Display(Name ="Employee comments")]
        [MaxLength(300)]
        public String RequestComment { get; set; }
        public bool Cancelled { get; set; }
        public String ApprovedById { get; set; }
    }

    public class AdminLeaveRequestViewVM
    {
        [Display(Name = "Totale Number of Requests")]
        public int TotaleRequests { get; set; }
        [Display(Name = "Approved Requests")]
        public int ApprovedRequests { get; set; }
        [Display(Name = "Pending Requests")]
        public int PendingRequests { get; set; }
        [Display(Name = "Rejected Requests")]
        public int RejectedRequests { get; set; }
        public List<LeaveRequestVM> LeaveRequests { get; set; }
    }

    public class CreateLeaveRequestVM
    {
        [Display(Name ="Start Day")]
        [Required]
        public String StartDate { get; set; }
        [Display(Name ="End Date")]
        [Required]
        public String EndDate { get; set; }
        public IEnumerable<SelectListItem> LeaveTypes { get; set; }
        [Display(Name ="Leave Type")]
        public int LeaveType { get; set; }
        public int LeaveTypeId { get; set; }
        [Display(Name = "Employee comments")]
        [MaxLength(300)]
        public String RequestComment { get; set; }

    }

    public class ViewMyLeaveVM
    {
        public List<LeaveAllocationVM> leaveAllocations { get; set; }
        public List<LeaveRequestVM> leaveRequests { get; set; }
    }
}
