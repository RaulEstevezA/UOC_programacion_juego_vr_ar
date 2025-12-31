using UnityEngine;
using TMPro; 

public class PuntuacionManager : MonoBehaviour
{
    public TextMeshProUGUI textoPuntos;
    private int puntuacion = 0;

    void Start()
    {
        ActualizarTexto();
    }

    public void SumarPuntos(int cantidad)
    {
        puntuacion += cantidad;
        ActualizarTexto();
    }

    public int ObtenerPuntuacion()
    {
        return puntuacion;
    }

    private void ActualizarTexto()
    {
        if (textoPuntos != null)
            textoPuntos.text = $"Puntos: {puntuacion}";
    }
}
