using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float timeDelay;
    [SerializeField] private FixedJoystick joystick;
    private Animator animator;
    private Rigidbody2D rig;
    private new Collider2D collider;
    public static bool isDead = false;
    public static bool isCompleted = false;
    private float time = 0f;
    private Vector2 startingPlayerPosition;
    private Vector2 moveDPad = Vector2.zero;
    private Vector2 moveJoystick = Vector2.zero;
    private Vector2 move = Vector2.zero;
    private bool isQuitBomb = true;
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
        // moveDPad.x = Input.GetAxisRaw("Horizontal");
        // moveDPad.y = Input.GetAxisRaw("Vertical");
        // if (moveDPad.x != 0 && moveDPad.y != 0) {
        //     moveDPad = Vector2.zero;
        // }
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
        moveJoystick = GetJoystick();
        move = moveDPad + moveJoystick;
        if (move == Vector2.zero)
        {
            animator.speed = 0;
            time = timeDelay;
        }
        else
        {
            animator.speed = 1;
            time += Time.fixedDeltaTime;
            animator.SetFloat(MoveXHash, move.x);
            animator.SetFloat(MoveYHash, move.y);
            if (time >= timeDelay)
            {
                if (move.x == 0)
                    AudioManager.instance.PlayAudioUpDown();
                else if (move.y == 0)
                    AudioManager.instance.PlayAudioLeftRight();
                time = 0f;
            }
        }
        rig.velocity = (moveDPad + moveJoystick) * GameData.speed;
    }
    private Vector2 GetJoystick()
    {
        if (!UIManager.instance.GetActiveJoystick())
            return Vector2.zero;
        Vector2 direction = joystick.Direction;
        if (direction.x >= 0.5f)
            direction.x = 1;
        else if (direction.x <= -0.5f)
            direction.x = -1;
        else
            direction.x = 0;
        if (direction.y >= 0.5f)
            direction.y = 1;
        else if (direction.y <= -0.5f)
            direction.y = -1;
        else
            direction.y = 0;
        if (direction.x != 0 && direction.y != 0)
            direction.x = 0;
        return direction;
    }
    public void PutBomb()
    {
        if (isDead || isCompleted)
            return;
        Vector3 pos = new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), 0);
        if (PoolBomb.instance.Spawn(pos))
        {
            isQuitBomb = false;
            Collider2D[] colliders = Physics2D.OverlapCircleAll(pos, 3.5f, LayerMask.GetMask("Enemy", "EnemyCanThrough"));
            foreach (Collider2D col in colliders)
            {
                col.GetComponent<Enemy>().CheckImpediment("Bomb");
            }
        }
    }
    public void Detonate()
    {
        if (isDead || isCompleted)
            return;
        if (GameData.detonator == 1 || GameData.hackDetonator)
        {
            PoolBomb.instance.Detonate();
        }
    }
    public void Die()
    {
        if (isCompleted || isDead)
            return;
        DevManager.instance.SetInteractableForButtonNextLevel(false);
        isDead = true;
        rig.velocity = Vector2.zero;
        AudioManager.instance.Stop();
        AudioManager.instance.PlayAudioJustDied();
        animator.enabled = true;
        animator.Play(DieHash);
        Invoke("Disable", 1f);
    }
    private void Disable()
    {
        gameObject.SetActive(false);
        GameManager.instance.Lose();
        moveDPad = Vector2.zero;
    }
    private void OnEnable()
    {
        isCompleted = false;
        isDead = false;
        transform.position = startingPlayerPosition;
    }

    public void OnMoveExit()
    {
        moveDPad = Vector2.zero;
    }
    public void OnMoveXEnter(int x)
    {
        if (isCompleted || isDead)
            return;
        moveDPad = new Vector2(x, 0);
    }
    public void OnMoveYEnter(int y)
    {
        if (isCompleted || isDead)
            return;
        moveDPad = new Vector2(0, y);
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
        if (collision.gameObject.tag == "ExitWay" && PoolEnemy.instance.enemyAlive == 0)
        {
            if (isCompleted)
                return;
            isCompleted = true;
            moveDPad = Vector2.zero;
            rig.velocity = Vector2.zero;
            animator.speed = 0;
            animator.Play(StartHash);
            AudioManager.instance.Stop();
            transform.position = collision.transform.position;
            AudioManager.instance.Stop();
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
                PoolBomb.instance.AddBomb();
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
                PoolBomb.instance.SetTriggerForBomb(true);
                break;
            case "FlamePass":
                GameData.flamePass = 1;
                break;
            case "WallPass":
                GameData.wallPass = 1;
                PoolBrick.instance.SetTriggerAllBricks(true);
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
