using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    public int numberSpawn;
    public int numberMiss;
    public float percentage;
    public int Score;
    public int Perfect = 500;
    public int Good = 300;
    public int Cool = 50;
    public int timePerfect;
    public int timeGood;
    public int timeCool;
    public int currentCombo = 0;
    public int highestCombo = 0;
    public float combo = 1f;
    public float comboIncrement = 0.1f;
    public int comboIncreaseThreshold = 5;

    public enum HitAccuracy
    {
        Perfect,
        Good,
        Cool,
        Miss
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    public void Start()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateScoreText(Score, percentage, currentCombo);
        }
    }

    public HitAccuracy CalculateHitAccuracy(LTDescr disappearTween, float outlineScaleTime)
    {
        float currentTime = disappearTween.passed;
        float perfectThreshold = outlineScaleTime * 0.4f;
        float goodThreshold = outlineScaleTime * 0.65f;
        float coolThreshold = outlineScaleTime * 0.85f;


        if (currentTime <= perfectThreshold)
        {
            return HitAccuracy.Perfect;
        }
        else if (currentTime <= goodThreshold)
        {
            return HitAccuracy.Good;
        }
        else if (currentTime <= coolThreshold)
        {
            return HitAccuracy.Cool;
        }
        else
        {
            return HitAccuracy.Miss;
        }
    }

    public void UpdateScore(HitAccuracy hitAccuracy)
    {
        switch (hitAccuracy)
        {
            case HitAccuracy.Perfect:
                Score += (int)(Perfect * combo);
                timePerfect++;
                IncreaseCombo();
                break;
            case HitAccuracy.Good:
                Score += (int)(Good * combo);
                timeGood++;
                IncreaseCombo();
                break;
            case HitAccuracy.Cool:
                Score += (int)(Cool * combo);
                timeCool++;
                IncreaseCombo();
                break;
            case HitAccuracy.Miss:
                numberMiss++;
                ResetCombo();
                break;
            default:
                break;
        }

        numberSpawn++;
        percentage = (float)numberSpawn / (numberSpawn + numberMiss) * 100;

        UIManager.Instance.UpdateScoreText(Score, percentage, currentCombo);
    }

    private void IncreaseCombo()
    {
        currentCombo++;
        if (currentCombo > highestCombo)
        {
            highestCombo = currentCombo;
        }


        if (currentCombo % comboIncreaseThreshold == 0)
        {
            combo += comboIncrement;
        }
    }

    private void ResetCombo()
    {
        currentCombo = 0;
        combo = 1f;
    }

}