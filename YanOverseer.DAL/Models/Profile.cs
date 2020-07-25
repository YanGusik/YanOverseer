namespace YanOverseer.DAL.Models
{
    public class Profile
    {
        public ulong Id { get; set; }
        public ulong DiscordId { get; set; }
        public ulong GuildId { get; set; }
        public int CountTextMessage { get; set; }
        public int CountMessageWithImage { get; set; }
        public int CountMessageWithUrl { get; set; }

        public string Alias { get; set; }
    }
}