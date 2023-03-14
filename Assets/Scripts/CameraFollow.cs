using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float minX;
    [SerializeField] private float maxX;
    [SerializeField] private float speed;
    [SerializeField] private float smoothTime;
    private Vector3 velocity = Vector3.zero;
    void Update()
    {
        if (player.transform.position.x < maxX && player.transform.position.x > minX)
        {
            Vector3 target = new Vector3(player.transform.position.x, transform.position.y, transform.position.z);
            /*transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, smoothTime);*/
            /*Vector3 target = new Vector3(player.transform.position.x, transform.position.y, transform.position.z);*/
            /*transform.position = Vector3.Lerp(transform.position, target, speed * Time.deltaTime);*/
            transform.position = target;
        }
    }
}
