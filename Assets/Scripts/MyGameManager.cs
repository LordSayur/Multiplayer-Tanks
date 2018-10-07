using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MyGameManager : NetworkManager 
{
    static MyGameManager instance;

    public static MyGameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<MyGameManager>();

                if (instance == null)
                {
                    instance = new GameObject().AddComponent<MyGameManager>();
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
}
