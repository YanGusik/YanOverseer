using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YanOverseer.BLL.Interfaces;
using YanOverseer.DAL;
using YanOverseer.DAL.Models;

namespace YanOverseer.BLL.Services
{
    public class ServerSettingsService : IServerSettingsService
    {
        private readonly MainContext _db;

        public ServerSettingsService(MainContext db)
        {
            _db = db;
        }

        public async Task<ServerSettings> GetServerSettingsByIdAsync(ulong id)
        {
            return await _db.ServerSettings.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task CreateServerSettingsAsync(ulong id, string moderatorRoleStep, string autoRoleName, bool autoRole, bool autoWelcomeMessage)
        {
            var serverSettings = new ServerSettings()
            {
                Id = id,
                ModeratorRoleName = moderatorRoleStep,
                AutoRoleName = autoRoleName,
                AutoRole = autoRole,
                AutoWelcomeMessage = autoWelcomeMessage
            };

            _db.ServerSettings.Add(serverSettings);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateServerSettingsAsync(ulong id, string moderatorRoleStep, string autoRoleName, bool autoRole, bool autoWelcomeMessage)
        {
            var serverSettings = await GetServerSettingsByIdAsync(id);

            if (serverSettings == null) return;

            serverSettings.ModeratorRoleName = moderatorRoleStep;
            serverSettings.AutoRoleName = autoRoleName;
            serverSettings.AutoRole = autoRole;
            serverSettings.AutoWelcomeMessage = autoWelcomeMessage;

            await _db.SaveChangesAsync();
        }

        public async Task DeleteServerSettingsAsync(ulong id)
        {
            var serverSettings = await GetServerSettingsByIdAsync(id);

            if (serverSettings == null) return;

            _db.ServerSettings.Remove(serverSettings);
            await _db.SaveChangesAsync();
        }
    }
}