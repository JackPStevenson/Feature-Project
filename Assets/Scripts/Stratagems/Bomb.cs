using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    bool active = false;

    [Header("General")]
    public float damage = 15;
    public float radius = 10;
    public float force = 50;
    [Header("Visuals")]
    public float fadeTime = 0.2f;

    float timer;

    void Start()
    {
        timer = fadeTime;
    }

    void Update()
    {
        // Only execute code if the bomb activates.
        if(active)
        {
            // Only deal damage and apply force for 1 frame.
            if (timer >= fadeTime) {
                // Enable visuals and scale object accordingly.
                transform.localScale = Vector3.one * radius * 2;
                GetComponent<MeshRenderer>().enabled = true;

                // Get all colliders nearby and loop through them.
                Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
                foreach (Collider col in colliders)
                {
                    Rigidbody rb = col.attachedRigidbody;

                    // Hurt any enemies found in the radius.
                    if (col.TryGetComponent<Enemy>(out Enemy enemy))
                        enemy.Hurt(damage);

                    // Apply force to any rigidbodies found in the radius.
                    if (rb != null)
                    {
                        Vector3 appliedForce = ((rb.position - transform.position).normalized + Vector3.up * 0.2f).normalized * force;
                        rb.gameObject.SendMessage("Ragdoll", null, SendMessageOptions.DontRequireReceiver);

                        rb.AddForceAtPosition(appliedForce, rb.position - Vector3.up * 0.5f, ForceMode.Force);
                    }
                }
            }

            // Let explosion visual linger for a moment.
            timer = Mathf.Max(timer - Time.deltaTime, 0);
            if (timer <= 0) Destroy(gameObject);
        }
    }

    void Activate()
    {
        active = true;
    }
}
