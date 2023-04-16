
using HospitalManagement.Models;
using HospitalManagement.Models.SearchModel;
using HospitalManagement_Entities.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using X.PagedList;

namespace HospitalManagement.Interfaces
{
    public interface ITiepNhan
    {
        Task<IPagedList<PhieuDatLich>> SearchByCondition(PhieuDatLichSearchModel model);
        Task<PhieuDatLich> GetPhieuDatLichById(string id);
        Task<PhieuDatLich> Edit(PhieuDatLich model);
        Task<HoaDon> CreatePK(PhieuKhamViewModel model);
        Task<BenhNhan> GetBN(string SDT);
        Task<IPagedList<PhieuKham>> GetListPhieuKham(PhieuKhamSearchModel model);
        Task<PhieuKham> GetPhieuKhamById(Guid id);
        Task<HoaDon> UpDateDichVu(string MaNV, Guid MaPK, List<ChiTietDV> chiTietDVs);
        Task<List<ChiTietDV>> GetListDVByPK(Guid MaPK);
        Task<bool> DeletePhieuDatLichById(string id);
    }
}
