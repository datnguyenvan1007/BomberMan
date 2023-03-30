using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
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
    [SerializeField] private GameObject levelObject;
    [SerializeField] private GameObject timeOut;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject exitGate;
    [SerializeField] private List<GameObject> mapsPrefab;
    [SerializeField] private List<GameObject> enemiesAndItemPrefab;
    private GameObject mapOfCurrentLevel;
    private GameObject enemiesAndItemOfCurrentLevel;
    private GameObject brickOverExitGate;
    private GameObject item;
    private GameObject brickOverItem;
    private List<Vector2> listOfBrickPositions;
    private List<Vector2> listOfPositions;
    private List<Vector2> listOfPositionsCanFillEnemy;
    private bool isPlayingLevel = false;
    private bool isActivedExitGate = false;
    private bool isActiveItem = false;
    private float timeRemain;

    private static GameManager instance;
    public static GameManager Instance { get => instance; }
    private void Awake()
    {
        Application.targetFrameRate = 60;
        GameManager.instance = this;
        GetValueForGameData();
        listOfBrickPositions = new List<Vector2>();
        listOfPositions = new List<Vector2>();
        for (int i = -4; i <= 6; i++)
        {
            for (int j = -12; j <= 22; j++)
            {
                if (i >= 3 && j <= -6)
                    continue;
                if (i % 2 == 0)
                    listOfPositions.Add(new Vector2(j, i));
                else
                {
                    if (j % 2 == 0)
                       listOfPositions.Add(new Vector2(j, i));
                }
            }
        }
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

    void FixedUpdate()
    {
        if (isPlayingLevel)
        {
            SetTimeGame();
            if (!isActivedExitGate)
                StartCoroutine(CanActiveExitGate());
            if (!isActiveItem && item)
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

        AudioManager.Instance.Stop();

        UIManager.Instance.SetActivePlayingScene(false);
        if (PlayerPrefs.GetInt("Stage", 1) >= enemiesAndItemPrefab.Count)
        {
            UIManager.Instance.ActiveWinScene();
            DeleteAllData();
            Invoke("RedirectHome", 2f);
            yield break;
        }

        UIManager.Instance.SetActiveButtonDetonator(GameData.detonator);
        UIManager.Instance.SetValueStageAndLeft(PlayerPrefs.GetInt("Stage", 1), PlayerPrefs.GetInt("Left", 2));
        UIManager.Instance.SetActiveStartingScene(true);
        BombSpawner.Instance.ExplodeAllBombs();

        AudioManager.Instance.PlayAudioLevelStart();

        yield return new WaitForSeconds(3.0f);
        UIManager.Instance.SetActiveStartingScene(false);
        InitMap();
        UIManager.Instance.SetActivePlayingScene(true);
        AudioManager.Instance.PlayAudioInGame();

        isPlayingLevel = true;
        timeRemain = 200;
    }
    void InitMap()
    {
        int index;
        index = Random.Range(0, mapsPrefab.Count);
        listOfBrickPositions.Clear();
        mapOfCurrentLevel = Instantiate(mapsPrefab[index], levelObject.transform);
        foreach (Transform brick in mapOfCurrentLevel.transform)
        {
            listOfBrickPositions.Add(brick.position);
        }
        enemiesAndItemOfCurrentLevel = Instantiate(enemiesAndItemPrefab[PlayerPrefs.GetInt("Stage", 1) - 1], levelObject.transform);
        ArrangeEnemies();
        HideExitGate(ref index);
        HideItem(index);
    }
    void ArrangeEnemies()
    {
        Transform enemies = enemiesAndItemOfCurrentLevel.transform.GetChild(0);
        listOfPositionsCanFillEnemy = listOfPositions.Except(listOfBrickPositions).ToList();
        foreach (Transform enemy in enemies)
        {
            enemy.position = GetPositionOfEnemy();
            listOfPositionsCanFillEnemy.Remove(enemy.position);
        }
    }
    Vector2 GetPositionOfEnemy()
    {
        int index = Random.Range(0, listOfPositionsCanFillEnemy.Count);
        return listOfPositionsCanFillEnemy[index];
    }
    void HideExitGate(ref int index)
    {
        index = Random.Range(0, listOfBrickPositions.Count);
        exitGate.transform.position = listOfBrickPositions[index];
        RaycastHit2D hit = Physics2D.Raycast(exitGate.transform.position, Vector3.forward, 0.5f, LayerMask.GetMask("Brick"));
        brickOverExitGate = hit.collider.gameObject;
        exitGate.SetActive(false);
        isActivedExitGate = false;
    }
    void HideItem(int indexOfExitGate)
    {
        int index;
        item = enemiesAndItemOfCurrentLevel.transform.GetChild(1).gameObject;
        do
        {
            index = Random.Range(0, listOfBrickPositions.Count);
        } while (index == indexOfExitGate);
        item.transform.position = listOfBrickPositions[index];
        RaycastHit2D hit = Physics2D.Raycast(item.transform.position, Vector3.forward, 0.5f, LayerMask.GetMask("Brick"));
        brickOverItem = hit.collider.gameObject;
        item.SetActive(false);
        isActiveItem = false;
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
        timeRemain -= Time.fixedDeltaTime;
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
        if (!brickOverItem.activeSelf)
        {
            item.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            item.GetComponent<Collider2D>().enabled = true;
            isActiveItem = true;
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
        Destroy(mapOfCurrentLevel);
        Destroy(enemiesAndItemOfCurrentLevel);
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
            Destroy(mapOfCurrentLevel);
            Destroy(enemiesAndItemOfCurrentLevel);
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