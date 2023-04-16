using HospitalManagement.Interfaces;
using HospitalManagement.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using X.PagedList;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Service
{
    public class TheLoaisvc : ITheLoai
    {
        private static int pageSize = 6;
        private readonly DataContext _context;

        public TheLoaisvc(DataContext context)
        {
            _context = context;

        }






        public async Task<Response<TheLoai>> Add(TheLoai model)
        {
            try
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    _context.Entry(model).State = EntityState.Added;
                    await _context.SaveChangesAsync();



                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new Response<TheLoai> { errorCode = 0, Obj = model };

                }
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException.Message.Contains("UNIQUE KEY"))
                {
                    return new Response<TheLoai> { errorCode = -1 };
                }

                return new Response<TheLoai> { errorCode = -2 };
            }
        }

        public async Task<TheLoai> Get(Guid id)
        {

            var item = await _context.TheLoai.FirstOrDefaultAsync(i => i.MaTL == id);

            if (item == null)
            {
                return null;
            }
            return item;


        }
        public async Task<Response<TheLoai>> Edit(TheLoai model)
        {
            try
            {
                using (var transaction = _context.Database.BeginTransaction())
                {



                    var existingTheLoai = await _context.TheLoai.FindAsync(model.MaTL);
                    existingTheLoai.TenTL = model.TenTL;

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new Response<TheLoai> { errorCode = 0, Obj = model };
                }

            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException.Message.Contains("UNIQUE KEY"))
                {
                    return new Response<TheLoai> { errorCode = -1 };
                }

                return new Response<TheLoai> { errorCode = -2 };
            }

        }

        public async Task<bool> Delete(Guid Id)
        {
            try
            {

                var find = await _context.TheLoai.FindAsync(Id);

                _context.TheLoai.Remove(find);
                await _context.SaveChangesAsync();

                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }

        }


        public async Task<IPagedList<TheLoai>> SearchByCondition(TheLoaiSearchModel model)
        {

            IEnumerable<TheLoai> listUnpaged;
            listUnpaged = _context.TheLoai.Where(x => string.IsNullOrWhiteSpace(
                model.TenTLSearch) ||
                EF.Functions.Collate(x.TenTL, "SQL_Latin1_General_Cp1_CI_AI").Contains(EF.Functions.Collate(model.TenTLSearch, "SQL_Latin1_General_Cp1_CI_AI"))
                ).OrderBy(x => x.TenTL);

            var listPaged = await listUnpaged.ToPagedListAsync(model.Page ?? 1, pageSize);


            if (listPaged.PageNumber != 1 && model.Page.HasValue && model.Page > listPaged.PageCount)
                return null;

            return listPaged;

        }



        protected IEnumerable<TheLoai> GetAllFromDatabase()
        {
            List<TheLoai> data = new List<TheLoai>();

            data = _context.TheLoai.ToList();

            return data;

        }

        public async Task<IEnumerable<TheLoai>> GetAll()
        {
            return await _context.TheLoai.ToListAsync();
        }
    }
}
