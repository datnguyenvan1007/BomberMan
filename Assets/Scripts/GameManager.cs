using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class GameData
{
    public static int numberOfBombs;
    public static float speed;
    public static int score;
    public static int flame;
    public static int wallPass;
    public static int bombPass;
    public static int flamePass;
    public static int mystery = 0;
    /*public static int left = 2;
    public static int stage = 1;*/
    /*public static float controllerOpacity = 45;
    public static int controllerType = 2;
    public static bool isFlipControls = false;
    public static bool isSound = true;*/
}
public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject level;
    [SerializeField] private List<GameObject> levelsPrefab;
    [SerializeField] private GameObject timeOut;
    [SerializeField] private GameObject player;
    private GameObject currentLevel;
    private float timeRemain = 200f;
    private GameObject exitGate;
    private GameObject brickOverExitGate;
    private GameObject items;
    private GameObject brickOverItems;
    private bool isPlayingLevel = false;
    private bool isActivedExitGate = false;
    private bool isActivedItems = false;
    private int index = 2;

    private static GameManager instance;
    public static GameManager Instance { get => instance; }
    private void Awake()
    {
        GameManager.instance = this;
    }

    void Start()
    {
        GetValueForGameData();
        for (int i = 1; i <= GameData.numberOfBombs; i++)
            BombSpawner.Instance.AddBomb();
        UIManager.Instance.SetControllerOpacity(PlayerPrefs.GetFloat("ControllerOpacity") / 100);
        //uiManager.SetAcitveControllerType(PlayerPrefs.GetInt("ControllerType"));
        UIManager.Instance.SetTimeGame(timeRemain);
        UIManager.Instance.SetGameScore(0);
        StartCoroutine(StartLevel());
    }

    void Update()
    {
        if (isPlayingLevel)
        {
            SetTimeGame();
            if (!isActivedExitGate)
                StartCoroutine(CanActiveExitGate());
            if (!isActivedItems && items)
                StartCoroutine(CanActiveItems());
        }
    }

    void GetValueForGameData()
    {
        GameData.speed = PlayerPrefs.GetFloat("Speed", 3f);
        GameData.numberOfBombs = PlayerPrefs.GetInt("NumberOfBombs", 1);
        GameData.flame = PlayerPrefs.GetInt("Flame", 1);
        GameData.score = PlayerPrefs.GetInt("Score");
        GameData.wallPass = PlayerPrefs.GetInt("WallPass");
        GameData.flamePass = PlayerPrefs.GetInt("FlamePass");
        GameData.bombPass = PlayerPrefs.GetInt("BombPass");
    }

    void SaveData()
    {
        PlayerPrefs.SetInt("Score", GameData.score);
        PlayerPrefs.SetFloat("Speed", GameData.speed);
        PlayerPrefs.SetInt("WallPass", GameData.wallPass);
        PlayerPrefs.SetInt("NumberOfBombs", GameData.numberOfBombs);
        PlayerPrefs.SetInt("FlamePass", GameData.flamePass);
        PlayerPrefs.SetInt("Flame", GameData.flame);
        PlayerPrefs.SetInt("BombPass", GameData.bombPass);
    }

    IEnumerator StartLevel()
    {
        player.SetActive(true);
        /*index = PlayerPrefs.GetInt("Left") + (PlayerPrefs.GetInt("Stage") - 1) * 4 - 1;*/
        index = PlayerPrefs.GetInt("Left", 2);

        AudioManager.Instance.Stop();

        UIManager.Instance.SetActivePlayingScene(false);
        if (index >= levelsPrefab.Count)
        {
            UIManager.Instance.ActiveWinScene();
            yield break;
        }
        UIManager.Instance.SetValueStageAndLeft(PlayerPrefs.GetInt("Stage", 1), PlayerPrefs.GetInt("Left", 2));
        UIManager.Instance.SetActiveStartingScene(true);

        AudioManager.Instance.PlayAudioLevelStart();
        yield return new WaitForSeconds(4f);
        UIManager.Instance.SetActiveStartingScene(false);
        if (index < levelsPrefab.Count)
            currentLevel = Instantiate(levelsPrefab[index], level.transform);

        UIManager.Instance.SetActivePlayingScene(true);
        AudioManager.Instance.PlayAudioInGame();

        exitGate = GameObject.Find("ExitGate");
        RaycastHit2D hit = Physics2D.Raycast(exitGate.transform.position, Vector3.forward);
        brickOverExitGate = hit.collider.gameObject;
        exitGate.SetActive(false);
        isActivedExitGate = false;

        GameObject itemsParent = GameObject.Find("Items");
        if (itemsParent.transform.childCount > 0)
        {
            items = itemsParent.transform.GetChild(0).gameObject;
            hit = Physics2D.Raycast(items.transform.position, Vector3.forward);
            brickOverItems = hit.collider.gameObject;
            items.SetActive(false);
            isActivedItems = false;
        }

        isPlayingLevel = true;

        timeRemain = 200;
    }
    void SetTimeGame()
    {
        if (timeRemain <= 0)
            return;
        if (timeRemain <= 0.1f)
        {
            TimeOut();
            PoolEnemy.Instance.enemyAlive += 7;
        }
        timeRemain -= Time.deltaTime;
        UIManager.Instance.SetTimeGame(timeRemain);
    }
    IEnumerator CanActiveExitGate()
    {
        if (!brickOverExitGate.activeSelf)
        {
            exitGate.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            exitGate.GetComponent<Collider2D>().enabled = true;
            isActivedExitGate = true;
        }
    }
    IEnumerator CanActiveItems()
    {
        if (!brickOverItems.activeSelf)
        {
            items.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            items.GetComponent<Collider2D>().enabled = true;
            isActivedItems = true;
        }
    }
    private void TimeOut()
    {
        timeOut.SetActive(true);
        foreach (Transform transform in timeOut.transform)
        {
            transform.gameObject.SetActive(true);
        }
    }
    public IEnumerator WinLevel()
    {
        yield return new WaitForSeconds(2f);
        isPlayingLevel = false;
        SaveData();
        PlayerPrefs.SetInt("Left", PlayerPrefs.GetInt("Left", 2) + 1);
        PlayerPrefs.SetInt("Stage", PlayerPrefs.GetInt("Stage", 1) + 1);
        Destroy(currentLevel);
        player.SetActive(false);
        StartCoroutine(StartLevel());
    }
    public void Lose()
    {
        StartCoroutine(GoToPreviousLevel());
    }
    private IEnumerator GoToPreviousLevel()
    {
        yield return new WaitForSeconds(2f);
        isPlayingLevel = false;
        if (PlayerPrefs.GetInt("Left", 2) > 0)
        {
            PlayerPrefs.SetInt("Left", PlayerPrefs.GetInt("Left", 2) - 1);
            Destroy(currentLevel);
            GetValueForGameData();
            UIManager.Instance.SetGameScore(0);
            StartCoroutine(StartLevel());
        }
        else
        {
            UIManager.Instance.SetActivePlayingScene(false);
            UIManager.Instance.ActiveLoseScene();
        }
    }
}