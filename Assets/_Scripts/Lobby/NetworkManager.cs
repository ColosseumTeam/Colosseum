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
    [SerializeField] private Button joinButton;

    [Header("Canvas")]
    [SerializeField] private Canvas lobbyCanvas;
    [SerializeField] private Canvas roomCanvas;

    private NetworkRunner runner;


    public async void JoinRoom()
    {
        joinButton.interactable = false;

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
            Debug.Log("Failed Connecting");
        }

        joinButton.interactable = true;
    }

    public async void DisconnectClicked()
    {
        await Disconnect();
    }

    public async Task Disconnect()
    {
        if (Runner == null)
            return;

        var events = Runner.GetComponent<NetworkEvents>();
        events.OnShutdown.RemoveListener(OnShutdown);

        await Runner.Shutdown();
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
