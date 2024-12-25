using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingController : MonoBehaviour
{
    public GameObject settingPanel;
    public GameObject menuPanel;

    public Slider volumeMasterSlider;
    public Slider volumeVFXSlider;
    public Button keyBindingButton; // Đổi tên để rõ ràng hơn
    public TextMeshProUGUI keyBindingText;
    [SerializeField] private bool isBindingValue = false;

    private void Start()
    {
        LoadSettings();
        UpdateUI();
    }

    private void LoadSettings()
    {
        if (GameDataManager.Instance != null)
        {
            volumeMasterSlider.value = GameDataManager.Instance.settings.volumeMaster;
            volumeVFXSlider.value = GameDataManager.Instance.settings.volumeVFX;
            UpdateKeyBindingText();
        }
    }

    private void UpdateUI()
    {
        OnChangeVolumeMasterSlider();
        OnChangeVolumeVFXSlider();
    }

    public void OnChangeVolumeMasterSlider()
    {
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.settings.volumeMaster = volumeMasterSlider.value;
            AudioListener.volume = GameDataManager.Instance.settings.volumeMaster;
        }
    }

    public void OnChangeVolumeVFXSlider()
    {
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.settings.volumeVFX = volumeVFXSlider.value;
        }
    }

    public void OnClickKeyBinding()
    {
        isBindingValue = !isBindingValue;
        if (isBindingValue)
        {
            keyBindingText.text = "Press any key to bind...";
            StartCoroutine(WaitForInput());
        }
        else
        {
            UpdateKeyBindingText();
        }
    }

    private IEnumerator WaitForInput()
    {
        while (isBindingValue)
        {
            if (Input.anyKeyDown)
            {
                foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(keyCode) && keyCode != KeyCode.Escape) // Loại trừ phím Escape nếu muốn
                    {
                        if (GameDataManager.Instance != null)
                        {
                            GameDataManager.Instance.settings.keyBinding = keyCode;
                            UpdateKeyBindingText();
                        }
                        isBindingValue = false;
                        yield break;
                    }
                }
            }
            yield return null;
        }
    }

    private void UpdateKeyBindingText()
    {
        if (GameDataManager.Instance != null)
        {
            string keyName;
            switch (GameDataManager.Instance.settings.keyBinding)
            {
                case KeyCode.Mouse0:
                    keyName = "Left Mouse Button";
                    break;
                case KeyCode.Mouse1:
                    keyName = "Right Mouse Button";
                    break;
                case KeyCode.Mouse2:
                    keyName = "Middle Mouse Button";
                    break;
                default:
                    keyName = GameDataManager.Instance.settings.keyBinding.ToString();
                    break;
            }
            keyBindingText.text = "Key Binding: " + keyName;
        }
    }

    public void ChangeToMenuPanel()
    {
        Debug.Log("Change to Menu Panel");
        if (menuPanel != null && settingPanel != null)
        {
            // Dùng LeanTween để ẩn MenuPanel
            LeanTween.scale(settingPanel, Vector3.zero, 0.5f)
                .setEase(LeanTweenType.easeInBack)
                .setOnComplete(() =>
                {
                    settingPanel.SetActive(false);
                    // Hiện SettingPanel
                    menuPanel.SetActive(true);
                    // Dùng LeanTween để hiện SettingPanel
                    LeanTween.scale(menuPanel, Vector3.one, 0.5f)
                        .setEase(LeanTweenType.easeOutBack);
                });
        }
    }
}