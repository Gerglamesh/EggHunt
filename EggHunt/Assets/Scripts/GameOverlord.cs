using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverlord : MonoBehaviour
{
    //Singleton pattern
    public static GameOverlord gameOverlordInstance;
    public static GameOverlord Instance { get { return gameOverlordInstance; } }

    public PlayerScript PlayerPrefab;
    public int PlayerStartingHealth;
    public int PlayerRespawns;
    public int SpawnTimeSec;
    public float enemySpawnCooldownTime;

    public EnemyScript EnemyPrefab;
    public EggScript EggPrefab;
    public List<GenericSpawnPoint> PlayerSpawnPoints = new List<GenericSpawnPoint>();
    public List<GenericSpawnPoint> EnemySpawnPoints = new List<GenericSpawnPoint>();
    public List<GenericSpawnPoint> EggSpawnPoints = new List<GenericSpawnPoint>();
    public int PlayerAmount;
    public int EnemiesAmount;

    private List<PlayerScript> players = new List<PlayerScript>();
    private List<EnemyScript> enemies = new List<EnemyScript>();
    private EggScript egg;

    private void Awake()
    {
        if (gameOverlordInstance != null && gameOverlordInstance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            gameOverlordInstance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
        
        //Place participating players at spawnpoints
        foreach (PlayerScript player in players)
        {
            GenericSpawnPoint spawnPoint =
                PlayerSpawnPoints[Random.Range(0, PlayerSpawnPoints.Count - 1)];
            player.transform.position = spawnPoint.transform.position;
        }

        SpawnEgg(egg);

        //Spawn all initial enemies
        StartCoroutine(SpawnEnemiesCoroutine(enemySpawnCooldownTime, egg.transform));      
    }

    // Update is called once per frame
    void Update()
    {
        HandleHotKeys();
    }

    private void Init()
    {
        while (players.Count < PlayerAmount)
        {
            players.Add(Instantiate(PlayerPrefab));
            players[players.Count - 1].Respawns = PlayerRespawns;
        }

        for (int enemyIdx = 0; enemyIdx < EnemiesAmount; enemyIdx++)
        {
            enemies.Add(Instantiate(EnemyPrefab));
            enemies[enemyIdx].gameObject.SetActive(false);
        }

        egg = Instantiate(EggPrefab);
    }

    private void SpawnEgg(EggScript egg)
    {
        GenericSpawnPoint spawnPoint =
                    EggSpawnPoints[Random.Range(0, EggSpawnPoints.Count - 1)];
        SpawnObject(spawnPoint.transform.position, egg.gameObject);
    }

    private IEnumerator SpawnEnemiesCoroutine(float cooldownTime, Transform target)
    {
        foreach (EnemyScript enemy in enemies)
        {
            yield return new WaitForSeconds(cooldownTime);
            SpawnEnemy(enemy, target);
            enemy.gameObject.SetActive(true);
        }
    }

    private void SpawnEnemy(EnemyScript enemy, Transform target)
    {
         GenericSpawnPoint spawnPoint =
                    EnemySpawnPoints[Random.Range(0, EnemySpawnPoints.Count - 1)];
                SpawnObject(spawnPoint.transform.position, enemy.gameObject);
        enemy.target = target;
    }

    public IEnumerator SpawnPlayerCoroutine(PlayerScript player)
    {
        Debug.Log($"Started spawn player coroutine on : {player}");   
        yield return new WaitForSeconds(SpawnTimeSec);
        GenericSpawnPoint spawnPoint =
                    PlayerSpawnPoints[Random.Range(0, PlayerSpawnPoints.Count - 1)];
        player.Spawn(spawnPoint);
        Debug.Log($"Ended spawn player coroutine on : {player}");
    }

    private void SpawnObject(Vector3 position, GameObject prefab)
    {
        prefab.transform.position = position;
    }

    public void EggCrash(EggScript egg, PlayerScript lastPlayer)
    {
        List<PlayerScript> recievingPlayers = new List<PlayerScript>();
        foreach (var player in players)
        {
            if (player != lastPlayer)
            {
                recievingPlayers.Add(player);
            }
        }

        PlayerScript recievingPlayer = recievingPlayers[Random.Range(0, recievingPlayers.Count)];
        recievingPlayer.PickUp(egg);        
    }

    public void HandleHotKeys()
    {
        if (Input.GetButtonUp("ResetKey"))
        {
            SceneManager.LoadScene(0);
            PlayerScript.playerIds = 0;
        }
    }
}
