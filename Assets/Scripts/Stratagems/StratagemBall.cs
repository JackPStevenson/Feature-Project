using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StratagemBall : MonoBehaviour
{
    public GameObject spawnedPrefab;
    public LayerMask hitMask;
    bool moving = true;
    public float settleAngle = 45;

    public Vector3 velocity;
    public Vector3 position;

    LineRenderer lineRenderer;
    public float lineDist = 10;

    GameObject spawnedPod;


    Vector3 lastPos;

    void Start()
    {
        lastPos = transform.position;
        position = transform.position;
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        float lerpTime = Time.time % Time.fixedDeltaTime;
        transform.position = Vector3.Lerp(lastPos, position, lerpTime / Time.fixedDeltaTime);
    }

    void FixedUpdate()
    {
        lastPos = position;

        if (!moving)
        {
            if (spawnedPod.transform.position == position)
                Destroy(gameObject);
        }
        else
        {

            Vector3 moveDelta = velocity * Time.deltaTime;

            if (Physics.Raycast(position, velocity.normalized, out RaycastHit hit, moveDelta.magnitude, hitMask))
            {
                position = hit.point;

                if (Vector3.Angle(Vector3.up, hit.normal) <= settleAngle)
                {
                    moving = false;

                    lineRenderer.positionCount = 2;
                    lineRenderer.SetPosition(0, position);
                    lineRenderer.SetPosition(1, position + (Vector3.up * lineDist));
                    lineRenderer.enabled = true;

                    spawnedPod = Instantiate(spawnedPrefab, SuperDestroyer.position, Quaternion.identity);
                    spawnedPod.GetComponent<Pod>().targetPos = position;
                }
                else
                {
                    velocity = Vector3.Reflect(velocity, hit.normal);
                }
            }
            else
            {
                position += moveDelta;
                velocity.y -= 9.81f * Time.deltaTime;
            }

            transform.forward = velocity.normalized;
        }
    }
}
