using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRegimentUpgrade
{
    public string GetId();
    public string GetName();
    public string GetDescription(bool future);
    public bool IsStackable();
    public int MaxStackLevel();
    public void Apply(Regiment reg);

    public int StackLevel();
}
