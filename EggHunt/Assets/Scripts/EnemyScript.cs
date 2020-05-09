using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    [Header("Movement Settings")]
    public float maxSpeed;
    public float rampUpSpeed;

    [Header("Target Settings")]
    public Transform target;

    private Rigidbody2D rb2d;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (target)
        {
            Vector2 targetPosition2 = new Vector2(target.position.x, target.position.y);
            Vector2 direction = targetPosition2 - rb2d.position;

            float dotValue = Vector2.Dot(direction.normalized, rb2d.velocity.normalized);
            if (dotValue < 0.95f)
            {
                float breakScalar = 50.0f;
                rb2d.AddForce(-rb2d.velocity.normalized * Time.deltaTime * breakScalar);
            }

            if (rb2d.velocity.magnitude <= maxSpeed)
            {
                rb2d.AddForce(direction.normalized * Time.deltaTime * rampUpSpeed * 100.0f);
            }
        }
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    public void Spawn(GenericSpawnPoint location)
    {
        this.transform.position = location.transform.position;
    }
}
