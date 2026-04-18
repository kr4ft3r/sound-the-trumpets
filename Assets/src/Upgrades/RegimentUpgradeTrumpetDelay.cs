using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegimentUpgradeTrumpetDelay : IRegimentUpgrade
{
    int level = 0;
    public void Apply(Regiment reg)
    {
        level++;
        reg.BonusTrumpetDelay = 10 * StackLevel();
    }

    public string GetDescription(bool future)
    {
        var lvl = StackLevel();
        if (future) lvl = StackLevel() + 1;

        return "Reduce signal delay by " + (10 * lvl) + "%";
    }

    public string GetId()
    {
        return "trumpet_delay";
    }

    public string GetName()
    {
        return "Quick Ears";
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
        return level;
    }
}
