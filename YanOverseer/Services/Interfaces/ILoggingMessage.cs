using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace YanOverseer.Services.Interfaces
{
    public interface ILoggingMessage
    {
        Task CreateAsync(DiscordChannel channel, DiscordMessage message);
        Task UpdateAsync(DiscordChannel channel, DiscordMessage message, string prevMessage);
        Task DeleteAsync(DiscordChannel channel, DiscordMessage message);
    }
}