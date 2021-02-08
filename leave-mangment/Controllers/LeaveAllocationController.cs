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
        private readonly IMapper _mapper;
        private readonly UserManager<Employee> _userManager;

        public LeaveAllocationController(
             ILeaveTypeRepository leaveTypeRepo,
             ILeaveAllocationRepository leavAllocationRepo,
             IMapper mapper,
            UserManager<Employee> userManager
            )
        {
            _leaveTypeRepo = leaveTypeRepo;
            _leavAllocationRepo = leavAllocationRepo;
            _mapper = mapper;
            _userManager = userManager;
        }

        // GET: LeaveAllocationController
        public ActionResult Index()
        {
            var leaveType = _leaveTypeRepo.FindAll().ToList();
            var mappedLeaveTypes = _mapper.Map<List<LeaveType>, List<LeaveTypeVM>>(leaveType);
            var model = new CreateLeaveAllocationVM
            {
                LeaveTypes = mappedLeaveTypes,
                NumberUpdated = 0
            };

            return View(model);

        }

        public ActionResult SetLeave(int id) 
        {
            var leaveType = _leaveTypeRepo.FindById(id);
            var emplyees = _userManager.GetUsersInRoleAsync("Employee").Result;
            foreach (var emp in emplyees)
            {
                if (_leavAllocationRepo.checkAllocation(id, emp.Id))
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
                _leavAllocationRepo.Create(leaveAllocation);
            }
            return RedirectToAction(nameof(Index));
        }

        public ActionResult listEmployees() 
        {
            var emplyees = _userManager.GetUsersInRoleAsync("Employee").Result;
            var model = _mapper.Map<List<EmployeeVM>>(emplyees);
            return View(model);
        }

        // GET: LeaveAllocationController/Details/5
        public ActionResult Details(String id)
        {
            var employee = _mapper.Map<EmployeeVM>(_userManager.FindByIdAsync(id).Result);
            var allocation = _mapper.Map<List<LeaveAllocationVM>>( _leavAllocationRepo.GetLeaveAllocationsByEmployee(id));
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
        public ActionResult Edit(int id)
        {
            var allocation = _leavAllocationRepo.FindById(id);
            var model = _mapper.Map<EditLeaveAllocation>(allocation);
            return View(model);
        }

        // POST: LeaveAllocationController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditLeaveAllocation model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                var recored = _leavAllocationRepo.FindById(model.id);
                recored.NumberOfDays = model.NUmberOfDays;
                var isSuccess = _leavAllocationRepo.Update(recored);
                if (!isSuccess) 
                {
                    ModelState.AddModelError("", "Error while saveing");
                    return View(model);
                }
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
    }
}
