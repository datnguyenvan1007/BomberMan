using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Text time;
    [SerializeField] private Text score;
    [SerializeField] private Text left;
    [SerializeField] private Text stage;
    [SerializeField] private GameObject startingScene;
    [SerializeField] private GameObject playingScene;
    [SerializeField] private GameObject winScene;
    [SerializeField] private GameObject loseScene;
    [SerializeField] private GameObject joystickControl;
    [SerializeField] private GameObject dpadControl;
    [SerializeField] private List<Image> controllers;
    [SerializeField] private Image soundOn;
    [SerializeField] private Image soundOff;
    [SerializeField] private Image dpadSelection;
    [SerializeField] private Image joystickSelection;
    [SerializeField] private Image flipOn;
    [SerializeField] private Image flipOff;
    [SerializeField] private Image buttonContinue;
    [SerializeField] private Image buttonMainMenu;
    [SerializeField] private Image buttonYesOfPromptPanel;
    [SerializeField] private Image buttonNoOfPromptPanel;
    [SerializeField] private Sprite[] spritesOn;
    [SerializeField] private Sprite[] spritesOff;
    [SerializeField] private Sprite[] spritesOfDpadSelection;
    [SerializeField] private Sprite[] spritesOfJoystickSelection;
    [SerializeField] private Sprite[] spritesOfButtonContinue;
    [SerializeField] private Sprite[] spritesOfButtonMainMenu;
    [SerializeField] private Sprite[] spritesOfButtonYes;
    [SerializeField] private Sprite[] spritesOfButtonNo;
    private static UIManager instance;
    public static UIManager Instance { get => instance; }
    private void Awake()
    {
        UIManager.instance = this;
    }
    private void Start()
    {
        SelectSound(PlayerPrefs.GetInt("Sound", 1));
    }
    public void OnStartingLevel()
    {
        playingScene.SetActive(false);
        stage.text = "STAGE " + PlayerPrefs.GetInt("Stage", 1);
        left.text = PlayerPrefs.GetInt("Left", 2).ToString();
        startingScene.SetActive(true);
    }
    public void OnPlayingLevel()
    {
        startingScene.SetActive(false);
        playingScene.SetActive(true);
    }
    public void SetControllerOpacity(float a)
    {
        foreach (var c in controllers)
        {
            c.color = new Color(c.color.r, c.color.g, c.color.b, a);
        }
    }
    public void SetAcitveControllerType(int type)
    {
        if (type == 1)
        {
            dpadControl.SetActive(false);
            joystickControl.SetActive(true);
        }
        else
        {
            dpadControl.SetActive(true);
            joystickControl.SetActive(false);
        }
    }
    public void SetGameScore(int s)
    {
        GameData.score += s;
        score.text = GameData.score.ToString();
    }
    public void SetTimeGame(float t)
    {
        time.text = ((int)t).ToString();
    }
    public void SetValueStageAndLeft(int stageValue, int leftValue)
    {
        stage.text = "STAGE " + stageValue;
        left.text = leftValue.ToString();
    }
    public void SetActivePlayingScene(bool isActive)
    {
        playingScene.SetActive(isActive);
    }
    public void SetActiveStartingScene(bool isActive)
    {
        startingScene.SetActive(isActive);
    }
    public void ActiveWinScene()
    {
        winScene.SetActive(true);
    }
    public void ActiveLoseScene()
    {
        loseScene.SetActive(true);
    }
    public void Pause()
    {
        Time.timeScale = 0;
    }
    public void SelectSound(int mode)
    {
        if (mode == 1)
        {
            soundOn.sprite = spritesOn[1];
            soundOff.sprite = spritesOff[0];
            AudioManager.Instance.Play();
            PlayerPrefs.SetInt("Sound", 1);
        }
        else
        {
            soundOn.sprite = spritesOn[0];
            soundOff.sprite = spritesOff[1];
            AudioManager.Instance.Stop();
            PlayerPrefs.SetInt("Sound", 0);
        }
    }
    public void OnPointerDownContinue()
    {
        buttonContinue.sprite = spritesOfButtonContinue[1];
    }
    public void OnPointerUpContinue()
    {
        buttonContinue.sprite = spritesOfButtonContinue[0];
        Time.timeScale = 1;
    }
    public void OnPointerDownMainMenu()
    {
        buttonMainMenu.sprite = spritesOfButtonMainMenu[1];
    }
    public void OnPointerUpMainMenu()
    {
        buttonMainMenu.sprite = spritesOfButtonMainMenu[0];
    }
    public void OnPointerDownButtonYesOfPromptPanel()
    {
        buttonYesOfPromptPanel.sprite = spritesOfButtonYes[1];
    }
    public void OnPointerUpButtonYesOfPromptPanel()
    {
        buttonYesOfPromptPanel.sprite = spritesOfButtonYes[0];
        SceneManager.LoadScene(0);
    }
    public void OnPointerDownButtonNoOfPromptPanel()
    {
        buttonNoOfPromptPanel.sprite = spritesOfButtonNo[1];
    }
    public void OnPointerUpButtonNoOfPromptPanel()
    {
        buttonNoOfPromptPanel.sprite = spritesOfButtonNo[0];
    }
}
