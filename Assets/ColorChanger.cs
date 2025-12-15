using UnityEngine;
using System.Collections.Generic;

public class ColorChanger : MonoBehaviour
{
    public List<Color> colors;          // Colors to choose from
    public List<Renderer> renderers;    // All car renderers (body, doors, hood, etc.)

    public void ChangeColor(int index)
    {
        if (index < 0 || index >= colors.Count)
            return;

        foreach (Renderer rend in renderers)
        {
            rend.material.color = colors[index];
        }
    }

    public void Red()
    {
        ChangeColor(0);
    }
    public void Yellow()
    {
        ChangeColor(1);
    }
    public void Aqua()
    {
        ChangeColor(2);
    }
    public void Orange()
    {
        ChangeColor(3);
    }

    public void Reset()
    {
        ChangeColor(4);
    }
    
}