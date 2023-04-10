using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float damping;
    [SerializeField] private RectTransform canvas;
    [SerializeField] private float maxX;
    [SerializeField] private float minX;
    [SerializeField] private float scaleX;
    private float scaleXOfScreen;
    private Vector3 velocity = Vector3.zero;
    private void Start() {
        scaleXOfScreen = canvas.localScale.x;
        maxX = maxX - (scaleXOfScreen - scaleX) * 800 / 4;
        minX = minX + (scaleXOfScreen - scaleX) * 800 / 4;
    }
    void FixedUpdate()
    {
        Vector3 targetPosition = target.position + offset;
        targetPosition.y = transform.position.y;
        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, damping);
    }
}
