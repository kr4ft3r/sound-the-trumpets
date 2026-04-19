using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Regiment
{
    public readonly int Order;
    public Color Color;
    float advanceDirection;
    
    Unit unit;
    Trumpet trumpet;
    Cannon cannon;
    float startX;
    float speed = FixedValues.BaseUnitSpeed;
    float unitStrength = FixedValues.BaseUnitStrength;

    float advancedFromX; // Last position when advance order was given, in ground taken, for determining traversed

    public List<IRegimentUpgrade> Upgrades = new List<IRegimentUpgrade>();

    public int BonusTrumpetCooldown = 0;
    public int BonusTrumpetDelay = 0;
    public bool HasCannon = false;
    string prepend;

    public Regiment(int order, Color color, float advanceDirection, string prepend)
    {
        Order = order;
        Color = color;
        this.advanceDirection = advanceDirection;
        this.prepend = prepend;
    }
    public string GetName()
    {
        string num = "th";
        if (Order == 1) num = "st";
        else if (Order == 2) num = "nd";
        else if (Order == 3) num = "rd";
        
        return prepend+Order+num+" Regiment";
    }
    public void Deploy(Unit unit,  Trumpet trumpet)
    {
        this.unit = unit;
        this.trumpet = trumpet;
        Battle.SignalSent += OnSignalSent;
        startX = unit.transform.position.x;
        advancedFromX = unit.groundTaken;

        Debug.LogWarning("Remember to implement Undeploy");
    }
    public void Undeploy()
    {
        unit = null;
        trumpet = null;
        this.cannon = null;
        Battle.SignalSent -= OnSignalSent;
    }
    public void DeployCannon(Cannon cannon)
    {
        this.cannon = cannon;
    }
    public void Engage()
    {
        unit.NextState = Unit.UnitState.Fighting;
        unit.ToNextState();
        trumpet.Deactivate();
        if (HasCannon) 
        { 
            cannon.Deactivate(); 
        }
    }
    public bool IsEngaged()
    {
        return unit.State == Unit.UnitState.Fighting;
    }
    public bool TrumpetReady()
    {
        return trumpet.State == Trumpet.TrumpetState.Ready;
    }
    public float GetGroundTaken()
    {
        return unit.groundTaken;
    }
    public int GetCurrentMorale()
    {
        return unit.GetMorale();
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
    public void AssignRandomUpgrade()
    {
        var pool = UpgradeManager.GetPossibleUpgradesForRegiment(this);
        if (pool.Count == 0)
        {
            Debug.Log("Random upgrade: No upgrades possible for " + GetName());

            return;
        }
        AssignUpgrade(pool[UnityEngine.Random.Range(0, pool.Count)]);
    }
    public void AssignUpgrade(IRegimentUpgrade upgrade)
    {
        foreach (var ownUpgrade in Upgrades)
        {
            if (ownUpgrade == upgrade)
            {
                ownUpgrade.Apply(this);
                return;
            }
        }
        Upgrades.Add(upgrade);
        upgrade.Apply(this);
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
        if (unit.GetMorale() != unitStrength && battle.timeElapsed > unit.LastHealTime+1f)//update per second
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
    public void UpdateCannon(Battle battle, Regiment targetReg)
    {
        /*if (cannonFuseTimer <= 0.00f) return;
        cannonFuseTimer -= battle.battleInterval;
        if (cannonFuseTimer > 0.00f) return;

        cannonFuseTimer = 0;*/
        if (cannon.EnableTimer > 0.00f)
        {
            cannon.EnableTimer -= battle.battleInterval;
            if (cannon.EnableTimer <= 0.00f)
            {
                cannon.Ready();
            }

            return;
        }
        if (cannon.FuseTimer > 0.00f)
        {
            cannon.FuseTimer -= battle.battleInterval;
            if (cannon.FuseTimer <= 0.00f)
            {
                cannon.Fired(15.0f);
            } else
            {
                return;
            }
        } else
        {
            return;
        }
        Debug.Log("Firing cannon!");
        var moraleDamage = UnityEngine.Random.Range(5f, 10f);
        targetReg.TakeArtilleryDamage(moraleDamage);
    }
    public void TakeArtilleryDamage(float damage)
    {
        if (unit.State == Unit.UnitState.Fighting) return;
        if (unit.State == Unit.UnitState.Holding)
        {
            damage *= 3;
        }
        Debug.Log(GetName() + " aimed by cannon, morale was " + unit.GetMorale());
        unit.TakeDamage(damage);
        Debug.Log(GetName() + " taken " + damage + " from cannon, morale is now " + unit.GetMorale());
    }

    void OnSignalSent(Regiment regiment, Faction faction, bool isSpecial)
    {
        if (regiment != this) return;

        if (isSpecial)
        {
            if (!HasCannon)
            {
                Debug.LogWarning("No cannon availabe for " + GetName());

                return;
            }
            if (cannon.Deactivated) return;
            Debug.Log("Lighting fuse!");
            //cannonFuseTimer = UnityEngine.Random.Range(1.0f, 4.0f);
            cannon.Fuse(UnityEngine.Random.Range(1.0f, 4.0f));

            return;
        } else
        {
            HandleTrumpetSignal(regiment, faction);
        }
    }
    public void HandleTrumpetSignal(Regiment regiment, Faction faction)
    {
        if (unit.State == Unit.UnitState.Fighting || unit.State == Unit.UnitState.Pursuing || unit.State == Unit.UnitState.Retreating) return;
        if (trumpet.State != Trumpet.TrumpetState.Ready) return; // TODO funny sound?
        Debug.Log("Signal for faction " + faction.ID + " regiment order " + regiment.Order);

        if (trumpet.Deactivated) return;

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
        unit.NextStateTimer = FixedValues.BaseTrumpetDelay - (FixedValues.BaseTrumpetDelay * (BonusTrumpetDelay * 0.01f));
        trumpet.Blow(FixedValues.BaseTrumpetCooldown - (FixedValues.BaseTrumpetCooldown * (BonusTrumpetCooldown * 0.01f)));
    }
}
