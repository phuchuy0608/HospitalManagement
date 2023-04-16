using HospitalManagement.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using X.PagedList;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using HospitalManagement.Interfaces;
using HospitalManagement.ADMIN.Shared.Helper;

namespace HospitalManagement.Service
{
    public class NhanVienYtesvc : INhanVienYte
    {
        private static int pageSize = 6;
        private readonly DataContext _context;


        public NhanVienYtesvc(DataContext context)
        {
            _context = context;


        }
        
        public async Task<NhanVienYte> Get(string id)
        {

            var item = await _context.NhanVienYte.Include(x => x.ChuyenKhoaNavigation)

                .FirstOrDefaultAsync(i => i.Id == id);


            if (item == null)
            {
                return null;
            }
            return item;


        }

        public async Task<Response<NhanVienYte>> Edit(string id, bool TrangThai)
        {
            try
            {
                using (var transaction = _context.Database.BeginTransaction())
                {

                    var existingNguoiDung = await _context.NhanVienYte.FindAsync(id);

                    existingNguoiDung.TrangThai = TrangThai;


                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new Response<NhanVienYte> { errorCode = 0, Obj = existingNguoiDung };
                }


            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException.Message.Contains("UNIQUE KEY"))
                {
                    return new Response<NhanVienYte> { errorCode = -1 };
                }

                return new Response<NhanVienYte> { errorCode = -2 };
            }




        }

        public async Task<IPagedList<NhanVienYte>> SearchByCondition(NhanVienYteSearchModel model)
        {

            IEnumerable<NhanVienYte> listUnpaged;
            if (model.TrangThai == true)
            {
                listUnpaged = _context.NhanVienYte.Include(x => x.ChuyenKhoaNavigation).Where(x =>
                   (string.IsNullOrWhiteSpace(model.KeyWordSearch)) ||
                   EF.Functions.Collate(x.HoTen, "SQL_Latin1_General_Cp1_CI_AI").Contains(EF.Functions.Collate(model.KeyWordSearch, "SQL_Latin1_General_Cp1_CI_AI")) ||
                   EF.Functions.Collate(x.PhoneNumber, "SQL_Latin1_General_Cp1_CI_AI").Contains(EF.Functions.Collate(model.KeyWordSearch, "SQL_Latin1_General_Cp1_CI_AI"))




                   ).OrderBy(x => x.HoTen);
            }
            else
            {
                listUnpaged = _context.NhanVienYte.Include(x => x.ChuyenKhoaNavigation).Where(x =>
                  ((string.IsNullOrWhiteSpace(model.KeyWordSearch)) ||
                  EF.Functions.Collate(x.HoTen, "SQL_Latin1_General_Cp1_CI_AI").Contains(EF.Functions.Collate(model.KeyWordSearch, "SQL_Latin1_General_Cp1_CI_AI")) ||
                  EF.Functions.Collate(x.PhoneNumber, "SQL_Latin1_General_Cp1_CI_AI").Contains(EF.Functions.Collate(model.KeyWordSearch, "SQL_Latin1_General_Cp1_CI_AI")))
                  && x.TrangThai == true


                  ).OrderBy(x => x.HoTen);
            }

            var listPaged = await listUnpaged.ToPagedListAsync(model.Page ?? 1, pageSize);


            if (listPaged.PageNumber != 1 && model.Page.HasValue && model.Page > listPaged.PageCount)
                return null;

            return listPaged;

        }


        public async Task<IEnumerable<NhanVienYte>> GetAllBS(Guid MaCK)
        {
            return await _context.NhanVienYte.Where(x => x.ChuyenKhoa == MaCK).ToListAsync();
        }
        protected IEnumerable<NhanVienYte> GetAllFromDatabase()
        {
            List<NhanVienYte> data = new List<NhanVienYte>();

            data = _context.NhanVienYte.ToList();

            return data;

        }
    }
}
