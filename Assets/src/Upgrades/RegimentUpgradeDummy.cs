using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegimentUpgradeDummy : IRegimentUpgrade
{
    int level = 0;
    public void Apply(Regiment reg)
    {
        level++;
    }

    public string GetDescription(bool future)
    {
        var lvl = StackLevel();
        if (future) lvl = StackLevel() + 1;

        return "Get " + (9999 - lvl).ToString() + " more to crash the game";
    }

    public string GetId()
    {
        return "dummy";
    }

    public string GetName()
    {
        return "Snuff Rations";
    }

    public bool IsStackable()
    {
        return true;
    }

    public int MaxStackLevel()
    {
        return 9999;
    }

    public int StackLevel()
    {
        return level;
    }
}
