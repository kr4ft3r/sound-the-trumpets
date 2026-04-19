using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager
{
    public static List<IRegimentUpgrade> GetPossibleUpgradesForRegiment(Regiment reg)
    {
        var possibleUpgrades = new List<IRegimentUpgrade>();
        var upgradeTypes = GetRegimentUpgradeTypes();
        for (int i = upgradeTypes.Count - 1; i >= 0; i--) {
            
            var upgradeType = upgradeTypes[i];
            Debug.Log("Cheaking " + upgradeType.GetName());
            bool foundActive = false;
            foreach(var activeUpgrade in reg.Upgrades)
            {
                Debug.Log("Checking " + activeUpgrade.GetId() + ":" +upgradeType.GetId());
                if (activeUpgrade.GetId() == upgradeType.GetId())
                {
                    Debug.Log("Found active");
                    foundActive = true;
                    if (!activeUpgrade.IsStackable() || (activeUpgrade.IsStackable() && activeUpgrade.StackLevel() == activeUpgrade.MaxStackLevel()))
                    {
                        Debug.Log("Skipping " + activeUpgrade.GetName());
                        continue;
                    }
                    Debug.Log("Addin another "+activeUpgrade.GetName());
                    possibleUpgrades.Add(activeUpgrade);
                    continue;
                }
            }
            if (!foundActive)
            {
                Debug.Log("Did not found active "+upgradeType.GetName());
                possibleUpgrades.Add(upgradeType);
                continue;
            }
        }

        return possibleUpgrades;
    }
    public static List<IRegimentUpgrade> GetRegimentUpgradeTypes()
    {
        return new List<IRegimentUpgrade> {
            new RegimentUpgradeTrumpetCooldown(),
            new RegimentUpgradeTrumpetDelay(),
            new RegimentUpgradeCannon(),
            new RegimentUpgradeDummy(),
        };
    }
}
