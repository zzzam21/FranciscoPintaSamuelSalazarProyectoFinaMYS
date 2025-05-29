using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class textFrecuencia : MonoBehaviour
{
    public TextMeshProUGUI texto;
    public void ActualizarTexto(float valor)
    {
        texto.text = "Frecuencia: " + valor.ToString("F2");
    }
}
