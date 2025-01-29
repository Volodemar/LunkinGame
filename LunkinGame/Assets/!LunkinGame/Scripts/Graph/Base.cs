using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Нода базы
/// </summary>	
public class Base : BaseNode
{
    public float resourceMultiplier = 1f; 

    private void OnDrawGizmos()
    {
        // Стиль
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.green; 
        style.fontSize = 16; 
        style.alignment = TextAnchor.MiddleCenter;

        // Отрисовка текста с множителем
        UnityEditor.Handles.Label(transform.position + Vector3.up * 0.7f, $"{resourceMultiplier}x", style);
    }
}