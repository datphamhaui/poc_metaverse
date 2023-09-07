using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandleWithNPC : NetworkBehaviour
{

    private float interactionRange = 3.0f; // The range at which the player can interact with the NPC.
    private GameObject npc; // The NPC GameObject.
    private GameObject chatNpcCanvas;
    private GameObject photonCanvas;
    GameObject camera;

    // Start is called before the first frame update
    protected void Awake()
    {

        npc = GameObject.Find("NPC");
        chatNpcCanvas = GameObject.Find("ChatView");
        photonCanvas = GameObject.Find("PhotoChatCanvas");
        camera = GameObject.Find("FreeLook Camera");


    }

    // Update is called once per frame
    void Update()
    {
        if (Object.HasInputAuthority)
        {
            bool isNear = IsNearNPC();
            if (isNear)
            {
                if (photonCanvas.activeSelf)
                {
                    chatNpcCanvas.transform.Find("PressToChat").gameObject.SetActive(true);
                }
                else
                {
                    chatNpcCanvas.transform.Find("PressToChat").gameObject.SetActive(false);

                }

                Transform textHello = npc.transform.Find("HelloThere");
                if (textHello)
                {
                    if (!textHello.gameObject.activeSelf)
                    {
                        textHello.gameObject.SetActive(true);

                    }
                }
                if (photonCanvas.activeSelf && Input.GetKeyDown(KeyCode.E))
                {
                    gameObject.GetComponent<PlayerInput>().enabled = false;
                    camera.SetActive(false);
                    photonCanvas.SetActive(false);
                    foreach (Transform child in chatNpcCanvas.transform)
                    {
                        child.gameObject.SetActive(true);
                    }
                }
            }
            else
            {
                Transform textHello = npc.transform.Find("HelloThere");
                textHello.gameObject.SetActive(false);
                chatNpcCanvas.transform.Find("PressToChat").gameObject.SetActive(false);
            }


            if (!gameObject.GetComponent<PlayerInput>().enabled && photonCanvas.activeSelf)
            {
                gameObject.GetComponent<PlayerInput>().enabled = true;
                camera.SetActive(true);
            }
        }



    }

    bool IsNearNPC()
    {
        float distance = Vector3.Distance(transform.position, npc.transform.position);
        Debug.Log("distance: " + distance);
        Debug.Log(" transform.position: " + transform.position);
        Debug.Log("transform.position: " + transform.position);
        return distance <= interactionRange;
    }
}
