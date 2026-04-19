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
    public float TimeUntilComputerDecision = 0;
    public string LeaderName;
    public Faction(int id, FactionSide side, string leaderName)
    {
        ID = id;
        this.Side = side;

        //IsHumanPlayer = false;
        if (id == 0) IsHumanPlayer = true; // TODO don't force here
        LeaderName = leaderName;

        InitRegiments();
    }
    protected void InitRegiments()
    {
        Regiments = new Regiment[FixedValues.RegimentsPerFaction];
        for (int i = 0; i < FixedValues.RegimentsPerFaction; i++) { 
            var reg = new Regiment(i + 1, FixedValues.GetFactionColor(ID), Side == FactionSide.Left ? 1f : -1f, LeaderName+"'s ");
            Regiments[i] = reg;
            //TODO remove test
            //if (i==3) reg.AssignRandomUpgrade();
        }

        TimeUntilComputerDecision = Random.Range(1f, 10f);
    }
    public void UpdateBattleState()
    {

    }

    public List<(Regiment,bool)> GetPossibleTrumpetSignals()
    {
        var possible = new List<(Regiment,bool)>();
        foreach(var reg in Regiments)
        {
            if (reg.TrumpetReady())
            {
                possible.Add((reg,false));
            }
        }

        return possible;
    }
}
