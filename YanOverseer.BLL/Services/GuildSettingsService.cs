using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YanOverseer.BLL.Interfaces;
using YanOverseer.DAL;
using YanOverseer.DAL.Models;

namespace YanOverseer.BLL.Services
{
    public class GuildSettingsService : IGuildSettingsService
    {
        private readonly MainContext _db;

        public GuildSettingsService(MainContext db)
        {
            _db = db;
        }

        public async Task<GuildSettings> GetGuildSettingsByIdAsync(ulong id)
        {
            return await _db.GuildSettings.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task CreateGuildSettingsAsync(ulong id, string autoRoleName, string moderatorRole, ulong сreateMessageChannel, ulong updateMessageChannel, ulong deleteMessageChannel, bool autoRole, bool autoWelcomeMessage, bool autoLogCreateMessage, bool autoLogUpdateMessage, bool autoLogDeleteMessage)
        {
            var guildSettings = new GuildSettings()
            {
                Id = id,
                AutoRoleName = autoRoleName,
                ModeratorRoleName = moderatorRole,
                AutoRole = autoRole,
                AutoWelcomeMessage = autoWelcomeMessage,
                AutoLogCreateMessage = autoLogCreateMessage,
                AutoLogUpdateMessage = autoLogUpdateMessage,
                AutoLogDeleteMessage = autoLogDeleteMessage,
                CreateMessageChannel = сreateMessageChannel,
                DeleteMessageChannel = deleteMessageChannel,
                UpdateMessageChannel = updateMessageChannel
            };

            _db.GuildSettings.Add(guildSettings);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateGuildSettingsAsync(ulong id, string autoRoleName, string moderatorRole, ulong сreateMessageChannel, ulong updateMessageChannel, ulong deleteMessageChannel, bool autoRole, bool autoWelcomeMessage, bool autoLogCreateMessage, bool autoLogUpdateMessage, bool autoLogDeleteMessage)
        {
            var guildSettings = await GetGuildSettingsByIdAsync(id);

            if (guildSettings == null) return;

            guildSettings.AutoRoleName = autoRoleName;
            guildSettings.ModeratorRoleName = moderatorRole;
            guildSettings.AutoRole = autoRole;
            guildSettings.AutoWelcomeMessage = autoWelcomeMessage;
            guildSettings.AutoLogCreateMessage = autoLogCreateMessage;
            guildSettings.AutoLogUpdateMessage = autoLogUpdateMessage;
            guildSettings.AutoLogDeleteMessage = autoLogDeleteMessage;
            guildSettings.CreateMessageChannel = сreateMessageChannel;
            guildSettings.DeleteMessageChannel = deleteMessageChannel;
            guildSettings.UpdateMessageChannel = updateMessageChannel;

            await _db.SaveChangesAsync();
        }

        public async Task DeleteGuildSettingsAsync(ulong id)
        {
            var guildSettings = await GetGuildSettingsByIdAsync(id);

            if (guildSettings == null) return;

            _db.GuildSettings.Remove(guildSettings);
            await _db.SaveChangesAsync();
        }
    }
}