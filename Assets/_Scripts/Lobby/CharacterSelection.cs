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
    [SerializeField] private List<CharacterData> characterDatas;

    [Header("# Text UI")]
    [SerializeField] private TextMeshProUGUI characterDescription;
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private TextMeshProUGUI characterConcept;
    [SerializeField] private TextMeshProUGUI characterStory;

    private GameObject clickedModel;


    private void Awake()
    {
        for (int i = 0; i < characterDatas.Count; i++)
        {
            int temp = i;
            Button icon = Instantiate(iconPrefab, iconGroup).GetComponent<Button>();
            icon.onClick.AddListener(() => ClickedIcon(temp));
        }
    }

    private void OnEnable()
    {
        // Init first character
        if (!clickedModel)
        {
            clickedModel = characterModelGroup.GetChild(0).gameObject;
            clickedModel.SetActive(true);
            characterDescription.text = characterDatas[0].CharacterDesciption;
            characterName.text = characterDatas[0].CharacterName;
            characterConcept.text = characterDatas[0].CharacterConcept;
            characterStory.text = characterDatas[0].CharacterStory;
        }
    }

    public void ClickedIcon(int index)
    {
        GameObject clickedCharacter = characterModelGroup.GetChild(index).gameObject;
        if (clickedModel != clickedCharacter)
        {
            clickedModel.SetActive(false);
            clickedModel = clickedCharacter;
            clickedModel.SetActive(true);

            characterDescription.text = characterDatas[index].CharacterDesciption;
            characterName.text = characterDatas[index].CharacterName;
            characterConcept.text = characterDatas[index].CharacterConcept;
            characterStory.text = characterDatas[index].CharacterStory;
        }
    }
}
