
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

    public class ChuyenKhoaController : BaseController
    {
        private readonly IChuyenKhoa _service;

        public ChuyenKhoaController(IChuyenKhoa service)
        {
            _service = service;

        }

        public async Task<IActionResult> Index(ChuyenKhoaSearchModel model)
        {

            if (!model.Page.HasValue) model.Page = 1;
            var listPaged = await _service.SearchByCondition(model);

            ViewBag.Names = listPaged;
            ViewBag.Data = model;
            return View(new ChuyenKhoaSearchModel());
        }


        [HttpGet]
        public async Task<IActionResult> PageList(ChuyenKhoaSearchModel model)
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
            return PartialView("_partialAdd", new ChuyenKhoa());
        }

        [HttpPost]
        public async Task<IActionResult> Add(ChuyenKhoa model)
        {
            if (ModelState.IsValid)
            {
                model.MaCK = Guid.NewGuid();
                var result = await _service.Add(model);
                if (result.errorCode == -1)
                {
                    ModelState.AddModelError("TenCK", "Tên chuyên khoa đã tồn tại");
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
                return PartialView("_partialDetail", _service.Get(id));
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ChuyenKhoa model)
        {

            if (ModelState.IsValid)
            {

                var result = await _service.Edit(model);
                if (result.errorCode == -1)
                {
                    ModelState.AddModelError("TenCK", "Tên chuyên khoa đã tồn tại");
                    return PartialView("_partialedit", model);
                }
                if (result.errorCode == 0)
                    return Json(new { status = 1, title = "", text = "Cập nhật thành công.", obj = "" }, new Newtonsoft.Json.JsonSerializerSettings());
                else
                    return Json(new { status = -2, title = "", text = "Cập nhật không thành công.", obj = "" }, new Newtonsoft.Json.JsonSerializerSettings());
            }
            return PartialView("_partialedit", model);
        }


    }
}


