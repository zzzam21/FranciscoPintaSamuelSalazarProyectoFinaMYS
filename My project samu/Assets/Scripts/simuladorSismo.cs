using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class simuladorSismo : MonoBehaviour
{
    // Start is called before the first frame update
    public float amplitud = 0.5f;
    public float frecuencia = 5f;

    private Vector3 posicionInicial;
    private float tiempo;
    private Rigidbody rb;

    void Start()
    {
        posicionInicial = transform.localPosition;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        tiempo += Time.deltaTime;
        float desplazamientoX = Mathf.Sin(tiempo * frecuencia) * amplitud;
        float desplazamientoZ = Mathf.Cos(tiempo * frecuencia * 0.8f) * amplitud * 0.5f;
        Vector3 nuevaPos = posicionInicial + new Vector3(desplazamientoX, 0, desplazamientoZ);
        rb.MovePosition(nuevaPos);
        
    }

    public void SetAmplitud(float valor)
    {
        amplitud = valor;
    }

    public void SetFrecuencia(float valor)
    {
        frecuencia = valor;
    }
}
