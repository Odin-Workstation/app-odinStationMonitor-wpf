using TacoStationMonitor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace TacoStationMonitor.Service
{
    public class DashboardService : IDashboardService
    {
        private readonly HttpClient _client = new();

        public async Task<DashboardState?> GetDashboardDataAsync(int partId)
        {
            try
            {
                DashboardState ds = new DashboardState();
                return ds;
            }
            catch
            {
                return null;
            }
        }

        public Task<DashboardState?> GetDashboardData()
        {
            try
            {
                DashboardState ds = new DashboardState();
                return Task.FromResult(ds);
                //return await _client.GetFromJsonAsync<DashboardState>(
                //    "http://localhost:5000/api/dashboard");
            }
            catch
            {
                return Task.FromResult<DashboardState?>(null);
                //return null;
            }
        }
    }
}
