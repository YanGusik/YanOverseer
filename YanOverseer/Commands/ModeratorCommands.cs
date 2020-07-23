using System;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace YanOverseer.Commands
{
    [RequireRolesAttribute("Артас", "Footman")]
    [Description("Moderator commands.")]
    public class ModeratorCommands
    {
        [Command("clear"), Description("Clear Chat from channel.")]
        public async Task ClearChat(CommandContext ctx, [Description("Count of messages to delete [2-90]")] int count, [Description("Is Force Delete")] bool isForce = false)
        {
            try
            {
                if (count > 90)
                {
                    throw new ArgumentException("Cannot delete more than 90 messages.");
                }

                if (isForce)
                {

                    var messages = await ctx.Channel.GetMessagesAsync(limit: count);
                    foreach (var message in messages)
                    {
                        await ctx.Channel.DeleteMessageAsync(message);
                        Thread.Sleep(300);
                    }
                }
                else
                {
                    var messages = await ctx.Channel.GetMessagesAsync(limit: count);
                    await ctx.Channel.DeleteMessagesAsync(messages);
                }
            }
            catch (Exception e)
            {
                var emoji = DiscordEmoji.FromName(ctx.Client, ":-1:");
                await ctx.RespondAsync($"{emoji} - {e.Message}");
            }
        }
    }
}
