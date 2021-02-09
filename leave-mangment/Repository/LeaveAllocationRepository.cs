using leave_mangment.contracts;
using leave_mangment.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_mangment.Repository
{
    public class LeaveAllocationRepository : ILeaveAllocationRepository
    {
        private readonly ApplicationDbContext _db;

        public LeaveAllocationRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<bool> checkAllocation(int leavetypeid, string empliyeeId)
        {
            var period = DateTime.Now.Year;
            var allocations = await FindAll();
            return  allocations.Where(q => q.EmployeeId == empliyeeId 
                                && q.LeaveTypeId == leavetypeid 
                                && q.Period == period)
                .Any();
        }


        public async Task<bool> Create(LeaveAllocation entity)
        {
            await _db.LeaveAllocations.AddAsync(entity);
            return await Save();
        }

        public async Task<bool> Delete(LeaveAllocation entity)
        {
            _db.LeaveAllocations.Remove(entity);
            return await Save();

        }

        public async Task<bool> exists(int id)
        {
            return await _db.LeaveTypes.AnyAsync(q => q.Id == id);
        }

        public async Task<ICollection<LeaveAllocation>> FindAll()
        {
            var leaveAllocation = await _db.LeaveAllocations
                .Include(q => q.LeaveType)
                .ToListAsync();
            return leaveAllocation;
        }

        public async Task<LeaveAllocation> FindById(int id)
        {
            return await _db.LeaveAllocations
                .Include(q => q.Employee)
                .Include(q => q.LeaveType)
                .FirstOrDefaultAsync(q => q.id == id);
        }

        public async Task<ICollection<LeaveAllocation>> GetLeaveAllocationsByEmployee(String employeeId)
        {
            var period = DateTime.Now.Year;
            var allocations = await FindAll();
                   return allocations.Where(q => q.EmployeeId == employeeId && q.Period == period)
                    .ToList();
        }

        public async Task<LeaveAllocation> GetLeaveAllocationsByEmployeeAndType(string employeeId, int typeId)
        {
            var period = DateTime.Now.Year;
            var allocations = await FindAll();
                    return allocations.FirstOrDefault(q => q.EmployeeId == employeeId 
                                                        && q.Period == period 
                                                        && q.LeaveTypeId == typeId);
                    
        }

        public async Task<bool> Save()
        {
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> Update(LeaveAllocation entity)
        {
            _db.LeaveAllocations.Update(entity);
            return await Save();
        }
    }
}
