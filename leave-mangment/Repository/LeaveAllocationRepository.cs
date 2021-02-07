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

        public bool checkAllocation(int leavetypeid, string empliyeeId)
        {
            var period = DateTime.Now.Year;
            return FindAll()
                .Where(q => q.EmployeeId == empliyeeId && q.LeaveTypeId == leavetypeid && q.Period == period)
                .Any();
        }

        public bool Create(LeaveAllocation entity)
        {
            _db.LeaveAllocations.Add(entity);
            return Save();
        }

        public bool Delete(LeaveAllocation entity)
        {
            _db.LeaveAllocations.Remove(entity);
            return Save();

        }

        public bool exists(int id)
        {
            return _db.LeaveTypes.Any(q => q.Id == id);
        }

        public ICollection<LeaveAllocation> FindAll()
        {
            var leaveAllocation = _db.LeaveAllocations
                .Include(q => q.LeaveType)
                .ToList();
            return leaveAllocation;
        }

        public LeaveAllocation FindById(int id)
        {
            return _db.LeaveAllocations
                .Include(q => q.Employee)
                .Include(q => q.LeaveType)
                .FirstOrDefault(q => q.id == id);
        }

        public ICollection<LeaveAllocation> GetLeaveAllocationsByEmployee(String id)
        {
            var period = DateTime.Now.Year;
            return FindAll()
                    .Where(q => q.EmployeeId == id && q.Period == period)
                    .ToList();
        }

        public bool Save()
        {
            return _db.SaveChanges() > 0;
        }

        public bool Update(LeaveAllocation entity)
        {
            _db.LeaveAllocations.Update(entity);
            return Save();
        }
    }
}
