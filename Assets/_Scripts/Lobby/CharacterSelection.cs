using UnityEngine;
using UnityEngine.UI;

public class CharacterSelection : MonoBehaviour
{
    [SerializeField] private Transform characterModelGroup;
    [SerializeField] private Transform iconGroup;
    [SerializeField] private GameObject iconPrefab;

    private GameObject clickedModel;


    private void Awake()
    {
        for (int i = 0; i < characterModelGroup.childCount; i++)
        {
            int temp = i;
            Button icon = Instantiate(iconPrefab, iconGroup).GetComponent<Button>();
            icon.onClick.AddListener(() => ClickedIcon(temp));
        }
    }

    private void OnEnable()
    {
        // for test
        if (!clickedModel)
        {
            clickedModel = characterModelGroup.GetChild(0).gameObject;
            clickedModel.SetActive(true);
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
        }
    }
}
