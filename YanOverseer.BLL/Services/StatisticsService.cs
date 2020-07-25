using System;
using System.Threading.Tasks;
using YanOverseer.BLL.Enums;
using YanOverseer.BLL.Interfaces;
using YanOverseer.DAL;

namespace YanOverseer.BLL.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly MainContext _db;
        private readonly IProfileService _profileService;

        public StatisticsService(MainContext db, IProfileService profileService)
        {
            _db = db;
            _profileService = profileService;
        }

        public async Task IncrementStatisticsAsync(ulong profileId, TypeMessage typeMessage)
        {
            var profile = await _profileService.GetProfileByIdAsync(profileId);

            if (profile == null) return;

            switch (typeMessage)
            {
                case TypeMessage.Text:
                    profile.CountTextMessage++;
                    break;
                case TypeMessage.Image:
                    profile.CountMessageWithImage++;
                    break;
                case TypeMessage.Url:
                    profile.CountMessageWithUrl++;
                    break;
                case TypeMessage.Any:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(typeMessage), typeMessage, null);
            }

            await _db.SaveChangesAsync();
        }

        public async Task UpdateStatisticsAsync(ulong profileId, int? countTextMessage = null, int? countMessageWithImage = null, int? countMessageWithUrl = null)
        {
            var profile = await _profileService.GetProfileByIdAsync(profileId);

            if (profile == null) return;

            if (countTextMessage != null)
                profile.CountTextMessage = (int) countTextMessage;
            if (countMessageWithImage != null)
                profile.CountMessageWithImage = (int) countMessageWithImage;
            if (countMessageWithUrl != null)
                profile.CountMessageWithUrl = (int) countMessageWithUrl;

            await _db.SaveChangesAsync();
        }

        public async Task ClearStatisticsAsync(ulong profileId)
        {
            var profile = await _profileService.GetProfileByIdAsync(profileId);

            if (profile == null) return;

            profile.CountTextMessage = 0;
            profile.CountMessageWithImage = 0;
            profile.CountMessageWithUrl = 0;

            await _db.SaveChangesAsync();
        }
    }
}