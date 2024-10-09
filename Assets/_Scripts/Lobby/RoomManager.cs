using Fusion;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class RoomManager : NetworkBehaviour
{
    [Header("Room Setting")]
    [SerializeField] private string gameSceneName = "Game";

    [Header("Reference")]
    [SerializeField] private Image myReadyCheckBox;
    [SerializeField] private Image enemyReadyCheckBox;
    [SerializeField] private RawImage myCharacterRawImage;
    [SerializeField] private RawImage enemyCharacterRawImage;
    [SerializeField] private CharacterSelection characterSelection;

    public bool isReady { get; private set; }
    public bool isEnemyReady { get; private set; }
    public RawImage MyCharacterRawImage { get { return myCharacterRawImage; } }
    public RawImage EnemyCharacterRawImage { get { return enemyCharacterRawImage; } }


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

    public override void Spawned()
    {
        base.Spawned();

        // 플레이어가 합류할 때 수행할 작업
        WhenPlayerJoined();
    }


    public void WhenPlayerJoined()
    {
        if (!HasStateAuthority) return;

        if (isReady)
        {
            RPC_GetReady();
        }
        enemyCharacterRawImage.enabled = true;
        RPC_CharacterSelected(characterSelection.ClickedIndex);
        enemyCharacterRawImage.texture = characterSelection.CharacterDatas[0].CharacterRenderTexture;
    }

    public void WhenPlayerLeft()
    {
        enemyReadyCheckBox.enabled = false;
        isEnemyReady = false;
        enemyCharacterRawImage.enabled = false;
    }

    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = false)]
    public void RPCEnemyPlayerLeft()
    {
        enemyCharacterRawImage.enabled = false;
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

    public void CharacterSelected(int index)
    {
        myCharacterRawImage.texture = characterSelection.CharacterDatas[index].CharacterRenderTexture;
        RPC_CharacterSelected(index);
    }

    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = false)]
    private void RPC_CharacterSelected(int index)
    {
        if (!enemyCharacterRawImage.enabled)
        {
            enemyCharacterRawImage.enabled = true;
        }

        enemyCharacterRawImage.texture = characterSelection.CharacterDatas[index].CharacterRenderTexture;
    }
}
