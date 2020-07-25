using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace YanOverseer.Services.Interfaces
{
    public interface ILoggingMessage
    {
        void Enable();
        void Disable();
        void Log(MessageCreateEventArgs e, DiscordChannel channel);
        void Log(MessageUpdateEventArgs e, DiscordChannel channel);
        void Log(MessageDeleteEventArgs e, DiscordChannel channel);
    }
}