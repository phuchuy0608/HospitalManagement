using HospitalManagement.Interfaces;
using HospitalManagement.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using System;
using HospitalManagement.Controllers.Admin;
using System.Linq;

namespace HospitalManagement.Controllers
{
    public class NguoiDungController : BaseController
    {
        private readonly INguoiDung _service;
        private readonly IWebHostEnvironment _env;
        public NguoiDungController(INguoiDung service, IWebHostEnvironment env)
        {
            _service = service;
            _env = env;
        }

        public async Task<ActionResult> Index(NguoiDungSearchModel model)
        {

            if (!model.Page.HasValue) model.Page = 1;
            var listPaged = await _service.SearchByCondition(model);

            ViewBag.Names = listPaged;
            ViewBag.Data = model;
            return View(new NguoiDungSearchModel());
        }


        [HttpGet]

        public async Task<ActionResult> PageList(NguoiDungSearchModel model)
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

            return PartialView("_partialAdd", new NguoiDung());

        }

        [HttpPost]
        public async Task<ActionResult> Add([Bind("Email,MatKhau,ConfirmPassword,HoTen,SDT,HinhAnh,ChucVu,TrangThai")] NguoiDung model, [FromForm] IFormFile file)
        {
            if (ModelState.IsValid)
            {
                string filePath = "";
                //IFormFile file = HttpContext.Request.Form.Files[0];
                var filePathDefault = "final.png";

                if (file == null)
                {
                    model.HinhAnh = filePathDefault;
                }
                else
                {
                    //file = HttpContext.Request.Form.Files[0];
                    var fileName = Path.GetFileName(DateTime.Now.ToString("ddMMyyyyss") + file.FileName);
                    model.HinhAnh = fileName;
                    filePath = Path.Combine(_env.WebRootPath, "images/NguoiDung", fileName);
                }

                model.MaNguoiDung = Guid.NewGuid();

                var result = await _service.Add(model);
                if (result.errorCode == -1)
                {
                    ModelState.AddModelError("Email", "Email đã tồn tại");
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
        public async Task<ActionResult> Edit(NguoiDung model)
        {
            var user = await _service.Get(model.MaNguoiDung);
            user.ChucVu = model.ChucVu;
            user.TrangThai = model.TrangThai;
            var result = await _service.Edit(user);
            if (result.errorCode == 0)
            {
                return Json(new { status = 1, title = "", text = "Cập nhật thành công.", obj = "" }, new Newtonsoft.Json.JsonSerializerSettings());
            }
            else
            {
                return Json(new { status = -2, title = "", text = "Cập nhật không thành công.", obj = "" }, new Newtonsoft.Json.JsonSerializerSettings());

            }

        }





        [HttpPost]
        public async Task<ActionResult> Delete(Guid id)
        {
            var user = await _service.Get(id);
            user.TrangThai = false;
            if (await _service.Edit(user) != null)
                return Json(new { status = 1, title = "", text = "Vô hiệu  thành công.", obj = "" }, new Newtonsoft.Json.JsonSerializerSettings());
            else
                return Json(new { status = -2, title = "", text = "Vô hiệu không thành công.", obj = "" }, new Newtonsoft.Json.JsonSerializerSettings());
        }


        [HttpPost]
        public async Task<ActionResult> Restore(Guid id)
        {
            var user = await _service.Get(id);
            user.TrangThai = true;
            if (await _service.Edit(user) != null)
                return Json(new { status = 1, title = "", text = "Khôi phục  thành công.", obj = "" }, new Newtonsoft.Json.JsonSerializerSettings());
            else
                return Json(new { status = -2, title = "", text = "Khôi phục không thành công.", obj = "" }, new Newtonsoft.Json.JsonSerializerSettings());
        }
    }
}
