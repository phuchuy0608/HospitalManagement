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
    public class Thuocsvc : IThuoc
    {
        private static int pageSize = 6;
        private readonly DataContext _context;

        public Thuocsvc(DataContext context)
        {
            _context = context;

        }

        public async Task<Response<Thuoc>> Add(Thuoc model)
        {
            try
            {
                using (var transaction = _context.Database.BeginTransaction())
                {

                    _context.Entry(model).State = EntityState.Added;
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                    return new Response<Thuoc> { errorCode = 0, Obj = model };
                }

            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException.Message.Contains("UNIQUE KEY"))
                {
                    return new Response<Thuoc> { errorCode = -1 };
                }

                return new Response<Thuoc> { errorCode = -2 };

            }
        }

        public async Task<Thuoc> Get(Guid id)
        {

            var item = await _context.Thuoc.FirstOrDefaultAsync(i => i.MaThuoc == id);

            if (item == null)
            {
                return null;
            }
            return item;

        }
        public async Task<Response<Thuoc>> Edit(Thuoc model)
        {
            try
            {
                using (var transaction = _context.Database.BeginTransaction())
                {

                    var existingThuoc = await _context.Thuoc.FindAsync(model.MaThuoc);
                    existingThuoc.TenThuoc = model.TenThuoc;
                    existingThuoc.Vitri = model.Vitri;
                    existingThuoc.DonGia = model.DonGia;
                    existingThuoc.ThongTin = model.ThongTin;
                    existingThuoc.TrangThai = model.TrangThai;
                    existingThuoc.HinhAnh = model.HinhAnh;

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new Response<Thuoc> { errorCode = 0, Obj = model };
                }


            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException.Message.Contains("UNIQUE KEY"))
                {
                    return new Response<Thuoc> { errorCode = -1 };
                }

                return new Response<Thuoc> { errorCode = -2 };
            }

        }

        public async Task<bool> Delete(Guid Id)
        {
            try
            {
                var find = await _context.Thuoc.FindAsync(Id);

                _context.Thuoc.Remove(find);
                await _context.SaveChangesAsync();

                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }


        }


        public async Task<IPagedList<Thuoc>> SearchByCondition(ThuocSearchModel model)
        {

            IEnumerable<Thuoc> listUnpaged;
            listUnpaged = _context.Thuoc.Where(x =>
            string.IsNullOrWhiteSpace(model.KeyWordSearch) ||
            EF.Functions.Collate(x.TenThuoc, "SQL_Latin1_General_Cp1_CI_AI").Contains(EF.Functions.Collate(model.KeyWordSearch, "SQL_Latin1_General_Cp1_CI_AI")) ||
            EF.Functions.Collate(x.ThongTin, "SQL_Latin1_General_Cp1_CI_AI").Contains(EF.Functions.Collate(model.KeyWordSearch, "SQL_Latin1_General_Cp1_CI_AI"))
            ).OrderBy(x => x.TenThuoc);

            if (!model.TrangThai)
            {
                listUnpaged = listUnpaged.Where(x => x.TrangThai);
            }
            var listPaged = await listUnpaged.ToPagedListAsync(model.Page ?? 1, pageSize);


            if (listPaged.PageNumber != 1 && model.Page.HasValue && model.Page > listPaged.PageCount)
                return null;

            return listPaged;

        }

        public List<Thuoc> GetAllThuoc()
        {
            return _context.Thuoc.Where(x => x.TrangThai).AsNoTracking().ToList();
        }
    }
}
