using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public Toggle leveToggle;
    public Toggle medioToggle;
    public Toggle fuerteToggle;

    public Toggle maderaToggle;
    public Toggle hormigonToggle;
    public Toggle metalToggle;

    public Toggle casaToggle;
    public Toggle almacenToggle;
    public Toggle edificioToggle;

    public GameObject casaPrefab;
    public GameObject almacenPrefab;
    public GameObject edificioPrefab;

    private GameObject estructuraActual;

    public simuladorSismo simulador; // Script del suelo

    public GameObject suelo; // Asigna el suelo desde el inspector


    public void IniciarSimulacion()
    {
        if (estructuraActual != null)
        {
            Destroy(estructuraActual);
        }

        // MAGNITUD
        if (leveToggle.isOn)
        {
            simulador.SetMagnitud(1); // Internamente defines amplitud y duración
        }
        else if (medioToggle.isOn)
        {
            simulador.SetMagnitud(2);
        }
        else if (fuerteToggle.isOn)
        {
            simulador.SetMagnitud(3);
        }

        // ESTRUCTURA
        GameObject prefabElegido = null;

        if (casaToggle.isOn)
            prefabElegido = casaPrefab;
        else if (almacenToggle.isOn)
            prefabElegido = almacenPrefab;
        else if (edificioToggle.isOn)
            prefabElegido = edificioPrefab;

        estructuraActual = Instantiate(prefabElegido, Vector3.zero, Quaternion.identity);

        // Instanciar en el origen

        // Buscar la base (por nombre o tag, aquí ejemplo por nombre)
        Transform baseEstructura = estructuraActual.transform.Find("Cimientos");

        if (baseEstructura != null)
        {
            FixedJoint joint = baseEstructura.GetComponent<FixedJoint>();
            Rigidbody rbSuelo = suelo.GetComponent<Rigidbody>();

            if (joint != null && rbSuelo != null)
            {
                joint.connectedBody = rbSuelo;
                Debug.Log("Joint conectado al suelo.");
            }
            else
            {
                Debug.LogWarning("No se encontró el FixedJoint o Rigidbody del suelo.");
            }
        }
        else
        {
            Debug.LogWarning("No se encontró la base de la estructura.");
        }

        // MATERIAL
        Material nuevoMaterial = null;
        float masa = 1f;

        if (maderaToggle.isOn)
        {
            nuevoMaterial = Resources.Load<Material>("Materials/Madera");
            masa = 2f;
        }
        else if (hormigonToggle.isOn)
        {
            nuevoMaterial = Resources.Load<Material>("Materials/Hormigon");
            masa = 6f;
        }
        else if (metalToggle.isOn)
        {
            nuevoMaterial = Resources.Load<Material>("Materials/Metal");
            masa = 8f;
        }

        // Cambiar materiales y masas a todos los bloques de la estructura
        foreach (Rigidbody rb in estructuraActual.GetComponentsInChildren<Rigidbody>())
        {
            string nombre = rb.gameObject.name.ToLower();
            if (nombre.Contains("viga") || nombre.Contains("columna"))
            {
                rb.mass = masa;
                rb.GetComponent<Renderer>().material = nuevoMaterial;
            }
        }
            
    }
    
    

}

