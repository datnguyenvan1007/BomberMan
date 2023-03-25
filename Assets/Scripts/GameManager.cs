using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public static class GameData
{
    public static int numberOfBombs;
    public static float speed;
    public static int score;
    public static int flame;
    public static int wallPass;
    public static int bombPass;
    public static int flamePass;
    public static int detonator;
    public static int mystery = 0;
}
public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject level;
    [SerializeField] private List<GameObject> levelsPrefab;
    [SerializeField] private GameObject timeOut;
    [SerializeField] private GameObject player;
    private GameObject currentLevel;
    private float timeRemain;
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
        Application.targetFrameRate = 60;
        GameManager.instance = this;
        GetValueForGameData();
    }

    void Start()
    {
        for (int i = 1; i <= GameData.numberOfBombs; i++)
            BombSpawner.Instance.AddBomb();
        UIManager.Instance.SetControllerOpacity(PlayerPrefs.GetFloat("ControllerOpacity", 45) / 100);
        UIManager.Instance.SetAcitveControllerType(PlayerPrefs.GetInt("ControllerType", 2));
        UIManager.Instance.SetTimeGame(timeRemain);
        UIManager.Instance.SetGameScore(0);
        StartCoroutine(LoadLevel());
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
        GameData.speed = PlayerPrefs.GetFloat("Speed", 3.5f);
        GameData.numberOfBombs = PlayerPrefs.GetInt("NumberOfBombs", 1);
        GameData.flame = PlayerPrefs.GetInt("Flame", 1);
        GameData.score = PlayerPrefs.GetInt("Score", 0);
        GameData.wallPass = PlayerPrefs.GetInt("WallPass", 0);
        GameData.flamePass = PlayerPrefs.GetInt("FlamePass", 0);
        GameData.bombPass = PlayerPrefs.GetInt("BombPass", 0);
        GameData.detonator = PlayerPrefs.GetInt("Detonator", 0);
    }

    void SaveData()
    {
        PlayerPrefs.SetInt("Score", GameData.score);
        PlayerPrefs.SetFloat("Speed", GameData.speed);
        PlayerPrefs.SetInt("Flame", GameData.flame);
        PlayerPrefs.SetInt("NumberOfBombs", GameData.numberOfBombs);
        PlayerPrefs.SetInt("WallPass", GameData.wallPass);
        PlayerPrefs.SetInt("FlamePass", GameData.flamePass);
        PlayerPrefs.SetInt("BombPass", GameData.bombPass);
        PlayerPrefs.SetInt("Detonator", GameData.detonator);
    }
    void SaveDataWhenLosing()
    {
        GameData.wallPass = 0;
        GameData.flamePass = 0;
        GameData.bombPass = 0;
        PlayerPrefs.SetInt("WallPass", GameData.wallPass);
        PlayerPrefs.SetInt("FlamePass", GameData.flamePass);
        PlayerPrefs.SetInt("BombPass", GameData.bombPass);
    }

    IEnumerator LoadLevel()
    {
        TimeOut(false);
        player.SetActive(true);
        /*index = PlayerPrefs.GetInt("Left") + (PlayerPrefs.GetInt("Stage") - 1) * 4 - 1;*/
        index = PlayerPrefs.GetInt("Left", 2);

        AudioManager.Instance.Stop();

        UIManager.Instance.SetActivePlayingScene(false);
        if (index >= levelsPrefab.Count)
        {
            UIManager.Instance.ActiveWinScene();
            DeleteAllData();
            Invoke("RedirectHome", 2f);
            yield break;
        }
        UIManager.Instance.SetValueStageAndLeft(PlayerPrefs.GetInt("Stage", 1), PlayerPrefs.GetInt("Left", 2));
        UIManager.Instance.SetActiveStartingScene(true);

        AudioManager.Instance.PlayAudioLevelStart();
        yield return new WaitForSeconds(3.0f);
        UIManager.Instance.SetActiveStartingScene(false);
        if (index < levelsPrefab.Count)
            currentLevel = Instantiate(levelsPrefab[index], level.transform);


        UIManager.Instance.SetActivePlayingScene(true);
        AudioManager.Instance.PlayAudioInGame();

        HideExitGate();
        HideItems();

        isPlayingLevel = true;

        timeRemain = 200;
    }
    void HideExitGate()
    {
        exitGate = GameObject.Find("ExitGate");
        RaycastHit2D hit = Physics2D.Raycast(exitGate.transform.position, Vector3.forward, 0.5f, LayerMask.GetMask("Brick"));
        brickOverExitGate = hit.collider.gameObject;
        exitGate.SetActive(false);
        isActivedExitGate = false;
    }
    void HideItems()
    {
        GameObject itemsParent = GameObject.Find("Items");
        if (itemsParent.transform.childCount > 0)
        {
            items = itemsParent.transform.GetChild(0).gameObject;
            RaycastHit2D hit = Physics2D.Raycast(items.transform.position, Vector3.forward, 0.5f, LayerMask.GetMask("Brick"));
            brickOverItems = hit.collider.gameObject;
            items.SetActive(false);
            isActivedItems = false;
        }
    }
    void SetTimeGame()
    {
        if (timeRemain <= 0)
            return;
        if (timeRemain <= 0.1f && !timeOut.activeSelf)
        {
            if (Player.isCompleted)
                return;
            TimeOut(true);
            PoolEnemy.Instance.enemyAlive += 7;
            return;
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
    private void TimeOut(bool isActive)
    {
        timeOut.SetActive(isActive);
        foreach (Transform transform in timeOut.transform)
        {
            transform.gameObject.SetActive(true);
        }
    }
    public IEnumerator WinLevel()
    {
        yield return new WaitForSeconds(2f);
        isPlayingLevel = false;
        GameData.mystery = 0;
        SaveData();
        PlayerPrefs.SetInt("Left", PlayerPrefs.GetInt("Left", 2) + 1);
        PlayerPrefs.SetInt("Stage", PlayerPrefs.GetInt("Stage", 1) + 1);
        Destroy(currentLevel);
        player.SetActive(false);
        StartCoroutine(LoadLevel());
    }
    public void Lose()
    {
        SaveDataWhenLosing();
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
            StartCoroutine(LoadLevel());
        }
        else
        {
            UIManager.Instance.SetActivePlayingScene(false);
            UIManager.Instance.ActiveLoseScene();
            DeleteAllData();
            Invoke("RedirectHome", 2f);
        }
    }
    void DeleteAllData()
    {
        PlayerPrefs.DeleteKey("Score");
        PlayerPrefs.DeleteKey("NumberOfBombs");
        PlayerPrefs.DeleteKey("Flame");
        PlayerPrefs.DeleteKey("WallPass");
        PlayerPrefs.DeleteKey("BombPass");
        PlayerPrefs.DeleteKey("FlamePass");
        PlayerPrefs.DeleteKey("Speed");
        PlayerPrefs.DeleteKey("Stage");
        PlayerPrefs.DeleteKey("Left");
    }
    void RedirectHome()
    {
        SceneManager.LoadScene(0);
    }
}