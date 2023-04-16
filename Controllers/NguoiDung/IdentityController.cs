using HospitalManagement.Helpers;
using HospitalManagement.Models;
using HospitalManagement_Entities.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace HospitalManagement.Controllers
{
    public class IdentityController : Controller
    {
        private readonly UserManager<NhanVienYte> _userManager;
        private readonly SignInManager<NhanVienYte> _signInManager;
        private readonly ILogger<IdentityController> _logger;
        private readonly IWebHostEnvironment _env;

        public IdentityController(SignInManager<NhanVienYte> signInManager, ILogger<IdentityController> logger, UserManager<NhanVienYte> userManager, IWebHostEnvironment env)
        {
            _signInManager = signInManager;
            _logger = logger;
            _userManager = userManager;
            _env = env;
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Redirect("~/Identity/Account/Login");

        }

        public async Task<IActionResult> initialLogin()
        {
            var user = await _userManager.GetUserAsync(User);
            if(user!=null)
            {
                if (user.ChucVu == 1)
                {
                    return Redirect("~/TiepNhan/ThemPhieuKham");
                }
                else if (user.ChucVu == 3)
                {
                    return Redirect("~/DuocSi/ToaThuoc");
                }
                else
                {
                    return Redirect("~/Bacsi/");
                }

            }    
            else
            {
                return Redirect("~/Identity/Account/Login");
            }
           
        }    


        public IActionResult NoneUserNVYT()
        {
            return View();
        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ChangeInfo()
        {
            var user = await _userManager.GetUserAsync(User);
            var model = new UpdateUser { UpdateEmail = user.Email, UpdateHoTen = user.HoTen, UpdateHinhAnh = user.Hinh, Id = user.Id, UpdateSDT = user.PhoneNumber };
            return PartialView("_Edit_User",model);
        }




        [HttpPost]
        public async Task<IActionResult> ChangeInfo(UpdateUser model, [FromForm] IFormFile file)
        {
            try
            {
                string filePath = "";


                var user = await _userManager.GetUserAsync(User);
                user.HoTen = model.UpdateHoTen;
                user.PhoneNumber = model.UpdateSDT;
                
                if (file != null)
                {
                    var fileName = Path.GetFileName(DateTime.Now.ToString("ddMMyyyyss") + file.FileName);
                    user.Hinh = fileName;
                    filePath = Path.Combine(_env.WebRootPath, "images/NguoiDung", fileName);
                }

                if (file != null)
                {
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                }

                await _userManager.UpdateAsync(user);
                await _signInManager.RefreshSignInAsync(user);

                return Json(new { status = 1, title = "", text = "Cập nhật thành công." }, new Newtonsoft.Json.JsonSerializerSettings());
            }

            catch (Exception)
            {

                return Json(new { status = -2, title = "", text = "Cập nhật không thành công.", obj = "" }, new Newtonsoft.Json.JsonSerializerSettings());
            }
        }


        [HttpGet]
        public IActionResult ChangePassword()
        {
            return PartialView("_ChangePassword");
        }


        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.GetUserAsync(User);

                    var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }

                        return PartialView("_ChangePassword", model);
                    }

                    await _signInManager.SignOutAsync();
                    return Json(new { status = 1, title = "", text = "Cập nhật thành công.", redirectUrL = Url.Action("ThemPhieuKham", "TiepNhan"), obj = "" }, new JsonSerializerSettings());

                }
                catch (Exception)
                {
                    return Json(new { status = -2, title = "", text = "Cập nhật không thành công.", obj = "" }, new Newtonsoft.Json.JsonSerializerSettings());
                }

            }
            return PartialView("_ChangePassword", model);
        }


        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel forgotPasswordModel)
        {
            var request = HttpContext.Request;

            if (ModelState.IsValid)
            {

                var user = await _userManager.FindByEmailAsync(forgotPasswordModel.Email);
                if (user == null)
                {
                    ViewBag.Error = "Email chưa được đăng ký !";
                    return View();
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callback = Url.Action(nameof(ResetPassword), "Identity", new { token, email = user.Email }, Request.Scheme);
                Helper.SendMail(forgotPasswordModel.Email, "[Mã Phúc Huy Hospital] - QUÊN MẬT KHẨU", $"Nhấn vào đây để đặt lại mật khẩu: <br><a href='{callback}'>Khôi phục mật khẩu</a>");
                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }
            return View();
        }

        public async Task<IActionResult> ChangeTheme(string ThemeUrl)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                user.Theme = ThemeUrl;
                await _userManager.UpdateAsync(user);
                await _signInManager.RefreshSignInAsync(user);
                return Json(new { status = 1, title = "", text = "Đổi chủ đề thành công." });
            }
            catch
            {
                return Json(new { status = 0, title = "", text = "Có lỗi xảy ra vui lòng kiểm tra lại." });
            }

          

        }


        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }


        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            var model = new ResetPasswordViewModel { Token = token, Email = email };
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPasswordModel)
        {
            if (!ModelState.IsValid)
                return View(resetPasswordModel);
            var user = await _userManager.FindByEmailAsync(resetPasswordModel.Email);
            if (user == null)
                RedirectToAction(nameof(ResetPasswordConfirmation));


            var resetPassResult = await _userManager.ResetPasswordAsync(user, resetPasswordModel.Token, resetPasswordModel.Password);

            if (!resetPassResult.Succeeded)
            {
                foreach (var error in resetPassResult.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }
                return View();
            }
            if (user.ChucVu == 1)
            {
                await _userManager.AddToRoleAsync(user, "nhanvien");
            }
            else if (user.ChucVu == 2)
            {
                await _userManager.AddToRoleAsync(user, "bacsi");
            }
            else
            {
                await _userManager.AddToRoleAsync(user, "duocsi");
            }
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(ResetPasswordConfirmation));
        }


        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
    }
}
