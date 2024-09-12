using Fusion;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviour
{
    [SerializeField] private NetworkRunner runnerPrefab;
    [SerializeField] private InputField roomText;
    [SerializeField] private int maxPlayerCount;

    private NetworkRunner runner;


    public async void StartGame()
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
            //StatusText.text = "";
            //PanelGroup.gameObject.SetActive(false);
        }
        else
        {
            //StatusText.text = $"Connection Failed: {startTask.Result.ShutdownReason}";
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
