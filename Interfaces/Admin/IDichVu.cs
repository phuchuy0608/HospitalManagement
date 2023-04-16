using HospitalManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace HospitalManagement.Interfaces
{
    public interface IDichVu
    {
        Task<Response<DichVu>> Add(DichVu model);
        Task<DichVu>Get(Guid id);
        Task<Response<DichVu>> Edit(DichVu model);
        Task<bool> Delete(Guid id);
        Task<IPagedList<DichVu>> SearchByCondition(DichVuSearchModel model);
        Task<IEnumerable<DichVu>> GetDichVu(Guid MaPK);
    }
}
