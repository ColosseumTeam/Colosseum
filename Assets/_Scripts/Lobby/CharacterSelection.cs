using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelection : MonoBehaviour
{
    [Header("# Reference")]
    [SerializeField] private Transform characterModelGroup;
    [SerializeField] private Transform iconGroup;
    [SerializeField] private GameObject iconPrefab;
    [SerializeField] private List<CharacterInfo> characterDatas;
    [SerializeField] private RoomManager roomManager;
    [SerializeField] private AudioClip characterSelectSound;
    [SerializeField] private CharacterSelectManager characterSelectManager;

    [Header("# UI")]
    [SerializeField] private RawImage myCharacterImage;
    [SerializeField] private RawImage enemyCharacterImage;
    [SerializeField] private RawImage selectedCharacterImage;
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private TextMeshProUGUI characterConcept;
    [SerializeField] private TextMeshProUGUI characterStory;
    [SerializeField] private Text characterNameText;
    [SerializeField] private Text characterConceptText;
    [SerializeField] private Text characterStoryText;

    private int clickedIndex;
    private AudioSource audioSource;

    public int ClickedIndex { get { return clickedIndex; } }
    public List<CharacterInfo> CharacterDatas { get { return characterDatas; } }


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        characterSelectManager = FindObjectOfType<CharacterSelectManager>();

        for (int i = 0; i < characterDatas.Count; i++)
        {
            int temp = i;
            Button icon = Instantiate(iconPrefab, iconGroup).GetComponent<Button>();
            icon.onClick.AddListener(() => ClickedIcon(temp));
            icon.onClick.AddListener(() => CharacterSelectSound());
        }
    }

    private void Start()
    {
        // Init first character
        selectedCharacterImage.texture = characterDatas[characterSelectManager.MyCharacterNumber].CharacterRenderTexture;
        //characterName.text = characterDatas[characterSelectManager.MyCharacterNumber].CharacterName;
        //characterConcept.text = characterDatas[characterSelectManager.MyCharacterNumber].CharacterConcept;
        //characterStory.text = characterDatas[characterSelectManager.MyCharacterNumber].CharacterStory;
        characterNameText.text = characterDatas[characterSelectManager.MyCharacterNumber].CharacterName;
        characterConceptText.text = characterDatas[characterSelectManager.MyCharacterNumber].CharacterConcept;
        characterStoryText.text = characterDatas[characterSelectManager.MyCharacterNumber].CharacterStory;
    }

    public void ClickedIcon(int index)
    {
        GameObject clickedCharacter = characterModelGroup.GetChild(index).gameObject;
        if (clickedIndex != index)
        {
            clickedIndex = index;
            selectedCharacterImage.texture = characterDatas[clickedIndex].CharacterRenderTexture;

            //characterName.text = characterDatas[index].CharacterName;
            //characterConcept.text = characterDatas[index].CharacterConcept;
            //characterStory.text = characterDatas[index].CharacterStory;
            characterNameText.text = characterDatas[index].CharacterName;
            characterConceptText.text = characterDatas[index].CharacterConcept;
            characterStoryText.text = characterDatas[index].CharacterStory;
        }
    }

    public void CharacterSelectSound()
    {
        audioSource.PlayOneShot(characterSelectSound);
    }

    public void SelectButton()
    {
        roomManager.Runner.GetComponent<PlayerJoinedStatusManager>().SetPlayerCharacter(characterDatas[clickedIndex].CharacterPrefab);
        roomManager.CharacterSelected(clickedIndex);
    }
}
