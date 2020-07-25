using System.ComponentModel.DataAnnotations.Schema;

namespace YanOverseer.DAL.Models
{
    public class ServerSettings
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public ulong Id { get; set; }

        public string ModeratorRoleName { get; set; }
        public string AutoRoleName { get; set; }

        public bool AutoRole { get; set; } = true;
        public bool AutoWelcomeMessage { get; set; } = true;

    }
}