using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIButtonEffects : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Button button;
    public Color normalColor = Color.white;
    public Color hoverColor = Color.yellow;
    public Color clickColor = Color.red;
    public AudioClip hoverSound;
    public AudioClip clickSound;

    private AudioSource audioSource;
    private Image buttonImage;

    void Start()
    {
        buttonImage = button.GetComponent<Image>();
        buttonImage.color = normalColor;

        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonImage.color = hoverColor;
        if (hoverSound != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonImage.color = normalColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        buttonImage.color = clickColor;
        if (clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
        // Дополнительные действия при нажатии кнопки
    }
}
