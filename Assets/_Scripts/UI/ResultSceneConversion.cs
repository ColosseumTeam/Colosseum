using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultSceneConversion : NetworkBehaviour
{
    [SerializeField] private SceneData sceneData;

    public void ResultSceneBringIn()
    {
        if (Runner.IsSceneAuthority)
        {
            Runner.LoadScene("ResultScene");
        }

    }
}
