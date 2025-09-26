using Exiled.Events.EventArgs.Player;
using InventorySystem.Items;

namespace GunHPPlugin
{
    public class EventHandlers
    {
        private readonly HPManager hpManager;

        public EventHandlers(HPManager manager)
        {
            hpManager = manager;
        }

        public void Register()
        {
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
        }

        public void Unregister()
        {
            Exiled.Events.Handlers.Player.Hurting -= OnHurting;
        }

        private void OnHurting(HurtingEventArgs ev)
        {
            if (ev.Attacker == null || ev.Attacker.CurrentItem == null)
                return;

            ItemType weaponType = ev.Attacker.CurrentItem.Type;
            float customDamage = hpManager.GetWeaponDamage(weaponType);

            if (customDamage == -1f)
                return;

            if (customDamage > 0)
                ev.Amount = customDamage;
            else if (customDamage < 0)
            {
                ev.IsAllowed = false;
                ev.Player.Heal(-customDamage);
            }
        }
    }
}
