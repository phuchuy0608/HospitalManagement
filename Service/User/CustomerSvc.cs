using HospitalManagement.Interfaces;
using HospitalManagement.Models;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using HospitalManagement.Infrastructure;

namespace HospitalManagement.Service
{
    public class CustomerSvc : ICustomer
    {
        private readonly DataContext _context;
        private IHubContext<SignalServer> _hubContext;
        public CustomerSvc(DataContext context, IHubContext<SignalServer> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }
        public async Task<PhieuDatLich> DatLich(PhieuDatLich model)
        {
            try
            {
                await _context.PhieuDatLich.AddAsync(model);
                await _context.SaveChangesAsync();
                await _hubContext.Clients.All.SendAsync("displayNotification", "");
                return model;
            }
            catch (Exception ex)
            {

                return null;
            }

        }

        public async Task<PhieuKham> GetLichSuKhamById(Guid MaPK)
        {
            return await _context.PhieuKham
                .Include(x => x.MaBNNavigation)
                .Include(x => x.ToaThuoc)
                .ThenInclude(x => x.ChiTietToaThuoc)
                .Include(x => x.HoaDon).ThenInclude(x => x.ChiTietDV)
                .ThenInclude(x => x.MaDVNavigation)
                .FirstOrDefaultAsync(x => x.MaPK == MaPK);
        }

        public async Task<PhieuDatLich> GetPhieuDat(string MaPhieu)
        {
            return await _context.PhieuDatLich.FindAsync(MaPhieu);
        }

        public async Task<List<PhieuKham>> SearchByPhoneNumber(string SDT)
        {
            return await _context.PhieuKham.Include(x => x.MaBNNavigation).Where(x => x.MaBNNavigation.SDT == SDT).Where(x => x.TrangThai == 2).ToListAsync();

        }

        public async Task<List<PhieuDatLich>> SearchDatLichByPhonenumber(string SDT)
        {
            return await _context.PhieuDatLich.Where(x => x.NgayKham > DateTime.Now).Where(x => x.SDT == SDT).ToListAsync();

        }
    }
}
