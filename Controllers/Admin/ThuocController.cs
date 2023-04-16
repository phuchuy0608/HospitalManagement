using Hangfire;
using HospitalManagement.Interfaces;
using HospitalManagement.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace HospitalManagement.Controllers.Admin
{
    public class ThuocController : BaseController
    {
        private readonly IThuoc _service;
        private readonly IWebHostEnvironment _env;
        private readonly ITienIch _tienIchRep;
        public ThuocController(IThuoc service, IWebHostEnvironment env, ITienIch tienIichRep)
        {
            _service = service;
            _env = env;
            _tienIchRep = tienIichRep;
        }

        public async Task<IActionResult> Index(ThuocSearchModel model)
        {

            if (!model.Page.HasValue) model.Page = 1;
            var listPaged = await _service.SearchByCondition(model);

            ViewBag.Names = listPaged;
            ViewBag.Data = model;
            return View(new ThuocSearchModel());
        }


        [HttpGet]

        public async Task<IActionResult> PageList(ThuocSearchModel model)
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
            return PartialView("_partialAdd", new Thuoc());
        }

        [HttpPost]
        public async Task<IActionResult> Add(Thuoc model, [FromForm] IFormFile file)
        {
            if (ModelState.IsValid)
            {
                string filePath = "";
                var filePathDefault = "drugs.jpg";

                if (file == null)
                {
                    model.HinhAnh = filePathDefault;
                }
                else
                {
                    var fileName = Path.GetFileName(DateTime.Now.ToString("ddMMyyyyss") + file.FileName);
                    model.HinhAnh = fileName;
                    filePath = Path.Combine(_env.WebRootPath, "images/imagesThuoc", fileName);
                }

                model.MaThuoc = Guid.NewGuid();
                var result = await _service.Add(model);
                if (result.errorCode == -1)
                {
                    ModelState.AddModelError("TenThuoc", "Tên thuốc đã tồn tại");
                    return PartialView("_partialAdd", model);
                }
                if (result.errorCode == 0)
                {
                    if (file != null)
                    {
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                    }
                    using (new BackgroundJobServer())
                    {
                        _tienIchRep.refreshCacheThuoc();
                    }
                    return Json(new { status = 1, title = "", text = "Thêm thành công.", obj = "" }, new Newtonsoft.Json.JsonSerializerSettings());
                }
                else
                {
                    return Json(new { status = -2, title = "", text = "Thêm không thành công.", obj = "" }, new Newtonsoft.Json.JsonSerializerSettings());
                }
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
        public async Task<IActionResult> Edit(Thuoc model, [FromForm] IFormFile file)
        {
            if (ModelState.IsValid)
            {
                string filePath = "";
                if (file != null)
                {
                    var fileName = Path.GetFileName(DateTime.Now.ToString("ddMMyyyyss") + file.FileName);
                    model.HinhAnh = fileName;
                    filePath = Path.Combine(_env.WebRootPath, "images/imagesThuoc", fileName);
                }

                var result = await _service.Edit(model);
                if (result.errorCode == -1)
                {
                    ModelState.AddModelError("TenThuoc", "Tên thuốc đã tồn tại");
                    return PartialView("_partialedit", model);
                }

                if (result.errorCode == 0)
                {
                    if (file != null)
                    {
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }
                    }
                    using (new BackgroundJobServer())
                    {

                        _tienIchRep.refreshCacheThuoc();
                    }
                    return Json(new { status = 1, title = "", text = "Cập nhật thành công.", obj = "" }, new Newtonsoft.Json.JsonSerializerSettings());
                }
                else
                {
                    return Json(new { status = -2, title = "", text = "Cập nhật không thành công.", obj = "" }, new Newtonsoft.Json.JsonSerializerSettings());
                }
            }
            return PartialView("_partialedit", model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var thuoc = await _service.Get(id);
            thuoc.TrangThai = false;
            if (await _service.Edit(thuoc) != null)
                return Json(new { status = 1, title = "", text = "Xoá thành công.", obj = "" }, new Newtonsoft.Json.JsonSerializerSettings());
            else
                return Json(new { status = -2, title = "", text = "Xoá không thành công.", obj = "" }, new Newtonsoft.Json.JsonSerializerSettings());
        }
        [HttpPost]
        public async Task<IActionResult> Restore(Guid id)
        {
            var thuoc = await _service.Get(id);
            thuoc.TrangThai = true;
            if (await _service.Edit(thuoc) != null)
                return Json(new { status = 1, title = "", text = "Khôi phục thành công.", obj = "" }, new Newtonsoft.Json.JsonSerializerSettings());
            else
                return Json(new { status = -2, title = "", text = "Thao tác không thành công.", obj = "" }, new Newtonsoft.Json.JsonSerializerSettings());
        }
    }
}
