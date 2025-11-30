using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Collections.Generic;

public class ARPlaneSpawner : MonoBehaviour
{
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

        // Obtenemos todos los planos
        List<ARPlane> planes = new List<ARPlane>();
        foreach (var p in planeManager.trackables)
            planes.Add(p);

        // Elegimos un plano aleatorio
        ARPlane randomPlane = planes[Random.Range(0, planes.Count)];

        // Elegimos un punto aleatorio dentro del plano
        Vector3 randomPoint = randomPlane.center
                              + new Vector3(
                                  Random.Range(-0.2f, 0.2f),
                                  0,
                                  Random.Range(-0.2f, 0.2f)
                                );

        // Colocar Tió sobre el plano
        tioInstance = Instantiate(tioPrefab, randomPoint, Quaternion.identity);

        // Que mire hacia la cámara siempre
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

        // Opcional: giro lateral (como pediste)
        tioInstance.transform.Rotate(0, -40, 0);
    }
}