using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ContentLevel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [TextArea(3, 10)] public string songData;
    [SerializeField] private GameObject levelContainer;
    [SerializeField] private Image levelImage;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private GameObject backgroundSongInfo;
    [SerializeField] private TextMeshProUGUI songDataText;
    [SerializeField] private AudioClip song;
    [SerializeField] private Button playButton;
    private Vector3 initialScale;

    void Start()
    {
        StartCoroutine(Init());
    }

    private IEnumerator Init()
    {
        yield return new WaitUntil(() => GameDataManager.Instance != null);
        yield return new WaitUntil(() => songDataText != null && songData != null);
        songDataText.text = songData;
        playButton.onClick.AddListener(OnPlayButtonClicked);
        initialScale = backgroundSongInfo.transform.localScale;
        backgroundSongInfo.transform.localScale = Vector3.zero;

        if (backgroundImage != null)
        {
            Color tempColor = backgroundImage.color;
            tempColor.a = 0f;
            backgroundImage.color = tempColor;
            Debug.Log("Initial backgroundImage alpha set to: " + backgroundImage.color.a);
        }
        else
        {
            Debug.LogError("backgroundImage is null! Make sure to assign it in the Inspector.");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        backgroundSongInfo.SetActive(true);
        if (backgroundImage != null)
        {
            Debug.Log("OnPointerEnter - Starting alpha animation for: " + backgroundImage.gameObject.name);

            Color currentColor = backgroundImage.color;
            LeanTween.value(gameObject, currentColor.a, 0.9f, 0.3f)
                .setEase(LeanTweenType.easeOutQuad)
                .setOnUpdate((float alpha) =>
                {
                    backgroundImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
                });
        }
        LeanTween.scale(backgroundSongInfo, initialScale, 0.3f).setEase(LeanTweenType.easeOutBack);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        LeanTween.scale(backgroundSongInfo, Vector3.zero, 0.3f).setEase(LeanTweenType.easeInBack).setOnComplete(() => backgroundSongInfo.SetActive(false));
        if (backgroundImage != null)
        {
            Debug.Log("OnPointerExit - Starting alpha animation for: " + backgroundImage.gameObject.name);

            Color currentColor = backgroundImage.color;
            LeanTween.value(gameObject, currentColor.a, 0f, 0.3f)
                .setEase(LeanTweenType.easeOutQuad)
                .setOnUpdate((float alpha) =>
                {
                    backgroundImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
                });
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("ContentLevel Clicked!");
    }

    private void OnPlayButtonClicked()
    {
        PlaySong();

    }

    private void PlaySong()
    {
        if (GameDataManager.Instance != null)
        {
            if (GameDataManager.Instance.settings != null)
            {
                GameDataManager.Instance.settings.bgm = song;
                GameDataManager.Instance.settings.bgImage = levelImage.sprite;
                SceneManager.LoadScene("GamePlay");
            }
            else
            {
                Debug.LogError("GameDataManager Settings is null!");
            }
        }
        else
        {
            Debug.LogError("GameDataManager Instance is null!");
        }
    }
}