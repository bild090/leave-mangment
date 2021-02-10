using AutoMapper;
using leave_mangment.contracts;
using leave_mangment.Data;
using leave_mangment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_mangment.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class LeaveAllocationController : Controller
    {

        private readonly ILeaveTypeRepository _leaveTypeRepo;
        private readonly ILeaveAllocationRepository _leavAllocationRepo;
        private readonly IUnitOfWork _UnitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<Employee> _userManager;

        public LeaveAllocationController(
             ILeaveTypeRepository leaveTypeRepo,
             ILeaveAllocationRepository leavAllocationRepo,
             IMapper mapper,
            UserManager<Employee> userManager,
            IUnitOfWork UnitOfWork
            )
        {
            _leaveTypeRepo = leaveTypeRepo;
            _leavAllocationRepo = leavAllocationRepo;
            _mapper = mapper;
            _userManager = userManager;
            _UnitOfWork = UnitOfWork;
        }

        // GET: LeaveAllocationController
        public async Task<ActionResult> Index()
        {
            var leaveType = await _UnitOfWork.LeaveTypes.FindAll();
            var mappedLeaveTypes = _mapper.Map<List<LeaveType>, List<LeaveTypeVM>>(leaveType.ToList());
            var model = new CreateLeaveAllocationVM
            {
                LeaveTypes = mappedLeaveTypes,
                NumberUpdated = 0
            };

            return View(model);

        }

        public async Task<ActionResult> SetLeave(int id) 
        {
            var leaveType = await _UnitOfWork.LeaveTypes.Find(q => q.Id == id);
            var emplyees = await _userManager.GetUsersInRoleAsync("Employee");
            var period = DateTime.Now.Year;
            foreach (var emp in emplyees)
            {
                if (await _UnitOfWork.LeaveAllocations.exists(q => q.EmployeeId == emp.Id && q.LeaveTypeId == id && q.Period == period))
                    continue;

                var allocation = new LeaveAllocationVM
                {
                    DateCreated = DateTime.Now,
                    EmployeeId = emp.Id,
                    LeaveTypeId = id,
                    NumberOfDays = leaveType.DefualtDays,
                    period = DateTime.Now.Year,

                };
                var leaveAllocation = _mapper.Map<LeaveAllocation>(allocation);
                await _UnitOfWork.LeaveAllocations.Create(leaveAllocation);
                await _UnitOfWork.Save();
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<ActionResult> listEmployees() 
        {
            var emplyees = await _userManager.GetUsersInRoleAsync("Employee");
            var model = _mapper.Map<List<EmployeeVM>>(emplyees);
            return View(model);
        }

        // GET: LeaveAllocationController/Details/5
        public async Task<ActionResult> Details(String id)
        {
            var findById = await _userManager.FindByIdAsync(id);
            var employee = _mapper.Map<EmployeeVM>(findById);
            var period = DateTime.Now.Year;
            var records = await _UnitOfWork.LeaveAllocations.FindAll(
                expression: q => q.EmployeeId == id && q.Period == period,
                Includes: new List<String> { "LeaveType" }
                );
            var allocation = _mapper.Map<List<LeaveAllocationVM>>(records);
            var model = new ViewAllocationVM
            {
                Employee = employee,
                LeaveAllocations = allocation
            };
            return View(model);
        }

        // GET: LeaveAllocationController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LeaveAllocationController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: LeaveAllocationController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var allocation = await _UnitOfWork.LeaveAllocations.Find(q => q.id == id,
                Includes: new List<string> { "Employee", "LeaveType" });
            var model = _mapper.Map<EditLeaveAllocation>(allocation);
            return View(model);
        }

        // POST: LeaveAllocationController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditLeaveAllocation model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                var recored =await _UnitOfWork.LeaveAllocations.Find(q => q.id == model.id);
                recored.NumberOfDays = model.NUmberOfDays;
                _UnitOfWork.LeaveAllocations.Update(recored);
                await _UnitOfWork.Save();
                return RedirectToAction(nameof(Details), new {id = model.EmployeeId });
            }
            catch
            {
                return View(model);
            }
        }

        // GET: LeaveAllocationController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: LeaveAllocationController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        protected override void Dispose(bool disposing)
        {
            _UnitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}
