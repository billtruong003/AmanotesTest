using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundAnimation : MonoBehaviour
{
    [Header("Animation Settings")]
    public float scaleAmount = 0.05f;    // Lượng scale (ví dụ: 0.2f tương đương scale 20%)
    public float scaleTime = 5f;       // Thời gian scale
    public LeanTweenType easeType = LeanTweenType.easeInOutSine; // Kiểu easing

    void Start()
    {
        AnimateLayer(transform); // Dùng chính transform của GameObject chứa script này
    }

    void AnimateLayer(Transform layer)
    {
        Vector3 originalScale = layer.localScale;
        // LeanTween.scaleX(layer.gameObject, originalScale.x + scaleAmount, scaleTime)
        //     .setEase(easeType)
        //     .setLoopPingPong();

        // LeanTween.scaleY(layer.gameObject, originalScale.y + scaleAmount, scaleTime)
        //     .setEase(easeType)
        //     .setLoopPingPong();

        LeanTween.scale(layer.gameObject, originalScale + new Vector3(scaleAmount, scaleAmount, 0f), scaleTime)
            .setEase(easeType)
            .setLoopPingPong();
    }
}