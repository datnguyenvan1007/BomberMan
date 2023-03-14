using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    [SerializeField] private float timeDelay;
    private Animator animator;
    private Rigidbody2D rig;
    private bool isDead = false;
    private float time = 0f;
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        BombSpawner.Instance.AddBomb();
    }
    void Update()
    {
        Move();
        Bomb();
    }
    public void Move()
    {
        if (isDead)
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
        if (InputManager.Instance.GetBomb())
        {
            Vector3 pos = new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), 0);
            BombSpawner.Instance.Spawn(pos);
            AudioManager.Instance.PlayAudioPutBomb();
        }
    }

    public void Destroy()
    {
        rig.velocity = Vector2.zero;
        isDead = true;
        animator.enabled = true;
        animator.Play("Die");
        Invoke("Disable", 1f);
    }
    private void Disable()
    {
        gameObject.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "ExitGate")
        {
            transform.position = collision.transform.position;
            animator.Play("Start");
            AudioManager.Instance.PlayAudioLevelComplete();
        }
    }
}
