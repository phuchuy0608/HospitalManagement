using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Web;
using X.PagedList;
using HospitalManagement.Interfaces;
using HospitalManagement.Models;
using HospitalManagement_Entities.Models.ViewModel;
using HospitalManagement.ADMIN.Shared.Helper;

namespace HospitalManagement.Services
{
    public class NguoiDungsvc : INguoiDung
    {
        private static int pageSize = 6;
        private readonly DataContext _context;
        public NguoiDungsvc(DataContext context)
        {
            _context = context;
        }
        public async Task<Response<NguoiDung>> Add(NguoiDung model)
        {
            try
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    model.MatKhau = MaHoaHelper.Mahoa(model.MatKhau);

                    _context.Entry(model).State = EntityState.Added;
                    await _context.SaveChangesAsync();



                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new Response<NguoiDung> { errorCode = 0, Obj = model };

                }


            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException.Message.Contains("UNIQUE KEY"))
                {
                    return new Response<NguoiDung> { errorCode = -1 };
                }

                return new Response<NguoiDung> { errorCode = -2 };
            }
        }

        public async Task<NguoiDung> Get(Guid id)
        {

            var item = await _context.NguoiDung

                .FirstOrDefaultAsync(i => i.MaNguoiDung == id);


            if (item == null)
            {
                return null;
            }
            return item;


        }
        public async Task<Response<NguoiDung>> Edit(NguoiDung model)
        {
            try
            {
                using (var transaction = _context.Database.BeginTransaction())
                {

                    var existingNguoiDung = await _context.NguoiDung.FindAsync(model.MaNguoiDung);
                    existingNguoiDung.Email = model.Email;
                    existingNguoiDung.HoTen = model.HoTen;
                    existingNguoiDung.SDT = model.SDT;
                    existingNguoiDung.HinhAnh = model.HinhAnh;
                    existingNguoiDung.ChucVu = model.ChucVu;
                    existingNguoiDung.TrangThai = model.TrangThai;
                    existingNguoiDung.MatKhau = model.MatKhau; //Doi mat khau


                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new Response<NguoiDung> { errorCode = 0, Obj = model };
                }


            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException.Message.Contains("UNIQUE KEY"))
                {
                    return new Response<NguoiDung> { errorCode = -1 };
                }

                return new Response<NguoiDung> { errorCode = -2 };
            }

        }

        public async Task<bool> Delete(Guid Id)
        {
            try
            {

                var find = await _context.NguoiDung.FindAsync(Id);


                _context.NguoiDung.Remove(find);
                await _context.SaveChangesAsync();

                return true;


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }

        }


        public async Task<IPagedList<NguoiDung>> SearchByCondition(NguoiDungSearchModel model)
        {

            IEnumerable<NguoiDung> listUnpaged = null;
            if (model.TrangThai == true)
            {
                listUnpaged = _context.NguoiDung.Where(x =>
                   (string.IsNullOrWhiteSpace(model.KeyWordSearch)) ||
                   EF.Functions.Collate(x.HoTen, "SQL_Latin1_General_Cp1_CI_AI").Contains(EF.Functions.Collate(model.KeyWordSearch, "SQL_Latin1_General_Cp1_CI_AI")) ||
                   EF.Functions.Collate(x.SDT, "SQL_Latin1_General_Cp1_CI_AI").Contains(EF.Functions.Collate(model.KeyWordSearch, "SQL_Latin1_General_Cp1_CI_AI"))
                   ).OrderBy(x => x.HoTen);
            }
            else
            {
                listUnpaged = _context.NguoiDung.Where(x =>
                  ((string.IsNullOrWhiteSpace(model.KeyWordSearch)) ||
                  EF.Functions.Collate(x.HoTen, "SQL_Latin1_General_Cp1_CI_AI").Contains(EF.Functions.Collate(model.KeyWordSearch, "SQL_Latin1_General_Cp1_CI_AI")) ||
                  EF.Functions.Collate(x.SDT, "SQL_Latin1_General_Cp1_CI_AI").Contains(EF.Functions.Collate(model.KeyWordSearch, "SQL_Latin1_General_Cp1_CI_AI")))
                  && x.TrangThai == true
                  ).OrderBy(x => x.HoTen);
            }

            var listPaged = await listUnpaged.ToPagedListAsync(model.Page ?? 1, pageSize);


            if (listPaged.PageNumber != 1 && model.Page.HasValue && model.Page > listPaged.PageCount)
                return null;

            return listPaged;

        }



        protected IEnumerable<NguoiDung> GetAllFromDatabase()
        {
            List<NguoiDung> data = new List<NguoiDung>();
            data = _context.NguoiDung.ToList();
            return data;

        }

        public NguoiDung Login(AdminLoginViewModel viewLogin)
        {
            var u = _context.NguoiDung.Where(
                p => p.Email.Equals(viewLogin.UserName)
                && p.MatKhau.Equals(MaHoaHelper.Mahoa(viewLogin.Password))).FirstOrDefault();
            return u;
        }
    }
}


