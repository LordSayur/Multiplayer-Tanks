using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerSetup : NetworkBehaviour
{
    [SyncVar(hook = "UpdateColor")]
    public Color m_playerColor;
    public string m_basename = "PLAYER";

    [SyncVar(hook = "UpdateName")]
    public int m_playerNum = 1;
    public Text m_playerNameText;

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (m_playerNameText != null)
        {
            m_playerNameText.enabled = false;
        }
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        CmdSetupPlayer();
    }

    private void Start()
    {
        if (!isLocalPlayer)
        {
            UpdateName(m_playerNum);
            UpdateColor(m_playerColor);
        }
    }

    [Command]
    void CmdSetupPlayer()
    {
        Game.Instance.AddPlayer(this);
        Game.Instance.m_playerCount++;
    }

    private void UpdateName(int pNum)
    {
        if (m_playerNameText != null)
        {
            m_playerNameText.enabled = true;
            m_playerNameText.text = m_basename + pNum.ToString();
        }
    }

    private void UpdateColor(Color pColor)
    {
        MeshRenderer[] meshes = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer mesh in meshes)
        {
            mesh.material.color = pColor;
        }
    }
}
