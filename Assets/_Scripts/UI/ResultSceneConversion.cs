using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultSceneConversion : NetworkBehaviour
{
    [SerializeField] private SceneData sceneData;

    public void ResultSceneBringIn(int winner)
    {
        if (HasStateAuthority)
        {
            sceneData.winPlayer = winner;

            Runner.LoadScene("ResultScene");
        }

    }
}
