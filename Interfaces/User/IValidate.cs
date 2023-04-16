using System;

namespace HospitalManagement.Interfaces
{
    public interface IValidate
    {
        bool ExistsChuyenKhoa(string ten);
        bool CheckNgayKham(DateTime? ngay);
    }
}
