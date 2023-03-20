using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> bombs;
    [SerializeField] private GameObject bombPrefab;
    private static BombSpawner instance;
    public static BombSpawner Instance { get => instance; }

    private void Awake()
    {
        BombSpawner.instance = this;
    }
    public void AddBomb()
    {
        GameObject bomb = Instantiate(bombPrefab);
        this.bombs.Add(bomb);
        bomb.transform.parent = transform;
        bomb.SetActive(false);
    }
    public void Destroy(GameObject bomb)
    {
        this.bombs.Add(bomb);
        bomb.SetActive(false);
    }

    public GameObject GetBombFromPool()
    {
        foreach (GameObject bomb in bombs)
        {
            bombs.Remove(bomb);
            return bomb;
        }
        return null;
    }
    public void Spawn(Vector3 position)
    {
        if (Physics2D.OverlapCircle(position, 0.1f, LayerMask.GetMask("Brick")))
            return;
        GameObject bomb = GetBombFromPool();
        if (bomb == null)
            return;
        AudioManager.Instance.PlayAudioPutBomb();
        bomb.GetComponent<Collider2D>().isTrigger = true;
        bomb.transform.position = position;
        bomb.SetActive(true);
    }
}
