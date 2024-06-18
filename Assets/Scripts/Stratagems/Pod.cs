using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pod : MonoBehaviour
{
    public float fallSpeed = 50;
    public Vector3 targetPos = Vector3.zero;
    public float deployTime = 1f;

    float timer;

    void Start()
    {
        timer = deployTime;
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, targetPos) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, fallSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = targetPos;
            timer = Mathf.Max(timer - Time.deltaTime, 0);

            if (timer <= 0)
            {
                BroadcastMessage("Activate");
                transform.GetChild(0).parent = null;
                Destroy(gameObject);
            }
        }
    }
}