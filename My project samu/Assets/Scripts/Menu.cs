using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    private float dañoTotal = 0f;

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

    private float tiempoRestante;
    private bool contadorActivo = false;

    void Start()
    {
        ConfigurarFisicasEstables();
        ConfigurarBotones();
        contadorTexto.text = "";
        dañosTexto.text = "Daño total: 0";
        
        // Suscribirse a eventos de daño
        simuladorSismo.OnDañoEstructural += RegistrarDaño;
    }

    void OnDestroy()
    {
        // Desuscribirse para evitar memory leaks
        simuladorSismo.OnDañoEstructural -= RegistrarDaño;
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
        // Detener simulación anterior
        simulador.DetenerSismo();
        contadorActivo = false;
        
        // Eliminar estructura existente
        if (estructuraActual != null)
        {
            Destroy(estructuraActual);
        }

        // Resetear contadores
        dañoTotal = 0f;
        dañosTexto.text = "Daño total: 0";
        contadorTexto.text = "";

        // Configurar magnitud del sismo
        ConfigurarMagnitudSismo();

        // Configurar duración
        float duracion = ObtenerDuracionSeleccionada();
        tiempoRestante = duracion;
        contadorActivo = true;

        // Seleccionar y crear el prefab adecuado
        GameObject prefabElegido = ObtenerPrefabSeleccionado();
        
        if (prefabElegido != null)
        {
            // Instanciar con pequeño offset vertical
            estructuraActual = Instantiate(prefabElegido, new Vector3(0, 0.5f, 0), Quaternion.identity);
            
            // Configurar conexión con el suelo
            ConfigurarConexionSuelo();
            
            // Configurar materiales y valores de daño
            ConfigurarElementosEstructurales();
            
            // Iniciar simulación con duración
            simulador.IniciarSismo(duracion);
            
            // Actualizar estado de botones
            iniciarBoton.interactable = false;
            detenerBoton.interactable = true;
        }
        else
        {
            Debug.LogWarning("No se ha seleccionado un prefab válido");
        }
    }

    private void ConfigurarElementosEstructurales()
    {
        ElementoEstructural.MaterialConstruccion material = new ElementoEstructural.MaterialConstruccion();

        // Determinar material seleccionado
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

        // Aplicar a todos los elementos estructurales
        foreach (Transform child in estructuraActual.transform)
        {
            ElementoEstructural elemento = child.GetComponent<ElementoEstructural>();
            if (elemento != null)
            {
                elemento.material = material;
                
                // Determinar si es viga o columna por nombre
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

    public void DetenerSimulacion()
    {
        FinalizarSimulacion("Simulación detenida");
    }

    private void FinalizarSimulacion(string mensaje)
    {
        simulador.DetenerSismo();
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

    private void RegistrarDaño(float valorDaño)
    {
        dañoTotal += valorDaño;
        dañosTexto.text = $"Daño total: {dañoTotal:F1}";
        
        // Efecto visual cuando hay daño
        StartCoroutine(AnimarTextoDaño());
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
        if (leveToggle.isOn)
        {
            simulador.SetMagnitud(1);
        }
        else if (medioToggle.isOn)
        {
            simulador.SetMagnitud(2);
        }
        else if (fuerteToggle.isOn)
        {
            simulador.SetMagnitud(3);
        }
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

    private IEnumerator EstabilizarEstructura()
    {
        yield return new WaitForFixedUpdate();
        
        if (estructuraActual != null)
        {
            Rigidbody[] rbs = estructuraActual.GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody rb in rbs)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }
}