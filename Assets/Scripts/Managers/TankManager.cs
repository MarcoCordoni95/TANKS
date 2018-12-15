using System;
using UnityEngine;

[Serializable] // sinifica per unity: quando hai un'istaza di questo puoi mostrarle nell'inspector, nonrmalmente non serve ma non ereditando da monobehaviour serve qui
public class TankManager { // non eredita da monobehaviour quindi non eridate funzioni come start, awake, fixedupdate, ecc...
    public Color m_PlayerColor;
    public Transform m_SpawnPoint;
    [HideInInspector] public int m_PlayerNumber; // per gli input di ogni tank
    [HideInInspector] public string m_ColoredPlayerText;
    [HideInInspector] public GameObject m_Instance; // per fare on/off al tank quando occorre
    [HideInInspector] public int m_Wins; // numero vittorie del tank

    private TankMovement m_Movement;
    private TankShooting m_Shooting;
    private GameObject m_CanvasGameObject; // per fare on/off con l'UI

    public void Setup() { // ricahimata dal gameManager
        m_Movement = m_Instance.GetComponent<TankMovement>();
        m_Shooting = m_Instance.GetComponent<TankShooting>();
        m_CanvasGameObject = m_Instance.GetComponentInChildren<Canvas>().gameObject; // per prenderlo dai children

        m_Movement.m_PlayerNumber = m_PlayerNumber;
        m_Shooting.m_PlayerNumber = m_PlayerNumber;

        m_ColoredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(m_PlayerColor) + ">PLAYER " + m_PlayerNumber + "</color>"; // per il tasto in HTML-like sull'UI della vittoria

        MeshRenderer[] renderers = m_Instance.GetComponentsInChildren<MeshRenderer>(); // prendo un array di MeshRenderer, i MeshRenderer sono quelle cose che mostrano gli oggetti 3d nella scena, tipo cingoli, torretta, ecc...

        for (int i = 0; i < renderers.Length; i++)
            renderers[i].material.color = m_PlayerColor; // per tutti i MeshRenderer trovati setto il loro material.color al m_PlayerColor
    }

    public void DisableControl() { // disattiva tutto
        m_Movement.enabled = false;
        m_Shooting.enabled = false;

        m_CanvasGameObject.SetActive(false);
    }

    public void EnableControl() { // attiva tutto
        m_Movement.enabled = true;
        m_Shooting.enabled = true;

        m_CanvasGameObject.SetActive(true);
    }
    
    public void Reset() { // resetta l'istanza, la disattiva e riattiva e ripristina posizione e direzione
        m_Instance.transform.position = m_SpawnPoint.position;
        m_Instance.transform.rotation = m_SpawnPoint.rotation;

        m_Instance.SetActive(false);
        m_Instance.SetActive(true);
    }
}
