using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UtilitiesScript;

public class PlayerScript : MonoBehaviour
{
    [Header("Movement Settings")]
    public float maxSpeed;
    public float rampUpSpeed;

    [Header("Health Settings")]
    public bool Invincible = false;
    public int MaxHealth;
    public int CurrentHealth;
    public int DamageCoolDownSec; 
    public int Respawns;

    public const float indicatorLength = 0.5f;
    public static int playerIds = 0;

    private int playerId;
    private bool IsDead;
    Rigidbody2D rb2d;
    SpriteRenderer spriteRenderer;
    private Transform eggCarry;
    private EggScript carrying = null;
    private Vector2 velocity;
    
    private void Awake()
    {
        CurrentHealth = MaxHealth;
    }

    // Start is called before the first frame update
    void Start()
    {
        playerId = playerIds++;
        
        eggCarry = transform.GetChild(0).transform; // TODO: Is there a better way to get a specific child object?
        rb2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = GetRandomColor();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 moveVector = Vector2.zero;
        Vector2 aimInput = Vector2.zero;

        if (!IsDead)
        {
            // Movement
            float horizontalInput = Input.GetAxisRaw("Horizontal_" + playerId);
            float verticalInput = Input.GetAxisRaw("Vertical_" + playerId);
            moveVector = new Vector2(horizontalInput, verticalInput).normalized;

            // Aiming
            float aimHorizontalInput = Input.GetAxisRaw("AimHorizontal_" + playerId);
            float aimVerticalInput = Input.GetAxisRaw("AimVertical_" + playerId);
            aimInput = new Vector2(aimHorizontalInput, aimVerticalInput).normalized;
        }

        Vector2.SmoothDamp(Vector2.zero, moveVector, ref velocity, rampUpSpeed, 1.0f);

        if (aimInput.sqrMagnitude > 0.0f)
        {
            Vector2 position = new Vector2(transform.position.x, transform.position.y);
            Vector2 indicatorLineEndPoint = position + aimInput * indicatorLength;
            Debug.DrawLine(transform.position, indicatorLineEndPoint);

            if (Input.GetButtonUp("Fire_" + playerId) && carrying)
            {
                Debug.Log(this.name + " throws " + carrying.name);
                Throw(aimInput);
            }
        }
    }

    void FixedUpdate()
    {
        Vector2 newPosition = rb2d.position + velocity * maxSpeed * Time.deltaTime;
        rb2d.MovePosition(newPosition);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Egg")
        {
            Debug.Log(this.name + " bumped into " + other.gameObject.name);
            EggScript egg = other.gameObject.GetComponent<EggScript>();
            PickUp(egg);
        }        
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log(this.name + " bumped into " + other.gameObject.name);
            PlayerScript otherPlayer = other.gameObject.GetComponent<PlayerScript>();
            if (carrying && carrying.IsReadyForTransfer())
            {
                Debug.Log(this.name + " transferred " + carrying.name + " to " + other.gameObject.name);
                Transfer(otherPlayer);
            }
        }
        else if (other.gameObject.tag == "Enemy" && !Invincible)
        {
            Debug.Log($"{this.name} bumped into {other.gameObject.name}!");

            LoseHealth(1);            
        }
    }

    public void LoseHealth(int amount)
    {
        this.CurrentHealth--;

        if (this.CurrentHealth <= 0)
        {
            if (carrying != null)
            {
                Debug.Log(this.name + " dropped " + carrying.name + "on the ground");
                Drop();
            }

            if (!IsDead)
            {
                Die();
            }            
        }
    }

    public void Die()
    {
        this.IsDead = true;
        this.GetComponent<Collider2D>().enabled = false;
        
        if (Respawns > 0)
        {
            StartCoroutine
            (
                GameOverlord
                .gameOverlordInstance
                .SpawnPlayerCoroutine(this)
            );
        }
    }

    public void PickUp(EggScript egg)
    {
        egg.PickUp(eggCarry);
        carrying = egg;
    }

    void Drop()
    {
        carrying.Drop();
        carrying = null;
    }

    void Transfer(PlayerScript otherPlayer)
    {
        EggScript egg = carrying;
        carrying = null;
        otherPlayer.PickUp(egg);
    }

    void Throw(Vector2 direction)
    {
        EggScript egg = carrying;
        carrying = null;
        egg.Throw(direction);
    }

    public int GetPlayerID()
    {
        return this.playerId;
    }

    public void Spawn(GenericSpawnPoint location)
    {
        this.CurrentHealth = MaxHealth;
        this.Respawns--;
        this.GetComponent<Collider2D>().enabled = true;
        this.IsDead = false;
        this.transform.position = location.transform.position;
    }    
}
