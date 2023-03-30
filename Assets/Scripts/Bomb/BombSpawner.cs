using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> bombs;
    [SerializeField] private GameObject bombPrefab;
    private List<GameObject> waitingToExplode = new List<GameObject>();
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
    public bool Spawn(Vector3 position)
    {
        if (Physics2D.OverlapCircle(position, 0.1f, LayerMask.GetMask("Brick")))
            return false;
        GameObject bomb = GetBombFromPool();
        if (bomb == null)
            return false;
        if (GameData.detonator == 1)
        {
            waitingToExplode.Add(bomb);
        }
        AudioManager.Instance.PlayAudioPutBomb();
        bomb.GetComponent<Collider2D>().isTrigger = true;
        bomb.transform.position = position;
        bomb.SetActive(true);
        return true;
    }
    public void Detonate()
    {
        if (waitingToExplode.Count > 0)
        {
            waitingToExplode[0].GetComponent<Bomb>().Explode();
            waitingToExplode.RemoveAt(0);
        }
    }
    public void ExplodeAllBombs()
    {
        foreach (Transform bomb in gameObject.transform)
        {
            if (bomb.gameObject.activeSelf)
                Destroy(bomb.gameObject);
        }
        waitingToExplode.Clear();
    }
}
