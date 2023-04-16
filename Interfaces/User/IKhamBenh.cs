using HospitalManagement.Models;
using HospitalManagement.Models.SearchModel;
using HospitalManagement_Entities.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using X.PagedList;

namespace HospitalManagement.Interfaces
{
    public interface IKhamBenh
    {
        Task<Response<PhieuKham>> AddToaThuoc(PhieuKham model, List<ChiTietBenhModel> ListCT);
        Task<PhieuKham> GetPK(Guid MaPK);

        Task<IEnumerable<PhieuKham>> GetLichSu(string Hoten, string SDT);
        Task<IPagedList<PhieuKham>> SearchByCondition(PhieuKhamSearchModel model);
        Task<STTPhieuKham> ChangeUuTien(Guid MaPK);
        Task<PhieuKham> GetLichSuKhamById(Guid MaPK);
    }
}
