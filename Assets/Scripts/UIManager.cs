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
    [SerializeField] private GameObject buttonExplode;
    [SerializeField] private RectTransform uiControlMovement;
    [SerializeField] private RectTransform uiControlBomb;
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
    [SerializeField] private Sprite[] spritesOfButtonOn;
    [SerializeField] private Sprite[] spritesOfButtonOff;
    [SerializeField] private Sprite[] spritesOfDpadSelection;
    [SerializeField] private Sprite[] spritesOfJoystickSelection;
    [SerializeField] private Sprite[] spritesOfButtonContinue;
    [SerializeField] private Sprite[] spritesOfButtonMainMenu;
    [SerializeField] private Sprite[] spritesOfButtonYes;
    [SerializeField] private Sprite[] spritesOfButtonNo;
    private List<Vector2> controllersPosition = new List<Vector2>();
    public static UIManager instance;
    private void Awake()
    {
        UIManager.instance = this;
    }
    private void Start()
    {
        controllersPosition.Add(uiControlMovement.anchoredPosition);
        controllersPosition.Add(uiControlBomb.anchoredPosition);
        SelectSound(PlayerPrefs.GetInt("Sound", 1));
        SelectControllerType(PlayerPrefs.GetInt("ControllerType", 2));
        SelectFlipControls(PlayerPrefs.GetInt("FlipControls", 0));
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
    public void SetActiveButtonDetonator(int type)
    {
        if (uiControlBomb.gameObject.activeSelf)
        {
            if (type == 1)
            {
                buttonExplode.SetActive(true);
            }
            else
            {
                buttonExplode.SetActive(false);
            }
        }
    }
    public void SetGameScore(int s)
    {
        GameData.score += s;
        score.text = GameData.score.ToString();
    }
    public void SetTimeGame(int t)
    {
        time.text = t.ToString();
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
            soundOn.sprite = spritesOfButtonOn[1];
            soundOff.sprite = spritesOfButtonOff[0];
            PlayerPrefs.SetInt("Sound", 1);
        }
        else
        {
            soundOn.sprite = spritesOfButtonOn[0];
            soundOff.sprite = spritesOfButtonOff[1];
            PlayerPrefs.SetInt("Sound", 0);
        }
    }
    public void SelectControllerType(int mode)
    {
        if (mode == 1)
        {
            dpadSelection.sprite = spritesOfDpadSelection[0];
            joystickSelection.sprite = spritesOfJoystickSelection[1];
            PlayerPrefs.SetInt("ControllerType", 1);
            joystickControl.SetActive(true);
            dpadControl.SetActive(false);
        }
        else
        {
            dpadSelection.sprite = spritesOfDpadSelection[1];
            joystickSelection.sprite = spritesOfJoystickSelection[0];
            PlayerPrefs.SetInt("ControllerType", 2);
            joystickControl.SetActive(false);
            dpadControl.SetActive(true);
        }
    }
    public void SelectFlipControls(int mode)
    {
        if (mode == 1)
        {
            flipOn.sprite = spritesOfButtonOn[1];
            flipOff.sprite = spritesOfButtonOff[0];
            PlayerPrefs.SetInt("FlipControls", 1);
            uiControlMovement.anchoredPosition = controllersPosition[1];
            uiControlBomb.anchoredPosition = controllersPosition[0];
        }
        else
        {
            flipOn.sprite = spritesOfButtonOn[0];
            flipOff.sprite = spritesOfButtonOff[1];
            PlayerPrefs.SetInt("FlipControls", 0);
            uiControlMovement.anchoredPosition = controllersPosition[0];
            uiControlBomb.anchoredPosition = controllersPosition[1];
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
        if (PlayerPrefs.GetInt("Sound", 1) == 1) 
            AudioManager.instance.UnMute();
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
        Time.timeScale = 1;
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
    public bool GetActiveJoystick() {
        return joystickControl.activeSelf;
    }
}
