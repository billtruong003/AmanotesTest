using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NoteSpawner : MonoBehaviour
{
    public Notes notePrefab;
    public Vector2 LimitX = new Vector2(-9, 9);
    public Vector2 LimitY = new Vector2(-4.5f, 2.5f);
    public Vector2 currentPos;
    public float stepSize = 2f;
    public float noteRadius = 0.5f;
    public TrailRenderer trailRenderer;
    public float trailMoveDuration = 0.2f;
    public int spectrumSampleSize = 2048;
    public float threshold = 0.1f;
    public float minInterval = 0.2f;

    private float[] spectrumData;
    private float lastBeatTime = 0;

    public float sensitivity = 2f;
    private float[] lowFreqEnergies;
    private int energyIndex = 0;
    private float avgLowFreqEnergy = 0;
    public int energyHistoryLength = 43;

    public float countdownDuration = 3f;
    private bool isPlaying = false;
    private bool isCountingDown = false;
    public TextMeshProUGUI countdownText;

    private float lastLowFrequencyEnergy = 0;
    public float onsetThreshold = 0.2f;

    public enum RhythmPattern { QuarterNotes, EighthNotes, Mixed }
    public RhythmPattern currentRhythmPattern = RhythmPattern.QuarterNotes;
    private int patternStep = 0;

    private int sequenceCounter = 1;
    public float sequenceResetInterval = 0.5f;
    private float lastSpawnTime = 0f;

    private List<Notes> spawnedNotes = new List<Notes>();

    void Start()
    {
        currentPos = new Vector2(Random.Range(LimitX.x, LimitX.y), Random.Range(LimitY.x, LimitY.y));
        spectrumData = new float[spectrumSampleSize];
        lowFreqEnergies = new float[energyHistoryLength];

        if (trailRenderer == null)
        {
            Debug.LogError("Trail Renderer is not assigned!");
        }

        StartCoroutine(StartCountdown());
    }

    IEnumerator StartCountdown()
    {
        UIManager.Instance.FadeOutBackground();
        isCountingDown = true;
        float timeLeft = countdownDuration;
        SoundManager.Instance.PlayBGM();

        if (trailRenderer != null)
        {
            trailRenderer.Clear();
        }

        while (timeLeft > 0)
        {
            countdownText.text = Mathf.CeilToInt(timeLeft).ToString();
            yield return new WaitForSeconds(0.1f);
            timeLeft -= 0.1f;
        }

        isPlaying = true;
        isCountingDown = false;

        countdownText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isPlaying && !isCountingDown)
        {
            if (SoundManager.Instance.getBGM != null && !SoundManager.Instance.getBGM.isPlaying)
            {
                isPlaying = false;
                UIManager.Instance.GameOver();
                return;
            }

            SoundManager.Instance.getBGM.GetSpectrumData(spectrumData, 0, FFTWindow.BlackmanHarris);

            float lowFrequencyEnergy = 0;
            int lowFreqRange = 15;
            for (int i = 0; i < lowFreqRange; i++)
            {
                lowFrequencyEnergy += spectrumData[i];
            }

            avgLowFreqEnergy = 0;
            for (int i = 0; i < energyHistoryLength; i++)
            {
                avgLowFreqEnergy += lowFreqEnergies[i];
            }
            avgLowFreqEnergy /= energyHistoryLength;

            float energyDifference = lowFrequencyEnergy - lastLowFrequencyEnergy;

            if (energyDifference > onsetThreshold && Time.time - lastBeatTime > minInterval)
            {
                lastBeatTime = Time.time;
                SpawnNote();
            }

            lastLowFrequencyEnergy = lowFrequencyEnergy;
            lowFreqEnergies[energyIndex] = lowFrequencyEnergy;
            energyIndex = (energyIndex + 1) % energyHistoryLength;
        }
    }

    private void SpawnNote()
    {
        if (Time.time - lastSpawnTime > sequenceResetInterval)
        {
            sequenceCounter = 1;
        }

        bool shouldSpawn = false;
        switch (currentRhythmPattern)
        {
            case RhythmPattern.QuarterNotes:
                shouldSpawn = true;
                break;
            case RhythmPattern.EighthNotes:
                if (Time.time - lastBeatTime > minInterval / 2f)
                {
                    shouldSpawn = true;
                    lastBeatTime = Time.time;
                }
                break;
            case RhythmPattern.Mixed:
                int[] mixedPattern = { 1, 1, 0, 1 };
                if (mixedPattern[patternStep] == 1)
                {
                    shouldSpawn = true;
                }
                patternStep = (patternStep + 1) % mixedPattern.Length;
                break;
        }

        if (shouldSpawn)
        {
            if (sequenceCounter > 8) sequenceCounter = 1;
            if (sequenceCounter == 1)
                currentPos = new Vector2(Random.Range(LimitX.x, LimitX.y), Random.Range(LimitY.x, LimitY.y));

            Vector2 direction = GetRandomDirection();
            Vector2 nextPos = currentPos + direction * stepSize;

            nextPos.x = Mathf.Clamp(nextPos.x, LimitX.x, LimitX.y);
            nextPos.y = Mathf.Clamp(nextPos.y, LimitY.x, LimitY.y);

            int attempts = 0;
            while (IsOverlapping(nextPos) && attempts < 16)
            {
                attempts++;
                direction = GetRandomDirection();
                nextPos = currentPos + direction * stepSize;
                nextPos.x = Mathf.Clamp(nextPos.x, LimitX.x, LimitX.y);
                nextPos.y = Mathf.Clamp(nextPos.y, LimitY.x, LimitY.y);
            }



            Notes note = NotePool.Instance.GetNote();
            note.transform.position = nextPos;
            note.SetPos(nextPos);
            note.SetSequence(sequenceCounter);

            sequenceCounter++;

            spawnedNotes.Add(note);

            if (trailRenderer != null && spawnedNotes.Count > 0)
            {
                if (spawnedNotes.Count == 1)
                {
                    trailRenderer.gameObject.SetActive(false);
                    trailRenderer.transform.position = note.transform.position;
                    trailRenderer.gameObject.SetActive(true);
                }
                else
                {
                    if (sequenceCounter == 2)
                    {
                        trailRenderer.gameObject.SetActive(false);
                        trailRenderer.transform.position = note.transform.position;
                        trailRenderer.gameObject.SetActive(true);
                    }
                    else
                    {
                        LeanTween.move(trailRenderer.gameObject, note.transform.position, trailMoveDuration);
                    }
                }
            }

            currentPos = nextPos;
            lastSpawnTime = Time.time;
            ScoreManager.Instance.numberSpawn++;
        }
    }

    public void SetRhythmPattern(RhythmPattern pattern)
    {
        currentRhythmPattern = pattern;
        patternStep = 0;
    }

    private Vector2 GetRandomDirection()
    {
        int randomDirection = Random.Range(0, 8);
        switch (randomDirection)
        {
            case 0:
                return Vector2.left;
            case 1:
                return new Vector2(-1, 1).normalized;
            case 2:
                return Vector2.up;
            case 3:
                return new Vector2(1, 1).normalized;
            case 4:
                return Vector2.right;
            case 5:
                return new Vector2(1, -1).normalized;
            case 6:
                return Vector2.down;
            case 7:
                return new Vector2(-1, -1).normalized;
            default:
                return Vector2.zero;
        }
    }

    private bool IsOverlapping(Vector2 pos)
    {
        return Physics2D.OverlapCircle(pos, noteRadius) != null;
    }
}