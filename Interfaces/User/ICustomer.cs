using HospitalManagement.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HospitalManagement.Interfaces
{
    public interface ICustomer
    {
        Task<PhieuDatLich> DatLich(PhieuDatLich model);
        Task<PhieuDatLich> GetPhieuDat(string MaPhieu);

        Task<List<PhieuKham>> SearchByPhoneNumber(string SDT);
        Task<PhieuKham> GetLichSuKhamById(Guid MaPK);

        Task<List<PhieuDatLich>> SearchDatLichByPhonenumber(string SDT);
    }
}
