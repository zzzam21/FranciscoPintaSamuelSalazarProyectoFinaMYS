using UnityEngine;

public class simuladorSismo : MonoBehaviour
{
    private Vector3 posicionInicial;
    private float tiempo;
    private Rigidbody rb;

    private float amplitudActual;
    private float frecuenciaActual;

    private float amplitudObjetivo;
    private float frecuenciaObjetivo;

    [Header("Configuración General")]
    public float velocidadTransicion = 2f;
    private bool sismoActivo = false;
    private float duracionSismo = 10f;
    private float tiempoTranscurrido = 0f;

    [Header("Configuración de Detención")]
    public float tiempoTransicionDetencion = 3.0f;
    private bool deteniendo = false;
    private float amplitudInicialDetencion;
    private float frecuenciaInicialDetencion;
    private float tiempoDetencionIniciado;

    public delegate void DanioEstructuralHandler(float valorDanio);
    public static event DanioEstructuralHandler OnDanioEstructural;
    
    public static void ReportarDanio(float valorDanio)
    {
        OnDanioEstructural?.Invoke(valorDanio);
    }

    void Start()
    {
        posicionInicial = transform.localPosition;
        rb = GetComponent<Rigidbody>();

        amplitudActual = 0f;
        frecuenciaActual = 0f;
        amplitudObjetivo = 0f;
        frecuenciaObjetivo = 0f;
        sismoActivo = false;
    }

    void Update()
    {
        if (deteniendo)
        {
            ProcesarDetencionGradual();
        }
        else if (sismoActivo)
        {
            ProcesarSismoActivo();
        }
    }

    private void ProcesarDetencionGradual()
    {
        float progreso = (Time.time - tiempoDetencionIniciado) / tiempoTransicionDetencion;
        
        if (progreso >= 1f)
        {
            FinalizarDetencion();
        }
        else
        {
            // Interpolación cuadrática para suavizado
            float factor = 1 - Mathf.Pow(progreso, 2);
            amplitudObjetivo = amplitudInicialDetencion * factor;
            frecuenciaObjetivo = frecuenciaInicialDetencion * factor;
            
            ActualizarMovimientoSismo();
        }
    }

    private void FinalizarDetencion()
    {
        sismoActivo = false;
        deteniendo = false;
        amplitudObjetivo = 0f;
        frecuenciaObjetivo = 0f;
        rb.MovePosition(posicionInicial);
        tiempoTranscurrido = 0f;
    }

    private void ProcesarSismoActivo()
    {
        tiempo += Time.deltaTime;
        tiempoTranscurrido += Time.deltaTime;

        if (tiempoTranscurrido >= duracionSismo)
        {
            DetenerSismo();
            return;
        }

        amplitudActual = Mathf.Lerp(amplitudActual, amplitudObjetivo, Time.deltaTime * velocidadTransicion);
        frecuenciaActual = Mathf.Lerp(frecuenciaActual, frecuenciaObjetivo, Time.deltaTime * velocidadTransicion);

        ActualizarMovimientoSismo();
    }

    private void ActualizarMovimientoSismo()
    {
        float desplazamientoX = Mathf.Sin(tiempo * frecuenciaActual) * amplitudActual;
        float desplazamientoZ = Mathf.Cos(tiempo * frecuenciaActual * 0.8f) * amplitudActual * 0.5f;

        Vector3 nuevaPos = posicionInicial + new Vector3(desplazamientoX, 0, desplazamientoZ);
        rb.MovePosition(nuevaPos);
    }

    public void IniciarSismo(float duracion)
    {
        duracionSismo = duracion;
        tiempoTranscurrido = 0f;
        sismoActivo = true;
        deteniendo = false;
        tiempo = 0f;
        amplitudActual = 0f;
        frecuenciaActual = 0f;
    }

    public void DetenerSismo()
    {
        if (!sismoActivo || deteniendo) return;
        
        deteniendo = true;
        tiempoDetencionIniciado = Time.time;
        amplitudInicialDetencion = amplitudActual;
        frecuenciaInicialDetencion = frecuenciaActual;
    }

    public void SetAmplitud(float valor)
    {
        amplitudObjetivo = valor;
    }

    public void SetFrecuencia(float valor)
    {
        frecuenciaObjetivo = valor;
    }

    public void SetMagnitud(int nivel)
    {
        switch (nivel)
        {
            case 1: // leve
                amplitudObjetivo = 0.2f;
                frecuenciaObjetivo = 2f;
                break;
            case 2: // medio
                amplitudObjetivo = 0.6f;
                frecuenciaObjetivo = 4f;
                break;
            case 3: // fuerte
                amplitudObjetivo = 1f;
                frecuenciaObjetivo = 6f;
                break;
        }
    }
}