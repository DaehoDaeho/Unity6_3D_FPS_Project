using UnityEngine;
using System.Collections.Generic;

public class EnemyWaveSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private Transform playerTarget;
    [SerializeField] private PlayerHealth playerHealth;

    [SerializeField] private int firstWaveEnemyCount = 3;
    [SerializeField] private int enemyIncreasePerWave = 2;
    [SerializeField] private int maxAliveEnemies = 5;
    [SerializeField] private float spawnInterval = 1.5f;
    [SerializeField] private float nextWaveDelay = 3.0f;

    private List<GameObject> spawnedEnemies = new List<GameObject>();

    private int currentWave = 0;
    private int enemiesToSpawn;
    private int spawnedInWave;
    private int aliveEnemyCount;

    private float spawnTimer;
    private float waveDelayTimer;
    private bool isWaveRunning;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartNextWave();
    }

    // Update is called once per frame
    void Update()
    {
        CountAliveEnemies();

        if (isWaveRunning == true)
        {
            RunWave();
        }
        else
        {
            WaitForNextWave();
        }
    }

    void RunWave()
    {
        if(spawnedInWave < enemiesToSpawn)
        {
            spawnTimer -= Time.deltaTime;
            if(spawnTimer <= 0.0f && aliveEnemyCount < maxAliveEnemies)
            {
                SpawnEnemy();
                spawnTimer = spawnInterval;
            }
        }
        else if(aliveEnemyCount <= 0)
        {
            isWaveRunning = false;
            waveDelayTimer = nextWaveDelay;
        }
    }

    void WaitForNextWave()
    {
        if(waveDelayTimer > 0.0f)
        {
            waveDelayTimer -= Time.deltaTime;
            return;
        }

        StartNextWave();
    }

    void StartNextWave()
    {
        currentWave++;
        enemiesToSpawn = firstWaveEnemyCount + (currentWave - 1) * enemyIncreasePerWave;
        spawnedInWave = 0;
        spawnTimer = 0;
        isWaveRunning = true;
    }

    void SpawnEnemy()
    {
        if(enemyPrefabs == null || spawnPoints == null || spawnPoints.Length == 0)
        {
            return;
        }

        Transform spawnPoint = spawnPoints[spawnedInWave % spawnPoints.Length];

        int randomIndex = Random.Range(0, enemyPrefabs.Length);

        GameObject enemyInstance = Instantiate(enemyPrefabs[randomIndex], spawnPoint.position, spawnPoint.rotation);

        if(enemyInstance != null)
        {
            EnemyChaseAgent chaseAgent = enemyInstance.GetComponent<EnemyChaseAgent>();

            if(chaseAgent != null)
            {
                chaseAgent.SetTarget(playerTarget, playerHealth);
            }
            else
            {
                RangedEnemy rangedEnemy = enemyInstance.GetComponent<RangedEnemy>();
                if(rangedEnemy != null)
                {
                    rangedEnemy.SetTarget(playerTarget, playerHealth);
                }
            }

            spawnedEnemies.Add(enemyInstance);
            spawnedInWave++;
        }
    }

    void CountAliveEnemies()
    {
        aliveEnemyCount = 0;

        for(int i=spawnedEnemies.Count-1; i>=0; i--)
        {
            GameObject enemy = spawnedEnemies[i];
            if(enemy == null)
            {
                spawnedEnemies.RemoveAt(i);
                continue;
            }

            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if(enemyHealth != null && enemyHealth.IsDead == false)
            {
                aliveEnemyCount++;
            }
            else if(enemyHealth != null && enemyHealth.IsDead == true)
            {
                spawnedEnemies.RemoveAt(i);
            }
        }
    }
}
