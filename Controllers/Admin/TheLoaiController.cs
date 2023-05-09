using HospitalManagement.Controllers.Admin;
using HospitalManagement.Interfaces;
using HospitalManagement.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace HospitalManagement.Controllers
{
    public class TheLoaiController : BaseController
    {
        private readonly ITheLoai _service;
        public TheLoaiController(ITheLoai service)
        {
            _service = service;
        }

        public async Task<ActionResult> Index(TheLoaiSearchModel model)
        {

            if (!model.Page.HasValue) model.Page = 1;
            var listPaged = await _service.SearchByCondition(model);

            ViewBag.Names = listPaged;
            ViewBag.Data = model;
            return View(new TheLoaiSearchModel());
        }


        [HttpGet]

        public async Task<ActionResult> PageList(TheLoaiSearchModel model)
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

            return PartialView("_partialAdd", new TheLoai());

        }
        [HttpPost]


        public async Task<ActionResult> Add(TheLoai model)
        {
            if (ModelState.IsValid)
            {
                model.MaTL = Guid.NewGuid();
                var result = await _service.Add(model);
                if (result.errorCode == -1)
                {
                    ModelState.AddModelError("TenTL", "Thể loại đã tồn tại");
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
        public async Task<ActionResult> Edit(Guid id)
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
        public async Task<ActionResult> Detail(Guid id)
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
        public async Task<ActionResult> Edit(TheLoai model)
        {
            if (ModelState.IsValid)
            {
                var result = await _service.Edit(model);
                if (result.errorCode == -1)
                {
                    ModelState.AddModelError("TenTL", "Thể loại đã tồn tại");
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
        public async Task<ActionResult> Delete(Guid id)
        {
            if (await _service.Delete(id))
                return Json(new { status = 1, title = "", text = "Xoá thành công.", obj = "" }, new Newtonsoft.Json.JsonSerializerSettings());
            else
                return Json(new { status = -2, title = "", text = "Xoá không thành công.", obj = "" }, new Newtonsoft.Json.JsonSerializerSettings());
        }
    }
}
