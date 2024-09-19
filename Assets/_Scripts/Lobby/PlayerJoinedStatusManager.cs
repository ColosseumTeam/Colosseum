using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerJoinedStatusManager : SimulationBehaviour, ISceneLoadDone, IPlayerJoined, IPlayerLeft
{
    public struct PlayerStatus : INetworkStruct
    {
        public SceneRef scene;
    }

    private Dictionary<PlayerRef, PlayerStatus> _playerStatusDictionary = new Dictionary<PlayerRef, PlayerStatus>();
    public event Action<SceneRef> onAllPlayersLoadedScene;


    private void Awake()
    {
        //if (FindAnyObjectByType<PlayerJoinedStatusManager>())
        //{
        //    Destroy(gameObject);
        //    return;
        //}
        DontDestroyOnLoad(this);
    }

    public void SceneLoadDone(in SceneLoadDoneArgs sceneInfo)
    {
        Debug.Log("SceneLoadDone");
        PlayerStatus playerStatus = new PlayerStatus { scene = sceneInfo.SceneRef };
        if (_playerStatusDictionary.TryAdd(Runner.LocalPlayer, playerStatus) == false)
            _playerStatusDictionary[Runner.LocalPlayer] = playerStatus;

        Rpc_SendPlayerStatus(Runner, Runner.LocalPlayer, _playerStatusDictionary[Runner.LocalPlayer]);
    }

    public void PlayerJoined(PlayerRef player)
    {
        Debug.Log("PlayerJoined");
        PlayerStatus playerStatus = new PlayerStatus { scene = default };

        if (player == Runner.LocalPlayer)
        {
            foreach (var activePlayer in Runner.ActivePlayers)
            {
                if (_playerStatusDictionary.TryAdd(activePlayer, playerStatus) == false)
                    _playerStatusDictionary[activePlayer] = playerStatus;
            }
        }

        if (_playerStatusDictionary.TryAdd(player, playerStatus) == false)
            _playerStatusDictionary[player] = playerStatus;
    }

    public void PlayerLeft(PlayerRef player)
    {
        Debug.Log("PlayerLeft");
        _playerStatusDictionary.Remove(player);
        Rpc_SendPlayerLeft(Runner, player);
    }

    [Rpc]
    public static void Rpc_SendPlayerStatus(NetworkRunner runner, PlayerRef player, PlayerStatus playerStatus)
    {
        var instance = runner.GetComponent<PlayerJoinedStatusManager>();
        if (instance._playerStatusDictionary.TryAdd(player, playerStatus) == false)
            instance._playerStatusDictionary[player] = playerStatus;
        Debug.Log("Rpc_SendPlayerStatus");

        instance.CheckAllPlayerLoadedScene();
    }

    [Rpc]
    public static void Rpc_SendPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        var instance = runner.GetComponent<PlayerJoinedStatusManager>();
        instance._playerStatusDictionary?.Remove(player);
    }

    void CheckAllPlayerLoadedScene()
    {
        SceneRef sceneRef = _playerStatusDictionary.Values.First().scene;

        foreach (var playerStatus in _playerStatusDictionary.Values)
        {
            if (playerStatus.scene != sceneRef)
                return;
        }

        Debug.Log("onAllPlayersLoadedScene");
        if (onAllPlayersLoadedScene != null)
        {
            onAllPlayersLoadedScene(sceneRef);
        }
    }
}
