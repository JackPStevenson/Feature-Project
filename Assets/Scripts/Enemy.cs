using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Rigidbody rb;
    CapsuleCollider col;

    [Header("Health")]
    public float maxHealth;
    public float currentHealth;

    [Header("Ragdoll")]
    public float ragdollDuration = 3;
    float ragdollLifetime;

    public Vector3 position { get { return transform.position; } set { transform.position = value; } }
    Vector3 baseScale;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();

        currentHealth = maxHealth;
        ragdollLifetime = ragdollDuration;
        baseScale = transform.localScale;
    }

    void Update()
    {
        // Scale enemy based on ragdoll lifetime.
        transform.localScale = baseScale * (ragdollLifetime / ragdollDuration);

        // Start to count down ragdoll timer if the enemy's health reaches 0.
        if(currentHealth == 0) ragdollLifetime = Mathf.Max(ragdollLifetime - Time.deltaTime, 0);

        // Destroy the enemy when their ragdoll timer expires.
        if(ragdollLifetime <= 0) Destroy(gameObject);
    }

    public float GetDistance(Vector3 pos)
    {
        return Vector3.Distance(position, pos);
    }

    public void Hurt(float damage)
    {
        // Reduce enemy health based on input.
        currentHealth = Mathf.Max(currentHealth - damage, 0);

        // If currentHealth reaches 0, turn the enemy into a ragdoll.
        if (currentHealth <= 0)
        {
            col.enabled = false;

            rb.constraints = RigidbodyConstraints.None;
            rb.velocity = Vector3.up * 5;
            rb.angularVelocity = Random.insideUnitSphere * 8;

            ragdollLifetime = ragdollDuration;
        }

    }

}
