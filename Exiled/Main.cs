using System;
using System.Collections.Generic;
using System.Linq;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items;
using ExiledItem = Exiled.API.Features.Items.Item;

namespace GunHPPlugin
{
    public class GunHPPlugin : Plugin<Config>
    {
        public override string Author => "emielasda";
        public override string Name => "GunHP";
        public override string Prefix => "gunhp";
        public override Version Version => new Version(1, 0, 0);

        public static GunHPPlugin Instance { get; private set; }
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
            if (eventHandlers != null)
                eventHandlers.Unregister();

            eventHandlers = null;
            HPManager = null;
            Instance = null;
            base.OnDisabled();
        }
    }

    public class HPManager
    {
        private readonly Dictionary<ItemType, float> weaponDamages = new Dictionary<ItemType, float>();

        public void SetWeaponDamage(ItemType weaponType, float damage)
        {
            weaponDamages[weaponType] = damage;
        }

        public float GetWeaponDamage(ItemType weaponType)
        {
            return weaponDamages.TryGetValue(weaponType, out float damage) ? damage : -1f;
        }
    }

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

            if (customDamage > 0)
                ev.Amount = customDamage;
        }
    }

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class GunHPCommand : ICommand
    {
        public string Command => "gunhp";
        public string[] Aliases => Array.Empty<string>();
        public string Description => "Give a weapon with custom damage to a player";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count < 3)
            {
                response = "Usage: gunhp <player id> <weapon id> <damage>";
                return false;
            }

            string[] args = arguments.ToArray();

            if (!int.TryParse(args[0], out int playerId))
            {
                response = "Invalid player ID!";
                return false;
            }

            if (!int.TryParse(args[1], out int weaponId))
            {
                response = "Invalid weapon ID!";
                return false;
            }

            if (!float.TryParse(args[2], out float damage))
            {
                response = "Invalid damage value!";
                return false;
            }

            Player player = Player.Get(playerId);
            if (player == null)
            {
                response = "Player not found!";
                return false;
            }

            try
            {
                if (!Enum.IsDefined(typeof(ItemType), weaponId))
                {
                    response = "Invalid weapon ID!";
                    return false;
                }

                ItemType weaponType = (ItemType)weaponId;

                if (player.CurrentItem != null)
                    player.RemoveItem(player.CurrentItem);

                player.AddItem(weaponType);
                GunHPPlugin.Instance.HPManager.SetWeaponDamage(weaponType, damage);

                response = $"Given {weaponType} to {player.Nickname} with {damage} damage";
                return true;
            }
            catch (Exception ex)
            {
                response = $"Error: {ex.Message}";
                return false;
            }
        }
    }
}
