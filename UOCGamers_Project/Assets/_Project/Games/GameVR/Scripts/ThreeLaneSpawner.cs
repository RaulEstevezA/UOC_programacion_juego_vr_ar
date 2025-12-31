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

    [Range(0f, 1f)]
    public float bullProbability = 0.6f;

    [Header("Spawn / Dificultad")]
    public float startInterval = 1.1f;
    public float minInterval = 0.35f;
    public float difficultyRampTime = 45f;
    public float maxSpawnTime = 60f;
    public float baseSpeed = 7f;
    public float speedIncrease = 7f;
    public float spawnDistance = 10f; // distancia delante del jugador

    [Header("Altura")]
    [Tooltip("Altura del suelo del microjuego. La base de la valla/toro se apoyará aquí.")]
    public float groundY = 0f;

    [Header("Opciones")]
    public bool autoRun = true;
    public bool debugMode = false;

    private float elapsed;
    private Coroutine loop;

    void Awake()
    {
        bullPrefabs = CleanArray(bullPrefabs);
        obstaclePrefabs = CleanArray(obstaclePrefabs);
    }

    void OnEnable()
    {
        if (autoRun)
            StartSpawning();
    }

    void OnDisable()
    {
        StopSpawning();
    }

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
        float timer = 0f;

        float startTime = Time.time;
        elapsed = Time.time - startTime;

        while (debugMode || MicrogameManager.Instance == null || MicrogameManager.Instance.IsRunning)
        {
            elapsed = Time.time - startTime;
            if (debugMode) timer += Time.deltaTime;

            // Rampa de dificultad
            float t = Mathf.Clamp01(elapsed / difficultyRampTime);
            float interval = Mathf.Lerp(startInterval, minInterval, t);
            float speed = baseSpeed + Mathf.Lerp(0f, speedIncrease, t);

            // Elegir carril
            int laneIdx = Random.Range(0, 3);
            Transform lane = laneIdx == 0 ? leftLane : (laneIdx == 1 ? centerLane : rightLane);

            // Elegir prefab (toro u obstáculo)
            bool isBull;
            GameObject prefab = GetRandomPrefab(out isBull);
            if (prefab == null)
            {
                Debug.LogError("❌ ERROR: No hay prefabs válidos para spawnear. Revisa los arrays de bullPrefabs/obstaclePrefabs.");
                yield return null;
                continue;
            }

            // Posición de spawn: delante del carril
            Vector3 spawnPos = lane.position + Vector3.forward * spawnDistance;

            // Instanciar (la altura exacta se corrige después)
            GameObject go = Instantiate(prefab, spawnPos, lane.rotation);

            // Ajustar altura: base del collider alineada con groundY (evita valla a medias)
            AlignObjectToGround(go);

            // Si es toro, orientarlo hacia el jugador (cámara)
            if (isBull)
            {
                OrientBullTowardsCamera(go);
            }

            // Configurar velocidad (y desactivar daño/puntos en debug si quieres pruebas sin penalización)
            var mover = go.GetComponent<MoveTowardsPlayer>();
            if (mover != null)
            {
                mover.speed = speed;
                if (debugMode)
                {
                    mover.damage = 0;
                    mover.scoreOnPass = 0;
                }
            }

            if (debugMode)
            {
                Debug.Log(
                    $"Spawned {prefab.name} en carril {laneIdx} | t={elapsed:F2}s | Intervalo={interval:F2}s | Velocidad={speed:F2}");
            }

            yield return new WaitForSeconds(interval);

            if (debugMode && timer >= maxSpawnTime)
                break;
        }

        Debug.Log(debugMode
            ? $"Spawner finished (DEBUG MODE) after {timer:F2} seconds"
            : "Spawner finished");

        loop = null;
    }

    /// <summary>
    /// Mueve el objeto en Y para que la base de su collider quede exactamente en groundY.
    /// Evita que la valla o el toro aparezcan cortados (hundidos o flotando).
    /// </summary>
    void AlignObjectToGround(GameObject go)
    {
        if (go == null) return;

        // Intentamos coger un collider en el root; si no, en los hijos
        Collider col = go.GetComponent<Collider>();
        if (col == null)
            col = go.GetComponentInChildren<Collider>();
        if (col == null)
            return;

        // Bounds en mundo: min.y es la base actual
        Bounds b = col.bounds;
        float bottomWorldY = b.min.y;

        // Cuánto hay que desplazar en Y para que esa base coincida con groundY
        float deltaY = groundY - bottomWorldY;

        go.transform.position += new Vector3(0f, deltaY, 0f);
    }

    /// <summary>
    /// Hace que el toro mire hacia la cámara (jugador) sólo en el plano horizontal.
    /// </summary>
    void OrientBullTowardsCamera(GameObject bull)
    {
        if (bull == null) return;

        Camera cam = Camera.main;
        if (cam == null) return;

        Transform t = bull.transform;

        // Objetivo con misma altura que el toro para que no incline la cabeza hacia arriba/abajo
        Vector3 target = cam.transform.position;
        target.y = t.position.y;

        t.LookAt(target);
    }

    // ========================== FUNCIONES AUXILIARES ==========================

    GameObject GetRandomPrefab(out bool isBull)
    {
        isBull = Random.value < bullProbability;
        GameObject[] sourceArray = isBull ? bullPrefabs : obstaclePrefabs;

        if (sourceArray == null || sourceArray.Length == 0)
        {
            isBull = false;
            return null;
        }

        GameObject[] safeArray = System.Array.FindAll(sourceArray, item => item != null);
        if (safeArray.Length == 0)
        {
            isBull = false;
            return null;
        }

        return safeArray[Random.Range(0, safeArray.Length)];
    }

    GameObject[] CleanArray(GameObject[] array)
    {
        if (array == null) return new GameObject[0];
        return System.Array.FindAll(array, item => item != null);
    }

    // ========================== GIZMOS PARA DEPURACIÓN ==========================

    void OnDrawGizmos()
    {
        if (leftLane != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(leftLane.position, 0.3f);
        }

        if (centerLane != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(centerLane.position, 0.3f);
        }

        if (rightLane != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(rightLane.position, 0.3f);
        }
    }
}
