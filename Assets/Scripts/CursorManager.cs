using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public Texture2D customCursor;           // Textura original do cursor
    public Vector2 hotSpot = Vector2.zero;   // Ponto de clique
    public CursorMode cursorMode = CursorMode.Auto;
    [Range(0.1f, 5f)]
    public float cursorScale = 1f;           // Escala do cursor no Inspector

    void Start()
    {
        if (customCursor != null)
        {
            // Aplica escala
            Texture2D scaledCursor = ScaleTexture(customCursor, cursorScale);
            Cursor.SetCursor(scaledCursor, hotSpot, cursorMode);
        }
        else
        {
            Debug.LogWarning("Cursor personalizado não atribuído no Inspetor!");
        }
    }

    // Redimensiona a textura mantendo a proporção
    Texture2D ScaleTexture(Texture2D source, float scale)
    {
        int newWidth = Mathf.RoundToInt(source.width * scale);
        int newHeight = Mathf.RoundToInt(source.height * scale);

        RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight);
        rt.filterMode = FilterMode.Point;

        RenderTexture.active = rt;
        Graphics.Blit(source, rt);

        Texture2D result = new Texture2D(newWidth, newHeight, TextureFormat.ARGB32, false);
        result.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
        result.Apply();

        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);

        return result;
    }
}
