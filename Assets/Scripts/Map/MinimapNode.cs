using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapNode : MonoBehaviour
{
    public Image image;

    public void SetVisited()
    {
        image.color = Color.white;
    }

    public void SetUnvisited()
    {
        image.color = new Color(1, 1, 1, 0.2f);
    }
}
