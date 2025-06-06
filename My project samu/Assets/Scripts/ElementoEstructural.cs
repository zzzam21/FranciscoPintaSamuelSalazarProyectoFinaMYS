using UnityEngine;

public class ElementoEstructural : MonoBehaviour
{
    public enum TipoElemento { Viga, Columna }
    public TipoElemento tipoElemento;
    public MaterialConstruccion material;
    
    private Rigidbody rb;
    private Vector3 posicionInicial;
    private bool haSufridoDaño = false;
    
    [System.Serializable]
    public class MaterialConstruccion
    {
        public string nombre;
        public float valor;
        public float alturaMaximaCaida = 0.5f;
    }
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        posicionInicial = transform.position;
    }
    
    void Update()
    {
        if (!haSufridoDaño && VerificarDaño())
        {
            haSufridoDaño = true;
            float valorDaño = CalcularValorDaño();
            simuladorSismo.ReportarDaño(valorDaño);
        }
    }
    
    private bool VerificarDaño()
    {
        // Verificar si el elemento se ha desprendido (ya no está conectado)
        if (transform.position.y < posicionInicial.y - material.alturaMaximaCaida)
        {
            return true;
        }
        
        // Verificar si la velocidad angular es demasiado alta
        if (rb.angularVelocity.magnitude > 2f)
        {
            return true;
        }
        
        return false;
    }
    
    private float CalcularValorDaño()
    {
        float valorBase = tipoElemento == TipoElemento.Viga ? 1.5f : 2f;
        return valorBase * material.valor;
    }
}