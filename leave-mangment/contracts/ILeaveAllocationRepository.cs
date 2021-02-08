using leave_mangment.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_mangment.contracts
{
    public interface ILeaveAllocationRepository : IRepositoryBase<LeaveAllocation>
    {
        bool checkAllocation(int leavetypeid, String empliyeeId);
        ICollection<LeaveAllocation> GetLeaveAllocationsByEmployee(String employeeId);
        LeaveAllocation GetLeaveAllocationsByEmployeeAndType(String employeeId, int typeId);
    }
}
