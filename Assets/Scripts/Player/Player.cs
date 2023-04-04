using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private float timeDelay;
    [SerializeField] private FixedJoystick joystick;
    private Animator animator;
    private Rigidbody2D rig;
    private new Collider2D collider;
    private bool isDead = false;
    public static bool isCompleted = false;
    private float time = 0f;
    private Vector2 startingPlayerPosition;
    private Vector2 moveVector = Vector2.zero;
    private bool isQuitBomb = true;
    public static bool hasJustPutBomb = false;
    private int MoveXHash = Animator.StringToHash("MoveX");
    private int MoveYHash = Animator.StringToHash("MoveY");
    private int StartHash = Animator.StringToHash("Start");
    private int DieHash = Animator.StringToHash("Die");
    private void Awake()
    {
        startingPlayerPosition = transform.position;
    }
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        collider = GetComponent<Collider2D>();
    }
    void FixedUpdate()
    {
        // moveVector.x = Input.GetAxisRaw("Horizontal");
        // moveVector.y = Input.GetAxisRaw("Vertical");
        Move();
    }
    private void Move()
    {
        if (isDead || isCompleted)
        {
            animator.SetFloat(MoveYHash, 0f);
            animator.SetFloat(MoveXHash, 0f);
            return;
        }
        if (moveVector == Vector2.zero && joystick.Direction == Vector2.zero)
        {
            animator.speed = 0;
            time = timeDelay;
        }
        else
        {
            animator.speed = 1;
            time += Time.fixedDeltaTime;
            if (moveVector.y == 0 && joystick.Direction.y == 0)
            {
                animator.SetFloat(MoveXHash, moveVector.x + joystick.Direction.x);
                animator.SetFloat(MoveYHash, moveVector.y + joystick.Direction.y);
                if (time >= timeDelay)
                {
                    AudioManager.instance.PlayAudioLeftRight();
                    time = 0f;
                }
            }
            if (moveVector.x == 0 && joystick.Direction.x == 0)
            {
                animator.SetFloat(MoveXHash, moveVector.x + joystick.Direction.x);
                animator.SetFloat(MoveYHash, moveVector.y + joystick.Direction.y);
                if (time >= timeDelay)
                {
                    AudioManager.instance.PlayAudioUpDown();
                    time = 0f;
                }
            }
        }
        rig.velocity = (moveVector + joystick.Direction) * GameData.speed;
    }
    public void PutBomb()
    {
        if (isDead || isCompleted)
            return;
        Vector3 pos = new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), 0);
        if (BombSpawner.instance.Spawn(pos))
        {
            hasJustPutBomb = true;
            isQuitBomb = false;
        }
    }
    public void Detonate()
    {
        if (isDead || isCompleted)
            return;
        if (GameData.detonator == 1 || GameData.hackDetonator)
        {
            BombSpawner.instance.Detonate();
        }
    }
    public void Die()
    {
        if (isCompleted || isDead)
            return;
        DevManager.instance.SetInteractableForButtonNextLevel(false);
        isDead = true;
        rig.velocity = Vector2.zero;
        AudioManager.instance.PlayAudioJustDied();
        animator.enabled = true;
        animator.Play(DieHash);
        Invoke("Disable", 1f);
    }
    private void Disable()
    {
        gameObject.SetActive(false);
        GameManager.instance.Lose();
        moveVector = Vector2.zero;
    }
    private void OnEnable()
    {
        isCompleted = false;
        isDead = false;
        transform.position = startingPlayerPosition;
    }

    public void OnMoveExit()
    {
        moveVector = Vector2.zero;
    }
    public void OnMoveXEnter(int x)
    {
        moveVector = new Vector2(x, 0);
    }
    public void OnMoveYEnter(int y)
    {
        moveVector = new Vector2(0, y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string tag = collision.gameObject.tag;
        if (tag == "Items")
        {
            collision.gameObject.SetActive(false);
            StartCoroutine(GetItems(collision));
            AudioManager.instance.PlayAudioFindTheItem();
        }
        if (tag == "Enemy" && GameData.mystery == 0 && !GameData.hackImmortal)
        {
            if (isQuitBomb)
                Die();
        }
        if (tag == "Explosion" && GameData.flamePass == 0 && GameData.mystery == 0 && !GameData.hackImmortal && !GameData.hackFlamePass)
        {
            Die();
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "ExitGate" && PoolEnemy.instance.enemyAlive == 0)
        {
            if (isCompleted)
                return;
            isCompleted = true;
            rig.velocity = Vector2.zero;
            animator.Play(StartHash);
            AudioManager.instance.Stop();
            transform.position = collision.transform.position;
            AudioManager.instance.PlayAudioLevelComplete();
            DevManager.instance.SetInteractableForButtonNextLevel(false);
            StartCoroutine(GameManager.instance.WinLevel());
        }

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Bomb")
        {
            isQuitBomb = true;
        }

    }
    private IEnumerator GetItems(Collider2D collision)
    {
        switch (collision.name)
        {
            case "Bombs":
                BombSpawner.instance.AddBomb();
                GameData.numberOfBombs++;
                break;
            case "Flames":
                GameData.flame++;
                GameData.hackFlame++;
                break;
            case "Speed":
                GameData.speed += 1;
                break;
            case "BombPass":
                GameData.bombPass = 1;
                BombSpawner.instance.SetTriggerForBomb(true);
                break;
            case "FlamePass":
                GameData.flamePass = 1;
                break;
            case "WallPass":
                GameData.wallPass = 1;
                GameManager.instance.SetTriggerAllBricks(true);
                break;
            case "Mystery":
                GameData.mystery = 1;
                yield return new WaitForSeconds(30f);
                GameData.mystery = 0;
                break;
            case "Detonator":
                GameData.detonator = 1;
                UIManager.instance.SetActiveButtonDetonator(1);
                break;
        }
    }

}
