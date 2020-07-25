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
            var autoRoleStep = new BoolStep("Should auto-assignment features be enabled?", autoRoleNameStep);
            var autoWelcomeMessageStep = new BoolStep("Should the welcome features be enabled?", autoRoleStep);

            string moderatorRole = "";
            string autoRoleName = "";
            bool autoRole = false;
            bool autoWelcomeMessage = false;

            moderatorRoleStep.OnValidResult += (result) => moderatorRole = result;
            autoRoleNameStep.OnValidResult += (result) => autoRoleName = result;
            autoRoleStep.OnValidResult += (result) => autoRole = result;
            autoWelcomeMessageStep.OnValidResult += (result) => autoWelcomeMessage = result;

            var inputDialogueHandler = new DialogueHandler(
                ctx.Client,
                ctx.Channel,
                ctx.User,
                autoWelcomeMessageStep
            );

            bool succeeded = await inputDialogueHandler.ProcessDialogue().ConfigureAwait(false);

            if (!succeeded) { return; }

            var serverSettingsService = Program.Container.Resolve<IServerSettingsService>();
            if (await serverSettingsService.GetServerSettingsByIdAsync(ctx.Guild.Id) == null)
                await serverSettingsService.CreateServerSettingsAsync(ctx.Guild.Id, moderatorRole, autoRoleName, autoRole, autoWelcomeMessage);
            else
                await serverSettingsService.UpdateServerSettingsAsync(ctx.Guild.Id, moderatorRole, autoRoleName, autoRole, autoWelcomeMessage);
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