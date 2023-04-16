
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HospitalManagement.Controllers.Admin;
using HospitalManagement.Interfaces;
using HospitalManagement.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace HospitalManagement.Controllers
{

    public class DichVuController : BaseController
    {
        private readonly IDichVu _service;
        public DichVuController(IDichVu service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index(DichVuSearchModel model)
        {

            if (!model.Page.HasValue) model.Page = 1;
            var listPaged = await _service.SearchByCondition(model);

            ViewBag.Names = listPaged;
            ViewBag.Data = model;
            return View(new DichVuSearchModel());
        }


        [HttpGet]
        public async Task<IActionResult> PageList(DichVuSearchModel model)
        {
            var listmodel = await _service.SearchByCondition(model);
            if (listmodel.Count() > 0)
            {

                if (!model.Page.HasValue) model.Page = 1;

                ViewBag.Names = listmodel;
                ViewBag.Data = model;

                return PartialView("_NameListPartial", listmodel);
            }
            else
            {
                return Json(new { status = -2, title = "", text = "Không tìm thấy", obj = "" }, new Newtonsoft.Json.JsonSerializerSettings());
            }

        }


        public IActionResult Add()
        {

            return PartialView("_partialAdd", new DichVu());

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(DichVu model)
        {
            model.TrangThai = true;
            if (ModelState.IsValid)
            {
                model.MaDV = Guid.NewGuid();
                var result = await _service.Add(model);
                if (result.errorCode == -1)
                {
                    ModelState.AddModelError("TenDV", "Tên dịch vụ đã tồn tại");
                    return PartialView("_partialAdd", model);
                }
                if (result.errorCode == 0)
                    return Json(new { status = 1, title = "", text = "Thêm thành công.", obj = "" }, new Newtonsoft.Json.JsonSerializerSettings());
                else
                    return Json(new { status = -2, title = "", text = "Thêm không thành công.", obj = "" }, new Newtonsoft.Json.JsonSerializerSettings());
            }
            return PartialView("_partialAdd", model);
        }


        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            if (await _service.Get(id) == null)
            {
                return NotFound(); ;
            }
            else
            {

                return PartialView("_partialedit", await _service.Get(id));
            }

        }


        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            if (await _service.Get(id) == null)
            {
                return NotFound(); ;
            }
            else
            {

                return PartialView("_partialDetail", await _service.Get(id));
            }
        }


        [HttpPost]

        public async Task<IActionResult> Edit(DichVu model)
        {
            if (ModelState.IsValid)
            {
                var result = await _service.Edit(model);
                if (result.errorCode == -1)
                {
                    ModelState.AddModelError("TenDV", "Tên dịch vụ đã tồn tại");
                    return PartialView("_partialedit", model);
                }
                if (result.errorCode == 0)
                    return Json(new { status = 1, title = "", text = "Cập nhật thành công.", obj = "" }, new Newtonsoft.Json.JsonSerializerSettings());
                else
                    return Json(new { status = -2, title = "", text = "Cập nhật không thành công.", obj = "" }, new Newtonsoft.Json.JsonSerializerSettings());
            }
            return PartialView("_partialedit", model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var dv = await _service.Get(id);
            dv.TrangThai = false;
            if (await _service.Edit(dv) != null)
                return Json(new { status = 1, title = "", text = "Xoá thành công.", obj = "" }, new Newtonsoft.Json.JsonSerializerSettings());
            else
                return Json(new { status = -2, title = "", text = "Xoá không thành công.", obj = "" }, new Newtonsoft.Json.JsonSerializerSettings());
        }


        [HttpPost]
        public async Task<IActionResult> Restore(Guid id)
        {
            var dv = await _service.Get(id);
            dv.TrangThai = true;
            if (await _service.Edit(dv) != null)
                return Json(new { status = 1, title = "", text = "Khôi phục thành công.", obj = "" }, new Newtonsoft.Json.JsonSerializerSettings());
            else
                return Json(new { status = -2, title = "", text = "Thao Tác không thành công.", obj = "" }, new Newtonsoft.Json.JsonSerializerSettings());
        }
    }
}


