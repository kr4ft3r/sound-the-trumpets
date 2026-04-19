using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public AnimationCurve InfantryStrengthCurve;

    Faction blueFaction;
    public Faction BlueFaction {  get { return blueFaction; } }
    Faction redFaction;
    public Faction RedFaction { get { return redFaction; } }

    Battle battle;

    public static GameManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        Battle.RoundResolved += OnRoundResolved;

        // TODO remove test calls
        InitFactions();
    }
    public void InitFactions()
    {
        blueFaction = new Faction(0, Faction.FactionSide.Left, "Prince Blue");
        redFaction = new Faction(1, Faction.FactionSide.Right, "Prince Red");
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnRoundResolved(bool hasWinner, Faction winningFaction)
    {
        var overlay = GameObject.Find("RoundEndOverlay").transform.Find("Screen").gameObject;
        overlay.SetActive(true);
        TextMeshPro roundEndText = overlay.transform.Find("Declaration").GetComponent<TextMeshPro>();
        if (hasWinner)
        {
            roundEndText.text = winningFaction.LeaderName + " Wins";
            Color textColor;
            if (winningFaction.ID == 0)
            {
                ColorUtility.TryParseHtmlString("#2B75C3", out textColor);
            } else
            {
                ColorUtility.TryParseHtmlString("#8A1D00", out textColor);
            }
            roundEndText.color = textColor;
        } else
        {
            roundEndText.text = "Round Draw";
            roundEndText.color = Color.white;
        }

        StartCoroutine(BattleEndCoroutine());
    }

    IEnumerator BattleEndCoroutine()
    {
        yield return new WaitForSeconds(3.0f);
        battle.StopUpdates();
        battle = null;
        // Upgrades scene
        SceneManager.LoadScene("UpgradesScene");
    }
}
