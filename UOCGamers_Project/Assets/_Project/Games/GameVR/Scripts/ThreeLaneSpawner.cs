using UnityEngine;
using System.Collections;

public class ThreeLaneSpawner : MonoBehaviour
{
    [Header("Carriles")]
    public Transform leftLane;
    public Transform centerLane;
    public Transform rightLane;

    [Header("Prefabs")]
    public GameObject[] bullPrefabs;
    public GameObject[] obstaclePrefabs;

    [Range(0f, 1f)] public float bullProbability = 0.6f;

    [Header("Spawn / Dificultad")]
    public float startInterval = 1.2f;
    public float minInterval = 0.5f;
    public float difficultyRampTime = 120f;
    public float maxSpawnTime = 120f;
    public float baseSpeed = 6f;
    public float speedIncrease = 2f;
    public float spawnDistance = 10f; // distancia delante del jugador

    [Header("Opciones")]
    public bool autoRun = true;
    public bool debugMode = false;

    float elapsed;
    Coroutine loop;

    void Awake()
    {
        bullPrefabs = CleanArray(bullPrefabs);
        obstaclePrefabs = CleanArray(obstaclePrefabs);
    }

    void OnEnable() { if (autoRun) StartSpawning(); }
    void OnDisable() { StopSpawning(); }

    public void StartSpawning()
    {
        if (loop == null)
        {
            Debug.Log(debugMode ? "Starting spawner (DEBUG MODE)..." : "Starting spawner...");
            loop = StartCoroutine(SpawnLoop());
        }
    }

    public void StopSpawning()
    {
        if (loop != null)
        {
            Debug.Log("Stopping spawner...");
            StopCoroutine(loop);
            loop = null;
        }
    }

    IEnumerator SpawnLoop()
    {
        elapsed = 0f;
        float timer = 0f;

        while (debugMode || (MicrogameManager.Instance == null || MicrogameManager.Instance.IsRunning))
        {
            elapsed += Time.deltaTime;
            if (debugMode) timer += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / difficultyRampTime);
            float interval = Mathf.Lerp(startInterval, minInterval, t);
            float speed = baseSpeed + Mathf.Lerp(0f, speedIncrease, t);

            int laneIdx = Random.Range(0, 3);
            Transform lane = laneIdx == 0 ? leftLane : laneIdx == 1 ? centerLane : rightLane;

            GameObject prefab = GetRandomPrefab();
            if (prefab == null)
            {
                Debug.LogError("❌ ERROR: No hay prefabs válidos para spawnear. Revisa los arrays.");
                yield return null;
                continue;
            }

            // Spawn delante del jugador para que se vean bien
            Vector3 spawnPos = lane.position + Vector3.forward * spawnDistance;
            spawnPos.y = lane.position.y; // mantener altura del carril
            var go = Instantiate(prefab, spawnPos, lane.rotation);

            var mover = go.GetComponent<MoveTowardsPlayer>();
            if (mover)
            {
                mover.speed = speed;
                if (debugMode)
                {
                    mover.damage = 0;
                    mover.scoreOnPass = 0;
                }
            }

            Debug.Log($"Spawned {prefab.name} at lane {laneIdx} | Time: {(debugMode ? timer : elapsed):F2}s | Interval: {interval:F2}s | Speed: {speed:F2}");

            yield return new WaitForSeconds(interval);

            if (debugMode && timer >= maxSpawnTime) break;
        }

        Debug.Log(debugMode ?
            $"Spawner finished (DEBUG MODE) after {timer:F2} seconds" :
            "Spawner finished");

        loop = null;
    }

    // ========================== FUNCIONES AUXILIARES ==========================
    GameObject GetRandomPrefab()
    {
        GameObject[] sourceArray = Random.value < bullProbability ? bullPrefabs : obstaclePrefabs;
        if (sourceArray == null || sourceArray.Length == 0) return null;

        GameObject[] safeArray = System.Array.FindAll(sourceArray, item => item != null);
        if (safeArray.Length == 0) return null;

        return safeArray[Random.Range(0, safeArray.Length)];
    }

    GameObject[] CleanArray(GameObject[] array)
    {
        if (array == null) return new GameObject[0];
        return System.Array.FindAll(array, item => item != null);
    }

    // ========================== GIZMOS PARA DEPURACION ==========================
    void OnDrawGizmos()
    {
        if (leftLane != null) { Gizmos.color = Color.red; Gizmos.DrawSphere(leftLane.position, 0.3f); }
        if (centerLane != null) { Gizmos.color = Color.green; Gizmos.DrawSphere(centerLane.position, 0.3f); }
        if (rightLane != null) { Gizmos.color = Color.blue; Gizmos.DrawSphere(rightLane.position, 0.3f); }
    }
}
