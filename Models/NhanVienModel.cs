using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HospitalManagement.Models
{
    public class NhanVienModel
    {


        [Required(ErrorMessage = "Vui lòng nhập Email")]
        [EmailAddress]

        public string Email { get; set; }


        [Required(ErrorMessage = "Bạn cần nhập mật khẩu")]
        [DataType(DataType.Password)]
        public string MatKhau { get; set; }

        [Required(ErrorMessage = "Bạn cần nhập lại mật khẩu")]
        [DataType(DataType.Password)]
        [Compare("MatKhau", ErrorMessage = "Mật khẩu không khớp")]

        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Bạn cần nhập họ tên")]
        public string HoTen { get; set; }

        [RegularExpression(@"^\(?([0-9]{3})[-. ]?([0-9]{4})[-. ]?([0-9]{3})$", ErrorMessage = "Số điện thoại không đúng")]
        [Required(ErrorMessage = "Bạn cần nhập số điện thoại")]
        public string SDTNV { get; set; }

        [Required(ErrorMessage = "Bạn cần chọn chức vụ")]
        public byte ChucVu { get; set; }
        public bool TrangThai { get; set; }
        public string Hinh { get; set; }
        public Guid? ChuyenKhoa { get; set; }
    }
}
