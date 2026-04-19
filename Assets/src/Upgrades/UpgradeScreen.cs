using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

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

    private void Awake()
    {
        UpgradeOption.RegimentUpgradeSelected += OnRegimentUpgradeSelected;
    }
    private void OnDestroy()
    {
        UpgradeOption.RegimentUpgradeSelected -= OnRegimentUpgradeSelected;
    }

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        DrawUpgradesForFaction(gameManager.BlueFaction);
        DrawUpgradesForFaction(gameManager.RedFaction);
    }
    List<UpgradeOption> allUpgradeOptions = new List<UpgradeOption>();
    void DrawUpgradesForFaction(Faction faction)
    {
        Regiment reg = faction.Regiments[Random.Range(0, FixedValues.RegimentsPerFaction)];
        if (faction.Side == Faction.FactionSide.Left)
        {
            blueTeamRegiment = reg;
        } else
        {
            redTeamRegiment = reg;
        }
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
        Debug.Log("Upgrade options max:"+maxOptions);
        Debug.Log(possible);
        while (options.Count < maxOptions)
        {
            var rndIndex = Random.Range(0, possible.Count);
            options.Add(possible[rndIndex]);
            possible.RemoveAt(rndIndex);
        }

        var upgradeOptions = new List<UpgradeOption>();
        for (int i = 0; i < options.Count; i++) {
            var optionGO = GameObject.Instantiate(
                UpgradeOptionPrefab, 
                new Vector3(-4.5f * (faction.Side == Faction.FactionSide.Right ? -1f : 1f), 1.5f - (i*1.5f),0), 
                Quaternion.identity);
            optionGO.GetComponent<UpgradeOption>().Configure(faction.Side == Faction.FactionSide.Left ? blueTeamRegiment : redTeamRegiment, i + 1 + (faction.Side == Faction.FactionSide.Right ? 6 : 0), options[i]);
            upgradeOptions.Add(optionGO.GetComponent<UpgradeOption>());
            allUpgradeOptions.Add(optionGO.GetComponent<UpgradeOption>() );
        }

        // AI selection
        if (!faction.IsHumanPlayer)
        {
            Debug.Log("Computer player " + faction.LeaderName + " is making the choice");
            var rnd = Random.Range(0, options.Count);
            var rndUpgrade = options[rnd];
            UpgradeOption.RegimentUpgradeSelected?.Invoke(faction.Side == Faction.FactionSide.Left ? blueTeamRegiment : redTeamRegiment, upgradeOptions[rnd], rndUpgrade);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnRegimentUpgradeSelected(Regiment regiment, UpgradeOption upgradeOption, IRegimentUpgrade upgrade)
    {
        Debug.Log("OnRegimentUpgradeSelected " + regiment.GetName() + " " + upgradeOption.Key + " " + upgrade.GetName());
        if (regiment == blueTeamRegiment)
        {
            if (player1Selected) return;
            player1Selected = true;
            blueTeamRegiment.AssignUpgrade(upgrade);
        } else
        {
            if (player2Selected) return;
            player2Selected = true;
            redTeamRegiment.AssignUpgrade(upgrade);
        }
        foreach (var opt in allUpgradeOptions)
        {
            if (opt.Regiment == regiment && opt != upgradeOption)
            {
                opt.transform.Find("Key").GetComponent<TextMeshPro>().alpha = 0.2f;
                opt.transform.Find("Name").GetComponent<TextMeshPro>().alpha = 0.2f;
                opt.transform.Find("Description").GetComponent<TextMeshPro>().alpha = 0.2f;
            }
        }

        if (player1Selected && player2Selected)
        {
            
            StartCoroutine(UpgradesDone());
        }
    }

    float doneCounter = 0.0f;
    IEnumerator UpgradesDone()
    {
        while(doneCounter < 1.0f)
        {
            doneCounter += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        SceneManager.LoadScene("SampleScene");
    }
}
