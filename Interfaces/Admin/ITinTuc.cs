using HospitalManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using X.PagedList;

namespace HospitalManagement.Interfaces
{
    public interface ITinTuc
    {
        Task<List<TinTuc>> GetTinTuc(Guid MaTL);
        Task<List<TinTuc>> GetTinMin(Guid MaTL);
        Task<TinTuc> Add(TinTuc model);
        Task<TinTuc> Get(Guid id);
        Task<TinTuc> Edit(TinTuc model);
        Task<bool> Delete(Guid id);
        Task<IPagedList<TinTuc>> SearchByCondition(TinTucSearchModel model);
        IEnumerable<NguoiDung> NguoiDungNav();
    }
}
