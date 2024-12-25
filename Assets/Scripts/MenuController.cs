using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public enum MenuFunction
    {
        GamePlay,
        Setting,
        Quit
    }

    public GameObject menuPanel;        // Panel của Main Menu
    public GameObject settingPanel;     // Panel của Setting    
    public string gamePlaySceneName;    // Tên scene của màn chơi

    public void HandleMenuClick(MenuComponent clickedComponent)
    {
        switch (clickedComponent.menuFunction)
        {
            case MenuFunction.GamePlay:
                PlayGame();
                break;
            case MenuFunction.Setting:
                ChangeToSettingPanel();
                break;
            case MenuFunction.Quit:
                Debug.Log("Quit Game!");
                Application.Quit();
                break;
            default:
                break;
        }
    }

    public void ChangeToSettingPanel()
    {
        Debug.Log("Change to Setting Panel");
        if (menuPanel != null && settingPanel != null)
        {
            // Dùng LeanTween để ẩn MenuPanel
            LeanTween.scale(menuPanel, Vector3.zero, 0.5f)
                .setEase(LeanTweenType.easeInBack)
                .setOnComplete(() =>
                {
                    menuPanel.SetActive(false);
                    // Hiện SettingPanel
                    settingPanel.SetActive(true);
                    // Dùng LeanTween để hiện SettingPanel
                    LeanTween.scale(settingPanel, Vector3.one, 0.5f)
                        .setEase(LeanTweenType.easeOutBack);
                });
        }
    }

    public void PlayGame()
    {
        Debug.Log("Play Game!");
        if (!string.IsNullOrEmpty(gamePlaySceneName))
        {
            SceneManager.LoadScene(gamePlaySceneName);
        }
        else
        {
            Debug.LogError("GamePlay scene name is not set!");
        }
    }
}