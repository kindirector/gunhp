using Exiled.API.Features;
using System;

namespace GunHPPlugin
{
    public class Main : Plugin<Config>
    {
        public override string Author => "emielasda";
        public override string Name => "GunHP";
        public override string Prefix => "gunhp";
        public override Version Version => new Version(1, 1, 0);

        public static Main Instance;
        public HPManager HPManager { get; private set; }
        private EventHandlers eventHandlers;

        public override void OnEnabled()
        {
            Instance = this;
            HPManager = new HPManager();
            eventHandlers = new EventHandlers(HPManager);
            eventHandlers.Register();
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            eventHandlers.Unregister();
            eventHandlers = null;
            HPManager = null;
            Instance = null;
            base.OnDisabled();
        }
    }
}
