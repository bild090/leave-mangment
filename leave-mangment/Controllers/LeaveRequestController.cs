using AutoMapper;
using leave_mangment.contracts;
using leave_mangment.Data;
using leave_mangment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_mangment.Controllers
{
    [Authorize]
    public class LeaveRequestController : Controller
    {
        private readonly ILeaveRequestRepository _leaveRequest;
        private readonly ILeaveTypeRepository _leaveTypeRepo;
        private readonly ILeaveAllocationRepository _leaveAllcoationRepo;
        private readonly IMapper _mapper;
        private readonly UserManager<Employee> _userManager;
        // GET: LeaveRequestController

        public LeaveRequestController(
            ILeaveRequestRepository leaveRequest,
            ILeaveTypeRepository leaveTypeRepo,
            UserManager<Employee> userManager,
            ILeaveAllocationRepository leaveAllcoationRepo,
            IMapper mapper

            )
        {
            _leaveRequest = leaveRequest;
            _leaveTypeRepo = leaveTypeRepo;
            _userManager = userManager;
            _leaveAllcoationRepo = leaveAllcoationRepo;
            _mapper = mapper;
        }
        [Authorize(Roles = "Administrator")]
        public ActionResult Index()
        {
            var leaveRequest = _leaveRequest.FindAll();
            var leaveRequestModel = _mapper.Map<List<LeaveRequestVM>>(leaveRequest);
            var model = new AdminLeaveRequestViewVM
            {
                TotaleRequests = leaveRequestModel.Count,
                ApprovedRequests = leaveRequestModel.Count(q => q.Approved == true),
                RejectedRequests = leaveRequestModel.Count(q => q.Approved == false),
                PendingRequests = leaveRequestModel.Count(q => q.Approved == null),
                LeaveRequests = leaveRequestModel
            };
            return View(model);
        }

        // GET: LeaveRequestController/Details/5
        public ActionResult Details(int id)
        {
            var leaveRequest = _leaveRequest.FindById(id);
            var model = _mapper.Map<LeaveRequestVM>(leaveRequest);
            return View(model);
        }

        public ActionResult ApproveRequest(int id)
        {
            try
            {
                var leaveRequest = _leaveRequest.FindById(id);
                var user = _userManager.GetUserAsync(User).Result;

                leaveRequest.Approved = true;
                leaveRequest.ApprovedById = user.Id;
                leaveRequest.RequestedActioned = DateTime.Now;

                var employeeId = leaveRequest.RequestingEmplyeeId;
                var leaveTypeId = leaveRequest.LeaveTypeId;
                var allocation = _leaveAllcoationRepo.GetLeaveAllocationsByEmployeeAndType(employeeId, leaveTypeId);
                var daysRequested = (int)(leaveRequest.EndDate - leaveRequest.StartDate).TotalDays;
                allocation.NumberOfDays -= daysRequested;

                _leaveAllcoationRepo.Update(allocation);
                _leaveRequest.Update(leaveRequest);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return RedirectToAction(nameof(Index));
            }

        }

        public ActionResult myLeave()
        {
            var user = _userManager.GetUserAsync(User).Result;
            var employeeId = user.Id;

            var leaveRequest = _leaveRequest.GetLeaveAllocationsByEmployee(employeeId);
            var leaveAllocation = _leaveAllcoationRepo.GetLeaveAllocationsByEmployee(employeeId);

            var employeeRequestModel = _mapper.Map<List<LeaveRequestVM>>(leaveRequest);
            var employeeAllocationModel = _mapper.Map<List<LeaveAllocationVM>>(leaveAllocation);

            var model = new ViewMyLeaveVM
            {
                leaveAllocations = employeeAllocationModel,
                leaveRequests = employeeRequestModel
            };
            return View(model);
        }

        public ActionResult RejectRequest(int id)
        {
            try
            {
                var leaveRequest = _leaveRequest.FindById(id);
                var user = _userManager.GetUserAsync(User).Result;
                leaveRequest.Approved = false;
                leaveRequest.ApprovedById = user.Id;
                leaveRequest.RequestedActioned = DateTime.Now;

                var isSuccess = _leaveRequest.Update(leaveRequest);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: LeaveRequestController/Create
        public ActionResult Create()
        {
            var leaveTypes = _leaveTypeRepo.FindAll();
            var leaveTypeItems = leaveTypes.Select(q => new SelectListItem
            {
                Text = q.Name,
                Value = q.Id.ToString()
            });
            var model = new CreateLeaveRequestVM
            {
                LeaveTypes = leaveTypeItems
            };
            return View(model);
        }

        // POST: LeaveRequestController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateLeaveRequestVM model)
        {
            try
            {
                var startDate = Convert.ToDateTime(model.StartDate);
                var endDate = Convert.ToDateTime(model.EndDate);
                var leaveTypes = _leaveTypeRepo.FindAll();
                var leaveTypeItems = leaveTypes.Select(q => new SelectListItem
                {
                    Text = q.Name,
                    Value = q.Id.ToString()
                });
                model.LeaveTypes = leaveTypeItems;
                
                if(!ModelState.IsValid)
                {
                    return View(model);
                }

                if(DateTime.Compare(startDate, endDate) > 0)
                {
                    ModelState.AddModelError("", "End Date Can Not Be Bigger Than Start Date");
                    return View(model);
                }
                var employee = _userManager.GetUserAsync(User).Result;
                var allocation = _leaveAllcoationRepo.GetLeaveAllocationsByEmployeeAndType(employee.Id, model.LeaveTypeId);
                int reqetedDays = (int)(endDate.Date - startDate.Date).TotalDays;

                if(reqetedDays > allocation.NumberOfDays)
                {
                    ModelState.AddModelError("", "You don't have enough days for this request");
                    return View(model);
                }

                var leaveRequestModel = new LeaveRequestVM
                {
                    RequestingEmplyeeId = employee.Id,
                    StartDate = startDate,
                    EndDate = endDate,
                    Approved = null,
                    RequestedDate = DateTime.Now,
                    RequestedActioned = DateTime.Now,
                    LeaveTypeId = model.LeaveTypeId,
                    RequestComment = model.RequestComment
                };

                var leaveRequest = _mapper.Map<LeaveRequest>(leaveRequestModel);
                var isSuccess = _leaveRequest.Update(leaveRequest);
                if(!isSuccess)
                {
                    ModelState.AddModelError("", "Somthing went wrong while submiting your record!");
                    return View(model);
                }

                return RedirectToAction("myLeave");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Somthing went wrong!");
                return View(model);
            }
        }

        public ActionResult CancelRequest(int id) 
        {
            var leaveRequest = _leaveRequest.FindById(id);
            leaveRequest.Cancelled = true;
            _leaveRequest.Update(leaveRequest);

            return RedirectToAction("myLeave");

        }

        // GET: LeaveRequestController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: LeaveRequestController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
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

        // GET: LeaveRequestController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: LeaveRequestController/Delete/5
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
