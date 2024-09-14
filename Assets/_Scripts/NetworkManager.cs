using Fusion;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetworkManager : NetworkBehaviour
{
    [Header("Network Setting")]
    [SerializeField] private NetworkRunner runnerPrefab;
    [SerializeField] private TMP_InputField roomText;
    [SerializeField] private int maxPlayerCount;
    [SerializeField] private string gameSceneName = "Game";

    [Header("Canvas")]
    [SerializeField] private Canvas lobbyCanvas;
    [SerializeField] private Canvas roomCanvas;

    [Header("Objects")]
    [SerializeField] private Image myReadyCheckBox;
    [SerializeField] private Image enemyReadyCheckBox;

    private NetworkRunner runner;
    [Networked] public bool isReady { get; private set; }
    [Networked] public bool isEnemyReady { get; private set; }


    public async void JoinRoom()
    {
        await Disconnect();

        runner = Instantiate(runnerPrefab);

        var events = runner.GetComponent<NetworkEvents>();
        events.OnShutdown.AddListener(OnShutdown);

        var sceneInfo = new NetworkSceneInfo();
        sceneInfo.AddSceneRef(SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex));

        StartGameArgs args = new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = roomText.text,
            PlayerCount = maxPlayerCount,
            Scene = sceneInfo,
        };

        var startTask = runner.StartGame(args);
        await startTask;

        if (startTask.Result.Ok)
        {
            lobbyCanvas.enabled = false;
            roomCanvas.enabled = true;
        }
        else
        {
            //StatusText.text = $"Connection Failed: {startTask.Result.ShutdownReason}";
            Debug.Log("Failed Connecting");
        }
    }

    public void ReadyButton()
    {
        if (!myReadyCheckBox.enabled)
        {
            myReadyCheckBox.enabled = true;
            //isReady = true;
            RPC_GetReady();
        }
        else
        {
            myReadyCheckBox.enabled = false;
            //isReady = false;
            RPC_GetNotReady();
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = false)]
    public void RPC_GetReady()
    {
        enemyReadyCheckBox.enabled = true;
        //isEnemyReady = true;

        Debug.Log("GetReadyRPC");
        //GameStart();
    }

    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = false)]
    public void RPC_GetNotReady()
    {
        enemyReadyCheckBox.enabled = false;
        // = false;

        Debug.Log("GetNotReadyRPC");
    }

    public void GameStart()
    {
        if (isReady && isEnemyReady)
        {
            runner.LoadScene(gameSceneName);
        }
    }

    public async void DisconnectClicked()
    {
        await Disconnect();
    }

    public async Task Disconnect()
    {
        if (runner == null)
            return;

        //StatusText.text = "Disconnecting...";
        //PanelGroup.interactable = false;
        var events = runner.GetComponent<NetworkEvents>();
        events.OnShutdown.RemoveListener(OnShutdown);

        await runner.Shutdown();
        runner = null;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnShutdown(NetworkRunner runner, ShutdownReason reason)
    {
        string shutdownStatus = $"Shutdown: {reason}";
        Debug.LogWarning(shutdownStatus);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
