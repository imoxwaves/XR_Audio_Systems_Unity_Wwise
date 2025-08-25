using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LamparaIntermitente : MonoBehaviour
{
    // Variables públicas
    public float tiempoDeParpadeo = 0.5f;
    public float tiempoDeDesvanecimiento = 0.2f;

    // Variables privadas para las luces, el material y las propiedades del shader
    private List<Light> lucesDeLampara;
    private Renderer lamparaRenderer;
    private Material instanciaMaterial;
    private int emissionColorID;

    // Almacenamos el color inicial del material
    private Color colorDeEmisionInicial;
    private Dictionary<Light, float> intensidadInicialDeLuces;

    void Start()
    {
        // 1. Encuentra todos los componentes de luz en los hijos
        lucesDeLampara = new List<Light>(GetComponentsInChildren<Light>());
        intensidadInicialDeLuces = new Dictionary<Light, float>();
        foreach (Light luz in lucesDeLampara)
        {
            intensidadInicialDeLuces.Add(luz, luz.intensity);
        }

        // 2. Obtiene el renderer y su material en el objeto principal
        lamparaRenderer = GetComponent<Renderer>();
        if (lamparaRenderer == null)
        {
            lamparaRenderer = GetComponentInChildren<Renderer>();
        }

        if (lamparaRenderer != null)
        {
            instanciaMaterial = lamparaRenderer.material;
            if (instanciaMaterial != null)
            {
                emissionColorID = Shader.PropertyToID("_EmissiveColor");
                colorDeEmisionInicial = instanciaMaterial.GetColor(emissionColorID);
            }
        }

        // Si se encontraron luces o material, iniciamos la corrutina
        if (lucesDeLampara.Count > 0 || instanciaMaterial != null)
        {
            StartCoroutine(Parpadear());
        }
        else
        {
            UnityEngine.Debug.LogError("No se encontraron luces ni Renderer en la lámpara.");
        }
    }

    IEnumerator Parpadear()
    {
        while (true)
        {
            // Encender todas las luces y el material
            foreach (Light luz in lucesDeLampara)
            {
                luz.enabled = true;
                luz.intensity = intensidadInicialDeLuces[luz];
            }
            if (instanciaMaterial != null)
            {
                instanciaMaterial.SetColor(emissionColorID, colorDeEmisionInicial);
            }

            yield return new WaitForSeconds(tiempoDeParpadeo);

            yield return StartCoroutine(Desvanecer());

            yield return new WaitForSeconds(tiempoDeParpadeo);
        }
    }

    IEnumerator Desvanecer()
    {
        float tiempoTranscurrido = 0f;

        while (tiempoTranscurrido < tiempoDeDesvanecimiento)
        {
            tiempoTranscurrido += Time.deltaTime;
            float factorDeDesvanecimiento = tiempoTranscurrido / tiempoDeDesvanecimiento;

            // Atenuar todas las luces y el material
            foreach (Light luz in lucesDeLampara)
            {
                luz.intensity = Mathf.Lerp(intensidadInicialDeLuces[luz], 0, factorDeDesvanecimiento);
            }
            if (instanciaMaterial != null)
            {
                instanciaMaterial.SetColor(emissionColorID, Color.Lerp(colorDeEmisionInicial, Color.black, factorDeDesvanecimiento));
            }

            yield return null;
        }

        // Asegurar que las luces y el material queden completamente apagados
        foreach (Light luz in lucesDeLampara)
        {
            luz.enabled = false;
        }
        if (instanciaMaterial != null)
        {
            instanciaMaterial.SetColor(emissionColorID, Color.black);
        }
    }
}