using HospitalManagement.Interfaces;
using HospitalManagement.Models;
using System;
using System.Linq;

namespace HospitalManagement.Service
{
    public class ValidateSvc: IValidate
    {
        private readonly DataContext _context;
        public ValidateSvc(DataContext context)
        {
            _context = context;
        }
        public bool CheckNgayKham(DateTime? ngay)
        {
            return ngay > DateTime.Now;
        }
        public bool ExistsChuyenKhoa(string ten)
        {
            return _context.ChuyenKhoa.Any(x => x.TenCK == ten);
        }
    }
}
