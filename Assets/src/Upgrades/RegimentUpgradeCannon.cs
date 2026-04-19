using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegimentUpgradeCannon : IRegimentUpgrade
{
    public void Apply(Regiment reg)
    {
        reg.HasCannon = true;
    }

    public string GetDescription(bool future)
    {
        return "Hold Shift and press regiment's key to fire. Only before engagement. Most damage against stationary. Random delay.";
    }

    public string GetId()
    {
        return "cannon";
    }

    public string GetName()
    {
        return "Artillery Detachment";
    }

    public bool IsStackable()
    {
        return false;
    }

    public int MaxStackLevel()
    {
        return 0;
    }

    public int StackLevel()
    {
        return 0;
    }
}
