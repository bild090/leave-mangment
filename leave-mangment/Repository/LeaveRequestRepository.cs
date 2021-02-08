using leave_mangment.contracts;
using leave_mangment.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_mangment.Repository
{
    public class LeaveRequestRepository : ILeaveRequestRepository
    {
        private readonly ApplicationDbContext _db;

        public LeaveRequestRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public bool Create(LeaveRequest entity)
        {
            _db.LeaveRequests.Add(entity);
            return Save();
        }

        public bool Delete(LeaveRequest entity)
        {
            _db.LeaveRequests.Remove(entity);
            return Save();

        }

        public bool exists(int id)
        {
            return _db.LeaveTypes.Any(q => q.Id == id);
        }

        public ICollection<LeaveRequest> FindAll()
        {
            return _db.LeaveRequests
                .Include(q =>q.RequestingEmplyee)
                .Include(q =>q.LeaveType)
                .Include(q =>q.ApprovedBy)
                .ToList();
        }

        public LeaveRequest FindById(int id)
        {
            return _db.LeaveRequests
                .Include(q => q.RequestingEmplyee)
                .Include(q => q.LeaveType)
                .Include(q => q.ApprovedBy)
                .FirstOrDefault(q => q.Id ==id);
        }

        public ICollection<LeaveRequest> GetLeaveAllocationsByEmployee(string employeeId)
        {
            var leaveRequests = FindAll()
                .Where(q => q.RequestingEmplyeeId == employeeId)
                .ToList();
            return leaveRequests;
        }

        public bool Save()
        {
            return _db.SaveChanges() > 0;
        }

        public bool Update(LeaveRequest entity)
        {
            _db.LeaveRequests.Update(entity);
            return Save();
        }
    }
}
