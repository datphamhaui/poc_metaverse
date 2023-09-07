using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : NetworkBehaviour
{

    private static NPC m_npc = null;
    public Vector3 position;
    public Transform helloThereChild;

    public static NPC Instance
    {
        get
        {
            return m_npc;
        }
    }
    // Start is called before the first frame update
    void Awake()
    {
        position = transform.position;
        helloThereChild = transform.Find("HelloThere");
        if (m_npc == null)
        {
            m_npc = this;
        }
            
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
