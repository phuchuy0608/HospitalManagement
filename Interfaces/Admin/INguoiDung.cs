using HospitalManagement.Models;
using HospitalManagement_Entities.Models.ViewModel;
using System.Threading.Tasks;
using System;
using X.PagedList;


namespace HospitalManagement.Interfaces
{
    public interface INguoiDung
    {
        Task<Response<NguoiDung>> Add(NguoiDung model);
        Task<NguoiDung> Get(Guid id);
        Task<Response<NguoiDung>> Edit(NguoiDung model);
        Task<bool> Delete(Guid Id);
        Task<IPagedList<NguoiDung>> SearchByCondition(NguoiDungSearchModel model);
        public NguoiDung Login(AdminLoginViewModel viewLogin);
    }
}
