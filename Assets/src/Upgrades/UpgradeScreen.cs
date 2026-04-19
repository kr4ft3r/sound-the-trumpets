using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpgradeScreen : MonoBehaviour
{
    public TextMeshPro TitleBlue;
    public TextMeshPro TitleRed;
    public GameObject UpgradeOptionPrefab;
    GameManager gameManager;
    Regiment blueTeamRegiment;
    Regiment redTeamRegiment;
    bool player1Selected = false;
    bool player2Selected = false;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        DrawUpgradesForFaction(gameManager.BlueFaction);
        DrawUpgradesForFaction(gameManager.RedFaction);
    }

    void DrawUpgradesForFaction(Faction faction)
    {
        Regiment reg = faction.Regiments[Random.Range(0, FixedValues.RegimentsPerFaction)];
        if (faction.Side == Faction.FactionSide.Left)
        {
            TitleBlue.text = "Select upgrade for " + reg.GetName();
        } else
        {
            TitleRed.text = "Select upgrade for "+reg.GetName();
        }

        var possible = UpgradeManager.GetPossibleUpgradesForRegiment(reg);
        int maxOptions = (possible.Count < 3 ? possible.Count : 3);
        var options = new List<IRegimentUpgrade>();
        while (options.Count < maxOptions)
        {
            var rndIndex = Random.Range(0, possible.Count);
            options.Add(possible[rndIndex]);
            possible.RemoveAt(rndIndex);
        }

        for (int i = 0; i < possible.Count; i++) {
            //GameObject.Instantiate
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
