using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private float timeDelay;
    private Animator animator;
    private Rigidbody2D rig;
    private new Collider2D collider;
    private PlayerInput input;
    private bool isDead = false;
    public static bool isCompleted = false;
    private float time = 0f;
    private Vector2 startingPlayerPosition;
    private Vector2 moveVector = Vector2.zero;
    private bool canPutBomb = true;
    private void Awake()
    {
        startingPlayerPosition = transform.position;
        input = new PlayerInput();
    }
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        collider = GetComponent<Collider2D>();
    }
    void FixedUpdate()
    {
        if (isDead || isCompleted)
            return;
        Move();
    }
    private void Move()
    {
        if (moveVector == Vector2.zero)
        {
            animator.enabled = false;
            time = timeDelay;
        }
        else
        {
            animator.enabled = true;
            time += Time.fixedDeltaTime;
            if (moveVector.y == 0)
            {
                if (moveVector.x > 0)
                    animator.Play("GoRight");
                else
                    animator.Play("GoLeft");
                if (time >= timeDelay)
                {
                    AudioManager.Instance.PlayAudioLeftRight();
                    time = 0f;
                }
            }
            if (moveVector.x == 0)
            {
                if (moveVector.y > 0)
                    animator.Play("GoBack");
                else
                    animator.Play("GoAhead");
                if (time >= timeDelay)
                {
                    AudioManager.Instance.PlayAudioUpDown();
                    time = 0f;
                }
            }
        }
        rig.velocity = moveVector * GameData.speed;
    }
    public void PutBomb()
    {
        if (isDead || isCompleted)
            return;
        if (canPutBomb)
        {
            Vector3 pos = new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), 0);
            if (BombSpawner.Instance.Spawn(pos))
            {
                canPutBomb = false;
            }
        }
    }
    public void Detonate()
    {
        if (isDead || isCompleted)
            return;
        if (GameData.detonator == 1)
        {
            BombSpawner.Instance.Detonate();
        }
    }
    public void Die()
    {
        if (isCompleted)
            return;
        if (isDead)
            return;
        isDead = true;
        rig.velocity = Vector2.zero;
        AudioManager.Instance.PlayAudioJustDied();
        animator.enabled = true;
        animator.Play("Die");
        Invoke("Disable", 1f);
    }
    private void Disable()
    {
        gameObject.SetActive(false);
        GameManager.Instance.Lose();
    }
    private void OnEnable()
    {
        input.Enable();
        input.Player.Move.performed += OnMovePerformed;
        input.Player.Move.canceled += OnMoveCanceled;
        input.Player.PutBomb.performed += OnPutBombPerformed;
        input.Player.Detonate.performed += OnDetonatePerformed;

        isCompleted = false;
        isDead = false;
        transform.position = startingPlayerPosition;
    }
    private void OnDisable()
    {
        input.Disable();
        input.Player.Move.performed -= OnMovePerformed;
        input.Player.Move.canceled -= OnMoveCanceled;
        input.Player.PutBomb.performed -= OnPutBombPerformed;
        input.Player.Detonate.performed -= OnDetonatePerformed;
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        moveVector = context.ReadValue<Vector2>();
        moveVector = new Vector2(Mathf.Round(moveVector.x), Mathf.Round(moveVector.y));
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveVector = Vector2.zero;
    }

    private void OnPutBombPerformed(InputAction.CallbackContext context)
    {
        PutBomb();
    }

    private void OnDetonatePerformed(InputAction.CallbackContext context)
    {
        Detonate();
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
            AudioManager.Instance.PlayAudioFindTheItem();
        }
        if (tag == "Enemy" && GameData.mystery == 0)
        {
            if (canPutBomb)
                Die();
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        string tag = collision.gameObject.tag;
        if (tag == "ExitGate" && PoolEnemy.Instance.enemyAlive == 0)
        {
            if (isCompleted)
                return;
            isCompleted = true;
            rig.velocity = Vector2.zero;
            animator.Play("Start");
            AudioManager.Instance.Stop();
            transform.position = collision.transform.position;
            AudioManager.Instance.PlayAudioLevelComplete();
            StartCoroutine(GameManager.Instance.WinLevel());
        }

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        string tag = collision.gameObject.tag;
        if (tag == "Bomb")
        {
            if (GameData.bombPass == 0)
                collision.GetComponent<Collider2D>().isTrigger = false;
            canPutBomb = true;
        }

    }
    private IEnumerator GetItems(Collider2D collision)
    {
        switch (collision.name)
        {
            case "Bombs":
                BombSpawner.Instance.AddBomb();
                GameData.numberOfBombs++;
                break;
            case "Flames":
                GameData.flame++;
                break;
            case "Speed":
                GameData.speed += 1;
                break;
            case "BombPass":
                GameData.bombPass = 1;
                break;
            case "FlamePass":
                GameData.flamePass = 1;
                break;
            case "WallPass":
                GameData.wallPass = 1;
                SetTriggerAllBricks();
                break;
            case "Mystery":
                GameData.mystery = 1;
                yield return new WaitForSeconds(30f);
                GameData.mystery = 0;
                break;
            case "Detonator":
                GameData.detonator = 1;
                UIManager.Instance.SetActiveButtonDetonator(1);
                break;
        }
    }
    private void SetTriggerAllBricks()
    {
        Transform brokenBricks = GameObject.Find("BrokenBricks").transform;
        foreach (Transform brick in brokenBricks)
        {
            brick.GetComponent<Brick>().SetTrigger(true);
        }
    }
}
