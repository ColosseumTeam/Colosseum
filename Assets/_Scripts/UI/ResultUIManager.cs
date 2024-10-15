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

    private void Awake()
    {
        characterSelectManager = FindObjectOfType<CharacterSelectManager>();

        if (characterSelectManager.Win)
        {
            switch (characterSelectManager.MyCharacterNumber)
            {
                case 0:
                    Instantiate(fightPlayer, winPosition.transform.position, winPosition.transform.rotation);
                    break;
                case 1:
                    Instantiate(rangePlayer, winPosition.transform.position, winPosition.transform.rotation);
                    break;
            }
            switch (characterSelectManager.EnemyCharacterNumber)
            {
                case 0:
                    Instantiate(fightPlayer, losePosition.transform.position, losePosition.transform.rotation);
                    break;
                case 1:
                    Instantiate(rangePlayer, losePosition.transform.position, losePosition.transform.rotation);
                    break;
            }
        }
        else
        {
            switch (characterSelectManager.MyCharacterNumber)
            {
                case 0:
                    Instantiate(fightPlayer, losePosition.transform.position, losePosition.transform.rotation);
                    break;
                case 1:
                    Instantiate(rangePlayer, losePosition.transform.position, losePosition.transform.rotation);
                    break;
            }
            switch (characterSelectManager.EnemyCharacterNumber)
            {
                case 0:
                    Instantiate(fightPlayer, winPosition.transform.position, winPosition.transform.rotation);
                    break;
                case 1:
                    Instantiate(rangePlayer, winPosition.transform.position, winPosition.transform.rotation);
                    break;
            }
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void LoadLobbyScene()
    {
        if (Runner.IsSceneAuthority)
        {
            Runner.LoadScene(SceneRef.FromIndex(0));
        }
    }
}
