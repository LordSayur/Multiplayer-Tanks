using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour {

    public Camera m_camera;

	void Start () {
        if (m_camera != null)
        {
            m_camera = Camera.main;
        }
	}
	
	void Update () {
        transform.LookAt(transform.position + m_camera.transform.rotation * Vector3.forward, m_camera.transform.rotation * Vector3.up);
	}
}
