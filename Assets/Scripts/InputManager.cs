using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EasyJoystick;

public class InputManager : MonoBehaviour
{
    [SerializeField] private GameObject uiControlMovement;
    [SerializeField] private GameObject DPad;
    [SerializeField] private GameObject joystick;
    [SerializeField] private GameObject uiControlBomb;
    [SerializeField] private GameObject buttonExplode;
    private Joystick joy;
    private float moveX = 0;
    private float moveY = 0;
    private bool isPressedBomb = false;
    private bool isPressedExplode = false;

    private void Start()
    {
        joy = joystick.GetComponent<Joystick>();
    }

    private static InputManager instance;
    public static InputManager Instance { get => instance; }
    private void Awake()
    {
        InputManager.instance = this;
    }
    public float GetAxisRaw(string axis)
    {
        if (uiControlMovement.activeSelf) { 
            if (DPad.activeSelf && !joystick.activeSelf)
            {
                if (axis.Equals("Horizontal"))
                {
                    return moveX;
                }
                if (axis.Equals("Vertical"))
                {
                    return moveY;
                }
            }
            if (!DPad.activeSelf && joystick.activeSelf)
            {
                if (axis.Equals("Horizontal"))
                {
                    return joy.Horizontal();
                }
                if (axis.Equals("Vertical"))
                {
                    return joy.Vertical();
                }
            }
        }
        return Input.GetAxisRaw(axis);
    }
    public bool GetBomb()
    {
        if (uiControlBomb.activeSelf)
            return isPressedBomb;
        return Input.GetKeyDown(KeyCode.Space);
    }
    public bool GetDetonator()
    {
        if (buttonExplode.activeSelf)
            return isPressedExplode;
        return Input.GetKeyDown(KeyCode.F);
    }
    public void Bomb(bool value)
    {
        isPressedBomb = value;
    }
    public void Detonate(bool value)
    {
        isPressedExplode = value;
    }
    public void SetValueMoveX(float value)
    {
        moveX = value;
    }
    public void SetValueMoveY(float value)
    {
        moveY = value;
    }
}
