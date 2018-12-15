using UnityEngine;

public class ShellExplosion : MonoBehaviour {

    public LayerMask m_TankMask; // riferimento al fatto che le esplosioni hanno effetto solo su ciò che sta sul layer del player
    public ParticleSystem m_ExplosionParticles; // per avviare i particellari durante un'esplosione
    public AudioSource m_ExplosionAudio; // per il suono dell'esplosione
    public float m_MaxDamage = 100f; // danni massimi a distanza minima
    public float m_ExplosionForce = 1000f; // forza dell'esplosione che respinge il tank a seconda della distanza dal centro
    public float m_MaxLifeTime = 2f; // tempo dell'esplosione, per sicurezza
    public float m_ExplosionRadius = 5f; // raggio dell'esplosione

    private void Start() {
        Destroy(gameObject, m_MaxLifeTime); // quando instanzion il proiettile se è ancora vivo dopo 2 secondi distruggilo
    }

    private void OnTriggerEnter(Collider other) { // richiamata quando qualcosa tocca il collider, funziona perchè abbiamo settato il flag Is Trigger
        // Find all the tanks in an area around the shell and damage them.
        // memorizzo in un array tutti i collider affetti dall'esplosione, per farlo creo una sfera entro cui vi sono i collider interessati, la sfera parte da transform.position, 
        // ha raggio m_ExplosionRadius ed ha effetto solo su ciò che sta sul layer dei tank
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TankMask); 

        for(int i = 0; i < colliders.Length; i++) { // itero sui colleder trovati
            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>(); // prendo il rigidbody di ogni collider

            if (!targetRigidbody) // controllo che il collider trovato abbia un rigidbody, sto lavorando sui tank che ce l'hanno quindi è inutile ma è un buon controllo 
                continue;

            targetRigidbody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius); // aggiungo forza al rigidbody, indico l'intensità, la posizione da cui applicarla e il raggio dell'esplosione
            TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth>(); // prendo la vita del tank prendendo come riferimento il suo rigidbody

            if (!targetHealth) 
                continue;

            float damage = CalculateDamage(targetRigidbody.position); // richiamo la funzione per calcolare i danni dicendogli la posizione del tank e quindi calcolandoli di conseguenza
            targetHealth.TakeDamage(damage); // applico i danni alla vita del tank
        }

        m_ExplosionParticles.transform.parent = null; // lo tolgo dai child per poterlo utilizzare anche dopo che abbiamo distrutto lo shell dopo 2 secondi
        m_ExplosionParticles.Play(); // avvio i particellari dell'esplosione
        m_ExplosionAudio.Play(); // avvio il suono dell'esplosione
        Destroy(m_ExplosionParticles.gameObject, m_ExplosionParticles.main.duration); // vogliamo distruggere il gameObject dove si trovano i particellari e non i particellari stessi, vive per la durata dei particellari
        Destroy(gameObject); // distruggo il gameObject dove mi trovo
    }

    private float CalculateDamage(Vector3 targetPosition) {
        // Calculate the amount of damage a target should take based on it's position.
        Vector3 explosionToTarget = targetPosition - transform.position; // vettore di distanza tra il tank e la posizione dell'esplosione, cioè la mia

        float explosionDistance = explosionToTarget.magnitude; // memorizzo la lunghezza del vettore, il valora va da 0 al raggio definito
        // ma noi vogliamo la distanza relativa come valore tra 0 e 1, pià mi avvicino a m_ExplosionRadius e più il valore sarà piccolo e va bene così perchè la distanza sarà maggiore e il danno minore
        float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;
        float damage = relativeDistance * m_MaxDamage; // vale m_MaxDamage se sono al centro dell'esplosione
        damage = Mathf.Max(0f, damage); // per essere sicuro che il danno non sia negativo, se lo fosse varrebbe 0, per evitare casi limite in cui il collider è nella sfera ma il tank ne è fuori
        return damage;
    }
}