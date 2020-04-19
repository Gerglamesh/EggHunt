using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UtilitiesScript
{
    public static Color GetRandomColor()
    {
        float r = Random.Range(0.0f, 1.0f);
        float g = Random.Range(0.0f, 1.0f);
        float b = Random.Range(0.0f, 1.0f);

        return new Color(r, g, b);
    }
}
