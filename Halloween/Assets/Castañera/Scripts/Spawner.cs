using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject chestnutPrefab;
    [SerializeField] private GameObject rockPrefab;

    [Header("Áreas de aparición (BoxCollider2D)")]
    [SerializeField] private BoxCollider2D[] spawnAreas;

    [Header("Ritmo y dificultad")]
    [SerializeField] private float initialInterval = 0.9f;
    [SerializeField] private float minInterval = 0.35f;
    [SerializeField] private float difficultyRampTime = 30f; // segundos
    [SerializeField, Range(0f, 1f)] private float initialRockChance = 0.2f;
    [SerializeField, Range(0f, 1f)] private float maxRockChance = 0.6f;

    [Header("Desfase vertical")]
    [SerializeField] private float extraSpawnYOffset = 0.3f;

    private float startTime;

    // NUEVO
    private bool spawningEnabled = false;
    private Coroutine spawnCoroutine;

    void Start()
    {
        startTime = Time.time;

        // Dejamos que sea el GameManager quien llame a SetSpawningEnabled(true)
        // Si quieres que encienda solo al inicio, puedes descomentar:
        // SetSpawningEnabled(true);
    }

    // NUEVO: lo llama el GameManager
    public void SetSpawningEnabled(bool enabled)
    {
        if (enabled)
        {
            if (spawningEnabled) return;

            spawningEnabled = true;
            // arrancamos la corrutina si no está en marcha
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
        // mientras no haya GameManager o no haya terminado la partida Y el spawner siga activo
        while ((CastanyeraGameManager.Instance == null || !CastanyeraGameManager.Instance.IsGameOver)
               && spawningEnabled)
        {
            float t = Mathf.Clamp01((Time.time - startTime) / difficultyRampTime);
            float interval = Mathf.Lerp(initialInterval, minInterval, t);
            float rockChance = Mathf.Lerp(initialRockChance, maxRockChance, t);

            TrySpawnOne(rockChance);

            yield return new WaitForSeconds(interval);
        }

        // Al salir del bucle, marcamos que ya no estamos spawneando
        spawnCoroutine = null;
    }

    // El resto igual…
    void TrySpawnOne(float rockChance)
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

        Instantiate(prefab, pos, Quaternion.identity);
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
