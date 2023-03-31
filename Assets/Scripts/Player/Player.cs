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
    private bool isDead = false;
    public static bool isCompleted = false;
    private float time = 0f;
    private Vector2 startingPlayerPosition;
    private Vector2 moveVector = Vector2.zero;
    private bool canPutBomb = true;
    public static bool hasJustPutBomb = false;
    private int MoveXHash = Animator.StringToHash("MoveX");
    private int MoveYHash = Animator.StringToHash("MoveY");
    private int StartHash = Animator.StringToHash("Start");
    private int DieHash = Animator.StringToHash("Die");
    private void Awake() {
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
        if (isDead || isCompleted)
            return;
        // moveVector.x = Input.GetAxisRaw("Horizontal");
        // moveVector.y = Input.GetAxisRaw("Vertical");
        // if (Input.GetKey(KeyCode.Space)) {
        //     PutBomb();
        // }
        // if (Input.GetKeyUp(KeyCode.F)) {
        //     Detonate();
        // }
        Move();
    }
    private void Move()
    {
        if (moveVector == Vector2.zero)
        {
            animator.speed = 0;
            time = timeDelay;
        }
        else
        {
            animator.speed = 1;
            time += Time.fixedDeltaTime;
            if (moveVector.y == 0)
            {
                animator.SetFloat(MoveYHash, moveVector.y);
                animator.SetFloat(MoveXHash, moveVector.x);
                if (time >= timeDelay)
                {
                    AudioManager.Instance.PlayAudioLeftRight();
                    time = 0f;
                }
            }
            if (moveVector.x == 0)
            {
                animator.SetFloat(MoveXHash, moveVector.x);
                animator.SetFloat(MoveYHash, moveVector.y);
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
                hasJustPutBomb = true;
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
        animator.Play(DieHash);
        Invoke("Disable", 1f);
    }
    private void Disable()
    {
        gameObject.SetActive(false);
        GameManager.Instance.Lose();
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
            AudioManager.Instance.PlayAudioFindTheItem();
        }
        if (tag == "Enemy" && GameData.mystery == 0)
        {
            if (canPutBomb)
                Die();
        }
        if (tag == "Explosion" && GameData.flamePass == 0 && GameData.mystery == 0)
        {
            Die();
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "ExitGate" && PoolEnemy.Instance.enemyAlive == 0)
        {
            if (isCompleted)
                return;
            isCompleted = true;
            rig.velocity = Vector2.zero;
            animator.Play(StartHash);
            AudioManager.Instance.Stop();
            transform.position = collision.transform.position;
            AudioManager.Instance.PlayAudioLevelComplete();
            StartCoroutine(GameManager.Instance.WinLevel());
        }

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Bomb")
        {
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
