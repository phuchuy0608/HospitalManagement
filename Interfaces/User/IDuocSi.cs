using HospitalManagement.Models;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using X.PagedList;

namespace HospitalManagement.Interfaces
{
    public interface IDuocSi
    {
        Task<IPagedList<ToaThuoc>> SearchToaThuoc(ToaThuocSearchModel model);
        Task<ToaThuoc> GetToaThuocByMaPhieu(Guid MaPhieu);

        Task<IEnumerable<ChiTietToaThuoc>> GetChiTiet(Guid MaPhieu);
        Task<STTTOATHUOC> ChangeSoUuTien(Guid maPK);

        Task<ToaThuoc> ThanhToanThuoc(Guid maPK, string maNV);
        Task<ToaThuoc> XacNhanThuocDangCho(Guid maPK);
    }
}
