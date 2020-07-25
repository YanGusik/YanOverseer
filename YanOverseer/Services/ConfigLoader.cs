using System;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using YanOverseer.Services.Interfaces;

namespace YanOverseer.Services
{
    public class ConfigLoader : IConfigLoader
    {
        private const string FileName = "config.json";

        public ConfigLoader()
        {
            if (!File.Exists(FileName)) File.Create(FileName).Close();
        }

        public void Save(Config config)
        {
            File.WriteAllText(FileName, JsonConvert.SerializeObject(config));
        }

        public Config Load()
        {
            try
            {
                var json = File.ReadAllText(FileName);
                var config = JsonConvert.DeserializeObject<Config>(json);
                if (config == null)
                    throw new Exception("File configution is empty");
                return config;
            }
            catch (FileNotFoundException e)
            {
                Debug.WriteLine($"File configutaion not found with name {e.FileName}");
                throw;
            }
        }
    }
}