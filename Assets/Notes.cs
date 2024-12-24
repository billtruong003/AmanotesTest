using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Notes : MonoBehaviour
{
    public int sequence;
    [SerializeField] private List<SpriteRenderer> noteSprites;
    [SerializeField] private TextMeshProUGUI sequenceText;
    [SerializeField] private GameObject outline;
    [SerializeField] private GameObject outlineScale;
    public float fadeDuration = 0.15f;
    public float outlineScaleTime = 1.2f;
    public Vector3 outlineScaleDownValue = new Vector3(0.3f, 0.3f, 0.3f);
    public Vector3 outlineDisappearScale = new Vector3(0.05f, 0.05f, 0.05f);
    private const float OutlineStartScaleMultiplier = 3f;
    private const float OutlineScaleDisappearScaleMultiplier = 0.5f;

    private Dictionary<SpriteRenderer, Color> initialSpriteColors = new Dictionary<SpriteRenderer, Color>();
    private Dictionary<SpriteRenderer, Vector3> initialSpriteScales = new Dictionary<SpriteRenderer, Vector3>(); // Store initial sprite scales
    private Color initialTextColor;
    private LTDescr disappearTween;
    private bool canInteract = false;
    private Vector3 initialOutlineLocalScale;
    private Vector3 initialOutlineScaleLocalScale;

    void Awake()
    {
        initialOutlineLocalScale = outline.transform.localScale;
        if (outlineScale != null)
        {
            initialOutlineScaleLocalScale = outlineScale.transform.localScale;
        }

        // Store initial scales of sprite renderers
        foreach (SpriteRenderer sprite in noteSprites)
        {
            initialSpriteScales.Add(sprite, sprite.transform.localScale);
        }
    }

    void Start()
    {
        foreach (SpriteRenderer sprite in noteSprites)
        {
            initialSpriteColors.Add(sprite, sprite.color);
        }
        initialTextColor = GetVertexColor(sequenceText);
    }

    void OnEnable()
    {
        ResetNote();
        ShowOutlineAnimation();
        canInteract = false;
    }

    public bool CanInteract()
    {
        return canInteract;
    }

    public void SetSequence(int sequence)
    {
        this.sequence = sequence;
        sequenceText.text = this.sequence.ToString();
    }

    private Color GetVertexColor(TextMeshProUGUI textComponent)
    {
        textComponent.ForceMeshUpdate();
        if (textComponent.textInfo == null || textComponent.textInfo.characterCount == 0)
        {
            return Color.white;
        }
        TMP_CharacterInfo charInfo = textComponent.textInfo.characterInfo[0];
        int materialIndex = charInfo.materialReferenceIndex;
        int vertexIndex = charInfo.vertexIndex;
        return textComponent.textInfo.meshInfo[materialIndex].colors32[vertexIndex];
    }

    private void SetVertexColor(TextMeshProUGUI textComponent, Color color)
    {
        textComponent.ForceMeshUpdate();
        for (int i = 0; i < textComponent.textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textComponent.textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;
            int materialIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;
            Color32[] colors = textComponent.textInfo.meshInfo[materialIndex].colors32;
            for (int j = 0; j < 4; j++)
            {
                colors[vertexIndex + j] = color;
            }
        }
        textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }

    private void ShowOutlineAnimation()
    {
        LeanTween.scale(outline, initialOutlineLocalScale * OutlineStartScaleMultiplier, 0f);

        disappearTween = LeanTween.scale(outlineScale, outlineScaleDownValue, outlineScaleTime)
            .setEaseOutQuad()
            .setOnStart(() => canInteract = true)
            .setOnComplete(OnTimedDisappear);
    }

    private void OnTimedDisappear()
    {
        if (canInteract)
        {
            Disappear();
        }
    }

    public void HandleInteraction()
    {
        if (!canInteract) return;

        if (disappearTween != null)
        {
            LeanTween.cancel(disappearTween.uniqueId);
            disappearTween = null;
        }
        SoundManager.Instance.PlayHitSound();
        HitAnimation();
        DisappearInteract();
        canInteract = false;
    }

    private void HitAnimation()
    {
        float punchScaleAmount = 0.15f;
        float punchTime = 0.18f;
        foreach (var sprite in noteSprites)
        {
            LeanTween.scale(sprite.gameObject, sprite.transform.localScale * (1f - punchScaleAmount), punchTime * 0.6f).setEaseOutQuad().setOnComplete(() => // Giảm thời gian shrink
            {
                LeanTween.scale(sprite.gameObject, initialSpriteScales[sprite], punchTime * 0.8f).setEaseOutBounce(); // Sử dụng easeOutBounce
            });
        }
        LeanTween.scale(sequenceText.gameObject, sequenceText.transform.localScale * (1f - punchScaleAmount), punchTime * 0.6f).setEaseOutQuad().setOnComplete(() => // Giảm thời gian shrink
        {
            LeanTween.scale(sequenceText.gameObject, sequenceText.transform.localScale, punchTime * 0.8f).setEaseOutBounce(); // Sử dụng easeOutBounce
        });
    }
    private void DisappearInteract()
    {
        if (!gameObject.activeInHierarchy) return;

        foreach (SpriteRenderer sprite in noteSprites)
        {
            LeanTween.alpha(sprite.gameObject, 0f, fadeDuration).setEaseInCubic();
        }

        LeanTween.scale(outline.gameObject, outlineDisappearScale, fadeDuration).setEaseInBack();
        LeanTween.alpha(outlineScale.gameObject, 0f, fadeDuration).setEaseInCubic();
        LeanTween.scale(outlineScale.gameObject, outlineDisappearScale * OutlineScaleDisappearScaleMultiplier, fadeDuration).setEaseInBack();

        LeanTween.value(sequenceText.gameObject, 1f, 0f, fadeDuration).setEaseInCubic()
            .setOnUpdate((float val) =>
            {
                if (!gameObject.activeInHierarchy) return;
                Color color = sequenceText.color;
                color.a = val;
                sequenceText.color = color;
            }).setOnComplete(() =>
            {
                NotePool.Instance.ReturnNote(this);
            });
    }


    private void Disappear()
    {
        if (!gameObject.activeInHierarchy || !canInteract) return;

        foreach (SpriteRenderer sprite in noteSprites)
        {
            LeanTween.alpha(sprite.gameObject, 0f, fadeDuration).setEaseInCubic();
        }

        LeanTween.scale(outline.gameObject, outlineDisappearScale, fadeDuration).setEaseInBack();
        LeanTween.alpha(outlineScale.gameObject, 0f, fadeDuration).setEaseInCubic();
        LeanTween.scale(outlineScale.gameObject, outlineDisappearScale * OutlineScaleDisappearScaleMultiplier, fadeDuration).setEaseInBack();

        LeanTween.value(sequenceText.gameObject, 1f, 0f, fadeDuration).setEaseInCubic()
            .setOnUpdate((float val) =>
            {
                if (!gameObject.activeInHierarchy) return;
                Color color = sequenceText.color;
                color.a = val;
                sequenceText.color = color;
            }).setOnComplete(() =>
            {
                SoundManager.Instance.PlayMissSound();
                NotePool.Instance.ReturnNote(this);
            });
    }

    private void ResetNote()
    {
        LeanTween.cancel(gameObject);
        LeanTween.cancel(sequenceText.gameObject);

        foreach (SpriteRenderer sprite in noteSprites)
        {
            LeanTween.cancel(sprite.gameObject);
            if (initialSpriteColors.ContainsKey(sprite))
            {
                sprite.color = initialSpriteColors[sprite];
            }
            Color spriteColor = sprite.color;
            spriteColor.a = 1f;
            sprite.color = spriteColor;
            if (initialSpriteScales.ContainsKey(sprite))
            {
                sprite.transform.localScale = initialSpriteScales[sprite]; // Reset sprite scale
            }
        }
        SetVertexColor(sequenceText, initialTextColor);
        Color textColor = sequenceText.color;
        textColor.a = 1f;
        sequenceText.color = textColor;
        sequenceText.transform.localScale = Vector3.one;

        // gameObject.SetActive(true); // Redundant as OnEnable is called

        outline.transform.localScale = initialOutlineLocalScale;
        if (outlineScale != null)
        {
            outlineScale.transform.localScale = initialOutlineScaleLocalScale;
            Color outlineScaleColor = outlineScale.GetComponent<SpriteRenderer>().color;
            outlineScaleColor.a = 1f;
            outlineScale.GetComponent<SpriteRenderer>().color = outlineScaleColor;
        }

        canInteract = false;
    }

    public void SetPos(Vector2 pos)
    {

    }
}