using HospitalManagement.Interfaces;
using HospitalManagement.Models.SearchModel;
using HospitalManagement.Models;
using HospitalManagement_Entities.Models.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using X.PagedList;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Linq;

namespace HospitalManagement.Service
{
    public class KhamBenhsvc:IKhamBenh
    {
        private readonly DataContext _context;
        public KhamBenhsvc(DataContext context)
        {
            _context = context;
        }
        //public async Task<IEnumerable<STTViewModel>> GetList(string MaBS)
        //{
        //    return await (from x in _context.PhieuKham
        //                  join y in _context.STTPhieuKham

        //                  on x.MaPK equals y.MaPhieuKham
        //                  join bn in _context.BenhNhan
        //                  on x.MaBN equals bn.MaBN
        //                  where x.MaBS == MaBS && y.TrangThai == false&&x.TrangThai ==0
        //                  select new STTViewModel
        //                  {
        //                      STT = y.STT,
        //                      HoTen = bn.HoTen,
        //                      UuTien = y.MaUuTien,
        //                      MaPK = x.MaPK
        //                  }).OrderByDescending(x => x.UuTien).ThenBy(x => x.STT).ToListAsync();
        //}
        public async Task<PhieuKham> GetLichSuKhamById(Guid MaPK)
        {
            return await _context.PhieuKham
                .Include(x => x.MaBNNavigation)
                .Include(x => x.ToaThuoc)
                .ThenInclude(x => x.ChiTietToaThuoc)
                .Include(x => x.HoaDon).ThenInclude(x => x.ChiTietDV)
                .ThenInclude(x => x.MaDVNavigation)
                .Include(x => x.MaBSNavigation)
                .FirstOrDefaultAsync(x => x.MaPK == MaPK);
        }
        public async Task<STTPhieuKham> ChangeUuTien(Guid MaPK)
        {
            try
            {
                var stt = await _context.STTPhieuKham.FindAsync(MaPK);
                stt.MaUuTien = "C";
                _context.Update(stt);
                await _context.SaveChangesAsync();
                return stt;
            }
            catch
            {
                return null;
            }

        }
        public async Task<PhieuKham> GetPK(Guid MaPK)
        {
            var item = await _context.PhieuKham.Include(x => x.MaBNNavigation).ThenInclude(x => x.PhieuKham).Include(x => x.ToaThuoc).ThenInclude(x => x.ChiTietToaThuoc).ThenInclude(x => x.MaThuocNavigation).Include(x => x.ChiTietBenh).ThenInclude(x => x.MaBenhNavigation).FirstOrDefaultAsync(x => x.MaPK == MaPK);
            return item;
        }
        public async Task<IEnumerable<PhieuKham>> GetLichSu(string Hoten, string SDT)
        {
            return await _context.PhieuKham.Include(x => x.MaBSNavigation).Include(x => x.MaBNNavigation).Where(x => x.MaBNNavigation.HoTen == Hoten && x.MaBNNavigation.SDT == SDT && x.TrangThai >= 1 && x.TrangThai <= 2).ToListAsync();
        }
        public async Task<Response<PhieuKham>> AddToaThuoc(PhieuKham model, List<ChiTietBenhModel> ListCT)
        {
            try
            {
                var checkBenh = 0;
                var uuTien = (await _context.PhieuKham.Include(x => x.STTPhieuKham).FirstOrDefaultAsync(x => x.MaPK == model.MaPK)).STTPhieuKham.MaUuTien;
                using (var transaction = _context.Database.BeginTransaction())
                {
                    if (model.ChiTietSinhHieu.Count > 0)
                    {
                        foreach (var item in model.ChiTietSinhHieu)
                        {
                            item.MaSinhHieu = Guid.NewGuid();
                            _context.ChiTietSinhHieu.Add(item);
                        }
                    }
                    var phieuKham = await _context.PhieuKham.Include(x => x.MaBSNavigation).FirstOrDefaultAsync(x => x.MaPK == model.MaPK);
                    phieuKham.Mach = model.Mach;
                    phieuKham.NhietDo = model.NhietDo;
                    phieuKham.HuyetAp = model.HuyetAp;
                    phieuKham.NgayTaiKham = model.NgayTaiKham;
                    phieuKham.NgayTaiKham = model.NgayTaiKham;
                    phieuKham.ChanDoan = model.ChanDoan;
                    phieuKham.TrangThai = 1;
                    foreach (var chitiet in ListCT)
                    {
                        var benh = await _context.Benh.FirstOrDefaultAsync(x => x.TenBenh == chitiet.TenBenh);
                        if (benh == null)
                        {
                            checkBenh = 1;
                            benh = new Benh { MaBenh = Guid.NewGuid(), TenBenh = chitiet.TenBenh, MaCK = Guid.Parse(phieuKham.MaBSNavigation.ChuyenKhoa.ToString()) };
                            _context.Entry(benh).State = EntityState.Added;
                            await _context.SaveChangesAsync();
                        }

                        foreach (var item in chitiet.TrieuChung)
                        {
                            List<SqlParameter> parms = new List<SqlParameter>
                            {
                                new SqlParameter { ParameterName = "@Mabenh", Value= benh.MaBenh },
                                new SqlParameter { ParameterName = "@TenTrieuChung", Value= item },
                            };
                            var result = _context.Database.ExecuteSqlRaw("EXEC dbo.AddCTrieuChung @Mabenh,@TenTrieuChung", parms.ToArray());
                        }
                        var chiTietBenh = new ChiTietBenh { MaBenh = benh.MaBenh, MaPK = phieuKham.MaPK, KetQuaKham = string.Join(",", chitiet.TrieuChung) };
                        await _context.ChiTietBenh.AddAsync(chiTietBenh);
                    }






                    _context.Update(phieuKham);
                    await _context.ToaThuoc.AddAsync(model.ToaThuoc);
                    var sttoathuoc = new STTTOATHUOC { MaPK = model.MaPK, STT = _context.STTTOATHUOC.Count() > 0 ? (_context.STTTOATHUOC.Max(x => x.STT) + 1) : 1, UuTien = uuTien };
                    await _context.STTTOATHUOC.AddAsync(sttoathuoc);
                    var sttpk = await _context.STTPhieuKham.FindAsync(model.MaPK);

                    _context.Update(sttpk);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    if (checkBenh == 1) return new Response<PhieuKham> { errorCode = 1, Obj = model };
                    else return new Response<PhieuKham> { errorCode = 0, Obj = model };
                }
            }
            catch
            {
                return new Response<PhieuKham> { errorCode = -1, Obj = model };
            }
        }
        public async Task<ToaThuoc> GetToaThuoc(Guid MaPK)
        {
            return await _context.ToaThuoc.Include(x => x.ChiTietToaThuoc).ThenInclude(x => x.MaThuocNavigation).Include(x => x.MaPhieuKhamNavigation).Include(x => x.MaPhieuKhamNavigation.MaBNNavigation).ThenInclude(x => x.PhieuKham).FirstOrDefaultAsync(x => x.MaPhieuKham == MaPK);
        }

