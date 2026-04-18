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
    float advancedFromX; // Last position when advance order was given, in ground taken, for determining traversed
    float unitStrength = FixedValues.BaseUnitStrength;

    public List<IRegimentUpgrade> Upgrades = new List<IRegimentUpgrade>();

    public float BonusTrumpetCooldown;

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
        trumpet.Deactivate();
    }
    public bool IsEngaged()
    {
        return unit.State == Unit.UnitState.Fighting;
    }
    public float GetGroundTaken()
    {
        return unit.groundTaken;
    }
    public int GetCurrentStrength()
    {
        return unit.Strength;
    }
    public void Pursue(int diff)
    {
        unit.FightState = Unit.UnitFightState.Pursuing;
        unit.FightDifference = diff;
    }
    public void Retreat(int diff)
    {
        unit.FightState = Unit.UnitFightState.Retreating;
        unit.FightDifference = diff;
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
            case Unit.UnitState.Holding:
                UpdateHolding(battle);
                break;
            case Unit.UnitState.Advancing:
                UpdateAdvancing(battle);
                break;
            case Unit.UnitState.Fighting:
                UpdateFighting(battle);
                break;
        }
    }
    void UpdateHolding(Battle battle) {
        if (unit.Strength != unitStrength && battle.timeElapsed > unit.LastHealTime+1f)//update per second
        {
            unit.UpdateStrengthHolding(unitStrength);
            unit.LastHealTime = battle.timeElapsed;
        }
    }
    void UpdateAdvancing(Battle battle)
    {
        unit.groundTaken += battle.battleInterval * speed;
        unit.UpdatePosition(startX, advanceDirection);
        unit.UpdateStrengthAdvancing(battle.InfantryStrengthCurve, unitStrength, unit.groundTaken - advancedFromX);
    }
    void UpdateFighting(Battle battle)
    {
        if (unit.FightState == Unit.UnitFightState.Retreating)
        {
            unit.groundTaken -= battle.battleInterval * speed * .5f;
        } else if (unit.FightState == Unit.UnitFightState.Pursuing)
        {
            unit.groundTaken += battle.battleInterval * speed * .5f;
        }
        unit.UpdatePosition(startX, advanceDirection);
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
                advancedFromX = unit.groundTaken;
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
