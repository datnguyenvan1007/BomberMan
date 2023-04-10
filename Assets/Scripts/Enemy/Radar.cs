using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radar : MonoBehaviour
{
    private SmartEnemy enemy;
    private Vector2 playerPosition;
    private bool isEntered = false;
    void Start()
    {
        enemy = transform.parent.GetComponent<SmartEnemy>();
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            isEntered = true;
            enemy.FindPath();
            playerPosition = enemy.GetPlayerPosition();
        }
    }
    private void OnTriggerStay2D(Collider2D other) {
        if (other.tag == "Player") {
            isEntered = false;
            enemy.FindPath();
        }
    }
    private void OnTriggerExit2D(Collider2D other) {
        if (other.tag == "Player") {
            float distance = 1;
            if ((!Player.isCompleted || !Player.isDead) && isEntered)
                distance = Vector2.Distance(playerPosition, enemy.GetPlayerPosition());
            if (distance > 0.5f) {
                enemy.isMoving = false;
                enemy.ClearPath();
            }
        }
    }
}
