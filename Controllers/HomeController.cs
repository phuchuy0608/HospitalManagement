using Hangfire;
using HospitalManagement.Helpers;
using HospitalManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System;
using HospitalManagement.Interfaces;
using HospitalManagement.Infrastructure;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using OtpSharp;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
using HospitalManagement_Entities.Models.ViewModel;
using System.Collections.Generic;
using System.Diagnostics;

namespace HospitalManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICustomer _service;
    
        private readonly ITinTuc _tintucService;
        private readonly IDuocSi _duocSiService;
     
        
        private OTPCLASS totp;
        private readonly IWebHostEnvironment _env;



        private IHubContext<SignalServer> _hubContext;

        public HomeController(ILogger<HomeController> logger,
                                ICustomer service,
                                
                                IHubContext<SignalServer> hubContext,
                                ITinTuc tintucService,
                                IDuocSi duocSiService,
                                
                               
                                OTPCLASS toptpRep,
                                IWebHostEnvironment env
            )
        {
            _logger = logger;
            _service = service;
           
            _hubContext = hubContext;
            _tintucService = tintucService;
            _duocSiService = duocSiService;
            
            
            totp = toptpRep;
            _env = env;

        }

        [HttpGet]
        public async Task<IActionResult> Index(Guid MaTL)
        {
            ViewBag.TL1 = await _tintucService.GetTinTuc(Guid.Empty);

            return View(await _tintucService.GetTinTuc(MaTL));
        }


        public IActionResult DatLich()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DatLich(PhieuDatLich model)
        {
            model.MaPhieu = "PK_" + (Helper.GetUniqueKey()).ToUpper();
            if (ModelState.IsValid)
            {
                if (model.NgayKham < DateTime.Now)
                {
                    ModelState.AddModelError("NgayKham", "Ngày khám phải sau ngày hiện tại");
                    return View(model);
                }
                if (((DateTime)model.NgayKham - DateTime.Now).Days > 60)
                {
                    ModelState.AddModelError("NgayKham", "Ngày đặt không quá 60 ngày");
                    return View(model);
                }

                var result = await _service.DatLich(model);
                if (result != null)
                {
                    if (model.Email != null)
                    {
                        var request = HttpContext.Request;
                        var _baseURL = $"{request.Scheme}://{request.Host}/Home/ResultDatLich?MaPhieu={model.MaPhieu}";

                        using (new BackgroundJobServer())
                        {
                            Helper.SendMail(model.Email, "[MPH - HOS] Xác nhận đặt lịch khám", message(model, _baseURL));
                        }


                    }
                    await _hubContext.Clients.All.SendAsync("ReceiveMessage", result.TenBN, result.NgaySinh?.ToString("dd/MM/yyyy"), result.SDT, result.NgayKham, result.MaPhieu);

                    return RedirectToAction("ResultDatLich", "Home", new { MaPhieu = model.MaPhieu });
                }
            }


            return View(model);

        }

        public async Task<IActionResult> ResultDatLich(string MaPhieu)
        {
            var model = await _service.GetPhieuDat(MaPhieu);
            if (model != null)
            {
                return View(model);
            }
            return RedirectToAction("DatLichError", "Home");
        }

        public IActionResult DatLichError()
        {
            return View();
        }

        public IActionResult LichSuDatLich()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private string message(PhieuDatLich model, string _baseURL)
        {

            var root = Path.Combine(_env.WebRootPath, "MailTheme");


            using (var reader = new System.IO.StreamReader(root + "/index.html"))
            {
                string readFile = reader.ReadToEnd();
                string StrContent = string.Empty;
                StrContent = readFile;
                //Assing the field values in the template
                StrContent = StrContent.Replace("{MaPhieu}", model.MaPhieu);
                StrContent = StrContent.Replace("{UrlResult}", _baseURL);


                return StrContent.ToString();
            }

        }

      


       


        public async Task<IActionResult> SearchByPhoneNumber(string SDT, string otp)
        {
            byte[] rfcKey = UTF8Encoding.ASCII.GetBytes(SDT);
            totp.Totp = new Totp(rfcKey, 120,
                                     OtpHashMode.Sha1, 6);
            if (totp.Totp.VerifyTotp(otp, out long timeStepMatched, new VerificationWindow(0, 0)))
            {
                var listPhieuKham = await _service.SearchByPhoneNumber(SDT);
                if (listPhieuKham.Count() > 0)
                {
                    return Json(JsonConvert.SerializeObject(listPhieuKham, Formatting.Indented,
                    new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    }));
                }
                else
                {
                    return Json(new { status = -2, title = "", text = "Không tìm thấy", obj = "" }, new Newtonsoft.Json.JsonSerializerSettings());
                }
            }
            else
            {
                return Json(new { status = -3, title = "", text = "Mã xác thực không đúng.", obj = "" }, new Newtonsoft.Json.JsonSerializerSettings());
            }

        }

      


        public async Task<IActionResult> SearchDatLichByPhoneNumber(string SDT, string otp)
        {
            byte[] rfcKey = UTF8Encoding.ASCII.GetBytes(SDT);
            totp.Totp = new Totp(rfcKey, 120,
                                     OtpHashMode.Sha1, 6);
            if (totp.Totp.VerifyTotp(otp, out long timeStepMatched, new VerificationWindow(0, 0)))
            {
                var listPhieuDatLich = await _service.SearchDatLichByPhonenumber(SDT);
                if (listPhieuDatLich.Count() > 0)
                {

                    return Json(JsonConvert.SerializeObject(listPhieuDatLich, Formatting.Indented,
                    new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    }));
                }
                else
                {

                    return Json(new { status = -2, title = "", text = "Không tìm thấy", obj = "" }, new Newtonsoft.Json.JsonSerializerSettings());
                }
            }
            else
            {
                return Json(new { status = -3, title = "", text = "Mã xác thực không đúng.", obj = "" }, new Newtonsoft.Json.JsonSerializerSettings());
            }

        }


   

        //Gen OTP
        public IActionResult Generate(string SDT)
        {
            if (!string.IsNullOrWhiteSpace(SDT))
            {
                byte[] rfcKey = UTF8Encoding.ASCII.GetBytes(SDT);

                // Generating TOTP
                totp.Totp = new Totp(rfcKey, 120,
                                        OtpHashMode.Sha1, 6);
                return Ok(totp.Totp.ComputeTotp());
            }
            else
            {
                return BadRequest();
            }
        }

    }


}

