using CommandSystem;
using Exiled.API.Features;
using System;
using System.Linq;

namespace GunHPPlugin
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class GunHPCommand : ICommand
    {
        public string Command => "gunhp";
        public string[] Aliases => Array.Empty<string>();
        public string Description => "Give a weapon with custom damage (negative heals)";

        private static readonly int[] AllowedWeaponIds =
        {
            13, 20, 21, 23, 24, 30, 39, 40, 41, 47, 52, 53, 62
        };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count < 3)
            {
                response = "Usage: gunhp <player id/*> <item id> <damage>";
                return false;
            }

            var args = arguments.ToArray();

            string playerArg = args[0];

            if (!int.TryParse(args[1], out var weaponId) || !Enum.IsDefined(typeof(ItemType), weaponId))
            {
                response = "Invalid weapon ID!";
                return false;
            }

            if (!float.TryParse(args[2], out var damage))
            {
                response = "Invalid damage value!";
                return false;
            }

            if (!AllowedWeaponIds.Contains(weaponId))
            {
                response = $"This weapon ID is not allowed! Allowed: {string.Join(", ", AllowedWeaponIds)}";
                return false;
            }

            var weaponType = (ItemType)weaponId;

            if (playerArg == "*")
            {
                int playersCount = 0;
                foreach (var player in Player.List)
                {
                    if (player.IsAlive)
                    {
                        if (player.CurrentItem != null)
                            player.RemoveItem(player.CurrentItem);

                        player.AddItem(weaponType);
                        Main.Instance.HPManager.SetWeaponDamage(weaponType, damage);
                        playersCount++;
                    }
                }

                response = $"Given {weaponType} (ID {weaponId}) to {playersCount} players with {damage} damage";
                return true;
            }
            else if (int.TryParse(playerArg, out int playerId))
            {
                var player = Player.Get(playerId);
                if (player == null)
                {
                    response = "Player not found!";
                    return false;
                }

                if (!player.IsAlive)
                {
                    response = "Player is not alive!";
                    return false;
                }

                if (player.CurrentItem != null)
                    player.RemoveItem(player.CurrentItem);

                player.AddItem(weaponType);
                Main.Instance.HPManager.SetWeaponDamage(weaponType, damage);

                response = $"Given {weaponType} (ID {weaponId}) to {player.Nickname} with {damage} damage";
                return true;
            }
            else
            {
                response = "Invalid player ID! Use number or '*' for all players";
                return false;
            }
        }
    }
}