using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace YanOverseer.Commands
{
    [Group("admin")]
    [Description("Administrative commands.")]
    [Hidden]
    public class AdminCommands
    {
        [Command("sudo")] [Description("Executes a command as another user.")] [Hidden] [RequireOwner]
        public async Task Sudo(CommandContext ctx, [Description("Member to execute as.")] DiscordMember member, [RemainingText] [Description("Command text to execute.")] string command)
        {
            await ctx.TriggerTypingAsync();

            try
            {
                var cmds = ctx.CommandsNext;

                await cmds.SudoAsync(member, ctx.Channel, command);
            }
            catch (Exception e)
            {
                var emoji = DiscordEmoji.FromName(ctx.Client, ":-1:");
                await ctx.RespondAsync(emoji);
            }
        }

        [Command("nick")] [Description("Gives someone a new nickname.")] [RequireOwner] [RequirePermissions(Permissions.ManageNicknames)]
        public async Task ChangeNickname(CommandContext ctx, 
            [Description("Member to change the nickname for.")] DiscordMember member, 
            [RemainingText] [Description("The nickname to give to that user.")] string newNickname)
        {
            await ctx.TriggerTypingAsync();

            try
            {
                await member.ModifyAsync(newNickname, reason: $"Changed by {ctx.User.Username} ({ctx.User.Id}).");
                var emoji = DiscordEmoji.FromName(ctx.Client, ":+1:");
                await ctx.RespondAsync(emoji);
            }
            catch (Exception ex)
            {
                var emoji = DiscordEmoji.FromName(ctx.Client, ":-1:");
                await ctx.RespondAsync(emoji);
            }
        }
    }
}