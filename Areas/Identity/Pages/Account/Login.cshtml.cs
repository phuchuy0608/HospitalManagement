using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using HospitalManagement.Models;

namespace HospitalManagement.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserManager<NhanVienYte> _userManager;
        private readonly SignInManager<NhanVienYte> _signInManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(SignInManager<NhanVienYte> signInManager,
            ILogger<LoginModel> logger,
            UserManager<NhanVienYte> userManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage ="Vui lòng nhập Email.")]
            [EmailAddress]
            public string Email { get; set; }

            [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        [BindProperty]
        public string Error { get; set; } = null;

        

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                Error = "Đăng nhập thất bại.";
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {

            returnUrl ??= Url.Content("~/TiepNhan/quanlydatlich");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var user = await _userManager.FindByNameAsync(Input.Email);
                if(user!=null)
                {
                    if (user.TrangThai == true)
                    {
                        var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                        if (result.Succeeded)
                        {
                            _logger.LogInformation("User logged in.");
                            if (user.ChucVu == 1)
                            {
                                return LocalRedirect("~/TiepNhan/ThemPhieuKham");
                            }
                            else if (user.ChucVu == 3)
                            {
                                return LocalRedirect("~/DuocSi/ToaThuoc");
                            }
                            else
                            {
                                return LocalRedirect("~/Bacsi/");
                            }
                        }
                        if (result.RequiresTwoFactor)
                        {
                            return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                        }
                        if (result.IsLockedOut)
                        {
                            _logger.LogWarning("User account locked out.");
                            return RedirectToPage("./Lockout");
                        }
                        else
                        {
                            //ModelState.AddModelError(string.Empty, "Đăng nhập thất bại.");
                            Error = "Đăng nhập thất bại.";
                            return Page();
                        }
                    }
                    else
                    {
                        return RedirectToAction("NoneUserNVYT", "Identity");
                    }
                }

                Error = "Đăng nhập thất bại.";
                return Page();
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
