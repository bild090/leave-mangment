﻿using leave_mangment.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_mangment.contracts
{
   public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<LeaveType> LeaveTypes { get; }
        IGenericRepository<LeaveRequest> LeaveRequests { get; }
        IGenericRepository<LeaveAllocation> LeaveAllocations { get; }

        Task Save();
    }
}
