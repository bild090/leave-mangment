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
        public async Task<bool> Create(LeaveRequest entity)
        {
           await _db.LeaveRequests.AddAsync(entity);
            return await Save();
        }

        public async Task<bool> Delete(LeaveRequest entity)
        {
            _db.LeaveRequests.Remove(entity);
            return await Save();

        }

        public async Task<bool> exists(int id)
        {
            return await _db.LeaveTypes.AnyAsync(q => q.Id == id);
        }

        public async Task<ICollection<LeaveRequest>> FindAll()
        {
            return await _db.LeaveRequests
                .Include(q =>q.RequestingEmplyee)
                .Include(q =>q.LeaveType)
                .Include(q =>q.ApprovedBy)
                .ToListAsync();
        }

        public async Task<LeaveRequest> FindById(int id)
        {
            return await _db.LeaveRequests
                .Include(q => q.RequestingEmplyee)
                .Include(q => q.LeaveType)
                .Include(q => q.ApprovedBy)
                .FirstOrDefaultAsync(q => q.Id ==id);
        }

        public async Task<ICollection<LeaveRequest>> GetLeaveAllocationsByEmployee(string employeeId)
        {
            var leaveRequests = await FindAll();
            return leaveRequests.Where(q => q.RequestingEmplyeeId == employeeId).ToList();
            
        }

        public async Task<bool> Save()
        {
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> Update(LeaveRequest entity)
        {
            _db.LeaveRequests.Update(entity);
            return await Save();
        }
    }
}
