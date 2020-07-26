using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using YanOverseer.BLL.Interfaces;
using YanOverseer.Commands;
using YanOverseer.DAL.Models;
using YanOverseer.Services;
using YanOverseer.Services.Interfaces;

namespace YanOverseer
{
    public class Bot
    {
        private readonly Config _config;
        private readonly ILoggingMessage _loggingMessage;
        private readonly IProfileService _profileService;
        private readonly IMessageService _messageService;
        private readonly IGuildSettingsService _guildSettingsService;
        private readonly IStatisticsProfile _stasticsProfile;

        public Bot(Config config, ILoggingMessage loggingMessage, IProfileService profileService, IMessageService messageService, IGuildSettingsService guildSettingsService, IStatisticsProfile stasticsProfile)
        {
            _config = config;
            _loggingMessage = loggingMessage;
            _profileService = profileService;
            _messageService = messageService;
            _guildSettingsService = guildSettingsService;
            _stasticsProfile = stasticsProfile;
        }

        public DiscordClient Client { get; set; }
        public CommandsNextModule Commands { get; set; }

        public async Task RunBotAsync()
        {
            var cfg = new DiscordConfiguration
            {
                Token = _config.Token,
                TokenType = TokenType.Bot,

                AutoReconnect = true,
                LogLevel = LogLevel.Debug,
                UseInternalLogHandler = true

            };

            Client = new DiscordClient(cfg);
            Client.Ready += Client_Ready;
            Client.GuildAvailable += Client_GuildAvailable;
            Client.ClientErrored += Client_ClientError;
            Client.MessageCreated += Client_MessageCreated;
            Client.MessageUpdated += Client_MessageUpdated;
            Client.MessageDeleted += Client_MessageDeleted;
            Client.GuildMemberAdded += Client_GuildMemberAdded;

            var ccfg = new CommandsNextConfiguration
            {
                StringPrefix = _config.CommandPrefix,
                EnableDms = true,
                EnableMentionPrefix = true,


            };

            Commands = Client.UseCommandsNext(ccfg);
            Commands.CommandExecuted += Commands_CommandExecuted;
            Commands.CommandErrored += Commands_CommandErrored;

            Commands.RegisterCommands<AdminCommands>();
            Commands.RegisterCommands<ModeratorCommands>();
            Commands.RegisterCommands<ProfileCommands>();

            Client.UseInteractivity(new InteractivityConfiguration()
            {
                Timeout = TimeSpan.FromMinutes(2)
            });

            await Client.ConnectAsync();

            await Task.Delay(-1);
        }

        #region Events

        private Task Client_Ready(ReadyEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "YanOverseer", "Client is ready to process events.", DateTime.Now);
            return Task.CompletedTask;
        }

        private async Task Client_GuildMemberAdded(GuildMemberAddEventArgs e)
        {
            var guildSettings = await _guildSettingsService.GetGuildSettingsByIdAsync(e.Guild.Id);

            if (guildSettings == null) return;

            if (guildSettings.AutoWelcomeMessage)
            {
                await SendWelcomeMessage(e.Member, GetWelcomeMessagEmbed(e.Member, e.Guild));
            }

            if (guildSettings.AutoRole)
            {
                await GrantDefaultRole(e.Member, guildSettings.AutoRoleName);
            }
        }

        private async Task Client_MessageCreated(MessageCreateEventArgs e)
        {
            var guildSettings = await _guildSettingsService.GetGuildSettingsByIdAsync(e.Guild.Id);

            if (guildSettings == null) return;

            var profile = await _profileService.GetOrCreateProfileAsync(e.Author.Id, e.Guild.Id);

            // Add message to BD
            await _messageService.CreateNewMessageAsync(e.Message.Id, e.Message.Content, e.Message.Timestamp, profile.Id);
            // Change statistics for Profile
            await _stasticsProfile.ChangeProfileStatisticsAsync(profile.Id, e.Message);

            // Log
            if (guildSettings.AutoLogCreateMessage)
            {
                var channel = await Client.GetChannelAsync(guildSettings.CreateMessageChannel);
                if (e.Message.Author.IsBot) return;
                if (e.Channel == channel) return;
                if (e.Message.Content.StartsWith(_config.CommandPrefix)) return;
                await _loggingMessage.CreateAsync(channel, e.Message);
            }
        }

