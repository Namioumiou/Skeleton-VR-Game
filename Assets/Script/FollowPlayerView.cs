using UnityEngine;

public class FollowPlayerView : MonoBehaviour
{
    [Header("References")]
    public Transform head;

    [Header("Positions")]
    public float followDistance = 2f;
    public float heightOffset = -0.5f;
    public float followSpeed = 5f;

    void LateUpdate()
    {
        if (!head) return;

        Vector3 targetPosition = head.position + head.forward * followDistance;
        targetPosition.y += heightOffset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

        Quaternion targetRotation = Quaternion.LookRotation(transform.position - head.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, followSpeed * Time.deltaTime);
    }
}
