using HospitalManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace HospitalManagement.Interfaces
{
    public interface IThuoc
    {
        Task<Response<Thuoc>> Add(Thuoc model);
        Task<Thuoc> Get(Guid id);
        Task<Response<Thuoc>> Edit(Thuoc model);
        Task<bool> Delete(Guid Id);
        Task<IPagedList<Thuoc>> SearchByCondition(ThuocSearchModel model);
        List<Thuoc> GetAllThuoc();
    }
}
