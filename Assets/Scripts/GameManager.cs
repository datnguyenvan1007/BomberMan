using System;
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
    public static int hackFlame;
    public static bool hackBomb;
    public static bool hackWallPass;
    public static bool hackFlamePass;
    public static bool hackBombPass;
    public static bool hackDetonator;
    public static bool hackImmortal;
}
public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject levelObject;
    [SerializeField] private GameObject timeOut;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject exitWay;
    [SerializeField] private List<GameObject> enemiesAndItemPrefab;
    private GameObject enemiesAndItemOfCurrentLevel;
    private GameObject brickOverExitGate;
    private GameObject item;
    private GameObject brickOverItem;
    private List<Vector2> playerSafePositions;
    private List<Vector2> listOfBrickPositions;
    private List<Vector2> listOfPositionsCanFillBrick;
    private List<Vector2> listOfPositionsCanFill;
    private bool isPlayingLevel = false;
    private bool isActivedExitGate = false;
    private bool isActiveItem = false;
    private int time = 200;
    private float timeRemain;
    private int totalBrick = 56;
    public static GameManager instance;
    private void Awake()
    {
        Application.targetFrameRate = 60;
        GameManager.instance = this;
        listOfBrickPositions = new List<Vector2>();
        playerSafePositions = new List<Vector2>();
        listOfPositionsCanFillBrick = new List<Vector2>();
        listOfPositionsCanFill = new List<Vector2>();
        for (int i = 6; i >= -4; i--)
        {
            for (int j = -12; j <= 22; j++)
            {
                if (i % 2 == 0)
                {
                    listOfPositionsCanFillBrick.Add(new Vector2(j, i));
                }
                else
                {
                    if (j % 2 == 0)
                    {
                        listOfPositionsCanFillBrick.Add(new Vector2(j, i));
                    }
                }
            }
        }
        for (int i = -12; i <= -5; i++)
        {
            for (int j = 6; j >= 2; j--)
            {
                playerSafePositions.Add(new Vector2(i, j));
            }
        }
        listOfPositionsCanFillBrick.Remove(player.transform.position);
        listOfPositionsCanFillBrick.Remove(player.transform.position + Vector3.right);
        listOfPositionsCanFillBrick.Remove(player.transform.position + Vector3.down);
    }

    void Start()
    {
        GetValueForGameData();
        GameData.hackBomb = false;
        for (int i = 1; i <= GameData.numberOfBombs; i++)
            PoolBomb.instance.AddBomb();
        UIManager.instance.SetControllerOpacity(PlayerPrefs.GetFloat("ControllerOpacity", 45) / 100);
        UIManager.instance.SetAcitveControllerType(PlayerPrefs.GetInt("ControllerType", 2));
        UIManager.instance.SetActiveButtonDetonator(GameData.detonator);
        UIManager.instance.SetTimeGame(time);
        UIManager.instance.SetGameScore(0);
        StartCoroutine(LoadLevel());
    }

    void FixedUpdate()
    {
        SetTimeGame();
    }

    void GetValueForGameData()
    {
        GameData.speed = PlayerPrefs.GetFloat("Speed", 3.5f);
        GameData.numberOfBombs = PlayerPrefs.GetInt("NumberOfBombs", 1);
        if (!GameData.hackBomb && GameData.numberOfBombs < PoolBomb.instance.Count())
        {
            PoolBomb.instance.RemoveLastBomb();
        }
        if (GameData.hackFlame == GameData.flame)
            GameData.hackFlame = PlayerPrefs.GetInt("Flame", 1);
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
        if (PlayerPrefs.GetInt("Left", 2) > 0) {
            PlayerPrefs.SetInt("Left", PlayerPrefs.GetInt("Left", 2) - 1);
        }
    }

    IEnumerator LoadLevel()
    {
        TimeOut(false);
        player.SetActive(true);

        AudioManager.instance.Stop();

        UIManager.instance.SetActivePlayingScene(false);
        if (PlayerPrefs.GetInt("Stage", 1) >= enemiesAndItemPrefab.Count)
        {
            UIManager.instance.ActiveWinScene();
            DeleteAllData();
            Invoke("RedirectHome", 2f);
            yield break;
        }

        UIManager.instance.SetValueStageAndLeft(PlayerPrefs.GetInt("Stage", 1), PlayerPrefs.GetInt("Left", 2));
        UIManager.instance.SetActiveStartingScene(true);
        PoolBomb.instance.ExplodeAllBombs();

        AudioManager.instance.PlayAudioLevelStart();

        yield return new WaitForSeconds(3.0f);
        UIManager.instance.SetActiveStartingScene(false);
        DevManager.instance.SetInteractableForButtonNextLevel(true);
        InitMap();
        UIManager.instance.SetActivePlayingScene(true);
        AudioManager.instance.PlayAudioInGame();

        isPlayingLevel = true;
        timeRemain = 200;
        time = 200;
    }
    void InitMap()
    {
        int index;
        ArrangeBricks();
        ArrangeExitGate(out index);
        enemiesAndItemOfCurrentLevel = Instantiate(enemiesAndItemPrefab[PlayerPrefs.GetInt("Stage", 1) - 1], levelObject.transform);
        ArrangeItem(index);
        ArrangeEnemies();
    }
    void ArrangeBricks()
    {
        int index;
        int count = listOfPositionsCanFillBrick.Count;
        listOfPositionsCanFill.Clear();
        listOfPositionsCanFill.AddRange(listOfPositionsCanFillBrick);
        totalBrick += ((int)(PlayerPrefs.GetInt("Stage", 1) / 5));
        listOfBrickPositions.Clear();
        for (int i = 1; i <= totalBrick; i++)
        {
            index = UnityEngine.Random.Range(0, count);
            PoolBrick.instance.Spawn(listOfPositionsCanFill[index]);
            listOfBrickPositions.Add(listOfPositionsCanFill[index]);
            listOfPositionsCanFill.RemoveAt(index);
            count--;
        }
    }
    void ArrangeEnemies()
    {
        Transform enemies = enemiesAndItemOfCurrentLevel.transform.GetChild(0);
        for (int i = 0; i < playerSafePositions.Count; i++) {
            listOfPositionsCanFill.Remove(playerSafePositions[i]);
        }
        foreach (Transform enemy in enemies)
        {
            int index = GetIndexPositionOfEnemy();
            enemy.position = listOfPositionsCanFill[index];
            try
            {
                listOfPositionsCanFill.RemoveRange(index - 5, index + 5);
            }
            catch (Exception)
            {
                listOfPositionsCanFill.RemoveAt(index);
            }
        }
    }
    int GetIndexPositionOfEnemy()
    {
        return UnityEngine.Random.Range(0, listOfPositionsCanFill.Count);
    }
    void ArrangeExitGate(out int index)
    {
        exitWay.GetComponent<Items>().Hide();
        index = UnityEngine.Random.Range(0, listOfBrickPositions.Count);
        exitWay.transform.position = listOfBrickPositions[index];
        RaycastHit2D hit = Physics2D.Raycast(exitWay.transform.position, Vector3.forward, 0.5f, LayerMask.GetMask("Brick"));
        hit.collider.GetComponent<Brick>().SetObjectCovered(exitWay);
    }
    void ArrangeItem(int indexOfExitWay)
    {
        int index;
        item = enemiesAndItemOfCurrentLevel.transform.GetChild(1).gameObject;
        do
        {
            index = UnityEngine.Random.Range(0, listOfBrickPositions.Count);
        } while (index == indexOfExitWay);
        item.transform.position = listOfBrickPositions[index];
        RaycastHit2D hit = Physics2D.Raycast(item.transform.position, Vector3.forward, 0.5f, LayerMask.GetMask("Brick"));
        hit.collider.GetComponent<Brick>().SetObjectCovered(item);
    }
    void SetTimeGame()
    {
        if (!isPlayingLevel)
            return;
        if (time == 0)
            return;
        timeRemain -= Time.fixedDeltaTime;
        if (timeRemain < 1f && !timeOut.activeSelf)
        {
            if (Player.isCompleted)
                return;
            TimeOut(true);
            PoolEnemy.instance.enemyAlive += 7;
            return;
        }
        if (timeRemain < time)
        {
            time--;
            UIManager.instance.SetTimeGame(time);
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
        PoolBrick.instance.DespawnAll();
        Destroy(enemiesAndItemOfCurrentLevel);
        player.SetActive(false);
        StartCoroutine(LoadLevel());
    }
    public void Lose()
    {
        StartCoroutine(GoToPreviousLevel());
    }
    private IEnumerator GoToPreviousLevel()
    {
        SaveDataWhenLosing();
        yield return new WaitForSeconds(2f);
        isPlayingLevel = false;
        if (PlayerPrefs.GetInt("Left", 2) > 0)
        {
            PoolBrick.instance.DespawnAll();
            Destroy(enemiesAndItemOfCurrentLevel);
            GetValueForGameData();
            UIManager.instance.SetGameScore(0);
            StartCoroutine(LoadLevel());
        }
        else
        {
            UIManager.instance.SetActivePlayingScene(false);
            UIManager.instance.ActiveLoseScene();
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
        PlayerPrefs.DeleteKey("Detonator");
    }
    void RedirectHome()
    {
        SceneManager.LoadScene(0);
    }
}