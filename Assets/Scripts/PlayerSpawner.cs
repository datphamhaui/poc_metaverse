using Fusion;
using UnityEngine;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public GameObject PlayerPrefab;

    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            // Garden
            Runner.Spawn(PlayerPrefab, new Vector3(-0.12376430f, 3.476770f, 5.5306730f), Quaternion.identity, player);
            // Space
            //Runner.Spawn(PlayerPrefab, new Vector3(0.023993490f, 5.5725440f, 3.6215210f), Quaternion.identity, player);
        }
    }
}