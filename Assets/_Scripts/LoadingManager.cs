using UnityEngine;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    [SerializeField] private Sprite[] images;
    [SerializeField] private Canvas loadingCanvas;
    [SerializeField] private Camera loadingCamera;
    [SerializeField] private Image screen;
    [SerializeField] private AudioClip bgClip;
    [SerializeField] private AudioClip bgEffectClip;


    private void Start()
    {
        int index = Random.Range(0, images.Length);
        screen.sprite = images[index];
        GetComponent<AudioSource>().PlayOneShot(bgEffectClip);
    }

    public void HideLoadingCanvas()
    {
        loadingCanvas.enabled = false;
        loadingCamera.gameObject.SetActive(false);

        GetComponent<AudioSource>().clip = bgClip;
        GetComponent<AudioSource>().Play();
    }
}
