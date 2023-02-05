using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {
    public static EnemySpawner instance;

    [Header("Enemies")]
    [SerializeField] private int maxEnemies;
    [SerializeField] private Transform enemyParent;
    [Space]
    [SerializeField] private EnemySpawnData[] enemies;

    [Header("Spawning")]
    [Tooltip("When the game isn't hectic and the world is functioning normally, birds spawn here.")]
    [SerializeField] private float enemySpawnRate = 1f;
    [SerializeField] private Transform[] spawnPoints;
    float totalEnemyProbabilities = 0;

    private int numEnemies { get => enemyParent.childCount; }

    [Serializable]
    public struct EnemySpawnData {
        [SerializeField] private GameObject prefab;
        [Tooltip("Relative to all other enemies.")]
        [SerializeField] private float probability;

        public GameObject Prefab { get => prefab; set => prefab = value; }
        public float Probability { get => probability; set => probability = value; }
    }
    
    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Debug.Log($"Enemy Spawner instance already exists on ({gameObject}), destroying this.");
            Destroy(this);
        }

        for (int i = 0; i < enemies.Length; i++) {
            totalEnemyProbabilities += enemies[i].Probability;
        }
    }

    private void Start() {
        StartCoroutine(ContinuouslySpawnEnemies());
    }

    private IEnumerator ContinuouslySpawnEnemies() {
        while (true) {
            yield return new WaitForSeconds(enemySpawnRate);
            if (numEnemies < maxEnemies) SpawnEnemy();
        }
    }

    private void SpawnEnemy() {
        Transform spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
        GameObject enemyPrefab = GetEnemySpawnData(enemies).Prefab;

        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity, enemyParent);
    }

    private EnemySpawnData GetEnemySpawnData(EnemySpawnData[] _esd) {
        float randomValue = UnityEngine.Random.Range(0, totalEnemyProbabilities);

        float currentProbability = 0;
        for (int i = 0; i < _esd.Length; i++) {
            currentProbability += _esd[i].Probability;
            if (randomValue <= currentProbability) return _esd[i];
        }

        return new EnemySpawnData();
    }

    public void DestroyAllBirds() {
        for (int i = 0; i < enemyParent.childCount; i++) {
            Destroy(enemyParent.GetChild(i).gameObject);
        }
    }
}
