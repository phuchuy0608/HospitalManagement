using HospitalManagement.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using X.PagedList;

namespace HospitalManagement.Interfaces
{
    public interface IChuyenKhoa
    {
        Task<Response<ChuyenKhoa>> Add(ChuyenKhoa model);
        Task<ChuyenKhoa> Get(Guid id);
        Task<Response<ChuyenKhoa>> Edit(ChuyenKhoa model);
        Task<bool> Delete(Guid Id);
        Task<IPagedList<ChuyenKhoa>> SearchByCondition(ChuyenKhoaSearchModel model);
        Task<IEnumerable<ChuyenKhoa>> GetAll();
        Task<IEnumerable<ChuyenKhoa>> GetChuyenKhoaHaveDoctor();
    }
}
