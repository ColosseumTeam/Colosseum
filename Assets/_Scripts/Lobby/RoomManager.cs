using Fusion;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class RoomManager : NetworkBehaviour, ISceneLoadDone
{
    public enum PreviousScene
    {
        Lobby,
        Result,
    }

    [Header("Room Setting")]
    [SerializeField] private string gameSceneName = "Game";

    [Header("Reference")]
    [SerializeField] private Image myReadyCheckBox;
    [SerializeField] private Image enemyReadyCheckBox;
    [SerializeField] private RawImage myCharacterRawImage;
    [SerializeField] private RawImage enemyCharacterRawImage;
    [SerializeField] private CharacterSelection characterSelection;
    [SerializeField] private Canvas startCanvas;
    [SerializeField] private Canvas roomCanvas;

    private CharacterSelectManager characterSelectManager;

    public bool isReady { get; private set; }
    public bool isEnemyReady { get; private set; }
    public RawImage MyCharacterRawImage { get { return myCharacterRawImage; } }
    public RawImage EnemyCharacterRawImage { get { return enemyCharacterRawImage; } }

    

    private void Awake()
    {
        characterSelectManager = FindObjectOfType<CharacterSelectManager>();
    }

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
        //WhenPlayerJoined();
    }


    public void WhenPlayerJoined()
    {
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

    [Rpc]
    public void RPCEnemyPlayerLeft()
    {
        enemyCharacterRawImage.enabled = false;
    }

    [Rpc(InvokeLocal = false)]
    private void RPC_GetReady()
    {
        enemyReadyCheckBox.enabled = true;
        isEnemyReady = true;

        RPC_GameStart();
    }

    [Rpc(InvokeLocal = false)]
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
        characterSelectManager.ChangeMyCharacter(index);
        RPC_CharacterSelected(index);
    }

    [Rpc(InvokeLocal = false)]
    private void RPC_CharacterSelected(int index)
    {
        if (!enemyCharacterRawImage.enabled)
        {
            enemyCharacterRawImage.enabled = true;
        }

        enemyCharacterRawImage.texture = characterSelection.CharacterDatas[index].CharacterRenderTexture;
        characterSelectManager.ChangeEnemyCharacter(index);
    }

    [Rpc]
    private void RPC_LoadLobbySceneFromResult()
    {
        startCanvas.enabled = false;
        roomCanvas.enabled = true;
        // Todo: 0 나중에 캐릭터에 맞게 바꿔줘야함.
        CharacterSelected(characterSelectManager.MyCharacterNumber);
    }

    public void SceneLoadDone(in SceneLoadDoneArgs sceneInfo)
    {
        RPC_LoadLobbySceneFromResult();
    }
}
