using UnityEngine;
using UnityEngine.UI;

public class TankShooting : MonoBehaviour {

    public int m_PlayerNumber = 1; // per sapere a quale input ci stiamo riferendo, di quale tank
    public Rigidbody m_Shell; // ci riferiamo al rigidbody del proiettile
    public Transform m_FireTransform; // per sapere da che parte sparare
    public Slider m_AimSlider; // per far rimpicciolire e ingrandire lo slider
    public AudioSource m_ShootingAudio; // suono
    public AudioClip m_ChargingClip; // clip
    public AudioClip m_FireClip;
    public float m_MinLaunchForce = 15f; // come quelli definiti per lo slider
    public float m_MaxLaunchForce = 30f;
    public float m_MaxChargeTime = 0.75f; // tempo che ci mette per caricare tra i 2 valori

    private string m_FireButton; // perchè gli input sono stringhe, c'è nè uno anche per sparare
    private float m_CurrentLaunchForce; // forza attuale 
    private float m_ChargeSpeed;
    private bool m_Fired; // per sparare solo 1 colpo per volta

    private void OnEnable() { // invocata ogni qualvolta viene attivato un tank 
        m_CurrentLaunchForce = m_MinLaunchForce; // regolata al minimo all'inizio
        m_AimSlider.value = m_MinLaunchForce; // regolata al minimo all'inizio
    }
    
    private void Start() { // attivata SOLO una volta
        m_FireButton = "Fire" + m_PlayerNumber; // es Fire1, come da input
        m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime; // la velocità è uguale a distanza/tempo
    }

    private void Update() {
        // Track the current state of the fire button and make decisions based on the current launch force.
        m_AimSlider.value = m_MinLaunchForce;

        if(m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired) { // alla carica massima quando non ho ancora sparato
            m_CurrentLaunchForce = m_MaxLaunchForce;
            Fire();
        }
        else if(Input.GetButtonDown(m_FireButton)){ // non siamo alla carica massima e abbiamo premuto il tasto per spare ma non l'abbiamo ancora rilasciato, stiamo spadando per la prima volta?
            m_Fired = false;
            m_CurrentLaunchForce = m_MinLaunchForce; // perchè ho premuto il tasto per la prima volta

            m_ShootingAudio.clip = m_ChargingClip; // salvo la clip
            m_ShootingAudio.Play(); // avvio l'audio
        }
        else if(Input.GetButton(m_FireButton) && !m_Fired) { // non siamo alla carica massima e abbiamo premuto il tasto per spare ma non l'abbiamo ancora rilasciato e non abbiamo ancora sparato
            m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime; // incrementa un po' la forza
            m_AimSlider.value = m_CurrentLaunchForce;
        }
        else if(Input.GetButtonUp(m_FireButton) && !m_Fired) { // rilascio il tasto e non ho ancora sparato
            Fire();
        }
    }

    private void Fire() {
        // Instantiate and launch the shell.
        m_Fired = true;

        Rigidbody shellInstance = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody; // instanzio il rigidbody del proiettile, as Rigidbody serve perchè la funzione ritorna un Object in realtà
        shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward; // la velocità è dalla parte davanti del transform del proitelle * la forza
        m_ShootingAudio.clip = m_FireClip; // questo ferma automaticamnte l'audio precedente
        m_ShootingAudio.Play();

        m_CurrentLaunchForce = m_MinLaunchForce; // per sicurezza
    }
}