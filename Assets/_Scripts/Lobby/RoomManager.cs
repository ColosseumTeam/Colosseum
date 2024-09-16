using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoomManager : NetworkBehaviour
{

    [Header("Room Setting")]
    [SerializeField] private string gameSceneName = "Game";
    [SerializeField] private Image myReadyCheckBox;
    [SerializeField] private Image enemyReadyCheckBox;

    [Networked] public bool isReady { get; private set; }
    [Networked] public bool isEnemyReady { get; private set; }


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

    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = false)]
    private void RPC_GetReady()
    {
        enemyReadyCheckBox.enabled = true;
        isEnemyReady = true;

        Debug.Log("GetReadyRPC");
        GameStart();
    }

    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = false)]
    private void RPC_GetNotReady()
    {
        enemyReadyCheckBox.enabled = false;
        isEnemyReady = false;

        Debug.Log("GetNotReadyRPC");
    }

    private void GameStart()
    {
        if (isReady && isEnemyReady)
        {
            Runner.LoadScene(gameSceneName);
        }
    }
}
