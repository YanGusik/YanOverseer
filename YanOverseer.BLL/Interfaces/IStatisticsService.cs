using System.Threading.Tasks;
using YanOverseer.BLL.Enums;

namespace YanOverseer.BLL.Interfaces
{
    public interface IStatisticsService
    {
        Task IncrementStatisticsAsync(ulong profileId, TypeMessage typeMessage);
        Task UpdateStatisticsAsync(ulong profileId, int? countTextMessage = null, int? countMessageWithImage = null, int? countMessageWithUrl = null);
        Task ClearStatisticsAsync(ulong profileId);
    }
}