using Hangfire;
using HospitalManagement.Infrastructure;
using HospitalManagement.Interfaces;
using HospitalManagement.Models.SearchModel;
using HospitalManagement.Models;
using HospitalManagement_Entities.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace HospitalManagement.Controllers
{
    [Authorize(Roles = "bacsi")]
    public class BacSiController : Controller
    {
        private readonly IKhamBenh _khambenhRep;

        private readonly UserManager<NhanVienYte> _userManager;
        private readonly IHubContext<SignalServer> _hubContext;
        private readonly ITienIch _tienichRep;

        public BacSiController(
            IKhamBenh khambenhRep,

            UserManager<NhanVienYte> userManager,
            IHubContext<SignalServer> hubContext,
            ITienIch tienichRep
            )

        {
            _khambenhRep = khambenhRep;

            _userManager = userManager;
            _hubContext = hubContext;
            _tienichRep = tienichRep;

        }



        [HttpGet]
        public async Task<IActionResult> ReloadPageSTT(PhieuKhamSearchModel model)
        {

            model.MaBS = (await _userManager.GetUserAsync(User)).Id;
            model.TrangThai = 0;
            var listmodel = await _khambenhRep.SearchByCondition(model);

            if (!model.Page.HasValue) model.Page = 1;

            ViewBag.Page = model.Page;
            ViewBag.Names = listmodel;
            ViewBag.Data = model;

            return PartialView("_listSTTPhieuKham", listmodel);
        }

        [Route("/bacsi")]
        [Route("/bacsi/Phieukham")]
        public async Task<IActionResult> PhieuKham(PhieuKhamSearchModel model)
        {
            model.MaBS = (await _userManager.GetUserAsync(User)).Id;
            model.TrangThai = 0;
            if (!model.Page.HasValue) model.Page = 1;
            var listPaged = await _khambenhRep.SearchByCondition(model);

            ViewBag.Names = listPaged;
            ViewBag.Page = model.Page;
            ViewBag.Data = model;
            return View();

        }

        public async Task<IActionResult> KhamBenh(string MaPK)
        {
            var item = await _khambenhRep.GetPK(Guid.Parse(MaPK));
            item.NgayTaiKham = item.NgayKham.AddDays(7);
            ViewBag.DataThuoc = JsonConvert.SerializeObject(_tienichRep.GetAllThuoc().OrderBy(x => x.TenThuoc).ThenBy(x => x.TenThuoc.Length), Formatting.Indented,
             new JsonSerializerSettings
             {
                 ReferenceLoopHandling = ReferenceLoopHandling.Ignore
             });
            ViewBag.LichSuKham = await _khambenhRep.GetLichSu(item.MaBNNavigation.HoTen, item.MaBNNavigation.SDT);
            ViewBag.PhieuKham = JsonConvert.SerializeObject(item, Formatting.Indented,
               new JsonSerializerSettings
               {
                   ReferenceLoopHandling = ReferenceLoopHandling.Ignore
               });
            return View(item);

        }

        public IActionResult GetListThuoc()
        {

            return PartialView("_partialToaThuoc", _tienichRep.GetAllThuoc());

        }
        [HttpPost]
        public IActionResult GetToaThuoc(PhieuKham phieukham)
        {

            var ListCTBenh = new List<ChiTietBenhModel>();

            foreach (var chitiet in phieukham.ChiTietBenh)
            {
                ListCTBenh.Add(new ChiTietBenhModel { TenBenh = chitiet.MaBenhNavigation.TenBenh, TrieuChung = chitiet.KetQuaKham.Split(',').ToList() });
            }
            ViewBag.LisTTC = ListCTBenh;
            return PartialView("_XacNhanKetQua", phieukham);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeUuTien(Guid MaPk)
        {
            var stt = await _khambenhRep.ChangeUuTien(MaPk);
            if (stt != null)
                return Json(new { status = 1, title = "", text = "Đổi thứ tự thành công.", obj = "" }, new JsonSerializerSettings());
            else
                return Json(new { status = -1, title = "", text = "Thao tác không thành công.", obj = "" }, new JsonSerializerSettings());
        }


        public async Task<IActionResult> GetJsonPK(string MaPK)
        {
            var item = await _khambenhRep.GetPK(Guid.Parse(MaPK));

            return Ok(item);

        }


        [Produces("application/json")]
        [HttpGet("search")]
        [Route("api/Benh/search")]
        public IActionResult Search()
        {
            try
            {
                string term = HttpContext.Request.Query["term"].ToString();
                var benh = (_tienichRep.SearchBenh(term)).Select(x => x.TenBenh);
                return Ok(benh);
            }
            catch
            {
                return BadRequest();
            }
        }

        [Produces("application/json")]
        [HttpGet("searchtrieuchung")]
        [Route("api/Benh/searchtrieuchung")]
        public IActionResult SearchTrieuChung()
        {
            try
            {
                string term = HttpContext.Request.Query["term"].ToString();
                var trieuchung = (_tienichRep.GetTrieuChung(term)).Select(x => x.TenTrieuChung);
                return Ok(trieuchung);
            }
            catch
            {
                return BadRequest();
            }
        }



        [HttpPost]
        public async Task<IActionResult> ThemToa(PhieuKham model, List<ChiTietBenhModel> ListCT)
        {
            foreach (var item in model.ToaThuoc.ChiTietToaThuoc)
            {
                item.DonGiaThuoc = (_tienichRep.GetThuoc(item.MaThuoc)).DonGia;
                item.GhiChu = $"Ngày uống {item.LanTrongNgay} lần, mỗi lần {item.VienMoiLan} viên,uống {(item.TruocKhian ? "trước khi ăn" : "sau khi ăn")},Uống {(item.Sang ? "Sáng" : "")}{(item.Trua ? ", trưa" : "")}{(item.Chieu ? ", chiều" : "")}.";
            }
            model.ChanDoan = string.Join(",", ListCT.Select(x => x.TenBenh).ToArray());

            var result = await _khambenhRep.AddToaThuoc(model, ListCT);

            if (result.errorCode >= 0)
            {
                if (result.errorCode == 1)
                {
                    using (new BackgroundJobServer())
                    {
                        _tienichRep.refreshCacheBenh();
                        _tienichRep.refreshCacheTrieuChung();
                    }

                }



                await _hubContext.Clients.All.SendAsync("SendToaThuoc");
                return Json(new { status = 1, title = "", text = "Gửi thành công.", redirectUrL = Url.Action("PhieuKham", "BacSi"), obj = "" }, new JsonSerializerSettings());
            }

            else
                return Json(new { status = -2, title = "", text = "Gửi không thành công", obj = "" }, new JsonSerializerSettings());
        }

        [HttpPost]
        public async Task<IActionResult> GetAutoFill(List<string> TenBenh)
        {
            var item = await _tienichRep.GetToaThuocFill(TenBenh);

            return Ok(item);


        }


        [HttpPost]
        public IActionResult XacNhanKetQua(PhieuKham model, List<string> ListTrieuChung, List<ChiTietBenhModel> ListCT)
        {
            if (ListTrieuChung != null && ListTrieuChung.Count > 0)

            {
                foreach (var item in model.ToaThuoc.ChiTietToaThuoc)
                {
                    item.MaThuocNavigation = new Thuoc();
                    item.MaThuocNavigation = _tienichRep.GetThuoc(item.MaThuoc);
                }
                ViewBag.LisTTC = ListCT;


                return PartialView("_XacNhanKetQua", model);
            }

            else
                return Json(new { status = -2, title = "", text = "Vui lòng nhập ít nhất một triệu chứng", obj = "" }, new JsonSerializerSettings());

        }



        [HttpPost]
        public IActionResult ReLoadThuoc(PhieuKham model)
        {
            if (model.ToaThuoc == null)
            {
                return Json(new { status = -2, title = "", text = "Chưa có toa thuốc nào cho bệnh này", obj = "" }, new JsonSerializerSettings());
            }
            else
            {
                var listhuocExist = model.ToaThuoc.ChiTietToaThuoc;
                var listNew = _tienichRep.GetAllThuoc();
                var listThuoc = listNew.Where(x => !listhuocExist.Any(y => y.MaThuoc == x.MaThuoc));
                ViewBag.Thuoc = listThuoc;
                return PartialView("_partialToaThuocOld", model.ToaThuoc);
            }

        }

        [HttpGet]
        public async Task<IActionResult> LichSuKham(PhieuKhamSearchModel model)
        {
            model.MaBS = (await _userManager.GetUserAsync(User)).Id;
            model.TrangThai = 1;
            if (!model.Page.HasValue) model.Page = 1;
            var listPaged = await _khambenhRep.SearchByCondition(model);

            ViewBag.Names = listPaged;
            ViewBag.Data = model;
            return View();
        }

        public async Task<IActionResult> PagePhieuKham(PhieuKhamSearchModel model)
        {

            model.MaBS = (await _userManager.GetUserAsync(User)).Id;
            model.TrangThai = 1;
            var listmodel = await _khambenhRep.SearchByCondition(model);

            if (!model.Page.HasValue) model.Page = 1;




            ViewBag.Names = listmodel;
            ViewBag.Data = model;

            return PartialView("_LichSuKham", listmodel);

        }


        public IActionResult ChiTietThuoc(Guid id)
        {
            if (_tienichRep.GetThuoc(id) == null)
            {
                return NotFound(); ;
            }
            else
            {


                return PartialView("_ChiTietThuoc", _tienichRep.GetThuoc(id));
            }
        }
        [HttpPost]
        public IActionResult GetJsonThuoc(PhieuKham model)
        {
            if (model.ToaThuoc == null)
            {
                return Json(new { status = -2, title = "", text = "Chưa có toa thuốc nào cho bệnh này", obj = "" }, new JsonSerializerSettings());
            }
            else
            {
                var listhuocExist = model.ToaThuoc.ChiTietToaThuoc;
                var listNew = _tienichRep.GetAllThuoc();
                var listThuoc = listNew.Where(x => !listhuocExist.Any(y => y.MaThuoc == x.MaThuoc));

                return Json(JsonConvert.SerializeObject(listThuoc.OrderBy(x => x.TenThuoc).ThenBy(x => x.TenThuoc.Length), Formatting.Indented,
               new JsonSerializerSettings
               {
                   ReferenceLoopHandling = ReferenceLoopHandling.Ignore
               }));
            }

        }
        [HttpGet]
        public IActionResult GetInforThuoc(Guid Mathuoc)
        {
            var result = _tienichRep.GetAllThuoc().FirstOrDefault(x => x.MaThuoc == Mathuoc);
            if (result != null)
            {
                return Json(JsonConvert.SerializeObject(result, Formatting.Indented,
               new JsonSerializerSettings
               {
                   ReferenceLoopHandling = ReferenceLoopHandling.Ignore
               }));
            }
            else
            {
                return NotFound();
            }

        }

        public IActionResult ThemThuoc()
        {
            return PartialView("_ThemThuoc");
        }



    }
}
