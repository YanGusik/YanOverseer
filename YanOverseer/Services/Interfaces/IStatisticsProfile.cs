using System.Threading.Tasks;
using DSharpPlus.Entities;

namespace YanOverseer.Services.Interfaces
{
    public interface IStatisticsProfile
    {
        Task ChangeProfileStatisticsAsync(ulong profileId, DiscordMessage message);
        Task ClearProfileStatisticsAsync(ulong profileId);
    }
}