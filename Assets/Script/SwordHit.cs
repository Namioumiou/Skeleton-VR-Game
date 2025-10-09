using UnityEngine;

public class SwordHit : MonoBehaviour
{
    public int swordDamage = 5;

    void OnTriggerEnter(Collider other)
    {
        SkeletonAI skeleton = other.GetComponent<SkeletonAI>();
        if (skeleton != null)
        {
            Vector3 hitDir = skeleton.transform.position - transform.position;
            skeleton.TakeHit(hitDir, swordDamage);
        }
    }

}
