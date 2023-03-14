using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class GameData
{
    public static float speed = 2f;
    public static int left = 2;
    public static int stage = 1;
    public static int score = 0;
    public static int flame = 1;
}
public class GameManager : MonoBehaviour
{
    [SerializeField] private Text time;
    [SerializeField] private Text score;
    [SerializeField] private Text left;
    [SerializeField] private Text stage;
    [SerializeField] private GameObject exitGate;
    private float timeRemain = 200f;
    private GameObject brickOverExitGate;

    private static GameManager instance;
    public static GameManager Instance { get => instance; }
    private void Awake()
    {
        GameManager.instance = this;
    }

    void Start()
    {
        time.text = timeRemain.ToString();
        score.text = GameData.score.ToString();
        left.text = GameData.left.ToString();
        RaycastHit2D hit = Physics2D.Raycast(exitGate.transform.position, Vector3.forward);
        brickOverExitGate = hit.collider.gameObject;
        exitGate.SetActive(false);
    }

    void Update()
    {
        SetTimeGame();
        StartCoroutine(CanActiveExitGate());
    }
    void SetTimeGame()
    {
        timeRemain -= Time.deltaTime;
        time.text = ((int)timeRemain).ToString();
    }
    IEnumerator CanActiveExitGate()
    {
        if (!brickOverExitGate.activeSelf)
        {
            exitGate.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            exitGate.GetComponent<Collider2D>().enabled = true;
        }
    }
    public void SetGameScore(int s) {
        GameData.score += s;
        score.text = GameData.score.ToString();
    }
    public void Pause()
    {
        Time.timeScale = 0;
    }
    public void Continue()
    {
        Time.timeScale = 1;
    }
}
