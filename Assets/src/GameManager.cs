using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Faction BlueFaction;
    public Faction RedFaction;

    GameObject unitPrefab;
    private void Awake()
    {
        unitPrefab = Resources.Load<GameObject>("Prefabs/Unit");

        BlueFaction = new Faction(0, Faction.FactionSide.Left);
        RedFaction = new Faction(1, Faction.FactionSide.Right);

        // TODO remove test calls

        InitBattleScreen();
    }
    public void InitBattleScreen()
    {
        SpawnFaction(BlueFaction);
        SpawnFaction(RedFaction);
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
                FixedValues.UnitSlotDistanceFromCenterX * xMod,
                FixedValues.FirstUnitSlotPositionY + (i * FixedValues.UnitColumnDistance) * downDir
                );
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
