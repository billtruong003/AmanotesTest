using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public Image Background;
    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI ComboText;
    public TextMeshProUGUI Percentage;
    public Image RadialPercentage;

    public GameObject GameOverMenu;
    public TextMeshProUGUI GameOverText;
    public Button RestartButton;
    public Button MainMenuButton;

    public Button PauseButton;
    public GameObject PauseMenuPanel;
    public TextMeshProUGUI CurrentPauseScore;
    public Button ResumeButton;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (PauseMenuPanel != null)
        {
            PauseMenuPanel.transform.localScale = Vector3.zero;
        }
        if (GameOverMenu != null)
        {
            GameOverMenu.transform.localScale = Vector3.zero;
        }

        if (PauseButton != null)
        {
            PauseButton.onClick.AddListener(PauseGame);
        }
        if (RestartButton != null)
        {
            RestartButton.onClick.AddListener(RestartGame);
        }
        if (MainMenuButton != null)
        {
            MainMenuButton.onClick.AddListener(GoToMainMenu);
        }
        if (ResumeButton != null)
        {
            ResumeButton.onClick.AddListener(ResumeGame);
        }
    }

    public void FadeOutBackground()
    {
        if (Background == null)
        {
            Debug.LogError("Background Image is not assigned!");
            return;
        }

        LeanTween.alpha(Background.rectTransform, 0f, 3f)
            .setEase(LeanTweenType.easeInElastic)
            .setOnComplete(() =>
            {
                Debug.Log("Background faded out completely.");
                VFXManager.Instance.DisableAllVFXOfType(VFXManager.VFXType.Shield);
            });
    }

    public void FadeInBackground()
    {
        if (Background == null)
        {
            Debug.LogError("Background Image is not assigned!");
            return;
        }

        LeanTween.alpha(Background.rectTransform, 180f / 255f, 3f)
            .setEase(LeanTweenType.easeInElastic)
            .setOnComplete(() =>
            {
                Debug.Log("Background faded in.");
            });
    }

    public void UpdateScoreText(int score, float percentage, int combo)
    {
        if (ScoreText != null)
        {
            ScoreText.text = string.Format("{0:D11}", score);
        }
        if (Percentage != null)
        {
            Percentage.text = string.Format("{0:F1}", percentage) + "%";
        }
        if (RadialPercentage != null)
        {
            RadialPercentage.fillAmount = percentage / 100f;
        }
        if (ComboText != null)
        {
            ComboText.text = "x" + combo.ToString();
        }
    }

    public void PauseGame()
    {
        Debug.Log("Game Paused");
        SoundManager.Instance.PauseBGM();
        if (PauseMenuPanel != null)
        {
            PauseMenuPanel.SetActive(true);
            PauseMenuPanel.transform.localScale = Vector3.zero;
            if (CurrentPauseScore != null && ScoreManager.Instance != null)
            {
                CurrentPauseScore.text = "Score: " + string.Format("{0:D11}", ScoreManager.Instance.Score);
            }
            LeanTween.scale(PauseMenuPanel, Vector3.one, 0.5f)
                .setEase(LeanTweenType.easeOutBack)
                .setIgnoreTimeScale(true)
                .setOnComplete(() =>
                {
                    Time.timeScale = 0;
                });
        }
    }

    public void ResumeGame()
    {
        Debug.Log("Game Resumed");
        Time.timeScale = 1;
        SoundManager.Instance.ResumeBGM();
        if (PauseMenuPanel != null)
        {
            LeanTween.scale(PauseMenuPanel, Vector3.zero, 0.3f).setEase(LeanTweenType.easeInBack)
                .setIgnoreTimeScale(true)
                .setOnComplete(() =>
                {
                    PauseMenuPanel.SetActive(false);
                });
        }
    }

    [Button]
    public void GameOver()
    {
        Debug.Log("Game Over");

        if (GameOverMenu != null)
        {
            GameOverMenu.SetActive(true);
            GameOverMenu.transform.localScale = Vector3.zero;
            if (ScoreManager.Instance != null && GameOverText != null)
            {
                GameOverText.text = "Your Score: " + string.Format("{0:D11}", ScoreManager.Instance.Score);
            }

            LeanTween.scale(GameOverMenu, Vector3.one, 0.5f).setEase(LeanTweenType.easeOutBack)
            .setIgnoreTimeScale(true)
            .setOnComplete(() =>
            {
                Time.timeScale = 0;
            });
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Menu");
    }
}