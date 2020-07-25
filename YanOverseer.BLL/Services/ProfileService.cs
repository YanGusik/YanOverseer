using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YanOverseer.BLL.Interfaces;
using YanOverseer.DAL;
using YanOverseer.DAL.Models;

namespace YanOverseer.BLL.Services
{
    public class ProfileService : IProfileService
    {
        private readonly MainContext _db;

        public ProfileService(MainContext db)
        {
            _db = db;
        }

        public async Task<Profile> GetOrCreateProfileAsync(ulong discordId, ulong guildId)
        {
            var profile = await _db.Profiles
                .Where(x => x.GuildId == guildId)
                .FirstOrDefaultAsync(x => x.DiscordId == discordId);

            if (profile != null) return profile;

            profile = new Profile
            {
                DiscordId = discordId,
                GuildId = guildId,
                CountMessageWithImage = 0,
                CountMessageWithUrl = 0,
                CountTextMessage = 0
            };

            _db.Profiles.Add(profile);

            await _db.SaveChangesAsync();

            return profile;
        }

        public async Task<Profile> GetProfileByIdAsync(ulong profileId)
        {
            return await _db.Profiles.FirstOrDefaultAsync(x => x.Id == profileId);
        }

        public async Task UpdateProfileAsync(ulong profileId, string alias)
        {
            var profile = await GetProfileByIdAsync(profileId);

            if (profile == null) return;

            profile.Alias = alias;

            await _db.SaveChangesAsync();
        }
    }
}