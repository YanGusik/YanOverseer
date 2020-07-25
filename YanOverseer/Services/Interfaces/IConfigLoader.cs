namespace YanOverseer.Services.Interfaces
{
    public interface IConfigLoader
    {
        void Save(Config config);
        Config Load();
    }
}