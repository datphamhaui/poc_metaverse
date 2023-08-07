using Fusion;
using UnityEngine;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined, ISimulationEnter, ISimulationExit
{
    public GameObject PlayerPrefab;

    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            Runner.Spawn(PlayerPrefab, new Vector3(-0.20826910f, 5.5695610f, 0.74999640f), Quaternion.identity, player);
        }
    }

    public void SimulationEnter()
    {
        Debug.Log("SimulationEnter");
        //girl.SetActive(true);
    }

    public void SimulationExit()
    {
        Debug.Log("SimulationExit");
        //girl.SetActive(false);
    }
}