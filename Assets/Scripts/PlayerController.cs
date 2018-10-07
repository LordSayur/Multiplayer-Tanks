using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PlayerHealth))]
[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(PlayerSetup))]
public class PlayerController : NetworkBehaviour
{

    PlayerHealth m_pHealth;
    PlayerMotor m_pMotor;
    PlayerSetup m_pSetup;
    PlayerShoot m_pShoot;

    Vector3 m_originalPosition;
    NetworkStartPosition[] m_spawnPoints;
    public GameObject m_spawnFX;

    public int m_score;

    public override void OnStartLocalPlayer()
    {
        m_originalPosition = transform.position;
        m_spawnPoints = FindObjectsOfType<NetworkStartPosition>();
    }

    void Start () {
        m_pHealth = GetComponent<PlayerHealth>();
        m_pMotor = GetComponent<PlayerMotor>();
        m_pSetup = GetComponent<PlayerSetup>();
        m_pShoot = GetComponent<PlayerShoot>();
	}
	
	void Update ()
    {
        if (!isLocalPlayer || m_pHealth.m_isDead)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            m_pShoot.Shoot();
        }

        Vector3 inputDirection = GetInput();
        if (inputDirection.sqrMagnitude > 0.25f)
        {
            m_pMotor.RotateChassis(inputDirection);
        }

        Vector3 turretDir = Utility.GetWorldPointFromScreenPoint(Input.mousePosition, m_pMotor.m_turret.position.y) - m_pMotor.m_turret.position;
        m_pMotor.RotateTurret(turretDir);
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer || m_pHealth.m_isDead)
        {
            return;
        }
        Vector3 inputDirection = GetInput();
        m_pMotor.MovePlayer(inputDirection);
    }

    Vector3 GetInput()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        return new Vector3(h, 0, v);
    }

    private void Disable()
    {
        StartCoroutine("RespawnRoutine");
    }

    IEnumerator RespawnRoutine()
    {
        transform.position = GetRandomSpawnPoint();
        m_pMotor.m_rigidbody.velocity = Vector3.zero;
        yield return new WaitForSeconds(3f);
        m_pHealth.Reset();

        if (m_spawnFX != null)
        {
            GameObject spawnFX = Instantiate(m_spawnFX, transform.position + Vector3.up * 0.5f, Quaternion.identity) as GameObject;
            Destroy(spawnFX, 3f);
        }
    }

    Vector3 GetRandomSpawnPoint()
    {
        if (m_spawnPoints != null)
        {
            if (m_spawnPoints.Length > 0)
            {
                NetworkStartPosition startPosition = m_spawnPoints[Random.Range(0, m_spawnPoints.Length)];
                return startPosition.transform.position;
            }
        }
        return m_originalPosition;
    }
}
