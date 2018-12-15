using UnityEngine;
using UnityEngine.UI;

public class TankHealth : MonoBehaviour {
    public float m_StartingHealth = 100f;          
    public Slider m_Slider;                        
    public Image m_FillImage; // per cambiare colore dell'immagine in base alla vita
    public Color m_FullHealthColor = Color.green; // per dare la possibilità di cambiarle
    public Color m_ZeroHealthColor = Color.red;
    public GameObject m_ExplosionPrefab;
    
    private AudioSource    m_ExplosionAudio;
    private ParticleSystem m_ExplosionParticles;
    private float m_CurrentHealth;
    private bool m_Dead;

    private void Awake() { //richiamata all'inizio del gioco
        m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>(); //creo un'instanza di m_ExplosionPrefab e prendo il suo component ParticleSystem
        m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();

        m_ExplosionParticles.gameObject.SetActive(false); // lo pongo a false perchè non voglio che esploda ad inizio gioco, uso SetActive e lo mantengo in memoria e non destroy per evitare che il garbage collector possa eliminarlo
    }
    
    private void OnEnable() { // eseguita alla creazione di un tank
        m_CurrentHealth = m_StartingHealth;
        m_Dead = false;

        SetHealthUI();
    }

    public void TakeDamage(float amount) {
        // Adjust the tank's current health, update the UI based on the new health and check whether or not the tank is dead.
        m_CurrentHealth -= amount;
        SetHealthUI(); // per aggiornare l'interfaccia

        if(m_CurrentHealth <= 0f && !m_Dead) {
            OnDeath();
        }
    }

    private void SetHealthUI() {
        // Adjust the value and colour of the slider.
        m_Slider.value = m_CurrentHealth;
        m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);  // lerp = linear interpolation tra il valore iniziale e quello finale, il terzo paramentro è la percentuale attuale
    }
    
    private void OnDeath() {
        // Play the effects for the death of the tank and deactivate it.
        m_Dead = true;
        m_ExplosionParticles.transform.position = transform.position; // per mettere i particellari dell'esplosione dove si trova il tank
        m_ExplosionParticles.gameObject.SetActive(true); // ri abilito l'esplosione
        m_ExplosionParticles.Play(); // avvio l'esplosione
        m_ExplosionAudio.Play(); // avvio il suono dell'esplosione
        gameObject.SetActive(false); // disabilito il tank
    }
}