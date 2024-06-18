using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperDestroyer : MonoBehaviour
{
    public static SuperDestroyer instance;
    public static Vector3 position
    {
        get { return instance.transform.position; }
        set { instance.transform.position = value; }
    }

    void Start()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public static Vector3 GetDirToShip(Vector3 pos)
    {
        return (position - pos).normalized;
    }
}
