using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Bullet : NetworkBehaviour {

    Rigidbody m_rigidBody;

    Collider m_collider;

    public ParticleSystem m_explosionFX;

    public float m_speed = 100f;

    public List<ParticleSystem> m_allParticles;

    public int m_lifetime = 5;

    public List<string> m_bounceTags;

    public List<string> m_collideTags;

    public int m_bounces = 2;

    public float m_damage = 1f;

	void Start () 
    {
        m_allParticles = GetComponentsInChildren<ParticleSystem>().ToList();
        m_rigidBody = GetComponent<Rigidbody>();
        m_collider = GetComponent<Collider>();

        StartCoroutine("SelfDestruct");
	}
	
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        CheckCollisions(collision);

        if (m_bounceTags.Contains(collision.gameObject.tag))
        {
            if (m_bounces <= 0)
            {
                Explode();
            }

            m_bounces--;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (m_rigidBody.velocity != Vector3.zero)
        {
            m_rigidBody.rotation = Quaternion.LookRotation(m_rigidBody.velocity);
        }
    }

    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(m_lifetime);
        Explode();
    }

    private void Explode()
    {
        m_collider.enabled = false;
        m_rigidBody.velocity = Vector3.zero;
        m_rigidBody.Sleep();

        foreach (var ps in GetComponentsInChildren<ParticleSystem>())
        {
            ps.Stop();
        }

        if (m_explosionFX != null)
        {
            m_explosionFX.transform.parent = null;
            m_explosionFX.Play();
            Destroy(m_explosionFX, 3f);
        }

        if (isServer)
        {
            foreach (MeshRenderer m in GetComponentsInChildren<MeshRenderer>())
            {
                m.enabled = false;
            }

            Destroy(gameObject);
        }
    }

    void CheckCollisions(Collision collision)
    {
        if (m_collideTags.Contains(collision.gameObject.tag))
        {
            Explode();
            PlayerHealth playerHealth = collision.gameObject.GetComponentInParent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.Damage(m_damage);
            }
        }
    }
}
