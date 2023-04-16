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
    public class ChuyenKhoasvc : IChuyenKhoa
    {
        private static int pageSize = 6;
        private readonly DataContext _context;

        public ChuyenKhoasvc(DataContext context)
        {
            _context = context;

        }

        public async Task<Response<ChuyenKhoa>> Add(ChuyenKhoa model)
        {
            try
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    _context.Entry(model).State = EntityState.Added;
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                    return new Response<ChuyenKhoa> { errorCode = 0, Obj = model };

                }

            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException.Message.Contains("UNIQUE KEY"))
                {
                    return new Response<ChuyenKhoa> { errorCode = -1 };
                }

                return new Response<ChuyenKhoa> { errorCode = -2 };

            }
        }

        public async Task<ChuyenKhoa> Get(Guid id)
        {

            var item = await _context.ChuyenKhoa.FindAsync(id);

            if (item == null)
            {
                return null;
            }
            return item;


        }
        public async Task<Response<ChuyenKhoa>> Edit(ChuyenKhoa model)
        {
            try
            {
                using (var transaction = _context.Database.BeginTransaction())
                {

                    var existingChuyenKhoa = await _context.ChuyenKhoa.FindAsync(model.MaCK);
                    existingChuyenKhoa.TenCK = model.TenCK;

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new Response<ChuyenKhoa> { errorCode = 0, Obj = model }; ;
                }


            }

            catch (DbUpdateException ex)
            {
                if (ex.InnerException.Message.Contains("UNIQUE KEY"))
                {
                    return new Response<ChuyenKhoa> { errorCode = -1 };
                }
                return new Response<ChuyenKhoa> { errorCode = -2 };

            }

        }

        public async Task<bool> Delete(Guid Id)
        {
            try
            {

                var find = await _context.ChuyenKhoa.FindAsync(Id);

                _context.ChuyenKhoa.Remove(find);
                await _context.SaveChangesAsync();

                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }


        }


        public async Task<IPagedList<ChuyenKhoa>> SearchByCondition(ChuyenKhoaSearchModel model)
        {

            IEnumerable<ChuyenKhoa> listUnpaged = null;
            listUnpaged = _context.ChuyenKhoa.Where(x =>
            string.IsNullOrWhiteSpace(model.TenCKSearch) ||
            EF.Functions.Collate(x.TenCK, "SQL_Latin1_General_Cp1_CI_AI")
            .Contains(EF.Functions.Collate(model.TenCKSearch, "SQL_Latin1_General_Cp1_CI_AI")))
            .OrderBy(x => x.TenCK);

            var listPaged = await listUnpaged.ToPagedListAsync(model.Page ?? 1, pageSize);

            if (listPaged.PageNumber != 1 && model.Page.HasValue && model.Page > listPaged.PageCount)
                return null;

            return listPaged;

        }

        public async Task<IEnumerable<ChuyenKhoa>> GetChuyenKhoaHaveDoctor()
        {
            return await (from Ck in _context.ChuyenKhoa
                          join
                    doctor in _context.NhanVienYte
                    on Ck.MaCK equals doctor.ChuyenKhoa
                          select Ck
                    ).Distinct().ToListAsync();
        }


        public async Task<IEnumerable<ChuyenKhoa>> GetAll()
        {

            return await _context.ChuyenKhoa.ToListAsync();

        }
    }
}
