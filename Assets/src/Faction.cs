using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Faction
{
    public enum FactionSide {  Left, Right }

    public int ID;
    public FactionSide Side;
    public Regiment[] Regiments;
    public bool IsHumanPlayer = false;
    public Faction(int id, FactionSide side)
    {
        ID = id;
        this.Side = side;

        if (id == 0) IsHumanPlayer = true; // TODO don't force here

        InitRegiments();
    }
    protected void InitRegiments()
    {
        Regiments = new Regiment[FixedValues.RegimentsPerFaction];
        for (int i = 0; i < FixedValues.RegimentsPerFaction; i++) { 
            var reg = new Regiment(i + 1, FixedValues.GetFactionColor(ID), Side == FactionSide.Left ? 1f : -1f);
            Regiments[i] = reg;
        }
    }
    public void UpdateBattleState()
    {

    }
}
