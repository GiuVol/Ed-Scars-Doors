using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomUtilities
{
    public static void SetLayerRecursively(GameObject gameObject, int newLayer)
    {
        if (newLayer < 0)
        {
            return;
        }

        gameObject.layer = newLayer;

        foreach (Transform child in gameObject.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}
