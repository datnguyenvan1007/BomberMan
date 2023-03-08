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
        bomb.SetActive(false);
        bomb.transform.parent = transform;
    }
    public void Destroy(GameObject bomb)
    {
        this.bombs.Add(bomb);
        bomb.SetActive(false);
    }

    public GameObject GetBombFromPool()
    {
        foreach (GameObject b in bombs)
        {
            bombs.Remove(b);
            return b;
        }
        return null;
    }
    public void Spawn(Vector3 position)
    {
        GameObject bomb = GetBombFromPool();
        if (bomb == null)
            return;
        bomb.transform.parent = transform;
        bomb.transform.position = position;
        bomb.SetActive(true);
    }
}
