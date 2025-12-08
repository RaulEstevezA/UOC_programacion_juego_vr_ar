using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Collections.Generic;

public class ARPlaneSpawner : MonoBehaviour
{
    // Referencia global al último Tió colocado
    public static GameObject CurrentTio { get; private set; }

    public GameObject tioPrefab;
    public Camera arCamera;

    private ARPlaneManager planeManager;
    private GameObject tioInstance;
    private bool placed = false;

    void Start()
    {
        planeManager = FindObjectOfType<ARPlaneManager>();
    }

    void Update()
    {
        if (placed) return;
        if (planeManager == null) return;

        // Si no hay planos todavía, esperamos
        if (planeManager.trackables.count == 0) return;

        // Obtenemos todos los planos detectados
        List<ARPlane> planes = new List<ARPlane>();
        foreach (var p in planeManager.trackables)
            planes.Add(p);

        if (planes.Count == 0) return;

        // Elegimos un plano aleatorio
        ARPlane randomPlane = planes[Random.Range(0, planes.Count)];

        // Elegimos un punto cerca del centro del plano
        Vector3 randomPoint = randomPlane.center
                              + new Vector3(
                                  Random.Range(-0.2f, 0.2f),
                                  0,
                                  Random.Range(-0.2f, 0.2f)
                                );

        // Colocar el Tió sobre el plano
        tioInstance = Instantiate(tioPrefab, randomPoint, Quaternion.identity);

        // Guardamos referencia global
        CurrentTio = tioInstance;

        // Marcamos que ya hemos colocado uno
        placed = true;
    }

    void LateUpdate()
    {
        if (!placed) return;
        if (tioInstance == null) return;

        // Sticker AR: siempre orientado a la cámara
        Vector3 lookPos = arCamera.transform.position;
        lookPos.y = tioInstance.transform.position.y;
        tioInstance.transform.LookAt(lookPos);

        // Giro lateral para que se vea mejor
        tioInstance.transform.Rotate(0, -40, 0);
    }
}

