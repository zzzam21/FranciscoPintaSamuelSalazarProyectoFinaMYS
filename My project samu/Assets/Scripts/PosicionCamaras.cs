using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PosicionCamaras : MonoBehaviour
{
    // Start is called before the first frame update
    public Camera camara;

    public Toggle toggleFrontal;
    public Toggle toggleLateral;
    public Toggle toggleEsquina;
    public Toggle toggleTrasero;

    private void Start()
    {
        // Asignar funciones a los eventos de cambio
        toggleFrontal.onValueChanged.AddListener(delegate { if (toggleFrontal.isOn) CambiarVista("frontal"); });
        toggleLateral.onValueChanged.AddListener(delegate { if (toggleLateral.isOn) CambiarVista("lateral"); });
        toggleEsquina.onValueChanged.AddListener(delegate { if (toggleEsquina.isOn) CambiarVista("esquina"); });
        toggleTrasero.onValueChanged.AddListener(delegate { if (toggleTrasero.isOn) CambiarVista("trasero"); });
    }

    void CambiarVista(string tipo)
    {
        switch (tipo)
        {
            case "frontal":
                camara.transform.position = new Vector3(-13, 4, 1);
                camara.transform.rotation = Quaternion.Euler(-2, -267, 0);
                break;
            case "lateral":
                camara.transform.position = new Vector3(3.5f, 4, -21);
                camara.transform.rotation = Quaternion.Euler(-5.561f, 0, 0);
                break;
            case "esquina":
                camara.transform.position = new Vector3(-11, 10, 19);
                camara.transform.rotation = Quaternion.Euler(15, -220, 0);
                break;
            case "trasero":
                camara.transform.position = new Vector3(17, 5, 0);
                camara.transform.rotation = Quaternion.Euler(-3, -90, 0);
                break;
        }
    }
}
