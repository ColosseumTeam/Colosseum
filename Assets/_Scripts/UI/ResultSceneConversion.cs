using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultSceneConversion : NetworkBehaviour
{
    [SerializeField] private SceneData sceneData;

    private void Update()
    {
        if (Input.GetKey(KeyCode.K))
        {
            SceneManager.LoadScene("ResultScene");
        }
    }

    public void ResultSceneBringIn(int winner)
    {
        if (!HasStateAuthority)
        {
            sceneData.winPlayer = winner;

            SceneManager.LoadScene("ResultScene");
        }

    }
}
