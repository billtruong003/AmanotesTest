using UnityEngine;
using TMPro;

namespace BillUtils
{
    public static class ColorVertexUtils
    {
        public static Color GetVertexColor(TextMeshProUGUI textComponent)
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

        public static void SetVertexColor(TextMeshProUGUI textComponent, Color color)
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
    }
    public static class ColorUtils
    {
        public static Color GenerateRandomColor()
        {
            float r = Random.Range(0f, 1f);
            float g = Random.Range(0f, 1f);
            float b = Random.Range(0f, 1f);
            return new Color(r, g, b);
        }

        public static Color GenerateRandomColor(float minComponentValue)
        {
            float r = Random.Range(minComponentValue / 255f, 1f);
            float g = Random.Range(minComponentValue / 255f, 1f);
            float b = Random.Range(minComponentValue / 255f, 1f);
            return new Color(r, g, b);
        }
        public static Color GeneratePastelColor(float minComponentValue = 155f)
        {
            float r = Random.Range(minComponentValue / 255f, 1f);
            float g = Random.Range(minComponentValue / 255f, 1f);
            float b = Random.Range(minComponentValue / 255f, 1f);
            return new Color(r, g, b);
        }
    }
}