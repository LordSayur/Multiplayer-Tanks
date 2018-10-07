using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerHealth : NetworkBehaviour
{
    [SyncVar(hook ="UpdateHealthBar")]
    float m_currentHealth;
    public float m_maxHealth = 3f;

    public GameObject m_deathPrefab;
    public RectTransform m_healthBar;
    [SyncVar]
    public bool m_isDead = false;

	void Start () 
    {
        Reset();
	}

    IEnumerator CountDown()
    {
        yield return new WaitForSeconds(1f);
        Damage(1f);
        UpdateHealthBar(m_currentHealth);

        yield return new WaitForSeconds(1f);
        Damage(1f);
        UpdateHealthBar(m_currentHealth);

        yield return new WaitForSeconds(1f);
        Damage(1f);
        UpdateHealthBar(m_currentHealth);
    }

    void UpdateHealthBar(float value)
    {
        m_healthBar.sizeDelta = new Vector2(value / m_maxHealth * 150f, m_healthBar.sizeDelta.y);
    }

    public void Damage(float damage)
    {
        if (!isServer)
        {
            return;
        }
        m_currentHealth-= damage;
        //UpdateHealthBar(m_currentHealth);

        if (m_currentHealth <= 0)
        {
            m_isDead = true;
            RpcDie();
        }
    }

    [ClientRpc]
    private void RpcDie()
    {
        if (m_deathPrefab)
        {
            GameObject deathFX = Instantiate(m_deathPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity) as GameObject;
            Destroy(deathFX, 3f);

            SetActiveState(false);

            gameObject.SendMessage("Disable");
        }
    }

    private void SetActiveState(bool state)
    {
        foreach (Collider c in GetComponentsInChildren<Collider>())
        {
            c.enabled = state;
        }

        foreach (Canvas c in GetComponentsInChildren<Canvas>())
        {
            c.enabled = state;
        }

        foreach (MeshRenderer c in GetComponentsInChildren<MeshRenderer>())
        {
            c.enabled = state;
        }
    }

    public void Reset()
    {
        m_currentHealth = m_maxHealth;

        SetActiveState(true);

        m_isDead = false;
    }
}
