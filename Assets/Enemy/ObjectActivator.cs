using UnityEngine;

public class ObjectActivator : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject sword;
    public GameObject shield;

    public bool hasSword = false;
    public bool hasShield = false;

    // Update is called once per frame
    void Update()
    {
        if (hasSword)
        {
            sword.SetActive(true);
        }
        else
        {
            sword.SetActive(false);
        }

        if (hasShield)
        {
            shield.SetActive(true);
        }
        else
        {
            shield.SetActive(false);
        }
    }
}
