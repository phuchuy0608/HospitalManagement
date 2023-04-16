

using HospitalManagement.Infrastructure;
using HospitalManagement.Interfaces;
using HospitalManagement.Models.SearchModel;
using HospitalManagement.Models;
using HospitalManagement.Share.Models.ViewModel;
using HospitalManagement_Entities.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace HospitalManagement.Controllers.TiepNhan
{

    [Authorize(Roles = "nhanvien")]
    public class TiepNhanController : Controller
    {
        private readonly ITiepNhan _service;
        private readonly IChuyenKhoa _chuyenkhoaRep;
        private readonly IDichVu _dichvuRep;
        private readonly INhanVienYte _nhanvienyteRep;
        private readonly IHubContext<RealtimeHub> _hubContext;
        private readonly UserManager<NhanVienYte> _userManager;
        private readonly IKhamBenh _khamBenh;
        private readonly IDuocSi _duocSiService;


        public TiepNhanController(
            ITiepNhan service,
            IChuyenKhoa chuyenKhoaRep,
            IDichVu dichvuRep,
            INhanVienYte nhanVienYteRep,
            IHubContext<RealtimeHub> hubContext,
            UserManager<NhanVienYte> userManager,
            IKhamBenh khamBenh,
            IDuocSi duocSiService
            )
        {
            _service = service;
            _chuyenkhoaRep = chuyenKhoaRep;
            _dichvuRep = dichvuRep;
            _nhanvienyteRep = nhanVienYteRep;
            _hubContext = hubContext;
            _userManager = userManager;
            _khamBenh = khamBenh;
            _duocSiService = duocSiService;
        }

        public async Task<IActionResult> ChiTietLichSuKham(Guid MaPK)
        {
            ViewBag.CTLichSuDichVu = await _dichvuRep.GetDichVu(MaPK);
            ViewBag.CTLichSuThuoc = await _duocSiService.GetChiTiet(MaPK);
            return PartialView("_PartialCT_LichSuKhamTiepNhan", await _khamBenh.GetLichSuKhamById(MaPK));
        }

        [Route("/tiepnhan/LichSuKhamTiepNhan")]
        public async Task<IActionResult> LichSuKhamTiepNhan(string hoTen, string SDT)
        {
            var result = await _khamBenh.GetLichSu(hoTen, SDT);
            if (result.Count() > 0)
            {
                return Json(JsonConvert.SerializeObject(result, Formatting.Indented,
                    new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    }));
            }
            else
            {
                return Json(new { status = -2 }, new JsonSerializerSettings());

            }
        }

        [Route("/tiepnhan")]
        [Route("/tiepnhan/ThemPhieuKham")]
        public async Task<IActionResult> ThemPhieuKham(string MaPhieu)
        {
            ViewBag.ListCK = new SelectList(await _chuyenkhoaRep.GetChuyenKhoaHaveDoctor(), "MaCK", "TenCK");

            ViewBag.ListDV = await _dichvuRep.GetDichVu(Guid.Empty);

            var model = await _service.GetPhieuDatLichById(MaPhieu);

            if (!string.IsNullOrWhiteSpace(MaPhieu))
            {

                var phieuKham = new PhieuKhamViewModel { MaPhieuDatLich = MaPhieu, HoTen = model.TenBN, SDT = model.SDT, Email = model.Email, NgaySinh = model.NgaySinh, UuTien = true };
                return View(phieuKham);

            }
            return View(new PhieuKhamViewModel());


        }

        public async Task<IActionResult> DeletePhieuDatLich(string MaPhieu)
        {
            var result = await _service.DeletePhieuDatLichById(MaPhieu);

            if (result == true)
            {
                return Json(new { status = 1, title = "", text = "Xóa thành công.", obj = "" }, new JsonSerializerSettings());
            }

            else
                return Json(new { status = -2, title = "", text = "Xóa không thành công", obj = "" }, new JsonSerializerSettings());
        }

        public async Task<JsonResult> DocTor_Bind(Guid MaCK)
        {

            var list = await _nhanvienyteRep.GetAllBS(MaCK);
            List<SelectListItem> ListBS = new List<SelectListItem>();
            foreach (var item in list)
            {
                ListBS.Add(new SelectListItem { Text = item.HoTen, Value = item.Id.ToString() });
            }
            return Json(ListBS, new JsonSerializerSettings());
        }

        public async Task<JsonResult> BenhNhan_bind(string SDT)
        {
            return Json(await _service.GetBN(SDT), new JsonSerializerSettings());
        }


        [HttpGet]
        public async Task<IActionResult> GetListDV(Guid MaPhieu)
        {

            return PartialView("_AddDichVu", await _dichvuRep.GetDichVu(MaPhieu));
        }

        [HttpPost]
        public async Task<IActionResult> XacNhanDichVu(PhieuKhamViewModel model)
        {
            if (model.dichVus != null)
            {

                ViewBag.BacSi = await _nhanvienyteRep.Get(model.MaBS.ToString());
                var result = new PhieuKhamViewModel { MaBS = model.MaBS, HoTen = model.HoTen, SDT = model.SDT, GioiTinh = model.GioiTinh, NgaySinh = model.NgaySinh, TrieuChung = model.TrieuChung, DiaChi = model.DiaChi, UuTien = model.UuTien };
                result.dichVus = new List<DichVu>();

                foreach (var item in model.dichVus)
                {
                    result.dichVus.Add(await _dichvuRep.Get(item.MaDV));
                }
                return PartialView("_XacNhanDichVu", result);

            }
            else
                return Json(new { status = -2, title = "", text = "Vui lòng chọn it nhất một dịch vụ", obj = "" }, new JsonSerializerSettings());

        }


        [HttpPost]
        public async Task<IActionResult> XacNhanCapNhat(Guid MaPK, ChiTietDV[] dichVus)
        {
            if (dichVus != null && dichVus.Count() > 0)
            {
                var model = await _service.GetPhieuKhamById(MaPK);
                ViewBag.BacSi = await _nhanvienyteRep.Get(model.MaBS.ToString());
                var result = new PhieuKhamViewModel { MaBS = model.MaBS, HoTen = model.MaBNNavigation.HoTen, SDT = model.MaBNNavigation.SDT, GioiTinh = model.MaBNNavigation.GioiTinh, NgaySinh = model.MaBNNavigation.NgaySinh, TrieuChung = model.TrieuChungSoBo, DiaChi = model.MaBNNavigation.DiaChi };
                result.dichVus = new List<DichVu>();

                foreach (var item in dichVus)
                {
                    result.dichVus.Add(await _dichvuRep.Get(item.MaDV));
                }
                return PartialView("_XacNhanDichVu", result);

            }
            else
                return Json(new { status = -2, title = "", text = "Vui lòng chọn it nhất một dịch vụ", obj = "" }, new JsonSerializerSettings());
        }


        [HttpPost]
        public async Task<IActionResult> UpdateCheckOut(Guid MaPK, ChiTietDV[] dichVus)
        {
            string MaNVHD = (await _userManager.GetUserAsync(User)).Id;
            var result = await _service.UpDateDichVu(MaNVHD, MaPK, dichVus.ToList());

            if (result != null)
            {



                return Json(new { status = 1, title = "", text = "Thêm thành công.", redirectUrL = Url.Action("ThemPhieuKham", "TiepNhan"), obj = "" }, new JsonSerializerSettings());
            }

            else
                return Json(new { status = -2, title = "", text = "Thêm không thành công", obj = "" }, new JsonSerializerSettings());
        }


        [HttpPost]
        public async Task<IActionResult> FinalCheckOut(PhieuKhamViewModel model)
        {
            model.MaNVHD = (await _userManager.GetUserAsync(User)).Id;
            var result = await _service.CreatePK(model);


            if (result != null)
            {
                var stt = new STTViewModel { STT = result.MaPKNavigation.STTPhieuKham.STT, HoTen = result.MaPKNavigation.MaBNNavigation.HoTen, UuTien = result.MaPKNavigation.STTPhieuKham.MaUuTien, MaPK = result.MaPK };
                await _hubContext.Clients.All.SendAsync("SentDocTor", model.MaBS);

                await _service.DeletePhieuDatLichById(model.MaPhieuDatLich);
                return Json(new { status = 1, title = "", text = "Thêm thành công.", redirectUrL = Url.Action("ThemPhieuKham", "TiepNhan"), obj = "" }, new JsonSerializerSettings());

            }

            else
                return Json(new { status = -2, title = "", text = "Thêm không thành công", obj = "" }, new JsonSerializerSettings());

        }



        public async Task<IActionResult> QuanLyDatLich(PhieuDatLichSearchModel model)
        {

            if (!model.Page.HasValue) model.Page = 1;
            var listPaged = await _service.SearchByCondition(model);

            ViewBag.Names = listPaged;
            ViewBag.Page = model.Page;
            ViewBag.Data = model;
            return View();
        }

        public async Task<IActionResult> CapNhatDichVu(PhieuKhamSearchModel model)
        {

            if (!model.Page.HasValue) model.Page = 1;
            var listPaged = await _service.GetListPhieuKham(model);

            ViewBag.Names = listPaged;
            ViewBag.Page = model.Page;
            ViewBag.Data = model;
            return View();
        }


        public async Task<IActionResult> ChiTietCapNhat(Guid MaPK)
        {
            var phieuKham = await _service.GetPhieuKhamById(MaPK);
            var listOld = await _service.GetListDVByPK(MaPK);


            var listNew = await _dichvuRep.GetDichVu(Guid.Empty);
            var listDV = listNew.Where(x => !listOld.Any(y => y.MaDV == x.MaDV));
            ViewBag.ListOld = listOld;
            ViewBag.DichVu = listDV;
            return View(phieuKham);
        }

        public async Task<IActionResult> ReLoadCapNhat(PhieuKhamSearchModel model)
        {

            var listmodel = await _service.GetListPhieuKham(model);

            if (!model.Page.HasValue) model.Page = 1;

            ViewBag.Names = listmodel;
            ViewBag.Page = model.Page;
            ViewBag.Data = model;

            return PartialView("_ListPhieuKham", listmodel);

        }
        public IActionResult ThemDichVuMoi()
        {
            return View();
        }

        public async Task<IActionResult> ChiTietDatLich(string id)
        {
            var chiTietDatLich = await _service.GetPhieuDatLichById(id);
            return PartialView("_ChiTietDatLich", chiTietDatLich);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var result = await _service.GetPhieuDatLichById(id);
            if (result == null)
            {
                return NotFound(); ;
            }
            else
            {

                return PartialView("_PartialViewEditLich", result);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PhieuDatLich model)
        {

            if (await _service.Edit(model) != null)
                return Json(new { status = 1, title = "", text = "Cập nhật thành công.", obj = "" }, new Newtonsoft.Json.JsonSerializerSettings());
            else
                return Json(new { status = -2, title = "", text = "Cập nhật không thành công.", obj = "" }, new Newtonsoft.Json.JsonSerializerSettings());


        }

        [HttpGet]
        public async Task<IActionResult> ReloadPage(PhieuDatLichSearchModel model)
        {
            var listmodel = await _service.SearchByCondition(model);

            if (!model.Page.HasValue) model.Page = 1;

            ViewBag.Names = listmodel;
            ViewBag.Page = model.Page;
            ViewBag.Data = model;

            return PartialView("_ListDatLich", listmodel);
        }

        /// <summary>
        /// QR CODE
        /// </summary>
        /// <returns></returns>
        public IActionResult QRCodeSample()
        {
            return View("QRCodeSample");
        }
        public IActionResult ScanPhieuDatLich()
        {
            return PartialView("_ScanPhieuDatLich");
        }

        public async Task<IActionResult> LoadThongTinByMaDatLich(string id)
        {
            var chiTietDatLich = await _service.GetPhieuDatLichById(id);
            return Ok(chiTietDatLich);
        }


    }
}