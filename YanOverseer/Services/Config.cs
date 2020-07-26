using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace YanOverseer.Services
{
    public class Config
    {
        [JsonProperty("token")] public string Token { get; private set; }

        [JsonProperty("prefix")] public string CommandPrefix { get; private set; }
    }
}