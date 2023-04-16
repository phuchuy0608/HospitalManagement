using HospitalManagement.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using X.PagedList;

namespace HospitalManagement.Interfaces
{
    public interface ITheLoai
    {
        Task<Response<TheLoai>> Add(TheLoai model);
        Task<TheLoai> Get(Guid id);
        Task<Response<TheLoai>>Edit(TheLoai model);
        Task<bool> Delete(Guid Id);
        Task<IPagedList<TheLoai>> SearchByCondition(TheLoaiSearchModel model);
        Task<IEnumerable<TheLoai>> GetAll();

    }
}
