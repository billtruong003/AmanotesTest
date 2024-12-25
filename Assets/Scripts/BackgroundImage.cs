using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundImage : MonoBehaviour
{
    [SerializeField] private GameObject backgroundQuad;

    void Start()
    {
        ApplyBackgroundImage();
    }

    private void ApplyBackgroundImage()
    {
        if (GameDataManager.Instance == null)
        {
            Debug.LogError("GameDataManager Instance is null!");
            return;
        }

        if (GameDataManager.Instance.settings == null)
        {
            Debug.LogError("GameDataManager settings is null!");
            return;
        }

        if (GameDataManager.Instance.settings.bgImage == null)
        {
            Debug.LogWarning("GameDataManager bgImage is null, no background image to apply.");
            return;
        }

        if (backgroundQuad == null)
        {
            Debug.LogError("backgroundQuad GameObject is not assigned in the Inspector!");
            return;
        }

        MeshRenderer meshRenderer = backgroundQuad.GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            Debug.LogError("MeshRenderer component not found on the assigned backgroundQuad GameObject.");
            return;
        }

        Material material = meshRenderer.material;
        if (material == null)
        {
            Debug.LogError("Material not found on the MeshRenderer of the backgroundQuad GameObject.");
            return;
        }

        Texture2D backgroundTexture = GameDataManager.Instance.settings.bgImage.texture;

        if (material.HasProperty("_MainTex"))
        {
            material.mainTexture = backgroundTexture;
        }
        else
        {
            Debug.LogWarning("Material does not have a _MainTex property (Albedo).");
        }

        if (material.HasProperty("_EmissionMap"))
        {
            material.SetTexture("_EmissionMap", backgroundTexture);
            if (material.HasProperty("_EmissionColor"))
            {
                material.SetColor("_EmissionColor", Color.white);
            }
            else
            {
                Debug.LogWarning("Material has _EmissionMap but not _EmissionColor.");
            }
            material.EnableKeyword("_EMISSION");
        }
        else
        {
            Debug.LogWarning("Material does not have an _EmissionMap property.");
        }
    }
}