using System.ComponentModel.DataAnnotations.Schema;

namespace YanOverseer.DAL.Models
{
    public class GuildSettings
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public ulong Id { get; set; }
        public string ModeratorRoleName { get; set; }
        public string AutoRoleName { get; set; }
        public ulong CreateMessageChannel { get; set; }
        public ulong UpdateMessageChannel { get; set; }
        public ulong DeleteMessageChannel { get; set; }

        public bool AutoRole { get; set; } = true;
        public bool AutoWelcomeMessage { get; set; } = true;
        public bool AutoLogCreateMessage { get; set; } = true;
        public bool AutoLogUpdateMessage { get; set; } = true;
        public bool AutoLogDeleteMessage { get; set; } = true;

    }
}