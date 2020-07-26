using System;
using System.Threading.Tasks;
using Autofac;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using YanOverseer.Attributes;
using YanOverseer.BLL.Interfaces;
using YanOverseer.Handlers.Dialogue;
using YanOverseer.Handlers.Dialogue.Steps;

namespace YanOverseer.Commands
{
    [Group("admin")]
    [Description("Administrative commands.")]
    [RequireUserType(UserTypeCheckMode.Any, UserType.Admin)]
    public class AdminCommands
    {
        [Command("configurate")]
        [Description("Configurate your server")]
        public async Task Configurate(CommandContext ctx)
        {
            
            var moderatorRoleStep = new TextStep("What is the name of the role of the moderator?", null);
            var autoRoleNameStep = new TextStep("What is the default role called when a new member joins?", moderatorRoleStep);
            var createMessageChannelStep = new UlongStep("Id Create Message Channel", autoRoleNameStep);
            var updateMessageChannelStep = new UlongStep("Id Update Message Channel", createMessageChannelStep);
            var deleteMessageChannelStep = new UlongStep("Id Delete Message Channel", updateMessageChannelStep);

            var autoRoleStep = new BoolStep("Включить ли автороль?", deleteMessageChannelStep);
            var autoWelcomeMessageStep = new BoolStep("Включить ли автоприветствие?", autoRoleStep);
            var autoLogCreateMessageStep = new BoolStep("Log when creating posts?", autoWelcomeMessageStep);
            var autoLogUpdateMessageStep = new BoolStep("Log when editing posts?", autoLogCreateMessageStep);
            var autoLogDeleteMessageStep = new BoolStep("Log when deleting posts?", autoLogUpdateMessageStep);

            bool autoRole = false;
            bool autoWelcomeMessage = false;
            var autoLogCreateMessage = false;
            var autoLogUpdateMessage = false;
            var autoLogDeleteMessage = false;

            string moderatorRole = "";
            string autoRoleName = "";
            ulong createMessageChannel = 0;
            ulong updateMessageChannel = 0;
            ulong deleteMessageChannel = 0;

            autoRoleStep.OnValidResult += (result) => autoRole = result;
            autoWelcomeMessageStep.OnValidResult += (result) => autoWelcomeMessage = result;
            autoLogCreateMessageStep.OnValidResult += (result) => autoLogCreateMessage = result;
            autoLogUpdateMessageStep.OnValidResult += (result) => autoLogUpdateMessage = result;
            autoLogDeleteMessageStep.OnValidResult += (result) => autoLogDeleteMessage = result;

            moderatorRoleStep.OnValidResult += (result) => moderatorRole = result;
            autoRoleNameStep.OnValidResult += (result) => autoRoleName = result;
            createMessageChannelStep.OnValidResult += (result) => createMessageChannel = result;
            updateMessageChannelStep.OnValidResult += (result) => updateMessageChannel = result;
            deleteMessageChannelStep.OnValidResult += (result) => deleteMessageChannel = result;

            var inputDialogueHandler = new DialogueHandler(
                    ctx.Client,
                    ctx.Channel,
                    ctx.User,
                    autoLogDeleteMessageStep
                );

            bool succeeded = await inputDialogueHandler.ProcessDialogue().ConfigureAwait(false);

            if (!succeeded) { return; }

            var guildSettingsService = Program.Container.Resolve<IGuildSettingsService>();
            if (await guildSettingsService.GetGuildSettingsByIdAsync(ctx.Guild.Id) == null)
                await guildSettingsService.CreateGuildSettingsAsync(ctx.Guild.Id, autoRoleName, moderatorRole, createMessageChannel, updateMessageChannel, deleteMessageChannel, autoRole, autoWelcomeMessage, autoLogCreateMessage, autoLogUpdateMessage, autoLogDeleteMessage);
            else
                await guildSettingsService.UpdateGuildSettingsAsync(ctx.Guild.Id, autoRoleName, moderatorRole, createMessageChannel, updateMessageChannel, deleteMessageChannel, autoRole, autoWelcomeMessage, autoLogCreateMessage, autoLogUpdateMessage, autoLogDeleteMessage);
        }

        [Command("sudo")]
        [Description("Executes a command as another user.")]
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

        [Command("nick")]
        [Description("Gives someone a new nickname.")]
        [RequirePermissions(Permissions.ManageNicknames)]
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