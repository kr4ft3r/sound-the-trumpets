using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegimentUpgradeTrumpetCooldown : IRegimentUpgrade
{
    public void Apply(Regiment reg)
    {
        reg.BonusTrumpetCooldown = 10 * StackLevel();
    }

    public string GetDescription(bool future)
    {
        var lvl = StackLevel();
        if (future) lvl = StackLevel() + 1;

        return "Reduce trumpet cooldown by "+(10 * lvl)+"%";
    }

    public string GetId()
    {
        return "trumpet_cooldown";
    }

    public string GetName()
    {
        return "Trumpeteer's Lungs";
    }

    public bool IsStackable()
    {
        return true;
    }

    public int MaxStackLevel()
    {
        return 9;
    }

    public int StackLevel()
    {
        return 0;
    }
}
