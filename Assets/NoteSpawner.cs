using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NoteSpawner : MonoBehaviour
{
    public Notes notePrefab;
    public Vector2 LimitX = new Vector2(-9, 9);
    public Vector2 LimitY = new Vector2(-4.5f, 4.5f);
    public Vector2 currentPos;
    public float stepSize = 2f; // Khoảng cách giữa các note
    public float noteRadius = 0.5f; // Bán kính của note (điều chỉnh cho phù hợp)
    public TrailRenderer trailRenderer;
    public float trailMoveDuration = 0.2f; // Thời gian di chuyển của trail
    public int spectrumSampleSize = 2048; // Số lượng mẫu phân tích phổ (nên là lũy thừa của 2)
    public float threshold = 0.1f;        // Ngưỡng năng lượng (không sử dụng trực tiếp trong code mới)
    public float minInterval = 0.2f;      // Khoảng thời gian tối thiểu giữa hai lần spawn

    private float[] spectrumData;         // Mảng lưu dữ liệu phổ
    private float lastBeatTime = 0;       // Thời điểm phát hiện beat cuối cùng

    public float sensitivity = 2f;        // Độ nhạy (không sử dụng trực tiếp trong code mới)
    private float[] lowFreqEnergies;       // Lưu trữ năng lượng tần số thấp cho mỗi lần phân tích
    private int energyIndex = 0;          // Chỉ số cho mảng lowFreqEnergies
    private float avgLowFreqEnergy = 0;   // Năng lượng tần số thấp trung bình
    public int energyHistoryLength = 43; // Số lượng mẫu lịch sử năng lượng

    public float countdownDuration = 3f;   // Thời gian đếm ngược
    private bool isPlaying = false;        // Cờ xác định nhạc đã bắt đầu phát hay chưa
    private bool isCountingDown = false;   // Cờ xác định đang trong thời gian đếm ngược hay không

    public TextMeshProUGUI countdownText; // UI Text để hiển thị thời gian đếm ngược

    // Phát hiện Onset
    private float lastLowFrequencyEnergy = 0;
    public float onsetThreshold = 0.2f;

    // Tạo nhịp điệu
    public enum RhythmPattern { QuarterNotes, EighthNotes, Mixed }
    public RhythmPattern currentRhythmPattern = RhythmPattern.QuarterNotes;
    private int patternStep = 0;

    private int sequenceCounter = 1; // Thêm bộ đếm sequence
    public float sequenceResetInterval = 0.5f; // Khoảng thời gian để reset sequence
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
        isCountingDown = true;
        float timeLeft = countdownDuration;
        SoundManager.Instance.PlayBGM();

        // Clear the trail at the beginning
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
            lastSpawnTime = Time.time; // Cập nhật thời điểm spawn cuối cùng
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