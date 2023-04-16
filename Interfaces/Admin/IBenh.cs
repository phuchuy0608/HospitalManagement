using System.Threading.Tasks;
using X.PagedList;
using HospitalManagement_Entities.Models.ViewModel;
using HospitalManagement.Models;
using System.Collections.Generic;
using System;

namespace HospitalManagement.Interfaces
{
    public interface IBenh
    {
        Task<Response<Benh>> Add(Benh model, List<CTrieuChungModel> TrieuChungs);
        Task<Benh> Get(Guid id);
        Task<Response<Benh>> Edit(Benh model, List<CTrieuChungModel> trieuchungs);
        Task<bool> Delete(Guid id);
        Task<IPagedList<Benh>> SearchByCondition(BenhSearchModel model);
        Task<IEnumerable<ChuyenKhoa>> ChuyenKhoaNav();
        List<Benh> GetAllBenh();
        List<TrieuChung> GetAllTrieuChung();
    }
}
