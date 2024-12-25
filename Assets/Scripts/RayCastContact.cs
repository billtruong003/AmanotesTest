using UnityEngine;

public class RayCastContact2D : MonoBehaviour
{
    public float interactionDelay = 0.05f;
    private float lastInteractionTime;
    public Color gizmoColor = Color.yellow; // Màu của gizmo

    void Update()
    {
        if (GameDataManager.Instance != null && Input.GetKeyDown(GameDataManager.Instance.settings.keyBinding))
        {
            if (Time.time - lastInteractionTime < interactionDelay)
            {
                return;
            }

            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null)
            {
                Debug.Log("Collider đã được hit: " + hit.collider.gameObject.name);
                Notes hitNote = hit.collider.GetComponent<Notes>();
                if (hitNote != null)
                {
                    Debug.Log("Tìm thấy script Notes trên: " + hitNote.gameObject.name + ", CanInteract = " + hitNote.CanInteract());
                    if (hitNote.CanInteract())
                    {
                        Debug.Log("Note tương tác: " + hitNote.gameObject.name);
                        hitNote.HandleInteraction();
                        lastInteractionTime = Time.time;
                    }
                }
                else
                {
                    Debug.Log("Không tìm thấy script Notes trên collider: " + hit.collider.gameObject.name);
                }
            }
            else
            {
                Debug.Log("Không có collider nào được hit.");
            }
        }
    }

    // Vẽ gizmo trong Scene View
    private void OnDrawGizmos()
    {
        if (Camera.main != null)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Gizmos.color = gizmoColor;
            // Vẽ một sphere nhỏ tại vị trí chuột
            Gizmos.DrawSphere(mousePos, 0.05f);
        }
    }
}