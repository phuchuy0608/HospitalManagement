using HospitalManagement.Models;
using HospitalManagement_Entities.Models.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace HospitalManagement.Interfaces
{
    public interface ITienIch
    {
        IEnumerable<Benh> SearchBenh(string KeyWord);
        void refreshCacheBenh();
        void refreshCacheTrieuChung();
        void refreshCacheThuoc();
        Task<List<ChiTietToaThuoc>> GetToaThuocFill(List<string> TenBenh);
        IEnumerable<TrieuChung> GetTrieuChung(string TenTrieuChung);
        IEnumerable<Thuoc> GetAllThuoc();
        Thuoc GetThuoc(Guid MaThuoc);
        List<ListResponse> GetListChanDoan(List<string> ListTrieuChung);
        List<ResponseChanDoan> KetQuaChanDoan(List<string> ListTrieuChung);
    }
}
