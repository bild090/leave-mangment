using AutoMapper;
using leave_mangment.contracts;
using leave_mangment.Data;
using leave_mangment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_mangment.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class LeaveTypeController : Controller
    {
        private readonly ILeaveTypeRepository _repo;
        private readonly IMapper _mapper;

        public LeaveTypeController(ILeaveTypeRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
        // GET: LeaveTypeController1
        public async Task<ActionResult> Index()
        {
            var leavetype = await _repo.FindAll();
            var model = _mapper.Map<List<LeaveType>, List<LeaveTypeVM>>(leavetype.ToList());
            return View(model);
        }

        // GET: LeaveTypeController1/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var isExists = await _repo.exists(id);

            if (!isExists){
                return NotFound();
            }
            var leavType = await _repo.FindById(id);
            var model = _mapper.Map<LeaveTypeVM>(leavType);
            return View(model);
        }

        // GET: LeaveTypeController1/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LeaveTypeController1/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(LeaveTypeVM model)
        {
            try
            {
                if (!ModelState.IsValid) {

                    return View(model);
                }
                var leaveType = _mapper.Map<LeaveType>(model);
                leaveType.DateCreated = DateTime.Now;

                var isSuccess = await _repo.Create(leaveType);
                if (!isSuccess) {
                    ModelState.AddModelError("", "somthing went wrong");
                    return View(model);
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "somthing went wrong");
                return View();
            }
        }

        // GET: LeaveTypeController1/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var isExists = await _repo.exists(id);

            if (!isExists)
            {
                return NotFound();
            }
            var leaveType = await _repo.FindById(id);
            var model = _mapper.Map<LeaveTypeVM>(leaveType);
            return View(model);
        }

        // POST: LeaveTypeController1/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, LeaveTypeVM model)
        {
            try
            {
                if ( !ModelState.IsValid) {
                    return View(model);
                }
                var leaveType = _mapper.Map<LeaveType>(model);
                var isSuccess = await _repo.Update(leaveType);

                if (!isSuccess) {
                    ModelState.AddModelError("", "somthin went wron");
                    return View(model);
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "somthin went wron");
                return View(model);
            }
        }

        // GET: LeaveTypeController1/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            var isExists = await _repo.exists(id);

            if (!isExists)
            {
                return NotFound();
            }
            var leaveType = await _repo.FindById(id);
            var model = _mapper.Map<LeaveTypeVM>(leaveType);
            return View(model);
        }

        // POST: LeaveTypeController1/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, IFormCollection collection)
        {
            try
            {
                var leaveType = await _repo.FindById(id);
                var isSuccess = _repo.Delete(leaveType);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
