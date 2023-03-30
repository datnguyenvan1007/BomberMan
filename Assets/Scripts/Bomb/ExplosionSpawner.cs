using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionSpawner : MonoBehaviour
{
    [SerializeField] private GameObject rootPrefab;
    [SerializeField] private GameObject bodyPrefab;
    [SerializeField] private GameObject headPrefab;
    private List<GameObject> explosions = new List<GameObject>();
    private static ExplosionSpawner instance;
    public static ExplosionSpawner Instance { get => instance; }
    private void Awake()
    {
        ExplosionSpawner.instance = this;
    }

    private void Spawn(GameObject objectPrefab, Vector2 position, Vector3 rotation)
    {
        GameObject ex = GetObjectFromPoolByName(objectPrefab.name);
        if (ex == null)
        {
            ex = Instantiate(objectPrefab, position, Quaternion.Euler(rotation));
            ex.name = objectPrefab.name;
        }
        else
        {
            ex.transform.Rotate(rotation);
        }
        ex.transform.parent = transform;
        ex.transform.position = position;
        ex.SetActive(true);
    }

    public void Explode(Transform trans)
    {
        Spawn(rootPrefab, trans.position, Vector3.zero);
        ExplodeDirection(trans, Vector2.up, new Vector3(0, 0, 0));
        ExplodeDirection(trans, Vector2.down, new Vector3(0, 0, 180));
        ExplodeDirection(trans, Vector2.left, new Vector3(0, 0, 90));
        ExplodeDirection(trans, Vector2.right, new Vector3(0, 0, -90));
    }
    private void ExplodeDirection(Transform trans, Vector2 direction, Vector3 rotation)
    {
        bool isSmallerRange = false;
        Vector2 pos = trans.position;
        int count = GameData.flame;
        RaycastHit2D[] hits = Physics2D.RaycastAll(trans.position, direction, GameData.flame);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.CompareTag("Wall"))
            {
                count = Mathf.RoundToInt(hit.distance);
                if (count < GameData.flame)
                {
                    isSmallerRange = true;
                }
                break;
            }
            if (hit.collider.CompareTag("Brick"))
            {
                count = Mathf.RoundToInt(hit.distance);
                if (count < GameData.flame)
                {
                    isSmallerRange = true;
                }
                Collider2D col = Physics2D.OverlapCircle(hit.transform.position, 0.4f, LayerMask.GetMask("EnemyCanThrough"));
                hit.collider.GetComponent<Brick>().Destroy();
                if (col)
                {
                    col.gameObject.GetComponent<Enemy>().Die();
                }
                break;
            }
            if (hit.collider.tag == "Bomb")
            {
                count = Mathf.RoundToInt(hit.distance);
                if (count < GameData.flame)
                {
                    isSmallerRange = true;
                }
                break;
            }
        }
        while (count > 1)
        {
            pos += direction;
            Spawn(bodyPrefab, pos, rotation);
            count--;
        }
        if (count == 0)
            return;
        if (isSmallerRange)
        {
            pos += direction;
            Spawn(bodyPrefab, pos, rotation);
        }
        else
        {
            pos += direction;
            Spawn(headPrefab, pos, rotation);
        }
    }

    private GameObject GetObjectFromPoolByName(string name)
    {
        foreach (GameObject ex in explosions) 
        {
            if (ex.name == name)
            {
                explosions.Remove(ex);
                return ex;
            }
        }
        return null;
    }
    public void Destroy(GameObject explosion)
    {
        explosion.transform.Rotate(0, 0, 360 - explosion.transform.eulerAngles.z);
        this.explosions.Add(explosion);
        explosion.SetActive(false);
    }
}