using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public Transform gunStem;
    public Transform gun;
    LineRenderer laser;
    bool active = false;

    [Header("General")]
    public float shootRadius = 15;
    public float dps = 3;
    public float duration = 10;
    [Header("Rotation")]
    public float turnSpeed = 45;
    public Vector2 pitchRange = new Vector2(-30, 90);

    Enemy target;
    float timer;


    Vector3 turretRot;

    void Start()
    {
        laser = gun.GetComponent<LineRenderer>();
        timer = duration;
    }

    void Update()
    {
        // Don't continue if turret isn't active yet
        if (!active)
            return;

        if (target != null)
        {
            // Get the target rotation that the gun tries to achieve,
            Vector3 targetRot = Quaternion.LookRotation((target.position - gun.position).normalized).eulerAngles;
            // Get angle between target and gun before limiting the gun's rotation.
            float angle = Quaternion.Angle(Quaternion.Euler(targetRot), gun.rotation);
            // Limit the pitch of the turret based on 
            targetRot.x = Mathf.Clamp(((targetRot.x + 180) % 360 - 180), -pitchRange.y, -pitchRange.x);

            // Rotate both the gun and the stem gradually towards the target.
            gun.rotation = Quaternion.RotateTowards(gun.rotation, Quaternion.Euler(targetRot), Time.deltaTime * turnSpeed);
            gunStem.rotation = Quaternion.Euler(0, gun.eulerAngles.y, 0);

            // Only let the gun shoot if the angle between the gun and the enemy is below the defined threshold.
            if (angle <= 5)
            {
                laser.positionCount = 2;
                laser.SetPosition(0, gun.position);
                laser.SetPosition(1, target.position);
                laser.enabled = true;
                target.Hurt(dps * Time.deltaTime);
            }
            else
            {
                laser.positionCount = 0;
                laser.enabled = false;
            }
        }
        else
        {
            laser.positionCount = 0;
            laser.enabled = false;
        }

        // Count down timer and destroy turret when its timer expires.
        timer = Mathf.Max(timer - Time.deltaTime, 0);
        if(timer <= 0)
            Destroy(gameObject);
    }

    void FixedUpdate()
    {
        FindClosestEnemy();
    }

    void FindClosestEnemy()
    {
        // Reset current target value and keep track of closest target.
        target = null;
        float closest = shootRadius * 2;

        // Get a list of colliders within the turret's radius
        Collider[] cols = Physics.OverlapSphere(transform.position, shootRadius);

        // Loop through each collider found.
        foreach (Collider col in cols)
        {
            // Only check colliders with an enemy script attached to them.
            if (col.TryGetComponent<Enemy>(out Enemy enemy) && enemy.currentHealth > 0)
            {
                // If the distance to the observed enemy is closer than the last closest observed, override the closest enemy to this one.
                float dist = enemy.GetDistance(transform.position);
                if (dist < closest)
                {
                    target = enemy;
                    closest = dist;
                }
            }
        }
    }

    void Activate()
    {
        active = true;
    }
}
