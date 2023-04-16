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
    public class DichVusvc : IDichVu
    {
        private static int pageSize = 6;
        private readonly DataContext _context;

        public DichVusvc(DataContext context)
        {
            _context = context;

        }
        public async Task<Response<DichVu>> Add(DichVu model)
        {
            try
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    _context.Entry(model).State = EntityState.Added;
                    await _context.SaveChangesAsync();



                    await transaction.CommitAsync();
                    return new Response<DichVu> { errorCode = 0, Obj = model };

                }

            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException.Message.Contains("UNIQUE KEY"))
                {
                    return new Response<DichVu> { errorCode = -1 };
                }

                return new Response<DichVu> { errorCode = -2 };
            }
        }

        public async Task<DichVu> Get(Guid id)
        {

            var item = await _context.DichVu

                .FirstOrDefaultAsync(i => i.MaDV == id);


            if (item == null)
            {
                return null;
            }
            return item;

        }
        public async Task<Response<DichVu>> Edit(DichVu model)
        {
            try
            {
                using (var transaction = _context.Database.BeginTransaction())
                {

                    var existingDichVu = await _context.DichVu.FindAsync(model.MaDV);
                    existingDichVu.TenDV = model.TenDV;
                    existingDichVu.DonGia = model.DonGia;

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new Response<DichVu> { errorCode = 0, Obj = model };
                }


            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException.Message.Contains("UNIQUE KEY"))
                {
                    return new Response<DichVu> { errorCode = -1 };
                }

                return new Response<DichVu> { errorCode = -2 };
            }

        }

        public async Task<bool> Delete(Guid Id)
        {
            try
            {

                var find = await _context.DichVu.FindAsync(Id);

                _context.DichVu.Remove(find);
                await _context.SaveChangesAsync();

                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }


        }


        public async Task<IPagedList<DichVu>> SearchByCondition(DichVuSearchModel model)
        {

            IEnumerable<DichVu> listUnpaged;
            listUnpaged = _context.DichVu.OrderBy(x => x.TenDV);

            listUnpaged = _context.DichVu.Where(x =>
                       string.IsNullOrWhiteSpace(model.KeyWordSearch) ||
                       EF.Functions.Collate(x.TenDV, "SQL_Latin1_General_Cp1_CI_AI").Contains(EF.Functions.Collate(model.KeyWordSearch, "SQL_Latin1_General_Cp1_CI_AI"))
                       )
                           .OrderBy(x => x.TenDV);


            if (!model.TrangThai)
            {
                listUnpaged = listUnpaged.Where(x => x.TrangThai);
            }

            var listPaged = await listUnpaged.ToPagedListAsync(model.Page ?? 1, pageSize);

            if (listPaged.PageNumber != 1 && model.Page.HasValue && model.Page > listPaged.PageCount)
                return null;

            return listPaged;

        }



        public async Task<IEnumerable<DichVu>> GetDichVu(Guid MaPK)
        {
            if (MaPK != Guid.Empty)
            {

                return await _context.DichVu.Where(x => _context.ChiTietDV.Any(y => y.MaDV == x.MaDV && y.MaHDNavigation.MaPK == MaPK)).ToListAsync();
            }
            return await _context.DichVu.ToListAsync();

        }

    }
}
