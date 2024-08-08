using System;
using Zenject;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using SiraUtil.Logging;

namespace BungOfflineRequest
{
    public class BungRequestController : IInitializable
    {
        [Inject] private readonly SiraLog _log;

        public BungRequestController(SiraLog log)
        {
            _log = log;
        }

        public async void Initialize()
        {
            string path = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), $".BungRequest/"));
            string jsonPath = Path.Combine(path, "requests.json");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                _log.Info("Created directory .BungRequest in local application data");
            }
            if (!File.Exists(jsonPath))
            {
                File.WriteAllText(jsonPath, "{\r\n    \"maps\": [\r\n    ]\r\n}");
                _log.Info("Created file requests.json in .BungRequest");
            } 
            Requests requests = JsonConvert.DeserializeObject<Requests>(File.ReadAllText(jsonPath));
            await Task.Delay(10000); // Suuuuuurely best way to run after bs+ does things yesyes :3
            if (requests.maps.Length > 0)
            {
                foreach (var map in requests.maps)
                {
                    CP_SDK.Chat.Service.BroadcastMessage($"!bsr {map.bsr}");
                    _log.Info($"Requested map {map.bsr}");
                    await Task.Delay(200);
                }
                File.WriteAllText(jsonPath, "{\r\n    \"maps\": [\r\n    ]\r\n}");
            }
        }
    }

    public class Requests
    {
        public Map[] maps;

        public Requests(Map[] maps)
        {
            this.maps = maps;
        }
    }

    public class Map
    {
        public string name;
        public string bsr;

        public Map(string name, string bsr)
        {
            this.name = name;
            this.bsr = bsr;
        }
    }
}
