using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject chestnutPrefab;
    [SerializeField] private GameObject rockPrefab;

    [Header("Spawn")]
    [SerializeField] private float initialInterval = 0.9f;
    [SerializeField] private float minInterval = 0.35f;
    [SerializeField] private float difficultyRampTime = 30f; // segundos
    [SerializeField, Range(0f, 1f)] private float initialRockChance = 0.2f;
    [SerializeField, Range(0f, 1f)] private float maxRockChance = 0.6f;

    private float startTime;
    private Camera cam;
    private float ySpawn;

    void Start()
    {
        cam = Camera.main;
        ySpawn = cam.ViewportToWorldPoint(new Vector3(0.5f, 1.1f, 0f)).y; // por encima de la pantalla
        startTime = Time.time;
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (GameManager.Instance == null || !GameManager.Instance.IsGameOver)
        {
            float t = Mathf.Clamp01((Time.time - startTime) / difficultyRampTime);
            float interval = Mathf.Lerp(initialInterval, minInterval, t);
            float rockChance = Mathf.Lerp(initialRockChance, maxRockChance, t);

            SpawnOne(rockChance);
            yield return new WaitForSeconds(interval);
        }
    }

    void SpawnOne(float rockChance)
    {
        Vector3 left = cam.ViewportToWorldPoint(new Vector3(0f, 0f, 0f));
        Vector3 right = cam.ViewportToWorldPoint(new Vector3(1f, 0f, 0f));
        float x = Random.Range(left.x + 0.5f, right.x - 0.5f);
        Vector3 pos = new Vector3(x, ySpawn, 0f);

        bool spawnRock = Random.value < rockChance;
        var prefab = spawnRock ? rockPrefab : chestnutPrefab;

        if (prefab != null) Instantiate(prefab, pos, Quaternion.identity);
    }
}
