using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Regiment
{
    public readonly int Order;
    public Color Color;
    
    Unit unit;
    Trumpet trumpet;
    public Regiment(int order, Color color)
    {
        Order = order;
        Color = color;
    }
    public string GetName()
    {
        string num = "th";
        if (Order == 1) num = "st";
        else if (Order == 2) num = "nd";
        else if (Order == 3) num = "rd";
        
        return Order+num+" Regiment";
    }
    public void Deploy(Unit unit,  Trumpet trumpet)
    {
        this.unit = unit;
        this.trumpet = trumpet;
        Battle.SignalSent += OnSignalSent;
        Debug.LogWarning("Remember to implement Undeploy");
    }
    public void UpdateBattleState(Battle battle)
    {
        // Update timers
        if (trumpet.EnableTimer > 0.00f)
        {
            trumpet.EnableTimer -= battle.battleInterval;
            if (trumpet.EnableTimer <= 0.00f)
            {
                trumpet.Ready();
            }
        }
        if (unit.NextStateTimer > 0.00f)
        {
            unit.NextStateTimer -= battle.battleInterval;
            if (unit.NextStateTimer <= 0.00f)
            {
                trumpet.Heard();
                unit.ToNextState();
            }
        }

        switch (unit.State) {
            case Unit.UnitState.Advancing:
                UpdateAdvancing();
                break;
            case Unit.UnitState.Fighting:
                UpdateFighting();
                break;
        }
    }
    void UpdateAdvancing()
    {
        //TODO
    }
    void UpdateFighting()
    {
        //TODO
    }
    public void Undeploy()
    {
        unit = null;
        trumpet = null;
        Battle.SignalSent -= OnSignalSent;
    }

    void OnSignalSent(Regiment regiment, Faction faction, bool isSpecial)
    {
        if (regiment != this) return;
        if (unit.State == Unit.UnitState.Fighting || unit.State == Unit.UnitState.Pursuing || unit.State == Unit.UnitState.Retreating) return;
        if (trumpet.State != Trumpet.TrumpetState.Ready) return; // TODO funny sound?
        Debug.Log("Signal for faction " + faction.ID + " regiment order " + regiment.Order);

        if (isSpecial)
        {
            Debug.LogWarning("Artillery not implemented");
            return;
        }

        switch (unit.State)
        {
            case Unit.UnitState.Holding:
                unit.NextState = Unit.UnitState.Advancing;
                // TODO sounds
                break;
            case Unit.UnitState.Advancing:
                unit.NextState = Unit.UnitState.Holding;
                break;
        }
        unit.NextStateTimer = FixedValues.BaseTrumpetDelay;
        trumpet.Blow(FixedValues.BaseTrumpetCooldown);//TODO better
    }
}
