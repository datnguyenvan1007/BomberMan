using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolBomb : MonoBehaviour
{
    [SerializeField] private List<GameObject> bombs = new List<GameObject>();
    [SerializeField] private GameObject bombPrefab;
    private List<GameObject> waitingToExplode = new List<GameObject>();
    public static PoolBomb instance;

    private void Awake()
    {
        PoolBomb.instance = this;
    }
    public void AddBomb()
    {
        GameObject bomb = Instantiate(bombPrefab);
        this.bombs.Add(bomb);
        bomb.transform.parent = transform;
        bomb.SetActive(false);
    }
    public void Despawn(GameObject bomb)
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
        if (Physics2D.OverlapCircle(position, 0.1f, LayerMask.GetMask("Brick", "Bomb", "Enemy", "EnemyCanThrough")))
            return false;
        GameObject bomb = GetBombFromPool();
        if (bomb == null)
            return false;
        if (GameData.detonator == 1 || GameData.hackDetonator)
        {
            waitingToExplode.Add(bomb);
        }
        AudioManager.instance.PlayAudioPutBomb();
        bomb.transform.position = position;
        bomb.SetActive(true);
        return true;
    }
    public void RemoveLastBomb() {
        bombs.RemoveAt(bombs.Count - 1);
        Destroy(transform.GetChild(transform.childCount - 1).gameObject);
    }
    public int Count() {
        return this.bombs.Count;
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
                Despawn(bomb.gameObject);
        }
        waitingToExplode.Clear();
    }
    public void SetTriggerForBomb(bool isTrigger) {
        foreach (Transform bomb in gameObject.transform)
        {
            if (bomb.gameObject.activeSelf)
                bomb.GetComponent<Collider2D>().isTrigger = isTrigger;
        }
    }
}
