using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdSpawner : MonoBehaviour {
    public static BirdSpawner instance;

    [Header("Birds")]
    [SerializeField] private int maxBirds;
    [SerializeField] private Transform birdParent;
    [Space]
    [SerializeField] private BirdSpawnData[] peacefulBirds;
    
    [Header("Spawning")]
    [Tooltip("When the game isn't hectic and the world is functioning normally, birds spawn here.")]
    [SerializeField] private float peacefulBirdSpawnRate = 2f;
    [SerializeField] private PeacefulBirdSpawnPoint[] peacefulSpawnPoints;
    [SerializeField] private float peacefulBirdHeadingRandomness = 0.1f;
    float totalPeacefulBirdProbabilities = 0;

    private int numBirds { get => birdParent.childCount; }

    [Serializable]
    public struct BirdSpawnData {
        [SerializeField] private GameObject prefab;
        [Tooltip("Relative to all other birds.")]
        [SerializeField] private float probability;

        public GameObject Prefab { get => prefab; set => prefab = value; }
        public float Probability { get => probability; set => probability = value; }
    }

    [Serializable]
    public struct PeacefulBirdSpawnPoint {
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private Transform direction;

        public Transform SpawnPoint { get => spawnPoint; set => spawnPoint = value; }
        public Transform Direction { get => direction; set => direction = value; }
    }

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Debug.Log($"Bird Spawner instance already exists on ({gameObject}), destroying this.");
            Destroy(this);
        }

        for (int i = 0; i < peacefulBirds.Length; i++) {
            totalPeacefulBirdProbabilities += peacefulBirds[i].Probability;
        }
    }

    private void Start() {
        StartCoroutine(ContinuouslySpawnPeacefulBirds());
    }

    private IEnumerator ContinuouslySpawnPeacefulBirds() {
        while (true) {
            yield return new WaitForSeconds(peacefulBirdSpawnRate);
            if (numBirds < maxBirds) SpawnBirdPeacefully();
        }
    }

    private void SpawnBirdPeacefully() {
        PeacefulBirdSpawnPoint spawnPoint = peacefulSpawnPoints[UnityEngine.Random.Range(0, peacefulSpawnPoints.Length)];
        GameObject birdPrefab = GetRandomBirdSpawnData(peacefulBirds).Prefab;

        GameObject bird = Instantiate(birdPrefab, spawnPoint.SpawnPoint.position, Quaternion.identity, birdParent) as GameObject;
        
        Vector3 target = spawnPoint.Direction.position - spawnPoint.SpawnPoint.position;
        target.x += UnityEngine.Random.Range(-peacefulBirdHeadingRandomness, peacefulBirdHeadingRandomness);
        target.z += UnityEngine.Random.Range(-peacefulBirdHeadingRandomness, peacefulBirdHeadingRandomness);
        bird.GetComponent<Bird>().PeacefulTarget = target.normalized;
    }

    private BirdSpawnData GetRandomBirdSpawnData(BirdSpawnData[] _bsd) {
        float randomValue = UnityEngine.Random.Range(0, totalPeacefulBirdProbabilities);

        float currentProbability = 0;
        for (int i = 0; i < _bsd.Length; i++) {
            currentProbability += _bsd[i].Probability;
            if (randomValue <= currentProbability) return _bsd[i];
        }

        return new BirdSpawnData();
    }

    public void DestroyAllBirds() {
        for (int i = 0; i < birdParent.childCount; i++) {
            Destroy(birdParent.GetChild(i).gameObject);
        }
    }
}
