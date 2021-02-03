using AutoMapper;
using leave_mangment.contracts;
using leave_mangment.Data;
using leave_mangment.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_mangment.Controllers
{
    public class LeaveTypeController : Controller
    {
        private ILeaveTypeRepository _repo;
        private IMapper _mapper;

        public LeaveTypeController(ILeaveTypeRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
        // GET: LeaveTypeController1
        public ActionResult Index()
        {
            var leavetype = _repo.FindAll().ToList();
            var model = _mapper.Map<List<LeaveType>, List<LeaveTypeMV>>(leavetype);
            return View(model);
        }

        // GET: LeaveTypeController1/Details/5
        public ActionResult Details(int id)
        {
            if (!_repo.exists(id)){
                return NotFound();
            }
            var leavType = _repo.FindById(id);
            var model = _mapper.Map<LeaveTypeMV>(leavType);
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
        public ActionResult Create(LeaveTypeMV model)
        {
            try
            {
                if (!ModelState.IsValid) {

                    return View(model);
                }
                var leaveType = _mapper.Map<LeaveType>(model);
                leaveType.DateCreated = DateTime.Now;

                var isSuccess = _repo.Create(leaveType);
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
        public ActionResult Edit(int id)
        {
            if (!_repo.exists(id)) {
                return NotFound();
            }
            var leaveType = _repo.FindById(id);
            var model = _mapper.Map<LeaveTypeMV>(leaveType);
            return View(model);
        }

        // POST: LeaveTypeController1/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, LeaveTypeMV model)
        {
            try
            {
                if ( !ModelState.IsValid) {
                    return View(model);
                }
                var leaveType = _mapper.Map<LeaveType>(model);
                var isSuccess = _repo.Update(leaveType);

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
        public ActionResult Delete(int id)
        {
            if (!_repo.exists(id))
            {
                return NotFound();
            }
            var leaveType = _repo.FindById(id);
            var model = _mapper.Map<LeaveTypeMV>(leaveType);
            return View(model);
        }

        // POST: LeaveTypeController1/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                var leaveType = _repo.FindById(id);
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
