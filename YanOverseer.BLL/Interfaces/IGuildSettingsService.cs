using System;
using System.Threading.Tasks;
using YanOverseer.DAL.Models;

namespace YanOverseer.BLL.Interfaces
{
    public interface IGuildSettingsService
    {
        Task<GuildSettings> GetGuildSettingsByIdAsync(ulong id);
        Task CreateGuildSettingsAsync(ulong id,  string autoRoleName, string moderatorRole, ulong сreateMessageChannel, ulong updateMessageChannel, ulong deleteMessageChannel, bool autoRole, bool autoWelcomeMessage, bool autoLogCreateMessage, bool autoLogUpdateMessage, bool autoLogDeleteMessage);
        Task UpdateGuildSettingsAsync(ulong id,  string autoRoleName, string moderatorRole, ulong сreateMessageChannel, ulong updateMessageChannel, ulong deleteMessageChannel, bool autoRole, bool autoWelcomeMessage, bool autoLogCreateMessage, bool autoLogUpdateMessage, bool autoLogDeleteMessage);
        Task DeleteGuildSettingsAsync(ulong id);
    }
}