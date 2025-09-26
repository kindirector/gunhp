using System.Collections.Generic;
using Exiled.API.Enums;

namespace GunHPPlugin
{
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
}
