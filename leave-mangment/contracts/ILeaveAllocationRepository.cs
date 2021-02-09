using leave_mangment.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_mangment.contracts
{
    public interface ILeaveAllocationRepository : IRepositoryBase<LeaveAllocation>
    {
        Task<bool> checkAllocation(int leavetypeid, String empliyeeId);
        Task<ICollection<LeaveAllocation>> GetLeaveAllocationsByEmployee(String employeeId);
        Task<LeaveAllocation> GetLeaveAllocationsByEmployeeAndType(String employeeId, int typeId);
    }
}
