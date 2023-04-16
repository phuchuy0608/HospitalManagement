using HospitalManagement.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using X.PagedList;
using HospitalManagement.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace HospitalManagement.Service
{
    public class TinTucsvc : ITinTuc
    {
        private static int pageSize = 6;
        private readonly DataContext _context;

        public TinTucsvc(DataContext context)
        {
            _context = context;

        }

        public async Task<TinTuc> Add(TinTuc model)
        {
            try
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    _context.Entry(model).State = EntityState.Added;
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

        public async Task<TinTuc> Get(Guid id)
        {

            var item = await _context.TinTuc.Include(x => x.MaTLNavigation).Include(x => x.MaNguoiVietNavigation)
                .FirstOrDefaultAsync(i => i.MaBaiViet == id);

            if (item == null)
            {
                return null;
            }
            return item;
        }

        public async Task<TinTuc> Edit(TinTuc model)
        {
            try
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    var existingTinTuc = _context.TinTuc.Find(model.MaBaiViet);
                    existingTinTuc.TieuDe = model.TieuDe;
                    existingTinTuc.NoiDung = model.NoiDung;
                    existingTinTuc.TrangThai = model.TrangThai;
                    existingTinTuc.MaNguoiViet = model.MaNguoiViet;
                    existingTinTuc.MaTL = model.MaTL;

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

        public async Task<bool> Delete(Guid Id)
        {
            try
            {

                var find = await _context.TinTuc.FindAsync(Id);
                _context.TinTuc.Remove(find);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public IEnumerable<NguoiDung> NguoiDungNav()
        {
            return _context.NguoiDung.ToList();
        }


        public async Task<IPagedList<TinTuc>> SearchByCondition(TinTucSearchModel model)
        {

            IEnumerable<TinTuc> listUnpaged;
            listUnpaged = _context.TinTuc.Include(x => x.MaTLNavigation).Where(x => string.IsNullOrWhiteSpace(model.TieuDeSearch) || EF.Functions.Collate(x.TieuDe, "SQL_Latin1_General_Cp1_CI_AI").Contains(EF.Functions.Collate(model.TieuDeSearch, "SQL_Latin1_General_Cp1_CI_AI"))).OrderBy(x => x.TieuDe);

            if (!string.IsNullOrWhiteSpace(model.MaTLSearch.ToString()))
            {
                listUnpaged = listUnpaged.Where(x => x.MaTL == model.MaTLSearch);
            }

            if (!model.TrangThaiSearch)

            {
                listUnpaged = listUnpaged.Where(x => x.TrangThai);
            }

            var listPaged = await listUnpaged.ToPagedListAsync(model.Page ?? 1, pageSize);
            if (listPaged.PageNumber != 1 && model.Page.HasValue && model.Page > listPaged.PageCount)
                return null;
            return listPaged;
        }



        protected IEnumerable<TinTuc> GetAllFromDatabase()
        {
            List<TinTuc> data = new List<TinTuc>();
            data = _context.TinTuc.ToList();
            return data;
        }



        public async Task<List<TinTuc>> GetTinTuc(Guid MaTL)
        {
            if (MaTL != Guid.Empty)
            {


                return await _context.TinTuc.OrderByDescending(x => x.ThoiGian).Include(x => x.MaTLNavigation).Where(x => x.MaTL == MaTL && x.TrangThai == true).ToListAsync();
            }
            else
            {

                return await (from tintuc in _context.TinTuc.Include(x => x.MaTLNavigation).Include(x => x.MaNguoiVietNavigation)
                              join theloai in _context.TheLoai.Include(x => x.TinTuc)
                              on tintuc.MaTL equals theloai.MaTL
                              where tintuc.TrangThai && theloai.TinTuc.Count >= 3
                              orderby tintuc.ThoiGian descending
                              select tintuc).ToListAsync();
            }
        }

        public async Task<List<TinTuc>> GetTinMin(Guid MaTL)
        {
            int numberOfrecords = 4;
            if (MaTL != Guid.Empty)
            {
                return await _context.TinTuc.Include(x => x.MaTLNavigation).Where(x => x.MaTL == MaTL && x.TrangThai == true).Take(numberOfrecords).ToListAsync();
            }
            else
            {
                return await _context.TinTuc.Include(x => x.MaTLNavigation).Where(x => x.TrangThai == true).Take(numberOfrecords).ToListAsync();
            }
        }
    }
}
