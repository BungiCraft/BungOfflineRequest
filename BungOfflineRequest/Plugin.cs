using IPA;
using SiraUtil.Zenject;
using IPALogger = IPA.Logging.Logger;

namespace BungOfflineRequest
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        private static IPALogger Logger;

        [Init]
        public void Init(IPALogger logger, Zenjector zenjector)
        {
            Logger = logger;
            
            zenjector.UseLogger(Logger);
            
            zenjector.Install(Location.Menu, container =>
            {
                container.BindInterfacesAndSelfTo<BungRequestController>().AsSingle().NonLazy();
             });
        }
    }
}
