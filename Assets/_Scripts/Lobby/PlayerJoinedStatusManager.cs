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
    public event Action<SceneRef, Transform> onAllPlayersLoadedScene;
    private RoomManager roomManager;
    private CharacterSelection characterSelection;
    // for test
    [SerializeField] private NetworkObject characterPrefab;


    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        roomManager = FindAnyObjectByType<RoomManager>();
        characterSelection = FindAnyObjectByType<CharacterSelection>();

        onAllPlayersLoadedScene += InstantiatePlayer;
    }

    public void SceneLoadDone(in SceneLoadDoneArgs sceneInfo)
    {
        PlayerStatus playerStatus = new PlayerStatus { scene = sceneInfo.SceneRef };
        if (_playerStatusDictionary.TryAdd(Runner.LocalPlayer, playerStatus) == false)
            _playerStatusDictionary[Runner.LocalPlayer] = playerStatus;

        Rpc_SendPlayerStatus(Runner, Runner.LocalPlayer, _playerStatusDictionary[Runner.LocalPlayer]);
    }

    public void PlayerJoined(PlayerRef player)
    {
        PlayerStatus playerStatus = new PlayerStatus { scene = default };

        if (player == Runner.LocalPlayer)
        {
            foreach (var activePlayer in Runner.ActivePlayers)
            {
                if (_playerStatusDictionary.TryAdd(activePlayer, playerStatus) == false)
                    _playerStatusDictionary[activePlayer] = playerStatus;
            }
        }
        else
        {
            // todo -> Guess why roomManager set null
            roomManager?.WhenPlayerJoined();
        }

        if (_playerStatusDictionary.TryAdd(player, playerStatus) == false)
            _playerStatusDictionary[player] = playerStatus;
    }

    public void PlayerLeft(PlayerRef player)
    {
        _playerStatusDictionary.Remove(player);

        if (player == Runner.LocalPlayer)
        {
            Rpc_SendPlayerLeft(Runner, player);
            roomManager?.RPCEnemyPlayerLeft();
        }
        else
        {
            roomManager?.WhenPlayerLeft();
        }
    }

    [Rpc]
    public static void Rpc_SendPlayerStatus(NetworkRunner runner, PlayerRef player, PlayerStatus playerStatus)
    {
        var instance = runner.GetComponent<PlayerJoinedStatusManager>();
        if (instance._playerStatusDictionary.TryAdd(player, playerStatus) == false)
            instance._playerStatusDictionary[player] = playerStatus;

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
        roomManager = FindAnyObjectByType<RoomManager>();
        characterSelection = FindAnyObjectByType<CharacterSelection>();

        SceneRef sceneRef = _playerStatusDictionary.Values.First().scene;

        foreach (var playerStatus in _playerStatusDictionary.Values)
        {
            if (playerStatus.scene != sceneRef)
                return;
        }

        if (Runner.SceneManager.MainRunnerScene.buildIndex == 2)
        {
            Transform spawnPosition;
            if (Runner.IsSharedModeMasterClient)
            {
                spawnPosition = FindObjectOfType<GameManager>().SpawnPosition1;
            }
            else
            {
                spawnPosition = FindObjectOfType<GameManager>().SpawnPosition2;
            }
            onAllPlayersLoadedScene(sceneRef, spawnPosition);
        }
    }

    public void SetPlayerCharacter(NetworkObject player)
    {
        characterPrefab = player;
    }

    private void InstantiatePlayer(SceneRef sceneRef, Transform spawnPosition)
    {
        if (sceneRef.AsIndex == 2)
        {
            NetworkObject player = Runner.Spawn(characterPrefab, spawnPosition.position, spawnPosition.localRotation, Runner.LocalPlayer);
            Runner.SetPlayerObject(Runner.LocalPlayer, player);

            FindObjectOfType<LoadingManager>().HideLoadingCanvas();
        }
    }
}
