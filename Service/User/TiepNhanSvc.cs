using HospitalManagement.Infrastructure;
using HospitalManagement.Interfaces;
using HospitalManagement.Models.SearchModel;
using HospitalManagement.Models;
using HospitalManagement_Entities.Models.ViewModel;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System;
using X.PagedList;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Linq;

namespace HospitalManagement.Service
{
    public class TiepNhanSvc:ITiepNhan
    {
        private readonly DataContext _context;
        private IHubContext<RealtimeHub> _hubContext;
        public TiepNhanSvc(DataContext context, IHubContext<RealtimeHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }
        public async Task<PhieuDatLich> Edit(PhieuDatLich model)
        {
            try
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    var existingLich = _context.PhieuDatLich.Find(model.MaPhieu);
                    existingLich.NgayKham = model.NgayKham;
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return model;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }
        public async Task<HoaDon> CreatePK(PhieuKhamViewModel model)
        {
            try
            {
                var maHD = "HD_" + DateTime.Now.ToString("ddMMyyyyhhmmss");
                var MaPK = Guid.NewGuid();
                var list = new List<string>();
                foreach (var item in model.dichVus)
                {
                    list.Add(item.MaDV.ToString());
                }
                var listContent = string.Join(",", list);
                AddPK(model, maHD, MaPK, listContent);
                var hd = await _context.HoaDon.Include(x => x.MaPKNavigation.MaBNNavigation).Include(x => x.MaNVNavigation).Include(x => x.MaPKNavigation).Include(x => x.MaPKNavigation.STTPhieuKham).Include(x => x.ChiTietDV).ThenInclude(x => x.MaDVNavigation).FirstOrDefaultAsync(x => x.MaHoaDon == maHD);
                return hd;
            }
            catch
            {
                return null;
            }
        }
        public void AddPK(PhieuKhamViewModel model, string maHD, Guid MaPK, string listContent)
        {
            try
            {
                List<SqlParameter> parms = new List<SqlParameter>
                {

                    new SqlParameter { ParameterName = "@HoTen", Value= model.HoTen },
                    new SqlParameter { ParameterName = "@SDT", Value= model.SDT },
                    new SqlParameter { ParameterName = "@NgaySinh", Value= model.NgaySinh },
                    new SqlParameter { ParameterName = "@GioiTinh", Value= model.GioiTinh },
                    new SqlParameter { ParameterName = "@DiaChi", Value= model.DiaChi },
                    new SqlParameter { ParameterName = "@Email", Value= string.IsNullOrWhiteSpace(model.Email)?DBNull.Value:model.Email },
                    new SqlParameter { ParameterName = "@MaBS", Value= model.MaBS },
                    new SqlParameter { ParameterName = "@TrieuChung", Value=string.IsNullOrEmpty(model.TrieuChung)?DBNull.Value:model.TrieuChung },
                    new SqlParameter { ParameterName = "@UuTien", Value= model.UuTien?"A":"B" },
                    new SqlParameter { ParameterName = "@MaNV", Value= model.MaNVHD },
                    new SqlParameter { ParameterName = "@MaPK", Value= MaPK },
                    new SqlParameter { ParameterName = "@MaHD", Value= maHD },
                    new SqlParameter { ParameterName = "@listDetail", Value= listContent }
                };
                var result = (_context.PhieuKham.FromSqlRaw("EXEC dbo.AddPhieuKhamBN @HoTen,@SDT,@NgaySinh,@GioiTinh,@DiaChi,@Email,@MaBS,@TrieuChung,@UuTien,@MaNV,@MaHD,@MaPK,@listDetail", parms.ToArray()).ToList()).FirstOrDefault();
            }
            catch { }
        }
        public async Task<BenhNhan> GetBN(string SDT)
        {
            return await _context.BenhNhan.FirstOrDefaultAsync(x => x.SDT == SDT);
        }
        public async Task<IPagedList<PhieuDatLich>> SearchByCondition(PhieuDatLichSearchModel model)
        {
            var listUnpaged = (_context.PhieuDatLich.Where(x =>
            (string.IsNullOrWhiteSpace(model.KeywordSearch) ||
            EF.Functions.Collate(x.TenBN, "SQL_Latin1_General_Cp1_CI_AI").Contains(EF.Functions.Collate(model.KeywordSearch, "SQL_Latin1_General_Cp1_CI_AI")) ||
            EF.Functions.Collate(x.SDT, "SQL_Latin1_General_Cp1_CI_AI").Contains(EF.Functions.Collate(model.KeywordSearch, "SQL_Latin1_General_Cp1_CI_AI")))).OrderByDescending(x => x.NgayKham));
            var listPaged = await listUnpaged.ToPagedListAsync(model.Page ?? 1, 10);
            if (listPaged.PageNumber != 1 && model.Page.HasValue && model.Page > listPaged.PageCount)
                return null;
            return listPaged;
        }
        public async Task<IPagedList<PhieuKham>> GetListPhieuKham(PhieuKhamSearchModel model)
        {
            var listUnpaged = (_context.PhieuKham.Include(x => x.MaBNNavigation).Where(x =>
            (string.IsNullOrWhiteSpace(model.KeywordSearch) ||
            EF.Functions.Collate(x.MaBNNavigation.HoTen, "SQL_Latin1_General_Cp1_CI_AI").Contains(EF.Functions.Collate(model.KeywordSearch, "SQL_Latin1_General_Cp1_CI_AI")) ||
            EF.Functions.Collate(x.MaBNNavigation.SDT, "SQL_Latin1_General_Cp1_CI_AI").Contains(EF.Functions.Collate(model.KeywordSearch, "SQL_Latin1_General_Cp1_CI_AI")))
            && x.TrangThai == 0).OrderByDescending(x => x.NgayKham));
            var listPaged = await listUnpaged.ToPagedListAsync(model.Page ?? 1, 10);
            if (listPaged.PageNumber != 1 && model.Page.HasValue && model.Page > listPaged.PageCount)
                return null;
            return listPaged;
        }
        public async Task<PhieuKham> GetPhieuKhamById(Guid id)
        {
            return await _context.PhieuKham.Include(x => x.STTPhieuKham).Include(x => x.MaBSNavigation).Include(x => x.MaBNNavigation).FirstOrDefaultAsync(x => x.MaPK == id);
        }
        public async Task<List<ChiTietDV>> GetListDVByPK(Guid MaPK)
        {
            return await (from pk in _context.PhieuKham
                          join hd in _context.HoaDon
                          on pk.MaPK equals (hd.MaPK)
                          join ctdv in _context.ChiTietDV
                          on hd.MaHoaDon equals (ctdv.MaHD)
                          where pk.MaPK == MaPK
                          select ctdv).ToListAsync();
        }
        public async Task<HoaDon> UpDateDichVu(string MaNV, Guid MaPK, List<ChiTietDV> chiTietDVs)
        {
            var maHD = "HD_" + DateTime.Now.ToString("ddMMyyyyhhmmss");
            var list = new List<string>();
            foreach (var item in chiTietDVs)
            {
                list.Add(item.MaDV.ToString());
            }
            var listContent = string.Join(",", list);
            try
            {
                List<SqlParameter> parms = new List<SqlParameter>
                {
                    new SqlParameter { ParameterName = "@MaNV", Value= MaNV },
                    new SqlParameter { ParameterName = "@MaPK", Value= MaPK,SqlDbType = SqlDbType.UniqueIdentifier },
                    new SqlParameter { ParameterName = "@MaHD", Value= maHD },
                    new SqlParameter { ParameterName = "@listDetail", Value= listContent }
                };
                var result = (_context.HoaDon.FromSqlRaw("EXEC UPDATEDV @MaNV,@MaPK,@MaHD,@listDetail", parms.ToArray()).ToList());
                await _context.SaveChangesAsync();
                if (result.Count > 0)
                    return result.FirstOrDefault();
                return null;
            }
            catch
            {
                return null;
            }
        }
        public async Task<PhieuDatLich> GetPhieuDatLichById(string id)
        {
            return await _context.PhieuDatLich.FindAsync(id);
        }
        public async Task<bool> DeletePhieuDatLichById(string id)
        {
            try
            {
                var find = await _context.PhieuDatLich.FindAsync(id);
                _context.PhieuDatLich.Remove(find);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }
    }
}
