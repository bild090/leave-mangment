﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace leave_mangment.Data
{
    public class LeaveRequest
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("RequestingEmplyeeId")]
        public Employee RequestingEmplyee { get; set; }
        public String RequestingEmplyeeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [ForeignKey("LeaveTypeId")]
        public LeaveType LeaveType { get; set; }
        public int LeaveTypeId { get; set; }
        public DateTime RequestedDate { get; set; }
        public DateTime? RequestedActioned { get; set; }
        public String RequestComment { get; set; }
        public bool?  Approved { get; set; }
        [ForeignKey("ApprovedById")]
        public Employee ApprovedBy { get; set; }
        public bool Cancelled { get; set; }
        public String ApprovedById { get; set; }
    }
}
