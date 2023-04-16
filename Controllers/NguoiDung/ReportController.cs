using HospitalManagement.Interfaces;
using HospitalManagement_Entities.Models.ViewModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using HospitalManagement.Controllers.Admin;
using System.Linq;

namespace HospitalManagement.Controllers.User
{
    public class ReportController : BaseController
    {
        private IWebHostEnvironment Environment;
        private readonly IReport _service;

        public ReportController(IWebHostEnvironment _environment, IReport service)
        {
            Environment = _environment;
            _service = service;
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ReportBenhNhan()
        {
            return View();
        }

        public IActionResult ReportBenh()
        {
            return View();
        }


        /// <summary>
        /// Xem và tải hóa đơn dịch vụ
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> ViewHoaDon()
        {

            return View(await _service.GetAllHoaDon());

        }


        [HttpGet]
        public async Task<IActionResult> Detail(string MaHD)
        {
            if (await _service.Get(MaHD) == null)
            {
                return NotFound(); ;
            }
            else
            {


                return PartialView("_partialDetail", await _service.Get(MaHD));
            }
        }

        /// <summary>
        /// Xem và tải hóa đơn thuốc
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ViewHoaDonThuoc()
        {
            return View(await _service.GetAllHoaDonThuoc());
        }

        [HttpGet]
        public async Task<IActionResult> DetailThuoc(string MaHD)
        {
            if (await _service.GetTTHDThuoc(MaHD) == null)
            {
                return NotFound();
            }
            else
            {
                return PartialView("_partialDetailThuoc", await _service.GetTTHDThuoc(MaHD));
            }
        }


        public IActionResult ThongKeDichVu(DateTime? ngayBatDau, DateTime? ngayKetThuc)
        {
            return View();
        }

        public async Task<IActionResult> ThongKeDichVuAction(DateTime? ngayBatDau, DateTime? ngayKetThuc)
        {
            if (ngayBatDau == null && ngayKetThuc == null)
            {
                ngayBatDau = DateTime.Now.AddMonths(-4);
                ngayKetThuc = DateTime.Now;
            }
            var listmodel = await _service.ThongKeDichVu((DateTime)ngayBatDau, (DateTime)ngayKetThuc);
            if (listmodel.errorCode == 0)
            {
                List<DataPoint> dataPoints = new List<DataPoint>();
                foreach (var item in (await _service.ThongKeDichVu((DateTime)ngayBatDau, (DateTime)ngayKetThuc)).Obj)
                {
                    dataPoints.Add(new DataPoint("Tháng " + item.Thang, (decimal)item.TongTien));
                }
                return Ok(new { dataPoints = dataPoints, dataTable = listmodel });

            }
            else
            {
                return BadRequest();
            }
        }

        public async Task<IActionResult> ThongKeHDThuoc(DateTime? ngayBatDau, DateTime? ngayKetThuc)
        {
            if (ngayBatDau == null && ngayKetThuc == null)
            {
                ngayBatDau = DateTime.Now.AddMonths(-4);
                ngayKetThuc = DateTime.Now;
            }
            var listmodel = await _service.ThongKeHDThuoc((DateTime)ngayBatDau, (DateTime)ngayKetThuc);
            if (listmodel.errorCode == 0)
            {
                List<DataPoint> dataPoints = new List<DataPoint>();
                foreach (var item in (await _service.ThongKeHDThuoc((DateTime)ngayBatDau, (DateTime)ngayKetThuc)).Obj)
                {
                    dataPoints.Add(new DataPoint("Tháng " + item.Thang, (decimal)item.TongTien));
                }
                return Ok(new { dataPoints = dataPoints, dataTable = listmodel });

            }
            else
            {
                return BadRequest();
            }
        }


        public async Task<IActionResult> ThongKeTongDoanhThu(DateTime? ngayBatDau, DateTime? ngayKetThuc)
        {
            if (ngayBatDau == null && ngayKetThuc == null)
            {
                ngayBatDau = DateTime.Now.AddMonths(-4);
                ngayKetThuc = DateTime.Now;
            }
            var listmodel = await _service.ThongKeTongDoanhThu((DateTime)ngayBatDau, (DateTime)ngayKetThuc);
            if (listmodel.errorCode == 0)
            {
                List<DataPoint> dataPoints = new List<DataPoint>();
                foreach (var item in (await _service.ThongKeTongDoanhThu((DateTime)ngayBatDau, (DateTime)ngayKetThuc)).Obj)
                {
                    dataPoints.Add(new DataPoint("Tháng " + item.Thang, (decimal)item.TongTien));
                }
                return Ok(new { dataPoints = dataPoints, dataTable = listmodel });


            }
            else
            {
                return BadRequest();
            }
        }

        public async Task<IActionResult> ThongKeBenh(DateTime? ngayBatDau, DateTime? ngayKetThuc)
        {
            if (ngayBatDau == null && ngayKetThuc == null)
            {
                ngayBatDau = DateTime.Now.AddMonths(-4);
                ngayKetThuc = DateTime.Now;
            }
            var listmodel = await _service.ThongKeBenh((DateTime)ngayBatDau, (DateTime)ngayKetThuc);
            if (listmodel.errorCode == 0)
            {
                List<DataPoint> dataPoints = new List<DataPoint>();
                foreach (var item in listmodel.Obj.OrderBy(x => x.soLuong))
                {
                    dataPoints.Add(new DataPoint(item.tenBenh, item.soLuong));
                }
                return Ok(new { dataPoints = dataPoints, dataTable = listmodel });


            }
            else
            {
                return BadRequest();
            }
        }

        public async Task<IActionResult> ThongKeSoLuongThuoc(DateTime? ngayBatDau, DateTime? ngayKetThuc)
        {
            if (ngayBatDau == null && ngayKetThuc == null)
            {
                ngayBatDau = DateTime.Now.AddMonths(-4);
                ngayKetThuc = DateTime.Now;
            }
            var listmodel = await _service.ThongKeSoLuongThuoc((DateTime)ngayBatDau, (DateTime)ngayKetThuc);
            if (listmodel.errorCode == 0)
            {
                List<DataPoint> dataPoints = new List<DataPoint>();
                foreach (var item in (await _service.ThongKeSoLuongThuoc((DateTime)ngayBatDau, (DateTime)ngayKetThuc)).Obj)
                {
                    dataPoints.Add(new DataPoint(item.tenThuoc, item.soLuong));
                }
                return Ok(new { dataPoints = dataPoints, dataTable = listmodel });
            }
            else
            {
                return BadRequest();
            }
        }

        public async Task<IActionResult> ThongKeLuotKham(DateTime? ngayBatDau, DateTime? ngayKetThuc)
        {
            if (ngayBatDau == null && ngayKetThuc == null)
            {
                ngayBatDau = DateTime.Now.AddMonths(-4);
                ngayKetThuc = DateTime.Now;
            }

            var listmodel = await _service.ThongKeLuotKham((DateTime)ngayBatDau, (DateTime)ngayKetThuc);
            if (listmodel.errorCode == 0)
            {
                List<DataPoint> dataPoints = new List<DataPoint>();
                foreach (var item in (await _service.ThongKeLuotKham((DateTime)ngayBatDau, (DateTime)ngayKetThuc)).Obj)
                {
                    dataPoints.Add(new DataPoint("Tháng " + item.thang.ToString(), item.luotKham));
                }
                return Ok(new { dataPoints = dataPoints, dataTable = listmodel });
            }
            else
            {
                return BadRequest();
            }
        }
        [HttpGet("LoadPagenation")]
        public IActionResult LoadPagenation(int? currentPage, int PageTotal, DateTime? NgayBD, DateTime? NgayKT, byte Type)
        {
            if (NgayBD == null && NgayKT == null)
            {
                NgayBD = DateTime.Now.AddMonths(-4);
                NgayKT = DateTime.Now;
            }

            ViewBag.currentPage = currentPage ?? 1;

            ViewBag.countPages = PageTotal;
            ViewBag.NgayBD = NgayBD;
            ViewBag.NgayKT = NgayKT;
            ViewBag.Type = Type;

            return PartialView("_Paging");
        }
        [HttpGet("PageList")]

        public IActionResult PageList(int? Page, string KeyWord, DateTime? NgayBatDau, DateTime? NgayKT, byte Type)
        {

            if (NgayBatDau == null && NgayKT == null)
            {
                NgayBatDau = DateTime.Now.AddMonths(-4);
                NgayKT = DateTime.Now;
            }

            // Trang hiện tại
            ViewBag.currentPage = Page ?? 1;

            var model = new HoaDonSearchModel { Page = Page, NgayBatDau = NgayBatDau, NgayKT = NgayKT, KeyWord = KeyWord, Type = Type };

            var listPaged = _service.SearchHDByCondition(model);

            return Ok(listPaged);

        }
    }
}
