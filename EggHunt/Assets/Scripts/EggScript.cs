using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggScript : MonoBehaviour
{
    Rigidbody2D rb2d;
    Collider2D collider2d;
    public float transferCooldown = 1.0f;
    public float throwSpeed = 4.0f;
    public float throwOffset;
    private float transferTimer;
    private PlayerScript lastPlayer;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        collider2d = GetComponent<Collider2D>();
        transferTimer = transferCooldown;
    }

    // Update is called once per frame
    void Update()
    {
        transferTimer -= Time.deltaTime;
    }

    public bool IsReadyForTransfer()
    {
        return (transferTimer < 0.0f);
    }

    public void PickUp(Transform newParent)
    {
        transform.parent = null;
        rb2d.Sleep();
        collider2d.enabled = false;
        transform.SetParent(newParent);
        lastPlayer = newParent.parent.gameObject.GetComponentInParent<PlayerScript>();
        transform.position = newParent.position;
        transferTimer = transferCooldown;
    }

    public void Drop()
    {
        rb2d.WakeUp();
        collider2d.enabled = true;
        transform.SetParent(null);
        transform.position += new Vector3(0.1f, 0.0f, 0.0f);
    }

    public void Throw(Vector2 direction)
    {
        rb2d.WakeUp();
        collider2d.enabled = true;
        transform.SetParent(null);
        transform.position += new Vector3(direction.x * throwOffset, direction.y * throwOffset, 0.0f);
        rb2d.velocity = direction * throwSpeed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag != "Player" && other.gameObject.tag != "Enemy")
        {
            Debug.Log($"{this.name} crashed against {other.gameObject.name}!");

            lastPlayer.LoseHealth(lastPlayer.MaxHealth / 2);
            GameOverlord.gameOverlordInstance.EggCrash(this, lastPlayer);
        }
    }
}
