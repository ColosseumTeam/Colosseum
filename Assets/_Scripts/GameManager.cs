using System;
using System.Collections;
using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    // for test. 나중에 캐릭터 선택창에서 정한 캐릭터 정보를 받아와 적용시켜줘야 함.
    [SerializeField] private NetworkObject characterPrefab;


    private void Start()
    {
        //StartCoroutine(InstantiatePlayer());
        //NetworkObject player = Instantiate(characterPrefab);
        //StartCoroutine(c_JoinRoom());
        //JoinRoom();
    }

    private IEnumerator InstantiatePlayer()
    {
        yield return new WaitUntil(() => Runner);

        NetworkObject player = Runner.Spawn(characterPrefab);
    }

    private IEnumerator c_JoinRoom()
    {
        yield return new WaitUntil(() => Runner);

        Debug.Log("StartJoinRoom");
        JoinRoom();
    }

    private async void JoinRoom()
    {
        var sceneInfo = new NetworkSceneInfo();
        sceneInfo.AddSceneRef(SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex));

        StartGameArgs args = new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = "asd",
            PlayerCount = 2,
            Scene = sceneInfo,
        };

        var startTask = Runner.StartGame(args);
        await startTask;

        if (startTask.Result.Ok)
        {
            Runner.Spawn(characterPrefab);
            Debug.Log("Connected");
        }
        else
        {
            //StatusText.text = $"Connection Failed: {startTask.Result.ShutdownReason}";
            Debug.Log("Failed Connecting");
        }
    }

    private void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            if (Runner)
            {
                Debug.Log("Has Runner");
                Debug.Log(Runner.AuthenticationValues);
            }
            else
            {
                Debug.Log("Doesn't have Runner");
            }
        }
    }
}
