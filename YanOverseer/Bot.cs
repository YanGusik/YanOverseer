using System;
using System.Threading.Tasks;
using Autofac;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using YanOverseer.Commands;
using YanOverseer.Services;

namespace YanOverseer
{
    public class Bot
    {
        private readonly Config _config;
        private readonly ILoggingMessage _loggingMessage;
        private DiscordChannel _secretChannel;

        public Bot(Config config, ILoggingMessage loggingMessage)
        {
            _config = config;
            _loggingMessage = loggingMessage;
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
                EnableMentionPrefix = true
            };

            Commands = Client.UseCommandsNext(ccfg);
            Commands.CommandExecuted += Commands_CommandExecuted;
            Commands.CommandErrored += Commands_CommandErrored;

            Commands.RegisterCommands<AdminCommands>();
            Commands.RegisterCommands<ModeratorCommands>();


            await Client.ConnectAsync();


            _secretChannel = await Client.GetChannelAsync(_config.SecretChatId);

            await Task.Delay(-1);
        }

        private Task Client_Ready(ReadyEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "YanOverseer", "Client is ready to process events.", DateTime.Now);
            return Task.CompletedTask;
        }

        private async Task Client_GuildMemberAdded(GuildMemberAddEventArgs e)
        {
            var channel = await e.Member.CreateDmChannelAsync();
            var embed = new DiscordEmbedBuilder
            {
                Description = $"Hey <@{e.Member.Id}>, welcome to **{e.Guild.Name}** Discord Server!\r\n\r\n" +
                              $"**You are the {e.Guild.MemberCount}th member :tada: **\r\n\r\n" +
                              $"Введи себя там хорошо и будь паянькой :3 \r\n" +
                              $"Вот тебе за прочитанное котика",
                ImageUrl = "https://sun9-39.userapi.com/c639830/v639830979/45459/AvphDx2dNLQ.jpg",
                Color = DiscordColor.Goldenrod
            };

            await channel.SendMessageAsync(embed: embed);


            await e.Member.GrantRoleAsync(e.Guild.GetRole(718102202081869945));
        }

        private Task Client_MessageCreated(MessageCreateEventArgs e)
        {
            _loggingMessage.Log(e, _secretChannel);
            return Task.CompletedTask;
        }

        private Task Client_MessageDeleted(MessageDeleteEventArgs e)
        {
            _loggingMessage.Log(e, _secretChannel);
            return Task.CompletedTask;
        }

        private Task Client_MessageUpdated(MessageUpdateEventArgs e)
        {
            _loggingMessage.Log(e, _secretChannel);
            return Task.CompletedTask;
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
    }
}