        private async Task Client_MessageDeleted(MessageDeleteEventArgs e)
        {
            var guildSettings = await _guildSettingsService.GetGuildSettingsByIdAsync(e.Guild.Id);

            if (guildSettings == null) return;

            // Delete message from BD
            await _messageService.DeleteMessageAsync(e.Message.Id);

            // Log
            if (guildSettings.AutoLogUpdateMessage)
            {
                var channel = await Client.GetChannelAsync(guildSettings.CreateMessageChannel);
                if (e.Message.Author.IsBot) return;
                if (e.Channel == channel) return;
                if (e.Message.Content.StartsWith(_config.CommandPrefix)) return;
                await _loggingMessage.DeleteAsync(channel, e.Message);
            }
        }

        private async Task Client_MessageUpdated(MessageUpdateEventArgs e)
        {
            var guildSettings = await _guildSettingsService.GetGuildSettingsByIdAsync(e.Guild.Id);

            if (guildSettings == null) return;

            // Update message from BD
            await _messageService.UpdateMessageAsync(e.Message.Id, e.Message.Content);

            // Log
            if (guildSettings.AutoLogUpdateMessage)
            {
                var channel = await Client.GetChannelAsync(guildSettings.CreateMessageChannel);
                if (e.Message.Author.IsBot) return;
                if (e.Channel == channel) return;
                if (e.Message.Content.StartsWith(_config.CommandPrefix)) return;
                var prevMessage = await _messageService.GetMessageByIdAsync(e.Message.Id);
                await _loggingMessage.UpdateAsync(channel, e.Message, prevMessage.Content);
            }
        }

        private Task Client_GuildAvailable(GuildCreateEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "YanOverseer", $"Guild available: {e.Guild.Name}", DateTime.Now);

            return Task.CompletedTask;
        }

        private Task Client_ClientError(ClientErrorEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Error, "YanOverseer", $"Exception occured: {e.Exception.GetType()}: {e.Exception.Message}", DateTime.Now);

            return Task.CompletedTask;
        }

        private Task Commands_CommandExecuted(CommandExecutionEventArgs e)
        {
            e.Context.Client.DebugLogger.LogMessage(LogLevel.Info, "YanOverseer", $"{e.Context.User.Username} successfully executed '{e.Command.QualifiedName}'", DateTime.Now);

            return Task.CompletedTask;
        }

        private async Task Commands_CommandErrored(CommandErrorEventArgs e)
        {
            e.Context.Client.DebugLogger.LogMessage(LogLevel.Error, "YanOverseer",
                $"{e.Context.User.Username} tried executing '{e.Command?.QualifiedName ?? "<unknown command>"}' but it errored: {e.Exception.GetType()}: {e.Exception.Message ?? "<no message>"}",
                DateTime.Now);

            if (e.Exception is ChecksFailedException ex)
            {
                var emoji = DiscordEmoji.FromName(e.Context.Client, ":no_entry:");

                var embed = new DiscordEmbedBuilder
                {
                    Title = "Access denied",
                    Description = $"{emoji} You do not have the permissions required to execute this command.",
                    Color = new DiscordColor(0xFF0000)
                };
                await e.Context.RespondAsync("", embed: embed);
            }
        }

        #endregion

        #region Method

        public DiscordEmbed GetWelcomeMessagEmbed(DiscordMember member, DiscordGuild guild)
        {
            return new DiscordEmbedBuilder
            {
                Description = $"Hey <@{member.Id}>, welcome to **{guild.Name}** Discord Server!\r\n\r\n" +
                              $"**You are the {guild.MemberCount}th member :tada: **\r\n\r\n" +
                              $"Введи себя там хорошо и будь паянькой :3 \r\n" +
                              $"Вот тебе за прочитанное котика",
                ImageUrl = "https://sun9-39.userapi.com/c639830/v639830979/45459/AvphDx2dNLQ.jpg",
                Color = DiscordColor.Goldenrod
            };
        }

        private async Task GrantDefaultRole(DiscordMember member, string roleName)
        {
            var role = member.Guild.Roles.FirstOrDefault(discordRole => discordRole.Name == roleName);
            if (role != null) await member.GrantRoleAsync(member.Guild.GetRole(role.Id));
        }

        private async Task SendWelcomeMessage(DiscordMember member, DiscordEmbed embed)
        {
            var channel = await member.CreateDmChannelAsync();
            await channel.SendMessageAsync(embed: embed);
        }

        #endregion
    }
}