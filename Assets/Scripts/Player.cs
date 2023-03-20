using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    [SerializeField] private float timeDelay;
    [SerializeField] private GameObject cam;
    [SerializeField] private float minX;
    [SerializeField] private float maxX;
    [SerializeField] private float smoothTime;
    private Vector3 velocity;
    private Animator animator;
    private Rigidbody2D rig;
    private Collider2D[] colliders;
    public static bool isDead = false;
    public static bool isCompleted = false;
    private float time = 0f;
    private Vector2 startingPlayerPosition;
    private Vector3 startingCameraPosition;
    private void Awake()
    {
        startingPlayerPosition = transform.position;
        startingCameraPosition = cam.transform.position;
    }
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        colliders = GetComponents<Collider2D>();
    }
    void Update()
    {
        Move();
        Bomb();
        if (transform.position.x < maxX && transform.position.x > minX)
        {
            Vector3 target = new Vector3(transform.position.x, cam.transform.position.y, cam.transform.position.z);
            cam.transform.position = Vector3.SmoothDamp(cam.transform.position, target, ref velocity, smoothTime);
        }
    }
    public void Move()
    {
        if (isDead || isCompleted)
            return;
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
        rig.velocity = new Vector2(move_x, move_y) * GameData.speed;
    }
    public void Bomb()
    {
        if (isCompleted)
            return;
        if (InputManager.Instance.GetBomb())
        {
            Vector3 pos = new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), 0);
            BombSpawner.Instance.Spawn(pos);
        }
    }

    public void Destroy()
    {
        if (isDead)
            return;
        colliders[0].isTrigger = true;
        AudioManager.Instance.PlayAudioJustDied();
        rig.velocity = Vector2.zero;
        isDead = true;
        animator.enabled = true;
        animator.Play("Die");
        Invoke("Disable", 1f);
    }
    private void Disable()
    {
        gameObject.SetActive(false);
        colliders[0].isTrigger = false;
        GameManager.Instance.Lose();
    }
    private void OnEnable()
    {
        isCompleted = false;
        isDead = false;
        transform.position = startingPlayerPosition;
        cam.transform.position = startingCameraPosition;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string tag = collision.gameObject.tag;
        if (tag == "ExitGate" && PoolEnemy.Instance.enemyAlive == 0)
        {
            if (isCompleted)
                return;
            isCompleted = true;
            animator.Play("Start");
            rig.velocity = Vector2.zero;
            transform.position = collision.transform.position;
            AudioManager.Instance.PlayAudioLevelComplete();
            StartCoroutine(GameManager.Instance.WinLevel());
        }
        if (GameData.bombPass == 1 && tag == "Bomb")
        {
            collision.GetComponent<Collider2D>().isTrigger = true;
        }
        if (GameData.wallPass == 1 && tag == "Brick")
        {
            collision.GetComponent<Collider2D>().isTrigger = true;
        }
        if (tag == "Items")
        {
            collision.gameObject.SetActive(false);
            StartCoroutine(GetItems(collision));  
            AudioManager.Instance.PlayAudioFindTheItem();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        string tag = collision.gameObject.tag;
        if (tag == "Bomb" || tag == "Brick")
        {
            collision.GetComponent<Collider2D>().isTrigger = false;
        }
    }
    private IEnumerator GetItems(Collider2D collision)
    {
        switch (collision.name)
        {
            case "Bombs":
                BombSpawner.Instance.AddBomb();
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
                yield return new WaitForSeconds(5f);
                GameData.mystery = 0;
                break;
        }
    }
}
