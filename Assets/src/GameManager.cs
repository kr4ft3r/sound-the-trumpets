using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public AnimationCurve InfantryStrengthCurve;

    Faction blueFaction;
    Faction redFaction;

    Battle battle;

    private void Awake()
    {

        // TODO remove test calls
        InitFactions();
    }
    public void InitFactions()
    {
        blueFaction = new Faction(0, Faction.FactionSide.Left);
        redFaction = new Faction(1, Faction.FactionSide.Right);
    }

    public void StartBattle()
    {
        if (battle) Destroy(battle);
        battle = gameObject.AddComponent<Battle>();
        battle.BlueFaction = blueFaction;
        battle.RedFaction = redFaction;
        battle.InfantryStrengthCurve = InfantryStrengthCurve;
    }

    // Start is called before the first frame update
    void Start()
    {
        // TODO remove test calls
        StartBattle();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
