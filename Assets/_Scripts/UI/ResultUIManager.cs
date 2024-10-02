using UnityEngine;
using UnityEngine.UI;

public class ResultUIManager : MonoBehaviour
{
    [SerializeField] private SceneData sceneData;

    [SerializeField] private GameObject losePosition;
    [SerializeField] private GameObject winPosition;
    [SerializeField] private GameObject rangePlayer;
    [SerializeField] private GameObject fightPlayer;

    [SerializeField] private RenderTexture[] characterTextures;
    [SerializeField] private Animator[] animators;
    [SerializeField] private RawImage winner;
    [SerializeField] private RawImage loser;

    private void Awake()
    {
        GameObject winObj;
        GameObject loseObj;

        if(sceneData.winPlayer == 0)
        {
            loseObj = Instantiate(rangePlayer, losePosition.transform.position, losePosition.transform.rotation);
            winObj = Instantiate(fightPlayer, winPosition.transform.position, winPosition.transform.rotation);

            loseObj.GetComponent<Animator>().SetTrigger("Idle");
            winObj.GetComponent<Animator>().SetTrigger("Dance");
        }
        else
        {
            loseObj = Instantiate(fightPlayer, losePosition.transform.position, losePosition.transform.rotation);
            winObj = Instantiate(rangePlayer, winPosition.transform.position, winPosition.transform.rotation);            

            loseObj.GetComponent<Animator>().SetTrigger("Idle");
            winObj.GetComponent<Animator>().SetTrigger("Dance");
        }
    }
}
