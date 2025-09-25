using Exiled.API.Interfaces;

namespace GunHPPlugin
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;
        public float DefaultGunDamage { get; set; } = 30f;
    }
}
