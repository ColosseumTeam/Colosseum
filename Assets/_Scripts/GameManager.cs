using Fusion;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    // for test. 나중에 캐릭터 선택창에서 정한 캐릭터 정보를 받아와 적용시켜줘야 함.
    [SerializeField] private NetworkObject characterPrefab;
    [SerializeField] private AimController aimController;

    public AimController AimController { get { return aimController; } }

    private PlayerJoinedStatusManager playerJoinedStatusManager;


    private void Awake()
    {
        playerJoinedStatusManager = FindAnyObjectByType<PlayerJoinedStatusManager>();
    }

    private void Start()
    {
        //playerJoinedStatusManager.onAllPlayersLoadedScene += InstantiatePlayer;
    }

    private void InstantiatePlayer(SceneRef sceneRef)
    {
        if (sceneRef.AsIndex == 2)
        {
            Debug.Log("Spawn player");
            NetworkObject player = Runner.Spawn(characterPrefab, Vector3.zero, Quaternion.identity, Runner.LocalPlayer);
            Runner.SetPlayerObject(Runner.LocalPlayer, player);
        }
    }
}
