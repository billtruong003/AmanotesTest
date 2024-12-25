using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Variables;

public class WorldSpaceInteraction : MonoBehaviour, IPointerDownHandler
{
    public static WorldSpaceInteraction Instance;
    public Camera worldSpaceCamera;
    public RawImage uiRawImage;
    public LayerMask noteLayerToDetect;
    private int noteLayer;
    private Vector3 initialTouchPosition;

    void Awake()
    {
        Instance = this;
    }

    void Destroy()
    {
        Instance = null;
    }

    void Start()
    {
        noteLayer = LayerMask.NameToLayer("Note");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (RectTransformUtility.RectangleContainsScreenPoint(uiRawImage.rectTransform, eventData.position, eventData.pressEventCamera))
        {
            initialTouchPosition = eventData.position;

            Vector3 worldPoint = worldSpaceCamera.ScreenToWorldPoint(eventData.position);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, worldSpaceCamera.transform.forward, 100f, noteLayerToDetect);

            if (hit.collider != null)
            {
                if (hit.collider.gameObject != null)
                {
                    Debug.Log("Hit: " + hit.collider.name);
                }
            }
        }
    }

    public void RefreshRenderTexture()
    {
        if (worldSpaceCamera != null)
        {
            RenderTexture currentRT = worldSpaceCamera.targetTexture;

            if (currentRT == null)
            {
                currentRT = new RenderTexture(Screen.width, Screen.height, 24);
                worldSpaceCamera.targetTexture = currentRT;
            }

            worldSpaceCamera.Render();

            if (uiRawImage != null)
            {
                uiRawImage.texture = currentRT;
            }

            /*
            if (worldSpaceCamera.targetTexture == currentRT && currentRT != originalRT)
            {
                worldSpaceCamera.targetTexture = originalRT;
                currentRT.Release();
                Destroy(currentRT);
            }
            */
        }
        else
        {
            Debug.LogWarning("World Space Camera is not assigned.");
        }
    }

}