using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class textAmplitud : MonoBehaviour
{
    public TextMeshProUGUI texto;
    public void ActualizarTexto(float valor)
    {   
        texto.text = "Amplitud: " + valor.ToString("F2");
    }

}
