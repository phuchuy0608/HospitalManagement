using HospitalManagement.Interfaces;
using HospitalManagement.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using X.PagedList;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace HospitalManagement.Service
{
    public class DuocSisvc : IDuocSi
    {
        private readonly DataContext _context;
        public DuocSisvc(DataContext context)
        {
            _context = context;
        }
        //Hàm change STT Toa Thuốc
        public async Task<STTTOATHUOC> ChangeSoUuTien(Guid maPK)
        {
            try
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    var existingSTT = await _context.STTTOATHUOC.FindAsync(maPK);
                    existingSTT.UuTien = "C";
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return existingSTT;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }
        //Get All Toa Thuốc Có Trạng Thái
        public async Task<IPagedList<ToaThuoc>> SearchToaThuoc(ToaThuocSearchModel model)
        {
            IEnumerable<ToaThuoc> listUnpaged = null;
            if (model.TrangThaiPK == 1)
            {
                listUnpaged = (_context.ToaThuoc.Include(x => x.STTTOATHUOC).Include(x => x.MaPhieuKhamNavigation).ThenInclude(x => x.MaBNNavigation).Where(x =>
                (string.IsNullOrWhiteSpace(model.KeywordSearch) ||
                EF.Functions.Collate(x.MaPhieuKhamNavigation.MaBNNavigation.HoTen, "SQL_Latin1_General_Cp1_CI_AI").Contains(EF.Functions.Collate(model.KeywordSearch, "SQL_Latin1_General_Cp1_CI_AI")) ||
                EF.Functions.Collate(x.MaPhieuKhamNavigation.MaBNNavigation.SDT, "SQL_Latin1_General_Cp1_CI_AI").Contains(EF.Functions.Collate(model.KeywordSearch, "SQL_Latin1_General_Cp1_CI_AI")))
                && x.TrangThai == model.TrangThai && x.MaPhieuKhamNavigation.TrangThai == 1 && x.STTTOATHUOC != null).OrderBy(x => x.STTTOATHUOC.UuTien).ThenBy(x => x.STTTOATHUOC.STT));
            }
            else
            {
                listUnpaged = (_context.ToaThuoc.Include(x => x.STTTOATHUOC).Include(x => x.MaPhieuKhamNavigation).ThenInclude(x => x.MaBNNavigation).Where(x =>
                (string.IsNullOrWhiteSpace(model.KeywordSearch) ||
                EF.Functions.Collate(x.MaPhieuKhamNavigation.MaBNNavigation.HoTen, "SQL_Latin1_General_Cp1_CI_AI").Contains(EF.Functions.Collate(model.KeywordSearch, "SQL_Latin1_General_Cp1_CI_AI")) ||
                EF.Functions.Collate(x.MaPhieuKhamNavigation.MaBNNavigation.SDT, "SQL_Latin1_General_Cp1_CI_AI").Contains(EF.Functions.Collate(model.KeywordSearch, "SQL_Latin1_General_Cp1_CI_AI")))
                && x.TrangThai == model.TrangThai && x.MaPhieuKhamNavigation.TrangThai == 2).OrderByDescending(x => x.MaPhieuKhamNavigation.NgayKham));
            }
            var listPaged = await listUnpaged.ToPagedListAsync(model.Page ?? 1, 10);
            if (listPaged.PageNumber != 1 && model.Page.HasValue && model.Page > listPaged.PageCount)
                return null;
            return listPaged;
        }
        public async Task<IEnumerable<ChiTietToaThuoc>> GetChiTiet(Guid MaPhieu)
        {
            return await _context.ChiTietToaThuoc.Include(x => x.MaThuocNavigation)
                .Where(x => x.MaPK == MaPhieu).ToListAsync();
        }
        public async Task<ToaThuoc> GetToaThuocByMaPhieu(Guid MaPhieu)
        {
            return await _context.ToaThuoc.Include(x => x.MaPhieuKhamNavigation).Include(x => x.MaPhieuKhamNavigation.MaBNNavigation).Include(x => x.ChiTietToaThuoc)
                .FirstOrDefaultAsync(x => x.MaPhieuKham == MaPhieu);
        }

        public async Task<ToaThuoc> ThanhToanThuoc(Guid maPK, string MaNV)
        {
            try
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    var maHD = "HD_" + DateTime.Now.ToString("ddMMyyyyhhmmss");
                    HoaDonThuoc hoadon = new HoaDonThuoc
                    {
                        MaHD = maHD,
                        MaNV = MaNV,
                        NgayHD = DateTime.Now,
                        MaPK = maPK,
                    };
                    var existingThuoc = await _context.ToaThuoc.Include(x => x.ChiTietToaThuoc).FirstOrDefaultAsync(x => x.MaPhieuKham == maPK);
                    hoadon.TongTien = (decimal)existingThuoc.ChiTietToaThuoc.Sum(x => x.SoLuong * x.DonGiaThuoc);
                    await _context.HoaDonThuoc.AddAsync(hoadon);
                    var stt = await _context.STTTOATHUOC.FindAsync(maPK);
                    existingThuoc.TrangThai = 1;
                    stt.UuTien = "B";
                    if (_context.ToaThuoc.Include(x => x.STTTOATHUOC).Where(x => x.TrangThai == 1 && x.STTTOATHUOC != null).Count() > 0)
                    {
                        stt.STT = _context.ToaThuoc.Include(x => x.STTTOATHUOC).Where(x => x.TrangThai == 1).Max(x => x.STTTOATHUOC.STT);
                    }
                    else
                    {
                        stt.STT = 1;
                    }
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    var phieuKham = await _context.PhieuKham.Include(x => x.MaBNNavigation).Include(x => x.ToaThuoc).ThenInclude(x => x.HoaDonThuoc).ThenInclude(x => x.MaNVNavigation).Include(x => x.ToaThuoc.ChiTietToaThuoc).ThenInclude(x => x.MaThuocNavigation).FirstOrDefaultAsync(x => x.MaPK == maPK);
                    return existingThuoc;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }
        public async Task<ToaThuoc> XacNhanThuocDangCho(Guid maPK)
        {
            try
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    var existingThuocDangPhat = await _context.ToaThuoc.FindAsync(maPK);
                    existingThuocDangPhat.TrangThai = 2;
                    var phieuKham = await _context.PhieuKham.FindAsync(maPK);
                    phieuKham.TrangThai = 2;
                    _context.Update(phieuKham);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return existingThuocDangPhat;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }
    }
}
