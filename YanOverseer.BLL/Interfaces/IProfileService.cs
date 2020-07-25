using System.Threading.Tasks;
using YanOverseer.DAL.Models;

namespace YanOverseer.BLL.Interfaces
{
    public interface IProfileService
    {
        Task<Profile> GetOrCreateProfileAsync(ulong discordId, ulong guildId);
        Task<Profile> GetProfileByIdAsync(ulong profileId);
        Task UpdateProfileAsync(ulong profileId, string status);
    }
}