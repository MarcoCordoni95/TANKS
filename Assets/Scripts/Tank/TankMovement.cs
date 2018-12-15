using UnityEngine;

public class TankMovement : MonoBehaviour {

    public int m_PlayerNumber = 1; // per dire che sono il player numero 1, utile per far apparire "il player 1 ha vinto!"
    public float m_Speed = 12f;
    public float m_TurnSpeed = 180f;
    public AudioSource m_MovementAudio;
    public AudioClip m_EngineIdling;
    public AudioClip m_EngineDriving;
    public float m_PitchRange = 0.2f;

    private string m_MovementAxisName;     
    private string m_TurnAxisName;         
    private Rigidbody m_Rigidbody;         
    private float m_MovementInputValue;    
    private float m_TurnInputValue;        
    private float m_OriginalPitch;         


    private void Awake() { // viene eseguito a prescindere che il tank sia on o off
        m_Rigidbody = GetComponent<Rigidbody>();
    }


    private void OnEnable () { // viene eseguito quando viene avviato il tank
        m_Rigidbody.isKinematic = false; // quando il tank è avviato esso non deve essere kinematic per motivi di efficenza
        m_MovementInputValue = 0f;
        m_TurnInputValue = 0f;
    }


    private void OnDisable () { // viene eseguito quando viene disattivato il tank
        m_Rigidbody.isKinematic = true; // se morto lo rendiamo kinematic per efficenza, non viene più effetto da nessuna forza
    }


    private void Start() {
        m_MovementAxisName = "Vertical" + m_PlayerNumber; // setto il nome degli assi, Vertical1 perchè è il nome definito in Edit -> Input
        m_TurnAxisName = "Horizontal" + m_PlayerNumber;

        m_OriginalPitch = m_MovementAudio.pitch; // store il pitch
    }
    

    private void Update() { // funzione in cui vengono calcolati gli input di movimento ogni frame
        // Store the player's input and make sure the audio for the engine is playing.
        m_MovementInputValue = Input.GetAxis(m_MovementAxisName);
        m_TurnInputValue = Input.GetAxis(m_TurnAxisName); 

        EngineAudio(); // per eseguire il corretto audio ad ogni frame
    }


    private void EngineAudio() {
        // Play the correct audio clip based on whether or not the tank is moving and what audio is currently playing.

        if((Mathf.Abs(m_MovementInputValue) < 0.1f) && (Mathf.Abs(m_TurnInputValue) < 0.1f)) { // quindi se è praticamente immobile

            if(m_MovementAudio.clip == m_EngineDriving) { // per essere sicuri di non eseguire quello sbagliato
                m_MovementAudio.clip = m_EngineIdling;
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();
            }
        }
        else {
            if (m_MovementAudio.clip == m_EngineIdling) { // per essere sicuri di non eseguire quello sbagliato
                m_MovementAudio.clip = m_EngineDriving;
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();
            }
        }
    }


    private void FixedUpdate() { // come Update ma viene usata per applicare il movimento, viene calcolata dopo ogni step della fisica
        // Move and turn the tank.
        Move();
        Turn();
    }


    private void Move() {
        // Adjust the position of the tank based on the player's input.
        Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime; // creo vettore = la direzione del tank * vettore che indica il movimento * velocità * proporziono la velocità ai secondi
        m_Rigidbody.MovePosition(m_Rigidbody.position + movement); // mi sposto dalla posizione attuale (m_Rigidbody.position) di movement passi, se usassi solo (m_Rigidbody + movement) mi sposterei dal centro del mondo sempre
    }


    private void Turn() {
        // Adjust the rotation of the tank based on the player's input.
        float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f); // i quaternion sono usati per memorizzare la rotazione ma sono definibili tramite vector3, ruotiamo solo attorno all'asse Y
        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
    }
}