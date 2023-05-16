using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NetworkRunner))]
public sealed class GameManager : SimulationBehaviour, IPlayerJoined, IPlayerLeft
{
	// PRIVATE MEMBERS

	[SerializeField]
	private NetworkObject _playerPrefab;
	[SerializeField]
	private Vector3 _playerOffset = new(2f, 0f, 0f);

	private Dictionary<PlayerRef, NetworkObject> _players = new(32);

	// IPlayerJoined INTERFACE

	void IPlayerJoined.PlayerJoined(PlayerRef playerRef)
	{
		if (Runner.IsServer == false)
			return;

		var position = _players.Count * _playerOffset;

		var player = Runner.Spawn(_playerPrefab, position, Quaternion.identity, inputAuthority: playerRef);

		_players.Add(playerRef, player);

		Runner.SetPlayerObject(playerRef, player);
	}

	// IPlayerLeft INTERFACE

	void IPlayerLeft.PlayerLeft(PlayerRef playerRef)
	{
		if (Runner.IsServer == false)
			return;

		if (_players.TryGetValue(playerRef, out NetworkObject player) == false)
			return;

		Runner.Despawn(player);
		_players.Remove(playerRef);
	}
}
