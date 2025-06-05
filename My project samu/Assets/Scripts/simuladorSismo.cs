using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class simuladorSismo : MonoBehaviour
{
    // Start is called before the first frame update

    private Vector3 posicionInicial;
    private float tiempo;
    private Rigidbody rb;

    private float amplitudActual;
    private float frecuenciaActual;

    private float amplitudObjetivo;
    private float frecuenciaObjetivo;

    public float velocidadTransicion = 2f;

    void Start()
    {
        posicionInicial = transform.localPosition;
        rb = GetComponent<Rigidbody>();

        amplitudActual = 0f;
        frecuenciaActual = 0f;
        amplitudObjetivo = 0f;
        frecuenciaObjetivo = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        tiempo += Time.deltaTime;

        // Transición suave usando Lerp
        amplitudActual = Mathf.Lerp(amplitudActual, amplitudObjetivo, Time.deltaTime * velocidadTransicion);
        frecuenciaActual = Mathf.Lerp(frecuenciaActual, frecuenciaObjetivo, Time.deltaTime * velocidadTransicion);

        float desplazamientoX = Mathf.Sin(tiempo * frecuenciaActual) * amplitudActual;
        float desplazamientoZ = Mathf.Cos(tiempo * frecuenciaActual * 0.8f) * amplitudActual * 0.5f;

        Vector3 nuevaPos = posicionInicial + new Vector3(desplazamientoX, 0, desplazamientoZ);
        rb.MovePosition(nuevaPos);

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
                amplitudObjetivo = 0.5f;
                frecuenciaObjetivo = 4f;
                break;
            case 3: // fuerte
                amplitudObjetivo = 1f;
                frecuenciaObjetivo = 6f;
                break;
        }
    }
}
