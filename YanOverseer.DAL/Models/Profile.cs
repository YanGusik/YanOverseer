namespace YanOverseer.DAL.Models
{
    public class Profile
    {
        public ulong Id { get; set; }
        public int CountTextMessage { get; set; }
        public int CountMessageWithImage { get; set; }
        public int CountMessageWithUrl { get; set; }

        public string Alias
        {
            get
            {
                if (CountTextMessage > 1000)
                    return "Активист";
                if (CountMessageWithUrl > 20)
                    return "Новостной репортер";
                if (CountMessageWithImage > 100)
                    return "Журналист";
                return "Новичек";
            }
        }
    }
}