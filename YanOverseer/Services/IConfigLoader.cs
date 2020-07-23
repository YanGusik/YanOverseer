namespace YanOverseer.Services
{
    public interface IConfigLoader
    {
        void Save(Config config);
        Config Load();
    }
}