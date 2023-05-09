using HospitalManagement.Constant;
using HospitalManagement.Interfaces;
using HospitalManagement.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using System.Threading.Tasks;
using System;
using HospitalManagement.Controllers.Admin;
using System.Linq;

namespace HospitalManagement.Controllers
{
        public class TinTucController : BaseController
        {
            private readonly ITinTuc _service;
            private readonly ITheLoai _theLoaiRep;
            private readonly IWebHostEnvironment _env;

            public TinTucController(ITinTuc service, ITheLoai theLoaiRep, IWebHostEnvironment env)
            {
                _service = service;
                _theLoaiRep = theLoaiRep;
                _env = env;
            }


            public async Task<ActionResult> Index(TinTucSearchModel model)
            {
                if (!model.Page.HasValue) model.Page = 1;
                var listPaged = await _service.SearchByCondition(model);
                ViewBag.MaNguoiViet = _service.NguoiDungNav();
                ViewBag.MaTL = await _theLoaiRep.GetAll();
                ViewBag.Names = listPaged;
                ViewBag.Data = model;
                return View(new TinTucSearchModel());
            }


            [HttpGet]

            public async Task<ActionResult> PageList(TinTucSearchModel model)
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


            public async Task<ActionResult> ThemTinTuc()
            {
                ViewBag.MaTL = new SelectList(await _theLoaiRep.GetAll(), "MaTL", "TenTL");
                return View(new TinTuc { TrangThai = true });
            }


            [HttpPost]
            public async Task<ActionResult> ThemTinTuc(TinTuc model)

            {


                string maNguoiDung = HttpContext.Session.GetString(SessionKey.Nguoidung.MaNguoiDung);
                model.MaBaiViet = Guid.NewGuid();
                model.MaNguoiViet = Guid.Parse(maNguoiDung);
                model.ThoiGian = DateTime.Now;
                if (await _service.Add(model) != null)

                    return
                        RedirectToAction("index", "Tintuc");
                else
                    return View(model);


            }

            [HttpGet]
            public async Task<ActionResult> Edit(Guid id)
            {
                ViewBag.MaTL = new SelectList(await _theLoaiRep.GetAll(), "MaTL", "TenTL");
                var item = await _service.Get(id);
                if (item == null)
                {
                    return NotFound(); ;
                }
                else
                {
                    ViewBag.MaNguoiViet = new SelectList(_service.NguoiDungNav(), "MaNguoiDung", "Email", (await _service.Get(id)).MaNguoiViet);
                    return PartialView("_partialedit", item);
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
                    ViewBag.MaNguoiViet = new SelectList(_service.NguoiDungNav(), "MaNguoiDung", "Email", (await _service.Get(id)).MaNguoiViet);
                    return PartialView("_partialDetail", await _service.Get(id));
                }
            }

            [HttpPost]
            public async Task<ActionResult> Preview(TinTuc model)
            {
                string maNguoiDung = HttpContext.Session.GetString(SessionKey.Nguoidung.MaNguoiDung);
                model.MaBaiViet = Guid.NewGuid();
                model.MaNguoiViet = Guid.Parse(maNguoiDung);
                ViewBag.TenTL = (await _theLoaiRep.Get(Guid.Parse(model.MaTL.ToString()))).TenTL;
                return PartialView("_partialPreview", model);
            }


            [HttpPost]
            public async Task<ActionResult> Edit(TinTuc model)
            {
                if (await _service.Edit(model) != null)
                return PartialView("_partialedit", model);

            else
                return RedirectToAction("index", "Tintuc");
            }


            [HttpPost]
            public async Task<ActionResult> Delete(Guid id)
            {
                var tin = await _service.Get(id);
                tin.TrangThai = false;
                if (await _service.Edit(tin) != null)
                    return Json(new { status = 1, title = "", text = "Xoá thành công.", obj = "" }, new Newtonsoft.Json.JsonSerializerSettings());
                else
                    return Json(new { status = -2, title = "", text = "Xoá không thành công.", obj = "" }, new Newtonsoft.Json.JsonSerializerSettings());
            }


            [HttpPost]
            public async Task<ActionResult> Restore(Guid id)
            {
                var tin = await _service.Get(id);
                tin.TrangThai = true;
                if (await _service.Edit(tin) != null)
                    return Json(new { status = 1, title = "", text = "Khôi phục thành công.", obj = "" }, new Newtonsoft.Json.JsonSerializerSettings());
                else
                    return Json(new { status = -2, title = "", text = "Khôi phục không thành công.", obj = "" }, new Newtonsoft.Json.JsonSerializerSettings());
            }

            public JsonResult UploadFile(IFormFile aUploadedFile)
            {
                var vReturnImagePath = string.Empty;
                if (aUploadedFile.Length > 0)
                {
                    var vFileName = Path.GetFileNameWithoutExtension(aUploadedFile.FileName);
                    var vExtension = Path.GetExtension(aUploadedFile.FileName);

                    string sImageName = vFileName + DateTime.Now.ToString("ddMMyyyyss");

                    var vImageSavePath = Path.Combine(_env.WebRootPath, "images/photos/") + sImageName + vExtension;
                    vReturnImagePath = "/images/photos/" + sImageName + vExtension;
                    ViewBag.Msg = vImageSavePath;
                    var path = vImageSavePath;

                    // Saving Image in Original Mode  
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        aUploadedFile.CopyTo(fileStream);
                    }
                    var vImageLength = new FileInfo(path).Length;
                    TempData["message"] = string.Format("Image was Added Successfully");

                }

                return Json(Convert.ToString(vReturnImagePath), new Newtonsoft.Json.JsonSerializerSettings());
            }
        }
    }
