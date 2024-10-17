using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class ResultUIManager : NetworkBehaviour
{
    [SerializeField] private CharacterSelectManager characterSelectManager;

    [SerializeField] private GameObject losePosition;
    [SerializeField] private GameObject winPosition;
    [SerializeField] private GameObject rangePlayer;
    [SerializeField] private GameObject fightPlayer;

    [SerializeField] private RenderTexture[] characterTextures;
    [SerializeField] private Animator[] animators;
    [SerializeField] private RawImage winner;
    [SerializeField] private RawImage loser;

    private GameObject winnerObj;


    private void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1.0f;
    }

    public override void Spawned()
    {
        base.Spawned();

        characterSelectManager = FindObjectOfType<CharacterSelectManager>();

        switch (characterSelectManager.Winner)
        {
            case 0:
                winnerObj = Instantiate(fightPlayer, winPosition.transform.position, winPosition.transform.rotation);
                winnerObj.GetComponent<Animator>().SetTrigger("Dance");
                break;
            case 1:
                winnerObj = Instantiate(rangePlayer, winPosition.transform.position, winPosition.transform.rotation);
                winnerObj.GetComponent<Animator>().SetTrigger("Dance");
                break;
        }
        switch (characterSelectManager.Loser)
        {
            case 0:
                Instantiate(fightPlayer, losePosition.transform.position, losePosition.transform.rotation);
                break;
            case 1:
                Instantiate(rangePlayer, losePosition.transform.position, losePosition.transform.rotation);
                break;
        }
    }

    public void LoadLobbyScene()
    {
        if (Runner.IsSceneAuthority)
        {
            Runner.LoadScene(SceneRef.FromIndex(0));
        }
    }
}
