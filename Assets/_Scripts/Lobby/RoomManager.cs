using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class RoomManager : NetworkBehaviour
{

    [Header("Room Setting")]
    [SerializeField] private string gameSceneName = "Game";
    [SerializeField] private Image myReadyCheckBox;
    [SerializeField] private Image enemyReadyCheckBox;

    public bool isReady { get; private set; }
    public bool isEnemyReady { get; private set; }


    public void ReadyButton()
    {
        if (!myReadyCheckBox.enabled)
        {
            myReadyCheckBox.enabled = true;
            isReady = true;
            RPC_GetReady();
        }
        else
        {
            myReadyCheckBox.enabled = false;
            isReady = false;
            RPC_GetNotReady();
        }
    }

    public void RPCWhenPlayerJoined()
    {
        if (isReady)
        {
            RPC_GetReady();
        }
    }

    public void RPCWhenPlayerLeft()
    {
        enemyReadyCheckBox.enabled = false;
        isEnemyReady = false;
    }

    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = false)]
    private void RPC_GetReady()
    {
        enemyReadyCheckBox.enabled = true;
        isEnemyReady = true;

        RPC_GameStart();
    }

    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = false)]
    private void RPC_GetNotReady()
    {
        enemyReadyCheckBox.enabled = false;
        isEnemyReady = false;
    }

    [Rpc]
    private void RPC_GameStart()
    {
        if (isReady && isEnemyReady && Runner.IsSceneAuthority)
        {
            Runner.LoadScene(SceneRef.FromIndex(2));
        }
    }
}
