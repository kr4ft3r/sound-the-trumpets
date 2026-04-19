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

    public int WinsBlueFaction = 0;
    public int WinsRedFaction = 0;

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
        WinsBlueFaction = 0;
        WinsRedFaction = 0;
    }

    public void ResetFactions()
    {
        WinsBlueFaction = 0;
        WinsRedFaction = 0;
        blueFaction.InitRegiments();
        redFaction.InitRegiments();
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
        if (blueFaction != null && redFaction != null)
        {
            bool startNewGame = false;
            if (!blueFaction.IsHumanPlayer && Input.GetKeyUp(KeyCode.Alpha1))
            {
                if (!redFaction.IsHumanPlayer) startNewGame = true;
                blueFaction.IsHumanPlayer = true;
                
            }
            if (!redFaction.IsHumanPlayer && Input.GetKeyUp(KeyCode.Alpha0))
            {
                if (!blueFaction.IsHumanPlayer) startNewGame = true;
                redFaction.IsHumanPlayer = true;
            }

            if (startNewGame) StartNewGame();
        }
    }

    void StartNewGame()
    {
        battle.StopUpdates();
        foreach (Regiment reg in BlueFaction.Regiments)
        {
            reg.Undeploy();
        }
        foreach (Regiment reg in RedFaction.Regiments)
        {
            reg.Undeploy();
        }
        ResetFactions();
        Destroy(battle);
        SceneManager.LoadScene("IntroScene");
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
                WinsBlueFaction++;
                GameObject.Find("BlueGeneral").transform.Find("Score").GetComponent<TextMeshPro>().text = WinsBlueFaction.ToString();
            } else
            {
                ColorUtility.TryParseHtmlString("#8A1D00", out textColor);
                WinsRedFaction++;
                GameObject.Find("RedGeneral").transform.Find("Score").GetComponent<TextMeshPro>().text = WinsRedFaction.ToString();
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
        foreach (Regiment reg in BlueFaction.Regiments) {
            reg.Undeploy();
        }
        foreach (Regiment reg in RedFaction.Regiments)
        {
            reg.Undeploy();
        }
        battle.StopUpdates();
        Destroy(battle);
        // Upgrades scene
        SceneManager.LoadScene("UpgradesScene");
    }
}
