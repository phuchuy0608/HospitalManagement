using System.Threading.Tasks;
using System;
using HospitalManagement.Interfaces;
using HospitalManagement.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Service
{
    public class AutoBackgroundSvc:IAutoBackground
    {
        private readonly DataContext _context;
        public AutoBackgroundSvc(DataContext context)
        {
            _context = context;
        }
        public async Task<bool> AutoDelete()
        {
            try
            {
                using (var transaction = _context.Database.BeginTransaction())
                {

                    if (_context.STTPhieuKham.Count() > 0 || _context.STTTOATHUOC.Count() > 0)
                    {
                        var result = _context.Database.ExecuteSqlRaw("EXEC dbo.sp_DeleteIdPK_TT");
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return true;

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;

            }
        }
    }
}