        public async Task<IPagedList<PhieuKham>> SearchByCondition(PhieuKhamSearchModel model)
        {
            IEnumerable<PhieuKham> listUnpaged = null;
            if (model.TrangThai == 0)
            {
                listUnpaged = (_context.PhieuKham.Include(x => x.MaBNNavigation).Include(x => x.STTPhieuKham).Where(x =>
                (string.IsNullOrWhiteSpace(model.KeywordSearch) ||
                EF.Functions.Collate(x.MaBNNavigation.HoTen, "SQL_Latin1_General_Cp1_CI_AI").Contains(EF.Functions.Collate(model.KeywordSearch, "SQL_Latin1_General_Cp1_CI_AI")) ||
                EF.Functions.Collate(x.MaBNNavigation.SDT, "SQL_Latin1_General_Cp1_CI_AI").Contains(EF.Functions.Collate(model.KeywordSearch, "SQL_Latin1_General_Cp1_CI_AI")))
                && x.MaBS == model.MaBS && x.TrangThai == 0).OrderBy(x => x.STTPhieuKham.MaUuTien).ThenBy(x => x.STTPhieuKham.STT));
            }
            else
            {
                listUnpaged = (_context.PhieuKham.Include(x => x.MaBNNavigation).Include(x => x.STTPhieuKham).Where(x =>
                (string.IsNullOrWhiteSpace(model.KeywordSearch) ||
                EF.Functions.Collate(x.MaBNNavigation.HoTen, "SQL_Latin1_General_Cp1_CI_AI").Contains(EF.Functions.Collate(model.KeywordSearch, "SQL_Latin1_General_Cp1_CI_AI")) ||
                EF.Functions.Collate(x.MaBNNavigation.SDT, "SQL_Latin1_General_Cp1_CI_AI").Contains(EF.Functions.Collate(model.KeywordSearch, "SQL_Latin1_General_Cp1_CI_AI")))
                && x.MaBS == model.MaBS && x.TrangThai >= 1 && x.TrangThai <= 2).OrderBy(x => x.STTPhieuKham.MaUuTien).ThenBy(x => x.STTPhieuKham.STT));
            }
            var listPaged = await listUnpaged.ToPagedListAsync(model.Page ?? 1, 10);
            if (listPaged.PageNumber != 1 && model.Page.HasValue && model.Page > listPaged.PageCount)
                return null;
            return listPaged;
        }
    }
}
