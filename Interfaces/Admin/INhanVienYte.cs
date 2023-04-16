using HospitalManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace HospitalManagement.Interfaces
{
    public interface INhanVienYte
    {
        
        Task<NhanVienYte> Get(String id);
        Task<IPagedList<NhanVienYte>> SearchByCondition(NhanVienYteSearchModel model);
        Task<IEnumerable<NhanVienYte>> GetAllBS(Guid MaCK);
        Task<Response<NhanVienYte>> Edit(string id, bool TrangThai);
    }
}
