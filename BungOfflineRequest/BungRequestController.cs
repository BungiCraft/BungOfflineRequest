using System;
using Zenject;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SiraUtil.Logging;
using CP_SDK.Chat;

namespace BungOfflineRequest
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class BungRequestController : IInitializable
    {
        [Inject] private readonly SiraLog _log;
        private Requests _requests;
        private string _path;
        private string _jsonPath;

        public BungRequestController(SiraLog log)
        {
            _log = log;
        }

        public void Initialize()
        {
            _path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), $".BungRequest/");
            _jsonPath = Path.Combine(_path, "requests.json");
            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
                _log.Info("Created directory \".BungRequest\" in local application data");
            }
            if (!File.Exists(_jsonPath))
            {
                File.WriteAllText(_jsonPath, "{\r\n    \"maps\": [\r\n    ]\r\n}");
                _log.Info("Created file \"requests.json\" in \".BungRequest\"");
            } 
            _requests = JsonConvert.DeserializeObject<Requests>(File.ReadAllText(_jsonPath));
            if (_requests.Maps.Length > 0)
            {
                Service.OnLoadingStateChanged += Service_OnLoadingStateChanged; // Returns false when loading is complete
            }
        }

        private async void Service_OnLoadingStateChanged(bool state)
        {
            try
            {
                if (state) return;
                Service.BroadcastMessage("BungOfflineRequest: Requesting maps :3c");
                _log.Info("Requesting maps :3c");
                await Task.Delay(5000); // Add 5-second delay to ensure that bs+ has loaded mod perms or smth
                foreach (var map in _requests.Maps)
                {
                    Service.BroadcastMessage($"!modadd {map.Bsr}");
                    _log.Info($"Requested map {map.Bsr}");
                    await Task.Delay(500); // Add .5-second delay between requests to make them look less "spammy"
                }
                File.WriteAllText(_jsonPath, "{\r\n    \"maps\": [\r\n    ]\r\n}");
            }
            catch (Exception e)
            {
                _log.Error(e);
            }
        }
    }

    public class Requests
    {
        public readonly Map[] Maps;

        public Requests(Map[] maps)
        {
            Maps = maps;
        }
    }

    // ReSharper disable once ClassNeverInstantiated.Global
    public class Map
    {
        public readonly string Name;
        public readonly string Bsr;

        public Map(string name, string bsr)
        {
            Name = name;
            Bsr = bsr;
        }
    }
}
