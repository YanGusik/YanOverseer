using System;
using System.Threading.Tasks;
using YanOverseer.DAL.Models;

namespace YanOverseer.BLL.Interfaces
{
    public interface IServerSettingsService
    {
        Task<ServerSettings> GetServerSettingsByIdAsync(ulong id);
        Task CreateServerSettingsAsync(ulong id, string moderatorRoleStep, string autoRoleName, bool autoRole, bool autoWelcomeMessage);
        Task UpdateServerSettingsAsync(ulong id, string moderatorRoleStep, string autoRoleName, bool autoRole, bool autoWelcomeMessage);
        Task DeleteServerSettingsAsync(ulong id);
    }
}