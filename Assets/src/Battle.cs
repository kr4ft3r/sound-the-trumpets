using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Battle : MonoBehaviour
{
    public static System.Action<Regiment, Faction, bool> SignalSent;
    public static System.Action<Regiment, Faction, bool> SignalReceived;

    public AnimationCurve InfantryStrengthCurve;

    public Faction BlueFaction;
    public Faction RedFaction;

    GameObject unitPrefab;
    GameObject trumpetPrefab;
    public float battleInterval { get; protected set; } = 0.05f;
    public float timeElapsed { get; protected set; } = 0;
    Coroutine mainCoroutine;
    private void Awake()
    {
        unitPrefab = Resources.Load<GameObject>("Prefabs/Unit");
        trumpetPrefab = Resources.Load<GameObject>("Prefabs/Trumpet");
    }

    // Start is called before the first frame update
    void Start()
    {
        SpawnFaction(BlueFaction);
        SpawnFaction(RedFaction);

        mainCoroutine = StartCoroutine(MainUpdate());
    }

    // Update is called once per frame
    void Update()
    {
        if (BlueFaction.IsHumanPlayer)
        {
            HandleBluePlayerHumanInput();
        }
        else
        {
            HandleComputerPlayerInput(BlueFaction);
        }

        if (RedFaction.IsHumanPlayer)
        {
            HandleRedPlayerHumanInput();
        } else
        {
            HandleComputerPlayerInput(RedFaction);
        }
    }

    void SpawnFaction(Faction faction)
    {
        float xMod = faction.Side == Faction.FactionSide.Left ? -1f : 1f;
        float downDir = -1;
        for (int i = 0; i < faction.Regiments.Count(); i++)
        {
            var reg = faction.Regiments[i];
            GameObject unit = GameObject.Instantiate(unitPrefab);
            var spr = unit.transform.Find("sprite").GetComponent<SpriteRenderer>();
            spr.color = reg.Color;
            spr.flipX = faction.Side == Faction.FactionSide.Right;
            unit.transform.position = new Vector2(
                FixedValues.UnitStartingDistanceFromCenterX * xMod,
                FixedValues.FirstUnitSlotPositionY + (i * FixedValues.UnitColumnDistance) * downDir
                );

            GameObject trumpet = GameObject.Instantiate(trumpetPrefab);
            trumpet.transform.position = new Vector2(FixedValues.UnitSlotDistanceFromCenterX * xMod, unit.transform.position.y - 0.2f);
            trumpet.transform.Find("sprite").GetComponent<SpriteRenderer>().flipX = faction.Side == Faction.FactionSide.Right;
            trumpet.GetComponent<Trumpet>().SetKey(i + (faction.Side == Faction.FactionSide.Right ? 7 : 1), faction.Side == Faction.FactionSide.Right ? -1f : 0f);

            reg.Deploy(unit.GetComponent<Unit>(), trumpet.GetComponent<Trumpet>());
        }
    }

    bool isFirstFrame = true;
    IEnumerator MainUpdate()
    {
        while (true)
        {
            timeElapsed += battleInterval;
            if (isFirstFrame)
            {
                isFirstFrame = false;
                yield return new WaitForSeconds(battleInterval);
            }
            UpdateFaction(BlueFaction);
            UpdateFaction(RedFaction);
            UpdateCombats();

            yield return new WaitForSeconds(battleInterval);
        }
    }

    void UpdateFaction(Faction faction)
    {
        faction.UpdateBattleState();
        for (int i = 0; i < faction.Regiments.Count(); i++)
        {
            var reg = faction.Regiments[i];
            reg.UpdateBattleState(this);
        }
    }

    void UpdateCombats()
    {
        for (int i = 0; i < FixedValues.RegimentsPerFaction; i++)
        {
            var leftReg = BlueFaction.Regiments[i];
            var rightReg = RedFaction.Regiments[i];

            // Artillery
            if (leftReg.HasCannon) leftReg.UpdateCannon(this, rightReg);
            if (rightReg.HasCannon) rightReg.UpdateCannon(this, leftReg);

            // Melee engagement

            bool engagementActive = false;
            if (!leftReg.IsEngaged() || !rightReg.IsEngaged()) // whatever
            {
                if (UnitsAreWithinEngagementDistance(leftReg, rightReg))
                {
                    Debug.Log("Start engagement");
                    engagementActive = true;
                    leftReg.Engage();
                    rightReg.Engage();

                    // And now, the actual combat, strength resolved in advance
                    int leftRelativeDiff = (int)(leftReg.GetCurrentMorale() - rightReg.GetCurrentMorale());
                    if (leftRelativeDiff > 0)
                    {
                        leftReg.Pursue(Mathf.Abs(leftRelativeDiff));
                        rightReg.Retreat(Mathf.Abs(leftRelativeDiff));
                    }
                    else if (leftRelativeDiff < 0)
                    {
                        leftReg.Retreat(Mathf.Abs(leftRelativeDiff));
                        rightReg.Pursue(Mathf.Abs(leftRelativeDiff));
                    }
                    else
                    {
                        // Stalemate
                    }
                    Debug.Log(leftRelativeDiff);
                }
            }
        }
    }

    void HandleBluePlayerHumanInput()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            SignalSent?.Invoke(BlueFaction.Regiments[0], BlueFaction, Input.GetKey(KeyCode.LeftShift));
        }
        else if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            SignalSent?.Invoke(BlueFaction.Regiments[1], BlueFaction, Input.GetKey(KeyCode.LeftShift));
        }
        else if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            SignalSent?.Invoke(BlueFaction.Regiments[2], BlueFaction, Input.GetKey(KeyCode.LeftShift));
        }
        else if (Input.GetKeyUp(KeyCode.Alpha4))
        {
            SignalSent?.Invoke(BlueFaction.Regiments[3], BlueFaction, Input.GetKey(KeyCode.LeftShift));
        }
    }
    void HandleRedPlayerHumanInput()
    {
        if (Input.GetKeyUp(KeyCode.Alpha7))
        {
            SignalSent?.Invoke(RedFaction.Regiments[0], RedFaction, Input.GetKey(KeyCode.RightShift));
        }
        else if (Input.GetKeyUp(KeyCode.Alpha8))
        {
            SignalSent?.Invoke(RedFaction.Regiments[1], RedFaction, Input.GetKey(KeyCode.RightShift));
        }
        else if (Input.GetKeyUp(KeyCode.Alpha9))
        {
            SignalSent?.Invoke(RedFaction.Regiments[2], RedFaction, Input.GetKey(KeyCode.RightShift));
        }
        else if (Input.GetKeyUp(KeyCode.Alpha0))
        {
            SignalSent?.Invoke(RedFaction.Regiments[3], RedFaction, Input.GetKey(KeyCode.RightShift));
        }
    }

    void HandleComputerPlayerInput(Faction faction)
    {
        faction.TimeUntilComputerDecision -= Time.deltaTime;
        if (faction.TimeUntilComputerDecision > 0.00f) 
        {
            return;
        }

        faction.TimeUntilComputerDecision = Random.Range(0.5f, 8.0f);

        var signals = faction.GetPossibleTrumpetSignals();
        if (signals.Count == 0) return;
        
        var rnd = signals[Random.Range(0, signals.Count)]; //(max exclusive)
        SignalSent?.Invoke(rnd.Item1, faction, rnd.Item2);
    }

    public bool UnitsAreWithinEngagementDistance(Regiment leftReg, Regiment rightReg) {
        return Mathf.Abs(leftReg.GetGroundTaken() - (FixedValues.GetFieldSize() - rightReg.GetGroundTaken())) <= FixedValues.EngagementDistance;
    }

    //public Regiment GetOpposingRegiment
}
