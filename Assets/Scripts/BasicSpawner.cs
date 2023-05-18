using Cinemachine;
using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private NetworkPrefabRef _playerPrefab;

    public CinemachineVirtualCamera _virtualCamera;

    private NetworkObject LocalPlayer;

    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("ConnectedToServer");
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        Debug.Log("ConnectFailed");
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {

        Debug.Log("ConnectRequest");
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        Debug.Log("CustomAuthenticationResponse");

    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        Debug.Log("DisconnectedFromServer");

    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        Debug.Log("HostMigration");

    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();
        Vector3 direction = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            direction += LocalPlayer.transform.forward;
        }

        //if (Input.GetKey(KeyCode.S))
        //{
        //    direction -= LocalPlayer.transform.forward;
        //}

        if (Input.GetKey(KeyCode.A))
        {
            direction += Quaternion.Euler(0, -5, 0) * LocalPlayer.transform.forward * 0.1f;
        }

        if (Input.GetKey(KeyCode.D))
        {
            direction += Quaternion.Euler(0, 5, 0) * LocalPlayer.transform.forward * 0.1f;
        }

        data.direction = direction.normalized;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            data.speed = 3;
        }
        else
        {
            data.speed = 1;
        }

        data.Jump = Input.GetKey(KeyCode.Space);

        input.Set(data);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        Debug.Log("InputMissing");

    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            Vector3 spawnPosition = new Vector3((player.RawEncoded % runner.Config.Simulation.DefaultPlayers) * 3,3.5f, 0);
            NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);
            _spawnedCharacters.Add(player, networkPlayerObject);
            if (player == runner.LocalPlayer)
            {
                _virtualCamera.Follow = networkPlayerObject.transform.GetChild(1).transform;
                LocalPlayer = networkPlayerObject;
                Debug.Log("Local Player");
            }
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            _spawnedCharacters.Remove(player);
        }
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
        Debug.Log("ReliableDataReceived");

    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        Debug.Log("SceneLoadDone");
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        Debug.Log("SceneLoadStart");

    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        Debug.Log("SessionListUpdated");

    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.Log("Shutdown");

    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        Debug.Log("UserSimulationMessage");

    }

    private NetworkRunner _runner;

    async void StartGame(GameMode mode)
    {
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;

        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "VarRoom",
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }

    private void OnGUI()
    {
        if (_runner == null)
        {
            if (GUI.Button(new Rect(0, 0, 200, 40), "Host"))
            {
                StartGame(GameMode.Host);
            }
            if (GUI.Button(new Rect(0, 40, 200, 40), "Join"))
            {
                StartGame(GameMode.Client);
            }
        }
    }
}
