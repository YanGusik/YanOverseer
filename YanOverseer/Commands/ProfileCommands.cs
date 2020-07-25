using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using YanOverseer.Attributes;
using YanOverseer.BLL.Interfaces;
using YanOverseer.Services.Interfaces;

namespace YanOverseer.Commands
{
    [Group("profile", CanInvokeWithoutSubcommand = true)]
    [Description("Profile commands.")]
    public class ProfileCommands
    {
        [Description("Get Profile Info")]
        public async Task ExecuteGroupAsync(CommandContext ctx, [Description("Member to check the profile")] DiscordMember member = null)
        {
            await ctx.TriggerTypingAsync();

            try
            {
                var user = member ?? ctx.Member;
                var profileService = Program.Container.Resolve<IProfileService>();
                var profile = await profileService.GetOrCreateProfileAsync(user.Id, user.Guild.Id);

                var embed = new DiscordEmbedBuilder
                {
                    Author = new DiscordEmbedBuilder.EmbedAuthor()
                    {
                        Name = user.Username,
                        IconUrl = user.AvatarUrl,
                    },
                    Description = $"**Profile <@{user.Id}>**\r\n" +
                                  $"CountTextMessage: {profile.CountTextMessage}\r\n" +
                                  $"CountMessageWithImage: {profile.CountMessageWithImage}\r\n" +
                                  $"CountMessageWithUrl: {profile.CountMessageWithUrl}",
                    Footer = new DiscordEmbedBuilder.EmbedFooter()
                    {
                        Text = $"Aliases: {profile.Alias}"
                    },
                    Color = new DiscordColor("#ff470f")
                };

                await ctx.Channel.SendMessageAsync(embed: embed);
            }
            catch (Exception ex)
            {
                var emoji = DiscordEmoji.FromName(ctx.Client, ":-1:");
                await ctx.RespondAsync(emoji);
            }
        }

        [RequireUserType(UserTypeCheckMode.MineOrParentAny, UserType.Moderator)]
        [Command("alias")]
        [Aliases("pa")]
        [Description("Set alias for Profile")]
        public async Task SetProfileAlias(CommandContext ctx, DiscordMember member, [RemainingText] [Description("Alias")] string alias)
        {
            await ctx.TriggerTypingAsync();

            try
            {
                var user = member;
                var profileService = Program.Container.Resolve<IProfileService>();
                var profile = await profileService.GetOrCreateProfileAsync(user.Id, user.Guild.Id);
                await profileService.UpdateProfileAsync(profile.Id, alias);
            }
            catch (Exception e)
            {
                var emoji = DiscordEmoji.FromName(ctx.Client, ":-1:");
                await ctx.RespondAsync($"{emoji} - {e.Message}");
            }
        }
    }
}
