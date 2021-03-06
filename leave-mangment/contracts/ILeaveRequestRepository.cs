﻿using leave_mangment.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_mangment.contracts
{
    public interface ILeaveRequestRepository : IRepositoryBase<LeaveRequest>
    {
         Task<ICollection<LeaveRequest>> GetLeaveAllocationsByEmployee(String employeeId);
    }
}
