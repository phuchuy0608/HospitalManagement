using HospitalManagement.Share.Models.ViewModel;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace HospitalManagement.Infrastructure
{
    public class RealtimeHub: Hub
    {
        public async Task SendMessage(string id,STTViewModel stt)
        {
            await Clients.All.SendAsync("ReceiveMessage", id, stt);
        }
    }
}
