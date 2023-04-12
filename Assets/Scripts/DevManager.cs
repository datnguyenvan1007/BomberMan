using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DevManager : MonoBehaviour
{
    [SerializeField] private Text immortalText;
    [SerializeField] private Text bombPassText;
    [SerializeField] private Text wallPassText;
    [SerializeField] private Text flamePassText;
    [SerializeField] private Text detonatorText;
    [SerializeField] private Button nextLevel;
    public static DevManager instance;
    private void Start() {
        instance = this;
        GameData.hackDetonator = false;
        GameData.hackFlame = GameData.flame;
        GameData.hackBombPass = false;
        GameData.hackImmortal = false;
        GameData.hackWallPass = false;
        GameData.hackFlamePass = false;
    }

    public void HackDetonator() {
        if (!GameData.hackDetonator) {
            GameData.hackDetonator = true;
            detonatorText.text = "On";
            UIManager.instance.SetActiveButtonDetonator(1);
        }
        else {
            GameData.hackDetonator = false;
            detonatorText.text = "Off";
            UIManager.instance.SetActiveButtonDetonator(GameData.detonator);
        }
    }
    public void HackBomPass() {
        if (!GameData.hackBombPass) {
            GameData.hackBombPass = true;
            bombPassText.text = "On";
            PoolBomb.instance.SetTriggerForBomb(true);
        }
        else {
            GameData.hackBombPass = false;
            bombPassText.text = "Off";
            if (GameData.bombPass == 0) {
                PoolBomb.instance.SetTriggerForBomb(false);
            }
        }
    }
    public void HackWallPass() {
        if (!GameData.hackWallPass) {
            GameData.hackWallPass = true;
            wallPassText.text = "On";
            PoolBrick.instance.SetTriggerAllBricks(true);
        }
        else {
            GameData.hackWallPass = false;
            wallPassText.text = "Off";
            if (GameData.wallPass == 0) {
                PoolBrick.instance.SetTriggerAllBricks(false);
            }
        }
    }
    public void HackFlamePass() {
        if (!GameData.hackFlamePass) {
            GameData.hackFlamePass = true;
            flamePassText.text = "On";
        }
        else {
            GameData.hackFlamePass = false;
            flamePassText.text = "Off";
        }
    }
    public void HackBomb() {
        PoolBomb.instance.AddBomb();
        GameData.hackBomb = true;
    }
    public void HackFlame() {
        GameData.hackFlame++;
    }
    public void HackImmotal() {
        if (!GameData.hackImmortal) {
            GameData.hackImmortal = true;
            immortalText.text = "On";
        }
        else {
            GameData.hackImmortal = false;
            immortalText.text = "Off";
        }
    }
    public void NextLevel() {
        this.SetInteractableForButtonNextLevel(false);
        StartCoroutine(GameManager.instance.WinLevel());
    }
    public void SetTimeScale(int timeScale) {
        Time.timeScale = timeScale;
    }
    public void SetInteractableForButtonNextLevel(bool isInteracted) {
        nextLevel.interactable = isInteracted;
    }
}
