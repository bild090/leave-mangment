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
        private readonly IUnitOfWork _UnitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<Employee> _userManager;
        // GET: LeaveRequestController

        public LeaveRequestController(
            UserManager<Employee> userManager,
            IMapper mapper,
            IUnitOfWork UnitOfWork
            )
        {
            _userManager = userManager;
            _mapper = mapper;
            _UnitOfWork = UnitOfWork;
        }
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> Index()
        {
            var leaveRequest = await _UnitOfWork.LeaveRequests.FindAll(
                Includes: new List<string> { "RequestingEmplyee", "LeaveType"});
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
        public async Task<ActionResult> Details(int id)
        {
            var leaveRequest = await _UnitOfWork.LeaveRequests.Find(q => q.Id == id,
                Includes: new List<string> { "ApprovedBy", "RequestingEmplyee", "LeaveType" });
            var model = _mapper.Map<LeaveRequestVM>(leaveRequest);
            return View(model);
        }

        public async Task<ActionResult> ApproveRequest(int id)
        {
            try
            {
                var leaveRequest = await _UnitOfWork.LeaveRequests.Find(q => q.Id == id);
                var user =await _userManager.GetUserAsync(User);

                leaveRequest.Approved = true;
                leaveRequest.ApprovedById = user.Id;
                leaveRequest.RequestedActioned = DateTime.Now;
                var period = DateTime.Now.Year;
                var employeeId = leaveRequest.RequestingEmplyeeId;
                var leaveTypeId = leaveRequest.LeaveTypeId;
                var allocation = await _UnitOfWork.LeaveAllocations.Find(q => q.EmployeeId == employeeId
                                                        && q.Period == period
                                                        && q.LeaveTypeId == leaveTypeId);

                var daysRequested = (int)(leaveRequest.EndDate - leaveRequest.StartDate).TotalDays;
                allocation.NumberOfDays -= daysRequested;

                _UnitOfWork.LeaveAllocations.Update(allocation);
                _UnitOfWork.LeaveRequests.Update(leaveRequest);
                await _UnitOfWork.Save();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return RedirectToAction(nameof(Index));
            }

        }

        public async Task<ActionResult> myLeave()
        {
            var user = await _userManager.GetUserAsync(User);
            var employeeId = user.Id;

            var leaveRequest = await _UnitOfWork.LeaveRequests.FindAll(q => q.RequestingEmplyeeId == employeeId);
            var leaveAllocation = await _UnitOfWork.LeaveAllocations.FindAll(q => q.EmployeeId == employeeId, 
                Includes: new List<string> { "LeaveType"});

            var employeeRequestModel = _mapper.Map<List<LeaveRequestVM>>(leaveRequest);
            var employeeAllocationModel = _mapper.Map<List<LeaveAllocationVM>>(leaveAllocation);

            var model = new ViewMyLeaveVM
            {
                leaveAllocations = employeeAllocationModel,
                leaveRequests = employeeRequestModel
            };
            return View(model);
        }

        public async Task<ActionResult> RejectRequest(int id)
        {
            try
            {
                var leaveRequest = await _UnitOfWork.LeaveRequests.Find(q => q.Id == id);
                var user = await _userManager.GetUserAsync(User);
                leaveRequest.Approved = false;
                leaveRequest.ApprovedById = user.Id;
                leaveRequest.RequestedActioned = DateTime.Now;

                _UnitOfWork.LeaveRequests.Update(leaveRequest);
                await _UnitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: LeaveRequestController/Create
        public async Task<ActionResult> Create()
        {
            var leaveTypes = await _UnitOfWork.LeaveTypes.FindAll();
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
        public async Task<ActionResult> Create(CreateLeaveRequestVM model)
        {
            try
            {
                var startDate = Convert.ToDateTime(model.StartDate);
                var endDate = Convert.ToDateTime(model.EndDate);
                var leaveTypes = await _UnitOfWork.LeaveTypes.FindAll();
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
                var period = DateTime.Now.Year;
                var employee = await _userManager.GetUserAsync(User);
                var allocation = await _UnitOfWork.LeaveAllocations.Find(q => q.EmployeeId == employee.Id
                                                        && q.Period == period
                                                        && q.LeaveTypeId == model.LeaveTypeId);
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
                 _UnitOfWork.LeaveRequests.Update(leaveRequest);
                await _UnitOfWork.Save();
                return RedirectToAction("myLeave");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Somthing went wrong!");
                return View(model);
            }
        }

        public async Task<ActionResult> CancelRequest(int id) 
        {
            var leaveRequest = await _UnitOfWork.LeaveRequests.Find(q => q.Id == id);
            leaveRequest.Cancelled = true;
            _UnitOfWork.LeaveRequests.Update(leaveRequest);
            await _UnitOfWork.Save();

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
        protected override void Dispose(bool disposing)
        {
            _UnitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}
