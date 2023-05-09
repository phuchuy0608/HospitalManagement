using HospitalManagement.Interfaces;
using HospitalManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HospitalManagement.Controllers
{
        [Authorize(Roles = "duocsi")]
        public class DuocSiController : Controller
        {
            private readonly IDuocSi _service;
            private readonly IThuoc _thuocService;
            private UserManager<NhanVienYte> _userManager;


            public DuocSiController(IDuocSi service, IThuoc thuocService, UserManager<NhanVienYte> userManager)
            {
                _service = service;
                _thuocService = thuocService;
                _userManager = userManager;
            }

            [HttpGet]
            public async Task<IActionResult> ReloadPage(ToaThuocSearchModel model)
            {

                //model.MaBS = (await _userManager.GetUserAsync(User)).Id;


                var listmodel = await _service.SearchToaThuoc(model);

                if (!model.Page.HasValue) model.Page = 1;




                ViewBag.Names = listmodel;
                ViewBag.TrangThai = model.TrangThai;
                ViewBag.Page = model.Page;
                ViewBag.Data = model;

                return PartialView("_ListToaThuoc", listmodel);
                //return Ok(listToaThuocCTT);
            }


            [HttpGet]
            public async Task<IActionResult> ReloadPageLichSu(ToaThuocSearchModel model)
            {
                var listmodel = await _service.SearchToaThuoc(model);

                if (!model.Page.HasValue) model.Page = 1;
                model.TrangThaiPK = 2;
                ViewBag.Names = listmodel;
                ViewBag.TrangThai = model.TrangThai;
                ViewBag.Page = model.Page;
                ViewBag.Data = model;

                return PartialView("_ListLichSu", listmodel);
                //return Ok(listToaThuocCTT);
            }


            public async Task<IActionResult> ChangeSoUuTien(Guid maPK)
            {
                if (await _service.ChangeSoUuTien(maPK) != null)
                    return Json(new { status = 1, title = "", text = "Thay đổi thành công." }, new Newtonsoft.Json.JsonSerializerSettings());
                else
                    return Json(new { status = -2, title = "", text = "Thay đổi thất bại." }, new Newtonsoft.Json.JsonSerializerSettings());
            }

            //Hàm thanh toán thuốc dùng trong Model ToaThuoc
            [HttpPost]
            public async Task<IActionResult> ThanhToan(Guid maPK)
            {
                var MaNV = (await _userManager.GetUserAsync(User)).Id;
                var result = await _service.ThanhToanThuoc(maPK, MaNV);
                if (result != null)
                {
                    return Json(new { status = 1, title = "", text = "Thanh toán thành công.", redirectUrL = Url.Action("ToaThuoc", "DuocSi"), obj = "" }, new JsonSerializerSettings());
                }
                else
                {
                    return Json(new { status = -2, title = "", text = "Thanh toán không thành công", obj = "" }, new JsonSerializerSettings());
                }
            }


            //Hàm xác nhận thuốc dùng trong Model ToaThuocDangPhat
            [HttpPost]
            public async Task<IActionResult> XacNhanThuocDangCho(Guid maPK)
            {
                var result = await _service.XacNhanThuocDangCho(maPK);
                if (result != null)
                {
                    return Json(new { status = 1, title = "", text = "Xác nhận thành công", redirectUrL = Url.Action("ToaThuocDangPhat", "DuocSi"), obj = "" }, new JsonSerializerSettings());
                }
                else
                {
                    return Json(new { status = -2, title = "", text = "Xác nhận không thành công", obj = "" }, new JsonSerializerSettings());
                }
            }




            [Route("DuocSi")]
            [Route("DuocSi/ToaThuoc")]
            public async Task<IActionResult> ToaThuoc(ToaThuocSearchModel model)
            {

                model.TrangThai = 0;
                model.TrangThaiPK = 1;
                if (!model.Page.HasValue) model.Page = 1;
                var listPaged = await _service.SearchToaThuoc(model);

                ViewBag.Names = listPaged;
                ViewBag.TrangThai = model.TrangThai;
                ViewBag.Page = model.Page;
                ViewBag.Data = model;
                return View();
            }

            public async Task<IActionResult> ChiTietToaThuoc(Guid MaPhieu)
            {
                var toaThuoc = await _service.GetToaThuocByMaPhieu(MaPhieu);
                ViewBag.CTToaThuoc = await _service.GetChiTiet(MaPhieu);

                return PartialView("_ChiTietToaThuoc", toaThuoc);
            }


            // Toathuoc/ToaThuocDangPhat
            public async Task<IActionResult> ToaThuocDangPhat(ToaThuocSearchModel model)
            {
                model.TrangThai = 1;
                model.TrangThaiPK = 1;
                if (!model.Page.HasValue) model.Page = 1;
                var listPaged = await _service.SearchToaThuoc(model);

                ViewBag.Names = listPaged;
                ViewBag.TrangThai = model.TrangThai;
                ViewBag.Page = model.Page;
                ViewBag.Data = model;
                return View();
            }

            public async Task<IActionResult> ChiTietDangPhat(Guid MaPhieu)
            {
                var toaThuoc = await _service.GetToaThuocByMaPhieu(MaPhieu);
                ViewBag.CTToaThuocDangPhat = await _service.GetChiTiet(MaPhieu);
                return PartialView("_ChiTietDangPhat", toaThuoc);
            }



            public async Task<IActionResult> LichSuThuoc(ToaThuocSearchModel model)
            {
                model.TrangThai = 2;
                model.TrangThaiPK = 2;
                if (!model.Page.HasValue) model.Page = 1;
                var listPaged = await _service.SearchToaThuoc(model);

                ViewBag.Names = listPaged;
                ViewBag.TrangThai = model.TrangThai;
                ViewBag.Page = model.Page;
                ViewBag.Data = model;
                return View();
            }
        
            public async Task<IActionResult> ChiTietLichSu(Guid MaPhieu)
            {
                var toaThuoc = await _service.GetToaThuocByMaPhieu(MaPhieu);
                ViewBag.CTLichSuThuoc = await _service.GetChiTiet(MaPhieu);
                return PartialView("_ChiTietLichSu", toaThuoc);
            }

            public async Task<IActionResult> DanhSachThuoc(ThuocSearchModel model)
            {
                if (!model.Page.HasValue) model.Page = 1;
                model.TrangThai = false;
                var listPaged = await _thuocService.SearchByCondition(model);

                ViewBag.Names = listPaged;
                ViewBag.Data = model;
                return View(new ThuocSearchModel());
            }


            [HttpGet]
            public async Task<IActionResult> PageList(ThuocSearchModel model)
            {

                var listmodel = await _thuocService.SearchByCondition(model);
                if (listmodel.Count() > 0)
                {

                    if (!model.Page.HasValue) model.Page = 1;




                    ViewBag.Names = listmodel;
                    ViewBag.Data = model;

                    return PartialView("_NameListThuoc", listmodel);
                    //return View("DanhSachThuoc", listmodel);
                }
                else
                {

                    return Json(new { status = -2, title = "", text = "Không tìm thấy", obj = "" }, new Newtonsoft.Json.JsonSerializerSettings());
                }


            }



            public async Task<IActionResult> ChiTietThuoc(Guid id)
            {
                if (await _thuocService.Get(id) == null)
                {
                    return NotFound(); ;
                }
                else
                {


                    return PartialView("_ChiTietThuoc", await _thuocService.Get(id));
                }
            }

        }
    }
