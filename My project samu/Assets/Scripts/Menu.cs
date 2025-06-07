using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Menu : MonoBehaviour
{
    [Header("Opciones de Sismo")]
    public Toggle leveToggle;
    public Toggle medioToggle;
    public Toggle fuerteToggle;

    [Header("Opciones de Material")]
    public Toggle maderaToggle;
    public Toggle hormigonToggle;
    public Toggle metalToggle;

    [Header("Opciones de Estructura")]
    public Toggle casaToggle;
    public Toggle almacenToggle;
    public Toggle edificioToggle;

    [Header("Duración del Sismo")]
    public TMP_Dropdown duracionDropdown;
    public TMP_Text contadorTexto;

    [Header("Contador de Daños")]
    public TMP_Text dañosTexto;
    public TMP_Text porcentajePerdidaText;
    private float dañoTotal = 0f;
    public TMP_Text ValorTotalText;

    [Header("Botones")]
    public Button iniciarBoton;
    public Button detenerBoton;

    [Header("Prefabs de Casas")]
    public GameObject casaMadera;
    public GameObject casaHormigon;
    public GameObject casaMetal;

    [Header("Prefabs de Almacenes")]
    public GameObject almacenMadera;
    public GameObject almacenHormigon;
    public GameObject almacenMetal;

    [Header("Prefabs de Edificios")]
    public GameObject edificioMadera;
    public GameObject edificioHormigon;
    public GameObject edificioMetal;

    private GameObject estructuraActual;
    public simuladorSismo simulador;
    public GameObject suelo;



    public ElementoEstructural.MaterialConstruccion material = new ElementoEstructural.MaterialConstruccion();
    private float tiempoRestante;
    private float TotalValor;
    private float perdidaPorc;
    private bool contadorActivo = false;
   

    void Start()
    {
        ConfigurarFisicasEstables();
        ConfigurarBotones();
        contadorTexto.text = "";
        dañosTexto.text = "Perdida total: $ 0";

        
        simuladorSismo.OnDanioEstructural += RegistrarDanio;
    }

    void OnDestroy()
    {
        simuladorSismo.OnDanioEstructural -= RegistrarDanio;
    }

    void Update()
    {
        if (contadorActivo)
        {
            tiempoRestante -= Time.deltaTime;
            contadorTexto.text = $"Tiempo restante: {Mathf.CeilToInt(tiempoRestante)}s";

            if (tiempoRestante <= 0)
            {
                FinalizarSimulacion("Sismo terminado");
            }
        }
    }

    private void ConfigurarFisicasEstables()
    {
        Rigidbody rbSuelo = suelo.GetComponent<Rigidbody>();
        if (rbSuelo != null)
        {
            rbSuelo.isKinematic = true;
        }
    }

    private void ConfigurarBotones()
    {
        iniciarBoton.onClick.AddListener(IniciarSimulacion);
        detenerBoton.onClick.AddListener(DetenerSimulacion);
        detenerBoton.interactable = false;
    }

    public void IniciarSimulacion()
    {
        simulador.DetenerSismo();
        contadorActivo = false;
        
        if (estructuraActual != null)
        {
            Destroy(estructuraActual);
        }

        dañoTotal = 0f;
        dañosTexto.text = "Perdida Total: $ 0";
        contadorTexto.text = "";

        ConfigurarMagnitudSismo();

        float duracion = ObtenerDuracionSeleccionada();
        tiempoRestante = duracion;
        contadorActivo = true;

        GameObject prefabElegido = ObtenerPrefabSeleccionado();
        
        if (prefabElegido != null)
        {
            estructuraActual = Instantiate(prefabElegido, new Vector3(0, 0.5f, 0), Quaternion.identity);
            ConfigurarConexionSuelo();
            ConfigurarElementosEstructurales();
            
            simulador.IniciarSismo(duracion);
            porcentajePerdidaText.text = $"Perdida: 0%";
            if (maderaToggle.isOn & casaToggle.isOn)
            {
                TotalValor = 1800000;
            }
            else if (maderaToggle.isOn & almacenToggle.isOn)
            {
                TotalValor = 4500000;
            }
            else if (maderaToggle.isOn & edificioToggle.isOn)
            {
                TotalValor = 5800000;
            }
            else if (hormigonToggle.isOn & casaToggle.isOn)
            {
                TotalValor = 7200000;
            }
            else if (hormigonToggle.isOn & almacenToggle.isOn)
            {
                TotalValor = 18000000;
            }
            else if (hormigonToggle.isOn & edificioToggle.isOn)
            {
                TotalValor = 23200000;
            }
            else if (metalToggle.isOn & casaToggle.isOn)
            {
                TotalValor = 18500000;
            }
            else if (metalToggle.isOn & almacenToggle.isOn)
            {
                TotalValor = 45000000;
            }
            else if (metalToggle.isOn & edificioToggle.isOn)
            {
                TotalValor = 58000000;
            }
            ValorTotalText.text = $"Valor Estructura: $ {TotalValor:F1}";
            iniciarBoton.interactable = false;
            detenerBoton.interactable = true;
        }
        else
        {
            Debug.LogWarning("No se ha seleccionado un prefab válido");
        }
    }

    public void DetenerSimulacion()
    {
        simulador.DetenerSismo();
        StartCoroutine(EsperarYReactivarBotones(simulador.tiempoTransicionDetencion));
    }

    private IEnumerator EsperarYReactivarBotones(float tiempoEspera)
    {
        yield return new WaitForSeconds(tiempoEspera);
        FinalizarSimulacion("Simulación detenida");
    }

    private void FinalizarSimulacion(string mensaje)
    {
        contadorActivo = false;
        contadorTexto.text = mensaje;
        iniciarBoton.interactable = true;
        detenerBoton.interactable = false;
        StartCoroutine(LimpiarTextoContador());
    }

    private IEnumerator LimpiarTextoContador()
    {
        yield return new WaitForSeconds(2f);
        contadorTexto.text = "";
    }

    private void RegistrarDanio(float valorDanio)
    {
        if (contadorActivo != false) 
        {
            
            dañoTotal += valorDanio;
            dañosTexto.text = $"Daño total: $ {dañoTotal:F1}";
            StartCoroutine(AnimarTextoDaño());
            perdidaPorc = (100 * dañoTotal) / TotalValor;
            porcentajePerdidaText.text = $"Perdida: {perdidaPorc:F1} %";
        }
    }

    private IEnumerator AnimarTextoDaño()
    {
        dañosTexto.color = Color.red;
        dañosTexto.transform.localScale = Vector3.one * 1.2f;
        yield return new WaitForSeconds(0.5f);
        dañosTexto.color = Color.white;
        dañosTexto.transform.localScale = Vector3.one;
    }

    private float ObtenerDuracionSeleccionada()
    {
        switch (duracionDropdown.value)
        {
            case 0: return 10f;
            case 1: return 15f;
            case 2: return 20f;
            default: return 10f;
        }
    }

    private void ConfigurarMagnitudSismo()
    {
        if (leveToggle.isOn) simulador.SetMagnitud(1);
        else if (medioToggle.isOn) simulador.SetMagnitud(2);
        else if (fuerteToggle.isOn) simulador.SetMagnitud(3);
    }

    private GameObject ObtenerPrefabSeleccionado()
    {
        if (casaToggle.isOn) return ObtenerPrefabCasa();
        else if (almacenToggle.isOn) return ObtenerPrefabAlmacen();
        else if (edificioToggle.isOn) return ObtenerPrefabEdificio();
        return null;
    }

    private GameObject ObtenerPrefabCasa()
    {
        if (maderaToggle.isOn) return casaMadera;
        if (hormigonToggle.isOn) return casaHormigon;
        if (metalToggle.isOn) return casaMetal;
        return null;
    }

    private GameObject ObtenerPrefabAlmacen()
    {
        if (maderaToggle.isOn) return almacenMadera;
        if (hormigonToggle.isOn) return almacenHormigon;
        if (metalToggle.isOn) return almacenMetal;
        return null;
    }

    private GameObject ObtenerPrefabEdificio()
    {
        if (maderaToggle.isOn) return edificioMadera;
        if (hormigonToggle.isOn) return edificioHormigon;
        if (metalToggle.isOn) return edificioMetal;
        return null;
    }

    private void ConfigurarConexionSuelo()
    {
        if (estructuraActual == null) return;

        Transform baseEstructura = estructuraActual.transform.Find("Cimientos");
        if (baseEstructura != null)
        {
            FixedJoint joint = baseEstructura.GetComponent<FixedJoint>();
            Rigidbody rbSuelo = suelo.GetComponent<Rigidbody>();

            if (joint != null && rbSuelo != null)
            {
                joint.connectedBody = rbSuelo;
                joint.enableCollision = true;
                joint.massScale = 4.0f;
                joint.connectedMassScale = 0.25f;
                
                Rigidbody rbCimientos = baseEstructura.GetComponent<Rigidbody>();
                if (rbCimientos != null)
                {
                    rbCimientos.interpolation = RigidbodyInterpolation.Interpolate;
                    rbCimientos.collisionDetectionMode = CollisionDetectionMode.Continuous;
                    rbCimientos.maxAngularVelocity = 5f;
                }
            }
        }
    }

    private void ConfigurarElementosEstructurales()
    {

        if (maderaToggle.isOn)
        {
            material.nombre = "Madera";
            material.valor = 1.0f;
            material.alturaMaximaCaida = 0.5f;
        }
        else if (hormigonToggle.isOn)
        {
            material.nombre = "Hormigón";
            material.valor = 1.5f;
            material.alturaMaximaCaida = 0.4f;
        }
        else if (metalToggle.isOn)
        {
            material.nombre = "Metal";
            material.valor = 2.0f;
            material.alturaMaximaCaida = 0.6f;
        }

        foreach (Transform child in estructuraActual.transform)
        {
            ElementoEstructural elemento = child.GetComponent<ElementoEstructural>();
            if (elemento != null)
            {
                elemento.material = material;
                
                if (child.name.ToLower().Contains("viga"))
                {
                    elemento.tipoElemento = ElementoEstructural.TipoElemento.Viga;
                }
                else if (child.name.ToLower().Contains("columna"))
                {
                    elemento.tipoElemento = ElementoEstructural.TipoElemento.Columna;
                }
            }
        }
    }
}