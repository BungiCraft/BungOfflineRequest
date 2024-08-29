using System;
using Zenject;
using System.IO;
using Newtonsoft.Json;
using SiraUtil.Logging;
using CP_SDK.Chat;

namespace BungOfflineRequest
{
    public class BungRequestController : IInitializable
    {
        [Inject] private readonly SiraLog _log;
        private Requests requests;
        private string path;
        private string jsonPath;

        public BungRequestController(SiraLog log)
        {
            _log = log;
        }

        public void Initialize()
        {
            path = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), $".BungRequest/"));
            jsonPath = Path.Combine(path, "requests.json");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                _log.Info("Created directory \".BungRequest\" in local application data");
            }
            if (!File.Exists(jsonPath))
            {
                File.WriteAllText(jsonPath, "{\r\n    \"maps\": [\r\n    ]\r\n}");
                _log.Info("Created file \"requests.json\" in \".BungRequest\"");
            } 
            requests = JsonConvert.DeserializeObject<Requests>(File.ReadAllText(jsonPath));
            if (requests.maps.Length > 0)
            {
                Service.OnLoadingStateChanged += Service_OnLoadingStateChanged; // Returns false when loading is complete
            }
        }

        private void Service_OnLoadingStateChanged(bool state)
        {
            if (!state)
            {
                Service.BroadcastMessage("BungOfflineRequest: Requesting maps :3c");
                _log.Info("Requesting maps :3c");
                foreach (var map in requests.maps)
                {
                    Service.BroadcastMessage($"!bsr {map.bsr}");
                    _log.Info($"Requested map {map.bsr}");
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
