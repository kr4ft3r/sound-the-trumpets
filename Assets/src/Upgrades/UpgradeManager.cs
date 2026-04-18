using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager
{
    public static List<IRegimentUpgrade> GetPossibleUpgradesForRegiment(Regiment reg)
    {
        var possibleUpgrades = new List<IRegimentUpgrade>();
        var upgradeTypes = GetRegimentUpgradeTypes();
        for (int i = upgradeTypes.Count; i >= 0; i--) {
            var upgradeType = upgradeTypes[i];
            bool foundActive = false;
            foreach(var activeUpgrade in reg.Upgrades)
            {
                if (activeUpgrade.GetId() == upgradeType.GetId())
                {
                    foundActive = true;
                    if (!activeUpgrade.IsStackable() || activeUpgrade.StackLevel() == activeUpgrade.MaxStackLevel())
                    {
                        continue;
                    }
                    possibleUpgrades.Add(activeUpgrade);
                }
                if (!foundActive)
                {
                    possibleUpgrades.Add(upgradeType);
                    continue;
                }
            }
        }

        return possibleUpgrades;
    }
    public static List<IRegimentUpgrade> GetRegimentUpgradeTypes()
    {
        return new List<IRegimentUpgrade> {
            new RegimentUpgradeTrumpetCooldown()
        };
    }
}
