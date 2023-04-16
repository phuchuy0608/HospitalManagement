using HospitalManagement.Models;
using HospitalManagement_Entities.Models.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace HospitalManagement.Interfaces
{
    public interface IReport
    {
        Task<IEnumerable<HoaDon>> GetAllHoaDon();
        Task<HoaDon> Get(string MaHD);
        Task<IEnumerable<HoaDonThuoc>> GetAllHoaDonThuoc();
        Task<HoaDonThuoc> GetTTHDThuoc(string MaHD);
        Task<Response<List<ThongKeDichVuViewModel>>> ThongKeDichVu(DateTime ngayBatDau, DateTime ngayKetThuc);
        Task<Response<List<ThongKeDichVuViewModel>>> ThongKeHDThuoc(DateTime ngayBatDau, DateTime ngayKetThuc);
        Task<Response<List<ThongKeDichVuViewModel>>> ThongKeTongDoanhThu(DateTime ngayBatDau, DateTime ngayKetThuc);
        Task<Response<List<ThongKeBenhViewModel>>> ThongKeBenh(DateTime ngayBatDau, DateTime ngayKetThuc);
        Task<Response<List<ThongKeSoLuongThuoc>>> ThongKeSoLuongThuoc(DateTime ngayBatDau, DateTime ngayKetThuc);
        Task<Response<List<ThongKeLuotKhamViewModel>>> ThongKeLuotKham(DateTime ngayBatDau, DateTime ngayKetThuc);
        PageResponse<ResponseHoaDon> SearchHDByCondition(HoaDonSearchModel model);
    }
}
