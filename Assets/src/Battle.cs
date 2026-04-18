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

    IEnumerator MainUpdate()
    {
        while (true)
        {
            UpdateFaction(BlueFaction);
            UpdateFaction(RedFaction);

            yield return new WaitForSeconds(battleInterval);
        }
    }

    void UpdateFaction(Faction faction)
    {
        faction.UpdateBattleState();
        foreach (var reg in faction.Regiments)
        {
            reg.UpdateBattleState(this);
        }
    }

    void HandleBluePlayerHumanInput()
    {
        if (Input.GetKeyUp(KeyCode.Keypad1))
        {
            SignalSent?.Invoke(BlueFaction.Regiments[0], BlueFaction, Input.GetKeyUp(KeyCode.LeftShift));
        }
        else if (Input.GetKeyUp(KeyCode.Keypad2))
        {
            SignalSent?.Invoke(BlueFaction.Regiments[1], BlueFaction, Input.GetKeyUp(KeyCode.LeftShift));
        }
        else if (Input.GetKeyUp(KeyCode.Keypad3))
        {
            SignalSent?.Invoke(BlueFaction.Regiments[2], BlueFaction, Input.GetKeyUp(KeyCode.LeftShift));
        }
        else if (Input.GetKeyUp(KeyCode.Keypad4))
        {
            SignalSent?.Invoke(BlueFaction.Regiments[3], BlueFaction, Input.GetKeyUp(KeyCode.LeftShift));
        }
    }
    void HandleRedPlayerHumanInput()
    {
        if (Input.GetKeyUp(KeyCode.Keypad7))
        {
            SignalSent?.Invoke(BlueFaction.Regiments[0], BlueFaction, Input.GetKeyUp(KeyCode.RightShift));
        }
        else if (Input.GetKeyUp(KeyCode.Keypad8))
        {
            SignalSent?.Invoke(BlueFaction.Regiments[1], BlueFaction, Input.GetKeyUp(KeyCode.RightShift));
        }
        else if (Input.GetKeyUp(KeyCode.Keypad9))
        {
            SignalSent?.Invoke(BlueFaction.Regiments[2], BlueFaction, Input.GetKeyUp(KeyCode.RightShift));
        }
        else if (Input.GetKeyUp(KeyCode.Keypad0))
        {
            SignalSent?.Invoke(BlueFaction.Regiments[3], BlueFaction, Input.GetKeyUp(KeyCode.RightShift));
        }
    }
}
