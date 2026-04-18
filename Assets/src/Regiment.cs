using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Regiment
{
    public readonly int Order;
    public Color Color;
    float advanceDirection;
    
    Unit unit;
    Trumpet trumpet;
    float startX;
    float speed = FixedValues.BaseUnitSpeed;
    public Regiment(int order, Color color, float advanceDirection)
    {
        Order = order;
        Color = color;
        this.advanceDirection = advanceDirection;
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
        startX = unit.transform.position.x;
        Debug.LogWarning("Remember to implement Undeploy");
    }
    public void Undeploy()
    {
        unit = null;
        trumpet = null;
        Battle.SignalSent -= OnSignalSent;
    }
    public void Engage()
    {
        unit.NextState = Unit.UnitState.Fighting;
        unit.ToNextState();
    }
    public bool IsEngaged()
    {
        return unit.State == Unit.UnitState.Fighting;
    }
    public float GetGroundTaken()
    {
        return unit.groundTaken;
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

        switch (unit.State)
        {
            case Unit.UnitState.Advancing:
                UpdateAdvancing(battle);
                break;
            case Unit.UnitState.Fighting:
                UpdateFighting();
                break;
        }
    }
    void UpdateAdvancing(Battle battle)
    {
        unit.groundTaken += battle.battleInterval * speed;
        unit.UpdatePosition(startX, advanceDirection);
    }
    void UpdateFighting()
    {
        //TODO
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
