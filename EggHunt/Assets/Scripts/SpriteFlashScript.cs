using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteFlashScript : MonoBehaviour
{
    private readonly SpriteRenderer renderer;
    private readonly Color origColor;

    public SpriteFlashScript(SpriteRenderer renderer)
    {
        this.renderer = renderer;
        origColor = renderer.color;
    }

    public IEnumerator FlashWhiteCoroutine(float duration, float interwall)
    {
        Color color = Color.white;
        color.a = 1;

        for (float i = duration; i >= 0; i -= interwall * 2)
        {
            renderer.color = color;
            yield return new WaitForSeconds(interwall);
            renderer.color = origColor;
            yield return new WaitForSeconds(interwall);
        }
    }
}
