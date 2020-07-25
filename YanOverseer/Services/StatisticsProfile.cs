using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using YanOverseer.BLL.Enums;
using YanOverseer.BLL.Interfaces;
using YanOverseer.Services.Interfaces;

namespace YanOverseer.Services
{
    public class StatisticsProfile : IStatisticsProfile
    {
        private readonly IStatisticsService _statisticsService;

        public StatisticsProfile(IStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        public async Task ChangeProfileStatisticsAsync(ulong profileId, DiscordMessage message)
        {
            var typeMessage = GetTypeMessage(message);
            await _statisticsService.IncrementStatisticsAsync(profileId, typeMessage);
        }

        public async Task ClearProfileStatisticsAsync(ulong profileId)
        {
            await _statisticsService.ClearStatisticsAsync(profileId);
        }

        public TypeMessage GetTypeMessage(DiscordMessage message)
        {
            if (message.Attachments.Any(messageAttachment =>
                messageAttachment.FileName.EndsWith(".png") || messageAttachment.FileName.EndsWith(".jpg")))
                return TypeMessage.Image;

            if (!string.IsNullOrWhiteSpace(message.Content))
            {
                if (message.Content.Contains("http") || message.Content.EndsWith("/") ||
                    message.Content.Contains(".com") || message.Content.Contains(".ru") ||
                    message.Content.Contains(".net"))
                    return TypeMessage.Url;
                return TypeMessage.Text;
            }

            return TypeMessage.Any;
        }
    }
}