using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject chestnutPrefab;
    [SerializeField] private GameObject rockPrefab;

    [Header("Áreas de aparición (BoxCollider2D)")]
    [SerializeField] private BoxCollider2D[] spawnAreas;

    [Header("Desfase vertical")]
    [SerializeField] private float extraSpawnYOffset = 0.3f;

    [Header("Control de spawn")]
    [SerializeField] private bool startEnabled = true;

    private float startTime;
    private bool spawningEnabled = false;
    private Coroutine spawnCoroutine;

    void Start()
    {
        startTime = Time.time;

        if (startEnabled)
        {
            SetSpawningEnabled(true);
        }
    }

    // Lo llama el GameManager
    public void SetSpawningEnabled(bool enabled)
    {
        if (enabled)
        {
            if (spawningEnabled) return;

            spawningEnabled = true;
            if (spawnCoroutine == null)
                spawnCoroutine = StartCoroutine(SpawnLoop());
        }
        else
        {
            spawningEnabled = false;
            if (spawnCoroutine != null)
            {
                StopCoroutine(spawnCoroutine);
                spawnCoroutine = null;
            }
        }
    }

    IEnumerator SpawnLoop()
    {
        while ((CastanyeraGameManager.Instance == null || !CastanyeraGameManager.Instance.IsGameOver)
               && spawningEnabled)
        {
            float elapsed = Time.time - startTime; // segundos desde el inicio

            // Obtenemos parámetros en función del tramo
            GetDifficultyForTime(elapsed, out float interval, out float rockChance, out float fallSpeedMultiplier);

            TrySpawnOne(rockChance, fallSpeedMultiplier);

            yield return new WaitForSeconds(interval);
        }

        spawnCoroutine = null;
    }

    // === Lógica de dificultad por tramos ===
    private void GetDifficultyForTime(float elapsedSeconds,
                                      out float interval,
                                      out float rockChance,
                                      out float fallSpeedMultiplier)
    {
        // Valores por defecto (último tramo)
        interval = 0.35f;
        rockChance = 0.5f;
        fallSpeedMultiplier = 2.8f;

        if (elapsedSeconds < 10f)
        {
            // 0–10 s: Aprendizaje
            interval = 0.9f;
            rockChance = 0.2f;
            fallSpeedMultiplier = 0.7f;
        }
        else if (elapsedSeconds < 25f)
        {
            // 10–25 s: Confianza y ritmo
            interval = 0.7f;
            rockChance = 0.3f;
            fallSpeedMultiplier = 1.0f;
        }
        else if (elapsedSeconds < 40f)
        {
            // 25–40 s: Tensión creciente
            interval = 0.5f;
            rockChance = 0.4f;
            fallSpeedMultiplier = 1.6f;
        }
        else if (elapsedSeconds < 55f)
        {
            // 40–55 s: Fase crítica
            interval = 0.35f;
            rockChance = 0.5f;
            fallSpeedMultiplier = 2.2f;
        }
        else
        {
            // 55–60 s: Cierre y resolución
            interval = 0.35f;
            rockChance = 0.5f;
            fallSpeedMultiplier = 2.8f;
        }
    }

    private void TrySpawnOne(float rockChance, float fallSpeedMultiplier)
    {
        if (spawnAreas == null || spawnAreas.Length == 0)
        {
            Debug.LogWarning("[Spawner] No hay spawnAreas asignadas.");
            return;
        }

        var area = spawnAreas[Random.Range(0, spawnAreas.Length)];
        if (area == null)
        {
            Debug.LogWarning("[Spawner] Alguna spawnArea está vacía.");
            return;
        }

        var b = area.bounds;
        float x = Random.Range(b.min.x, b.max.x);
        float y = b.max.y + extraSpawnYOffset;

        Vector3 pos = new Vector3(x, y, 0f);

        bool spawnRock = Random.value < rockChance;
        var prefab = spawnRock ? rockPrefab : chestnutPrefab;
        if (prefab == null)
        {
            Debug.LogWarning("[Spawner] Falta asignar chestnutPrefab o rockPrefab.");
            return;
        }

        GameObject instance = Instantiate(prefab, pos, Quaternion.identity);

        // Aplicar velocidad según tramo
        var falling = instance.GetComponent<FallingItem>();
        if (falling != null)
        {
            falling.SetSpeedMultiplier(fallSpeedMultiplier);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (spawnAreas == null) return;
        Gizmos.color = new Color(0.2f, 1f, 0.3f, 0.25f);
        foreach (var a in spawnAreas)
        {
            if (a == null) continue;
            Gizmos.DrawCube(a.bounds.center + Vector3.up * 0.001f, a.bounds.size);
        }
    }
}