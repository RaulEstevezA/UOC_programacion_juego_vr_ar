using UnityEngine;
using TMPro; 

public class PuntuacionManager : MonoBehaviour
{
    public TextMeshProUGUI textoPuntos;
    private int puntuacion = 0;

    public void SumarPuntos(int cantidad)
    {
        puntuacion += cantidad;
        textoPuntos.text = puntuacion.ToString();
    }
}
