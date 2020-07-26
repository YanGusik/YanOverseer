using System;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using YanOverseer.BLL.Interfaces;
using YanOverseer.Services.Interfaces;

namespace YanOverseer.Services
{
    public class LoggingMessage : ILoggingMessage
    {
        public async Task CreateAsync(DiscordChannel channel, DiscordMessage message)
        {
            var embed = new DiscordEmbedBuilder
            {
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    Name = message.Author.Username,
                    IconUrl = message.Author.AvatarUrl
                },
                Description = $"**Message sent by <@{message.Author.Id}> deleted in <#{message.Channel.Id}>**\r\n"
                              + $"{message.Content}",
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = $"Author: {message.Author.Id} | Message ID: {message.Id}"
                },
                Color = DiscordColor.Purple
            };

            await channel.SendMessageAsync(embed: embed);
        }

        public async Task UpdateAsync(DiscordChannel channel, DiscordMessage message, string prevMessage)
        {
            var embed = new DiscordEmbedBuilder
            {
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    Name = message.Author.Username,
                    IconUrl = message.Author.AvatarUrl
                },
                Description =
                            $"**Message edited in <#{message.Channel.Id}>** [Jump to message](https://discordapp.com/channels/{channel.Guild.Id}/{message.Channel.Id}/{message.Id})\r\n" +
                            "**Before**\r\n" +
                            $"{prevMessage}\r\n" +
                            $"**After**\r\n{message.Content}",
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = $"User ID: {message.Author.Id}"
                },
                Color = new DiscordColor("#117ea6")
            };

            await channel.SendMessageAsync(embed: embed);
        }

        public async Task DeleteAsync(DiscordChannel channel, DiscordMessage message)
        {
            var embed = new DiscordEmbedBuilder
            {
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    Name = message.Author.Username,
                    IconUrl = message.Author.AvatarUrl
                },
                Description = $"**Message sent by <@{message.Author.Id}> deleted in <#{message.Channel.Id}>**\r\n"
                              + $"{message.Content}",
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = $"Author: {message.Author.Id} | Message ID: {message.Id}"
                },
                Color = new DiscordColor("#ff470f")
            };

            await channel.SendMessageAsync(embed: embed);
        }
    }

    // TODO: Delete Trash Unuseful Code
    //public class LoggingMessage : ILoggingMessage
    //{
    //    private readonly Config _config;
    //    private readonly IMessageService _messageService;
    //    private bool _isEnabled = true;

    //    public LoggingMessage(Config config, IMessageService messageService)
    //    {
    //        _config = config;
    //        _messageService = messageService;
    //    }

    //    public void Enable()
    //    {
    //        _isEnabled = true;
    //    }

    //    public void Disable()
    //    {
    //        _isEnabled = false;
    //    }

    //    public async void Log(MessageCreateEventArgs e, DiscordChannel channel)
    //    {
    //        if (!IsValid(e)) return;

    //        if (channel == null) throw new ArgumentException("SecretChannel not found in Discord");

    //        await channel.SendMessageAsync($"{e.Message.Author.Username} - {e.Message.Content}");
    //    }

    //    // TODO: It is necessary to reconsider the responsibility of this class.
    //    // At the moment it logs messages, finds previous messages and uses the Logical Layer (Core), refactoring is necessary
    //    public async void Log(MessageUpdateEventArgs e, DiscordChannel channel)
    //    {
    //        if (!IsValid(e)) return;

    //        if (channel == null) throw new ArgumentException("SecretChannel not found in Discord");

    //        var prevMessage = await _messageService.GetMessageByIdAsync(e.Message.Id);

    //        if (prevMessage == null)
    //            return;

    //        var embed = new DiscordEmbedBuilder
    //        {
    //            Author = new DiscordEmbedBuilder.EmbedAuthor
    //            {
    //                Name = e.Message.Author.Username,
    //                IconUrl = e.Message.Author.AvatarUrl
    //            },
    //            Description =
    //                $"**Message edited in <#{e.Channel.Id}>** [Jump to message](https://discordapp.com/channels/{e.Guild.Id}/{e.Channel.Id}/{e.Message.Id})\r\n" +
    //                "**Before**\r\n" +
    //                $"{prevMessage.Content}\r\n" +
    //                $"**After**\r\n{e.Message.Content}",
    //            Footer = new DiscordEmbedBuilder.EmbedFooter
    //            {
    //                Text = $"User ID: {e.Message.Author.Id}"
    //            },
    //            Color = new DiscordColor("#117ea6")
    //        };

    //        await _messageService.UpdateMessageAsync(prevMessage.Id, e.Message.Content);

    //        await channel.SendMessageAsync(embed: embed);
    //    }

    //    public async void Log(MessageDeleteEventArgs e, DiscordChannel channel)
    //    {
    //        if (!IsValid(e)) return;

    //        if (channel == null) throw new ArgumentException("SecretChannel not found in Discord");

    //        var embed = new DiscordEmbedBuilder
    //        {
    //            Author = new DiscordEmbedBuilder.EmbedAuthor
    //            {
    //                Name = e.Message.Author.Username,
    //                IconUrl = e.Message.Author.AvatarUrl
    //            },
    //            Description = $"**Message sent by <@{e.Message.Author.Id}> deleted in <#{e.Channel.Id}>**\r\n"
    //                          + $"{e.Message.Content}",
    //            Footer = new DiscordEmbedBuilder.EmbedFooter
    //            {
    //                Text = $"Author: {e.Message.Author.Id} | Message ID: {e.Message.Id}"
    //            },
    //            Color = new DiscordColor("#ff470f")
    //        };

    //        await channel.SendMessageAsync(embed: embed);
    //    }

    //    public bool IsValid(MessageCreateEventArgs e)
    //    {
    //        if (e.Channel.Id == _config.SecretChatId) return false;
    //        if (e.Message.Content == null) return false;
    //        if (e.Message.Content.StartsWith(_config.CommandPrefix) || e.Message.Content.StartsWith(".") ||
    //            _isEnabled == false || e.Message.Author.IsBot) return false;
    //        return true;
    //    }

    //    public bool IsValid(MessageUpdateEventArgs e)
    //    {
    //        if (e.Channel.Id == _config.SecretChatId) return false;
    //        if (e.Message.Content == null) return false;
    //        if (_isEnabled == false || e.Message.Author.IsBot) return false;
    //        return true;
    //    }

    //    public bool IsValid(MessageDeleteEventArgs e)
    //    {
    //        if (e.Channel.Id == _config.SecretChatId) return false;
    //        if (e.Message.Content == null) return false;
    //        if (_isEnabled == false || e.Message.Author.IsBot) return false;
    //        return true;
    //    }
    //}
}