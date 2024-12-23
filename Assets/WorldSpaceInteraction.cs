using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Variables;

public class WorldSpaceInteraction : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public Camera worldSpaceCamera;
    public RawImage uiRawImage;
    public LayerMask noteLayerToDetect;
    private int noteLayer;
    private NoteDrag currentlyDraggingNote;
    private Vector3 initialTouchPosition;

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
                NoteBase note = hit.collider.GetComponent<NoteBase>();
                if (note != null)
                {
                    if (note.noteType == NoteType.Tap)
                    {
                        note.Hit();
                    }
                    else if (note.noteType == NoteType.DragStraight || note.noteType == NoteType.DragCurve)
                    {
                        currentlyDraggingNote = note as NoteDrag;
                        currentlyDraggingNote.StartDrag();
                    }
                }
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (currentlyDraggingNote != null)
        {
            //Debug.Log("Dragging");
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (currentlyDraggingNote != null)
        {
            currentlyDraggingNote.EndDrag();
            currentlyDraggingNote = null;
        }
        else
        {
            // Detect swipe to left or right
            Vector3 delta = (Vector3)eventData.position - initialTouchPosition;
            if (Mathf.Abs(delta.x) > 50f) // Threshold for swipe detection
            {
                if (delta.x > 0)
                {
                    Debug.Log("Swipe Right");
                }
                else
                {
                    Debug.Log("Swipe Left");
                }
            }
        }
    }
}