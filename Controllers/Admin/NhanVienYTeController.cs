using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using HospitalManagement.Controllers.Admin;
using HospitalManagement.Interfaces;
using HospitalManagement.Models;

namespace HospitalManagement.Controllers
{
    public class NhanVienYTeController : BaseController
    {
        private readonly INhanVienYte _service;
        private readonly IChuyenKhoa _chuyenkhoaRep;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<NhanVienYte> _userManager;

        public NhanVienYTeController(INhanVienYte service, IChuyenKhoa chuyenkhoaRep, UserManager<NhanVienYte> userManager, IWebHostEnvironment env)
        {
            _service = service;
            _chuyenkhoaRep = chuyenkhoaRep;
            _userManager = userManager;
            _env = env;

        }

        public async Task<ActionResult> Index(NhanVienYteSearchModel model)
        {

            if (!model.Page.HasValue) model.Page = 1;
            var listPaged = await _service.SearchByCondition(model);
            ViewBag.ChuyenKhoa = await _chuyenkhoaRep.GetAll();


            ViewBag.Names = listPaged;
            ViewBag.Data = model;
            return View(new NhanVienYteSearchModel());
        }

        [HttpGet]
        public async Task<ActionResult> PageList(NhanVienYteSearchModel model)
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

        public async Task<ActionResult> Add()
        {
            ViewBag.ChuyenKhoa = new SelectList(await _chuyenkhoaRep.GetAll(), "MaCK", "TenCK");

            return PartialView("_partialAdd", new NhanVienModel());

        }

        [HttpPost]
        public async Task<ActionResult> Add([Bind("Email,MatKhau,ConfirmPassword,HoTen,SDTNV,ChucVu,TrangThai,Hinh,ChuyenKhoa")] NhanVienModel model, [FromForm] IFormFile file)
        {
            if (ModelState.IsValid)
            {
                string filePath = "";
                var filePathDefault = "final.png";

                if (file == null)
                {
                    model.Hinh = filePathDefault;
                }
                else
                {
                    var fileName = Path.GetFileName(DateTime.Now.ToString("ddMMyyyyss") + file.FileName);
                    model.Hinh = fileName;
                    filePath = Path.Combine(_env.WebRootPath, "images/NguoiDung", fileName);
                }



                var user = new NhanVienYte { UserName = model.Email, Email = model.Email, HoTen = model.HoTen, ChucVu = model.ChucVu, TrangThai = model.TrangThai, Hinh = model.Hinh, ChuyenKhoa = model.ChuyenKhoa, PhoneNumber = model.SDTNV };
                var result = await _userManager.CreateAsync(user, model.MatKhau);


                if (result.Succeeded)
                {

                    if (model.ChucVu == 1)

                        await _userManager.AddToRoleAsync(user, "nhanvien");

                    else if (model.ChucVu == 2)


                        await _userManager.AddToRoleAsync(user, "bacsi");
                    else
                        await _userManager.AddToRoleAsync(user, "duocsi");

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
                    foreach (var error in result.Errors)
                    {
                        if (error.Code.Contains("Email") || error.Code.Contains("UserName"))
                        {
                            ModelState.AddModelError("Email", error.Description);
                        }
                        if (error.Code.Contains("Password"))
                        {
                            ModelState.AddModelError("MatKhau", "Mật khẩu không hợp lệ");
                        }

                    }
                    return PartialView("_partialAdd", model);

                }
            }
            return PartialView("_partialAdd", model);


        }



        [HttpGet]
        public async Task<ActionResult> Detail(Guid id)
        {
            var item = await _service.Get(id.ToString());
            if (item == null)
            {
                return NotFound(); ;
            }
            else
            {
                ViewBag.ChuyenKhoa = new SelectList(await _chuyenkhoaRep.GetAll(), "MaCK", "TenCK", item.ChuyenKhoa);


                return PartialView("_partialDetail", item);
            }
        }


        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            var user = await _service.Get(id);
            user.TrangThai = false;
            var item = await _service.Edit(id, false);
            if (item.errorCode == 0)
                return Json(new { status = 1, title = "", text = "Xoá thành công.", obj = "" }, new Newtonsoft.Json.JsonSerializerSettings());
            else
                return Json(new { status = -2, title = "", text = "Xoá không thành công.", obj = "" }, new Newtonsoft.Json.JsonSerializerSettings());
        }
        [HttpPost]
        public async Task<ActionResult> Restore(string id)
        {
            var user = await _service.Get(id);
            user.TrangThai = false;
            var item = await _service.Edit(id, true);
            if (item.errorCode == 0)
                return Json(new { status = 1, title = "", text = "Khôi phục thành công.", obj = "" }, new Newtonsoft.Json.JsonSerializerSettings());
            else
                return Json(new { status = -2, title = "", text = "Khôi phục không thành công.", obj = "" }, new Newtonsoft.Json.JsonSerializerSettings());
        }
    }
}
