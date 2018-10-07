using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Game : NetworkBehaviour 
{

    public Text m_messageText;

    public int m_minPlayers = 2;
    int m_maxPlayers = 4;

    [SyncVar]
    public int m_playerCount = 0;

    public Color[] m_playerColors = { Color.yellow, Color.red, Color.blue, Color.black };

    static Game instance;

    public List<PlayerController> m_allPlayers;

    public List<Text> m_nameLabelText;
    public List<Text> m_playerScoreText;

    public static Game Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Game>();

                if (instance == null)
                {
                    instance = new GameObject().AddComponent<Game>();
                }
            }

            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartCoroutine("GameLoopRoutine");
    }

    IEnumerator GameLoopRoutine()
    {
        yield return StartCoroutine("EnterLobby");
        yield return StartCoroutine("PlayGame");
        yield return StartCoroutine("EndGame");
    }

    IEnumerator EnterLobby()
    {
        if (m_messageText != null)
        {
            m_messageText.gameObject.SetActive(true);
            m_messageText.text = "Waiting for players";
        }

        while (m_playerCount < m_minPlayers)
        {
            DisablePlayer();
            yield return null;
        }
    }

    IEnumerator PlayGame()
    {
        EnablePlayer();
        UpdateScoreboard();
        if (m_messageText != null)
        {
            m_messageText.gameObject.SetActive(false);
        }
        yield return null;
    }

    IEnumerator EndGame()
    {
        yield return null;
    }

    void SetPlayerState(bool state)
    {
        PlayerController[] allPlayers = GameObject.FindObjectsOfType<PlayerController>();

        foreach (PlayerController player in allPlayers)
        {
            player.enabled = state;
        }
    }

    void EnablePlayer()
    {
        SetPlayerState(true);
    }

    void DisablePlayer()
    {
        SetPlayerState(false);
    }

    public void AddPlayer(PlayerSetup pSetup)
    {
        if (m_playerCount < m_maxPlayers)
        {
            m_allPlayers.Add(pSetup.GetComponent<PlayerController>());
            pSetup.m_playerColor = m_playerColors[m_playerCount];
            pSetup.m_playerNum = m_playerCount + 1;
        }
    }

    [ClientRpc]
    void RpcUpdateScoreboard(string[] playerNames, string[] playerScores)
    {
        for (int i = 0; i < m_playerCount; i++)
        {
            if (playerNames[i] != null)
            {
                m_nameLabelText[i].text = playerNames[i];
            }

            if (playerScores[i] != null)
            {
                m_playerScoreText[i].text = playerScores[i].ToString();
            }
        }
    }

    public void UpdateScoreboard()
    {
        if (isServer)
        {
            string[] names = new string[m_playerCount];
            string[] scores = new string[m_playerCount];
            for (int i = 0; i < m_playerCount; i++)
            {
                names[i] = m_allPlayers[i].GetComponent<PlayerSetup>().m_playerNameText.text;
                scores[i] = m_allPlayers[i].m_score.ToString();
            }

            RpcUpdateScoreboard(names, scores);
        }
    }
}
