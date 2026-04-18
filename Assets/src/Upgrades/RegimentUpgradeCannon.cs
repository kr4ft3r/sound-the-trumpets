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
        return "Hold L or R (depending on faction) Shift while pressing regiment's number to fire the cannon. Only possible before engagement and does most damage against standing units.";
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
