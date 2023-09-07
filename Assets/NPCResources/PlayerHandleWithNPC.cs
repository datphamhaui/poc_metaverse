using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandleWithNPC : NetworkBehaviour
{

    private float interactionRange = 3.0f; // The range at which the player can interact with the NPC.
    private Vector3 npcPosition; // The NPC GameObject.
    private Transform npcHello; // The NPC GameObject.
    private GameObject chatNpcCanvas;
    private GameObject photonCanvas;
    GameObject camera;

    // Start is called before the first frame update
    protected void Awake()
    {

        npcPosition = NPC.Instance.position;
        npcHello = NPC.Instance.helloThereChild;
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

                Transform textHello = npcHello;
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
                Transform textHello = npcHello;
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
        float distance = Vector3.Distance(transform.position, npcPosition);
        Debug.Log("distance: " + distance);
        Debug.Log(" transform.position: " + transform.position);
        Debug.Log("transform.position: " + transform.position);
        return distance <= interactionRange;
    }
}
