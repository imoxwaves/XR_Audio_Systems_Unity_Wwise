using UnityEngine;
using System.Collections;

public class LamparaIntermitente : MonoBehaviour
{
    public Light luzDeLampara;
    public float tiempoDeParpadeo = 0.5f;
    public float tiempoDeDesvanecimiento = 0.2f;
    public float intensidadMaxima = 2.0f;

    private float intensidadOriginal;

    void Start()
    {
        // Almacena la intensidad original de la luz
        intensidadOriginal = luzDeLampara.intensity;
        // Inicia el parpadeo
        StartCoroutine(Parpadear());
    }

    IEnumerator Parpadear()
    {
        while (true)
        {
            // Encender la luz de golpe
            luzDeLampara.intensity = intensidadMaxima;
            luzDeLampara.enabled = true;

            // Esperar el tiempo de parpadeo antes de empezar a atenuar
            yield return new WaitForSeconds(tiempoDeParpadeo);

            // Iniciar la coroutine para el desvanecimiento
            yield return StartCoroutine(Desvanecer());

            // Esperar un tiempo antes de volver a encender
            yield return new WaitForSeconds(tiempoDeParpadeo);
        }
    }

    IEnumerator Desvanecer()
    {
        float tiempoTranscurrido = 0f;
        float intensidadActual = luzDeLampara.intensity;

        // Bucle para reducir la intensidad gradualmente
        while (tiempoTranscurrido < tiempoDeDesvanecimiento)
        {
            tiempoTranscurrido += Time.deltaTime;
            float factorDeDesvanecimiento = tiempoTranscurrido / tiempoDeDesvanecimiento;

            // Usar una curva de atenuación para un efecto más natural
            luzDeLampara.intensity = Mathf.Lerp(intensidadActual, 0, factorDeDesvanecimiento);

            yield return null; // Espera un frame
        }

        luzDeLampara.enabled = false; // Asegúrate de que la luz se apague completamente
    }
}