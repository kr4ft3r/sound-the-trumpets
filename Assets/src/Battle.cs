using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Battle : MonoBehaviour
{
    public static System.Action<Regiment, Faction, bool> SignalSent;
    public static System.Action<Regiment, Faction, bool> SignalReceived;

    public Faction BlueFaction;
    public Faction RedFaction;

    GameObject unitPrefab;
    GameObject trumpetPrefab;
    public float battleInterval { get; protected set; } = 0.05f;
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
        
        if (RedFaction.IsHumanPlayer)
        {
            HandleRedPlayerHumanInput();
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

            reg.Deploy(unit.GetComponent<Unit>(), trumpet.GetComponent<Trumpet>());
        }
    }

    bool isFirstFrame = true;
    IEnumerator MainUpdate()
    {
        while (true)
        {
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
            if (!leftReg.IsEngaged() || !rightReg.IsEngaged()) // whatever
            {
                if (UnitsAreWithinEngagementDistance(leftReg, rightReg))
                {
                    Debug.Log("Start engagement");
                    leftReg.Engage();
                    rightReg.Engage();
                }
            }
        }
    }

    void HandleBluePlayerHumanInput()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            SignalSent?.Invoke(BlueFaction.Regiments[0], BlueFaction, Input.GetKeyUp(KeyCode.LeftShift));
        }
        else if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            SignalSent?.Invoke(BlueFaction.Regiments[1], BlueFaction, Input.GetKeyUp(KeyCode.LeftShift));
        }
        else if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            SignalSent?.Invoke(BlueFaction.Regiments[2], BlueFaction, Input.GetKeyUp(KeyCode.LeftShift));
        }
        else if (Input.GetKeyUp(KeyCode.Alpha4))
        {
            SignalSent?.Invoke(BlueFaction.Regiments[3], BlueFaction, Input.GetKeyUp(KeyCode.LeftShift));
        }
    }
    void HandleRedPlayerHumanInput()
    {
        if (Input.GetKeyUp(KeyCode.Alpha7))
        {
            SignalSent?.Invoke(BlueFaction.Regiments[0], BlueFaction, Input.GetKeyUp(KeyCode.RightShift));
        }
        else if (Input.GetKeyUp(KeyCode.Alpha8))
        {
            SignalSent?.Invoke(BlueFaction.Regiments[1], BlueFaction, Input.GetKeyUp(KeyCode.RightShift));
        }
        else if (Input.GetKeyUp(KeyCode.Alpha9))
        {
            SignalSent?.Invoke(BlueFaction.Regiments[2], BlueFaction, Input.GetKeyUp(KeyCode.RightShift));
        }
        else if (Input.GetKeyUp(KeyCode.Alpha0))
        {
            SignalSent?.Invoke(BlueFaction.Regiments[3], BlueFaction, Input.GetKeyUp(KeyCode.RightShift));
        }
    }

    public bool UnitsAreWithinEngagementDistance(Regiment leftReg, Regiment rightReg) {
        return Mathf.Abs(leftReg.GetGroundTaken() - (FixedValues.GetFieldSize() - rightReg.GetGroundTaken())) <= FixedValues.EngagementDistance;
    }

    //public Regiment GetOpposingRegiment
}
