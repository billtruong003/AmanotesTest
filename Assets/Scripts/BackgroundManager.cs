using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] public MeshRenderer quadRenderer;
    [SerializeField] private float countdownDuration = 3f; // Add countdown duration here

    private Color originalColor;

    void Start()
    {
        InitializeBackground();
        if (quadRenderer != null && quadRenderer.material != null)
        {
            originalColor = quadRenderer.material.color;
            FadeToBlack(countdownDuration);
            // Invoke("ResetBackgroundColorDelayed", countdownDuration + 0.5f); // Reset after countdown + a small delay
        }
        else
        {
            Debug.LogError("Quad Renderer or its material is not assigned!");
        }
    }

    void InitializeBackground()
    {
        if (cam == null || quadRenderer == null)
        {
            Debug.LogError("Camera or MeshRenderer not found!");
            return;
        }

        float targetAspectRatio = 16f / 9f;
        float cameraHeight = cam.orthographicSize * 2f;
        float targetWidth = cameraHeight * targetAspectRatio;

        Bounds quadBounds = quadRenderer.GetComponent<MeshFilter>().mesh.bounds;
        float quadWidth = quadBounds.size.x;
        float quadHeight = quadBounds.size.y;

        float scaleX = targetWidth / quadWidth;
        float scaleY = cameraHeight / quadHeight;

        quadRenderer.transform.localScale = new Vector3(scaleX, scaleY, 1f);
    }

    public void ReInitializeBackground()
    {
        InitializeBackground();
    }

    public void FadeToBlack(float duration)
    {
        if (quadRenderer != null && quadRenderer.material != null)
        {
            LeanTween.value(gameObject, quadRenderer.material.color, Color.black, duration)
                .setOnUpdateColor(UpdateBackgroundColor);
        }
    }

    private void ResetBackgroundColorDelayed()
    {
        ResetBackgroundColor(0.5f); // Fade back to original color over 0.5 seconds
    }

    public void ResetBackgroundColor(float duration)
    {
        if (quadRenderer != null && quadRenderer.material != null)
        {
            LeanTween.value(gameObject, quadRenderer.material.color, originalColor, duration)
                .setOnUpdateColor(UpdateBackgroundColor);
        }
    }

    private void UpdateBackgroundColor(Color newColor)
    {
        if (quadRenderer != null && quadRenderer.material != null)
        {
            quadRenderer.material.color = newColor;
        }
    }
}