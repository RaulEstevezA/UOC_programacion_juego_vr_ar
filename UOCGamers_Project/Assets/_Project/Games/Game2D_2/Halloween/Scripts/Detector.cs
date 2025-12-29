using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem; // Importante: Nueva librería
using System.Collections.Generic;

public class Detector : MonoBehaviour
{
    void Update()
    {
        // En el nuevo sistema se detecta el clic así:
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Mouse.current.position.ReadValue();

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            if (results.Count > 0)
            {
                foreach (var result in results)
                {
                    Debug.Log("El clic golpeó a: " + result.gameObject.name);
                }
            }
            else
            {
                Debug.Log("El clic no golpeó ningún elemento de UI.");
            }
        }
    }
}