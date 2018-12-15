using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    public int m_NumRoundsToWin = 5; // round necessari per vincere il gioco
    public float m_StartDelay = 3f;
    public float m_EndDelay = 3f;
    public CameraControl m_CameraControl;
    public Text m_MessageText;
    public GameObject m_TankPrefab;
    public TankManager[] m_Tanks; // è un array di tank manager per gestire ciascuno ogni tank, per questo vogliamo mostrarlo nell'inspector

    private int m_RoundNumber; // numero del round
    private WaitForSeconds m_StartWait; // per mettere un delay iniziale nella coroutine
    private WaitForSeconds m_EndWait; // per mettere un delay finale nella coroutine
    private TankManager m_RoundWinner; // riferimento al tank manager per stampare il messaggio all'inizio del round
    private TankManager m_GameWinner; // riferimento al tank manager per stampare il messaggio alla fine del round

    private void Start() {
        m_StartWait = new WaitForSeconds(m_StartDelay);
        m_EndWait = new WaitForSeconds(m_EndDelay);

        SpawnAllTanks();
        SetCameraTargets();

        StartCoroutine(GameLoop()); // faccio partire una coroutine passandogli la funzione GameLoop, usa una couroutine perchè all'interno di GameLoop usa la funzione yield che sospende la funzione per tot tempo, poi
                                    // ritorna nella funzione e a seconda di una certa condizione ricomincia ad eseguire Gameloop dall'inizio, ritorna un IEnumerator
    }

    private void SpawnAllTanks() { // instazio tutti i tank
        for (int i = 0; i < m_Tanks.Length; i++) {
            m_Tanks[i].m_Instance = Instantiate(m_TankPrefab, m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation) as GameObject;
            m_Tanks[i].m_PlayerNumber = i + 1;
            m_Tanks[i].Setup();
        }
    }

    private void SetCameraTargets() {
        Transform[] targets = new Transform[m_Tanks.Length]; // della stessa lunghezza del tank manager array

        for (int i = 0; i < targets.Length; i++) // itero e setto ogni target alla pozione del tank in modo che la chemera sappia su cosa zoomare e come muoversi in presenza di più tank
            targets[i] = m_Tanks[i].m_Instance.transform;

        m_CameraControl.m_Targets = targets; // assegno i target alla camera
    }

    private IEnumerator GameLoop() {
        yield return StartCoroutine(RoundStarting()); // inizio la coroutine RoundStarting, solo al termine di essa a cui ho lasciato il "comando" con yield potrò proseguire alla prossima istruzione
        yield return StartCoroutine(RoundPlaying());
        yield return StartCoroutine(RoundEnding());

        if (m_GameWinner != null) // se il gioco è concluso ricarica il livello
            SceneManager.LoadScene(0);
        else 
            StartCoroutine(GameLoop()); // se non è concluso inizio una nuova coroutine di GameLoop
    }

    private IEnumerator RoundStarting() { // azioni da compiere prima dell'inizio del round
        ResetAllTanks();
        DisableTankControl();

        m_CameraControl.SetStartPositionAndSize();

        m_RoundNumber++;
        m_MessageText.text = "ROUND " + m_RoundNumber; // per far apparire a schermo il numero del round

        yield return m_StartWait; // aspetta per 3 secondi
    }

    private IEnumerator RoundPlaying() {
        EnableTankControl();

        m_MessageText.text = ""; // per togliere "ROUND N" dal centro dello schermo; oppure "m_MessageText.text = string.empty;"

        while(!OneTankLeft()) // itero finchè la condizione è vera, cioè finchè rimangono almeno 2 tank
            yield return null; // ritorno nulla per stare nel ciclo finchè non rimane 1 tank o meno
    }

    private IEnumerator RoundEnding() {
        DisableTankControl();

        m_RoundWinner = null; // per dire che non so chi ha vinto e dovrà controllare
        m_RoundWinner = GetRoundWinner(); // controllo qual è l'unico tank rimasto attivo e lo definsco come vincitore

        if (m_RoundWinner != null) // aumento il numero di vittorie del tank se ce nè uno
            m_RoundWinner.m_Wins++;

        m_GameWinner = GetGameWinner(); // se c'+ un tank che ha vinto m_NumRoundsToWin volte allora lo ritorno come vincitore del gioco, altrimenti null

        string message = EndMessage();
        m_MessageText.text = message; // per far apparire il messaggio

        yield return m_EndWait;
    }
    
    private bool OneTankLeft() {
        int numTanksLeft = 0;

        for (int i = 0; i < m_Tanks.Length; i++) {
            if (m_Tanks[i].m_Instance.activeSelf)
                numTanksLeft++;
        }

        return numTanksLeft <= 1;
    }

    private TankManager GetRoundWinner() {
        for (int i = 0; i < m_Tanks.Length; i++) {
            if (m_Tanks[i].m_Instance.activeSelf)
                return m_Tanks[i];
        }

        return null;
    }

    private TankManager GetGameWinner() {
        for (int i = 0; i < m_Tanks.Length; i++) {
            if (m_Tanks[i].m_Wins == m_NumRoundsToWin)
                return m_Tanks[i];
        }

        return null;
    }

    private string EndMessage() {
        string message = "DRAW!"; // per dafault dico che è pari

        if (m_RoundWinner != null)
            message = m_RoundWinner.m_ColoredPlayerText + " WINS THE ROUND!"; // se c'è un vincitore del round, m_RoundWinner.m_ColoredPlayerText per colorare il messagio come il player

        message += "\n\n\n\n"; // aggiungo dei ritorni a capo

        for (int i = 0; i < m_Tanks.Length; i++)
            message += m_Tanks[i].m_ColoredPlayerText + ": " + m_Tanks[i].m_Wins + " WINS\n"; // aggiungo sotto al testo del vincitore del round il numero di vittorie per ogni tank

        if (m_GameWinner != null)
            message = m_GameWinner.m_ColoredPlayerText + " WINS THE GAME!"; // se c'è un vinciore del gioco lo indico del messaggio

        return message;
    }

    private void ResetAllTanks() {
        for (int i = 0; i < m_Tanks.Length; i++)        
            m_Tanks[i].Reset();        
    }

    private void EnableTankControl() {
        for (int i = 0; i < m_Tanks.Length; i++)
            m_Tanks[i].EnableControl();        
    }

    private void DisableTankControl() {
        for (int i = 0; i < m_Tanks.Length; i++)
            m_Tanks[i].DisableControl();        
    }
}