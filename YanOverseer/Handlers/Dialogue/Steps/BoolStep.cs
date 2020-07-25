using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;

namespace YanOverseer.Handlers.Dialogue.Steps
{
    public class BoolStep : DialogueStepBase
    {

        private IDialogueStep _nextStep;

        public BoolStep(string content, IDialogueStep nextStep) : base(content)
        {
            _nextStep = nextStep;
        }

        public Action<bool> OnValidResult { get; set; } = delegate { };

        public override IDialogueStep NextStep => _nextStep;

        public void SetNextStep(IDialogueStep nextStep)
        {
            _nextStep = nextStep;
        }

        public override async Task<bool> ProcessStep(DiscordClient client, DiscordChannel channel, DiscordUser user)
        {
            var embedBuilder = new DiscordEmbedBuilder
            {
                Title = $"Please Respond Below",
                Description = $"{user.Mention}, {_content}",
            };

            embedBuilder.AddField("To Stop The Dialogue", "Use the ?cancel command");

            var interactivity = client.GetInteractivityModule();

            while (true)
            {
                var embed = await channel.SendMessageAsync(embed: embedBuilder).ConfigureAwait(false);

                OnMessageAdded(embed);

                var messageResult = await interactivity.WaitForMessageAsync(
                    x => x.ChannelId == channel.Id && x.Author.Id == user.Id).ConfigureAwait(false);

                OnMessageAdded(messageResult.Message);

                if (messageResult.Message.Content.Equals("?cancel", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }


                if (messageResult.Message.Content.Contains("true") || messageResult.Message.Content.Contains("false") ||
                    messageResult.Message.Content.Contains("1") || messageResult.Message.Content.Contains("0"))
                {
                    if (bool.TryParse(messageResult.Message.Content, out bool result))
                    {
                        OnValidResult(result);
                    }
                    
                    return false;
                }

                await TryAgain(channel, $"Your input is not a [true,false,1,0]").ConfigureAwait(false);
            }
        }
    }
}