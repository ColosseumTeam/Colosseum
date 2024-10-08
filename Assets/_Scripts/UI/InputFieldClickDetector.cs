using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputFieldClickDetector : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject soundCanvas;

    private InputField inputField; 

    void Start()
    {
        inputField = GetComponent<InputField>(); 
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        soundCanvas.GetComponent<KebordSound>().KeyBordSoundCheck(true);
    }
}
