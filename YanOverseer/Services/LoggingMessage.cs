using System;
using Autofac;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace YanOverseer.Services
{
    public class LoggingMessage : ILoggingMessage
    {
        private bool _isEnabled = true;
        private Config _config;

        public LoggingMessage(Config config)
        {
            _config = config;
        }

        public void Enable()
        {
            _isEnabled = true;
        }

        public void Disable()
        {
            _isEnabled = false;
        }

        public async void Log(MessageCreateEventArgs e, DiscordChannel channel)
        {
            if (!IsValid(e)) return;

            if (channel == null)
            {
                throw new ArgumentException("SecretChannel not found in Discord");
            }

            await channel.SendMessageAsync($"{e.Message.Author.Username} - {e.Message.Content}");
        }

        // TODO: Not working, because prev message not saving
        public async void Log(MessageUpdateEventArgs e, DiscordChannel channel)
        {
            if (!IsValid(e)) return;

            if (channel == null)
            {
                throw new ArgumentException("SecretChannel not found in Discord");
            }

            var embed = new DiscordEmbedBuilder
            {
                Author = new DiscordEmbedBuilder.EmbedAuthor()
                {
                    Name = e.Message.Author.Username,
                    IconUrl = e.Message.Author.AvatarUrl,
                },
                Description = $"**Message edited in <#{e.Channel.Id}>** [Jump to message](https://discordapp.com/channels/{e.Guild.Id}/{e.Channel.Id}/{e.Message.Id})\r\n" +
                              $"**Before**\r\n" +
                              $"{e.Message.Content} |not work functional|\r\n" +
                              $"**After**\r\n{e.Message.Content}",
                Footer = new DiscordEmbedBuilder.EmbedFooter()
                {
                    Text = $"User ID: {e.Message.Author.Id}"
                },
                Color = new DiscordColor("#117ea6")
            };

            await channel.SendMessageAsync(embed: embed);
        }

        public async void Log(MessageDeleteEventArgs e, DiscordChannel channel)
        {
            if (!IsValid(e)) return;

            if (channel == null)
            {
                throw new ArgumentException("SecretChannel not found in Discord");
            }

            var embed = new DiscordEmbedBuilder
            {
                Author = new DiscordEmbedBuilder.EmbedAuthor()
                {
                    Name = e.Message.Author.Username,
                    IconUrl = e.Message.Author.AvatarUrl,
                },
                Description = $"**Message sent by <@{e.Message.Author.Id}> deleted in <#{e.Channel.Id}>**\r\n"
                              + $"{e.Message.Content}",
                Footer = new DiscordEmbedBuilder.EmbedFooter()
                {
                    Text = $"Author: {e.Message.Author.Id} | Message ID: {e.Message.Id}"
                },
                Color = new DiscordColor("#ff470f")
            };

            await channel.SendMessageAsync(embed: embed);
        }

        public bool IsValid(MessageCreateEventArgs e)
        {
            if (e.Channel.Id == _config.SecretChatId) return false;
            if (e.Message.Content == null) return false;
            if (e.Message.Content.StartsWith(_config.CommandPrefix) ||
                _isEnabled == false || e.Message.Author.IsBot) return false;
            return true;
        }

        public bool IsValid(MessageUpdateEventArgs e)
        {
            if (e.Channel.Id == _config.SecretChatId) return false;
            if (e.Message.Content == null) return false;
            if (e.Message.Content.StartsWith(_config.CommandPrefix) ||
                _isEnabled == false || e.Message.Author.IsBot) return false;
            return true;
        }

        public bool IsValid(MessageDeleteEventArgs e)
        {
            if (e.Channel.Id == _config.SecretChatId) return false;
            if (e.Message.Content == null) return false;
            if (e.Message.Content.StartsWith(_config.CommandPrefix) ||
                _isEnabled == false || e.Message.Author.IsBot) return false;
            return true;
        }

    }
}