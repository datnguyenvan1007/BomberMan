using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolBrick : MonoBehaviour
{
    [SerializeField] private GameObject brickPrefab;
    [SerializeField] private List<GameObject> bricks = new List<GameObject>();
    public static PoolBrick instance;
    void Awake()
    {
        foreach (Transform brick in transform)
        {
            bricks.Add(brick.gameObject);
        }
        instance = this;
    }

    public void Spawn(Vector3 position)
    {
        GameObject brick = GetBrickFromPool();
        if (brick == null)
        {
            brick = Instantiate(brickPrefab, position, Quaternion.identity);
            brick.name = brickPrefab.name;
        }
        brick.transform.parent = transform;
        brick.transform.position = position;
        brick.SetActive(true);
    }

    public GameObject GetBrickFromPool()
    {
        foreach (GameObject brick in bricks)
        {
            bricks.Remove(brick);
            return brick;
        }
        return null;
    }
    public IEnumerator Despawn(GameObject brick)
    {
        yield return new WaitForSeconds(0.3f);
        this.bricks.Add(brick);
        brick.SetActive(false);
    }
    public void DespawnAll()
    {
        foreach (Transform brick in transform)
        {
            if (brick.gameObject.activeSelf)
                StartCoroutine(Despawn(brick.gameObject));
        }
    }
    public void SetTriggerAllBricks(bool isTrigger)
    {
        foreach (Transform brick in transform)
        {
            if (brick.gameObject.activeSelf)
                brick.GetComponent<Brick>().SetTrigger(isTrigger);
        }
    }
}
