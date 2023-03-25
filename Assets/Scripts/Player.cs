using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
    public bool canPutBomb = true;
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
    void Update()
    {
        if (isDead || isCompleted)
            return;
        Move();
        Bomb();
        Detonate();
    }
    private void Move()
    {
        float move_x = InputManager.Instance.GetAxisRaw("Horizontal");
        float move_y = InputManager.Instance.GetAxisRaw("Vertical");
        if (move_x != 0 && move_y == 0)
        {
            time += Time.deltaTime;
            if (!animator.isActiveAndEnabled)
                animator.enabled = true;
            if (move_x > 0)
                animator.Play("GoRight");
            else
                animator.Play("GoLeft");
            if (time >= timeDelay)
            {
                AudioManager.Instance.PlayAudioLeftRight();
                time = 0f;
            }
        }
        if (move_y != 0)
        {
            time += Time.deltaTime;
            if (!animator.isActiveAndEnabled)
                animator.enabled = true;
            if (move_y > 0)
                animator.Play("GoBack");
            else
                animator.Play("GoAhead");
            if (time >= timeDelay)
            {
                AudioManager.Instance.PlayAudioUpDown();
                time = 0f;
            }
        }
        if (move_x == 0f && move_y == 0f && !isDead)
        {
            animator.enabled = false;
            time = timeDelay;
        }
        rig.MovePosition((Vector2)transform.position + new Vector2(move_x, move_y) * GameData.speed * Time.deltaTime);
    }
    private void Bomb()
    {
        if (InputManager.Instance.GetBomb() && canPutBomb)
        {
            Vector3 pos = new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), 0);
            if (BombSpawner.Instance.Spawn(pos))
            {
                canPutBomb = false;
            }
        }
    }

    private void Detonate()
    {
        if (GameData.detonator == 1 && InputManager.Instance.GetDetonator())
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
        AudioManager.Instance.PlayAudioJustDied();
        isDead = true;
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
        isCompleted = false;
        isDead = false;
        transform.position = startingPlayerPosition;
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
            if (!canPutBomb)
                Enemy.Instance.ReverseDirection();
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
            animator.Play("Start");
            rig.velocity = Vector2.zero;
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
                break;
            case "Mystery":
                GameData.mystery = 1;
                yield return new WaitForSeconds(30f);
                GameData.mystery = 0;
                break;
            case "Detonator":
                GameData.detonator = 1;
                UIManager.Instance.AcitveButtonExplode();
                break;
        }
    }
}
