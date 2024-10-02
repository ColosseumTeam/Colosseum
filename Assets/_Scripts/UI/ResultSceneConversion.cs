using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultSceneConversion : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKey(KeyCode.K))
        {
            SceneManager.LoadScene("ResultScene");
        }
    }

    public void ResultSceneBringIn()
    {
        SceneManager.LoadScene("ResultScene");
    }
}
