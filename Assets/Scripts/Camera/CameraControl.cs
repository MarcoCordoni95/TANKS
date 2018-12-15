using UnityEngine;

public class CameraControl : MonoBehaviour {

    public float m_DampTime = 0.2f; // è il tempo approsimativamente richiesto per il movimento della camera
    public float m_ScreenEdgeBuffer = 4f; // per evitare che i tank raggiungano i lati dello schermo
    public float m_MinSize = 6.5f; // dimensione minima della camera 
    [HideInInspector] public Transform[] m_Targets; // vettore di posizioni dei tank, [HideInInspector] serve per evitare che appaia nell'inspector perchè non deve essere modificata da li ma lasciandola comunque public

    private Camera m_Camera;  // riferimento alla camera
    private float m_ZoomSpeed; // velocità di zoom
    private Vector3 m_MoveVelocity; //velocità di movimento
    private Vector3 m_DesiredPosition; // posizione che la camera cerca di raggiungere, con 2 tank la posizione sarà il centro tra i 2

    private void Awake() { // memorizzo la camera
        m_Camera = GetComponentInChildren<Camera>(); // il componente sta tra i figli dell'oggetto quindi uso GetComponentInChildren
    }

    private void FixedUpdate() { // usiamo FixedUpdate anche se la camera non è fisica e non colpisce nulla perchè comunque gestisce cose fisiche che bisogna tenere sincornizzate
        Move();
        Zoom();
    }

    private void Move() {
        FindAveragePosition(); // setta la posizione desiderata della camera a quella media e la salva dentro m_DesiredPosition
        // SmoothDamp si usa per spostare la camera dalla posizione attuale a quella desiderata ad una certa velocità in proporizione ai secondi, ref perchè quel valore sarà modificato dalla funzione ma poi sarà ripristinato
        transform.position = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime); 
    }

    private void FindAveragePosition() {
        Vector3 averagePos = new Vector3(); 
        int numTargets = 0;

        for (int i = 0; i < m_Targets.Length; i++) { // itero sul numero di tank

            if (!m_Targets[i].gameObject.activeSelf) // se il tank considerato non è attivo perchè è già morto allora saltiamo alla prossima iterazione con i=i+1
                continue;

            averagePos += m_Targets[i].position; // se invece è attivo sommo la sua posizione al vettore averagePos creato
            numTargets++; // aumento di 1 il numero di target perchè questo tank è vivo
        }

        if (numTargets > 0) // se ci sono tank attivi
            averagePos /= numTargets; // dividi l'averagePos trovato per il numero di tank attivi

        averagePos.y = transform.position.y; // mantieni la posizione Y del averagePos con quella attuale
        m_DesiredPosition = averagePos; // salva l'averagePos trovata  
    }

    private void Zoom() { // molto simile alla struttura del move
        float requiredSize = FindRequiredSize(); // come prima ma questa ritorna direttamente il valore
        m_Camera.orthographicSize = Mathf.SmoothDamp(m_Camera.orthographicSize, requiredSize, ref m_ZoomSpeed, m_DampTime); // come prima con il valore requiredSize calcolato
    }

    private float FindRequiredSize() {
        Vector3 desiredLocalPos = transform.InverseTransformPoint(m_DesiredPosition); // vogliamo trovare la size dalla posizione desiderata e non quella attuale
        float size = 0f; // cerchiamo la taglia più grande richiesta per la camera da tutti i tank

        for (int i = 0; i < m_Targets.Length; i++) { // come prima

            if (!m_Targets[i].gameObject.activeSelf) // come prima
                continue;

            Vector3 targetLocalPos = transform.InverseTransformPoint(m_Targets[i].position); // Otherwise, find the position of the target in the camera's local space
            Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos; // Find the position of the target from the desired position of the camera's local space

            size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.y)); // trovo il massimo tra la size già memorizzata e la Y trovata, verticalmente c'è solo size
            size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.x) / m_Camera.aspect); // anche per X dobbiamo trovare quanto ingrandire o ridurre la camera, divido per m_Camera.aspect perchè orizzontalmente lo schermo è size * aspect
        }
        
        size += m_ScreenEdgeBuffer; // aggiungo un margine per evitare che i tank tocchino il margine della camera
        size = Mathf.Max(size, m_MinSize); // mi assicuro di non zoomare troppo prendendo il massimo tra la size trovata e m_MinSize
        return size;
    }

    public void SetStartPositionAndSize() { // verrà utilizzato da un GameManager per risettare le posizioni e zoom iniziali della camera, qui si determinano solo i valori che dovrebbero avere, update farà il resto da solo
        FindAveragePosition();
        transform.position = m_DesiredPosition;
        m_Camera.orthographicSize = FindRequiredSize();
    }
}