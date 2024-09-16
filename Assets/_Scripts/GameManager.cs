using System;
using System.Collections;
using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : NetworkBehaviour
{
    // for test. 나중에 캐릭터 선택창에서 정한 캐릭터 정보를 받아와 적용시켜줘야 함.
    [SerializeField] private NetworkObject characterPrefab;


    private void Start()
    {
        StartCoroutine(InstantiatePlayer());
        //NetworkObject player = Instantiate(characterPrefab);
    }

    private IEnumerator InstantiatePlayer()
    {
        yield return new WaitUntil(() => Runner);

        //NetworkObject player = Instantiate(characterPrefab);
        Runner.InstantiateInRunnerScene(characterPrefab);
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
