using UnityEngine;

/// <summary>
/// Нода Шахта
/// </summary>	
public class Mine : BaseNode
{
    /// <summary>
    /// Множитель времени добычи в шахте
    /// </summary>
    public float miningTimeMultiplier = 1f;

    private void OnDrawGizmos()
    {
        // Стиль
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.cyan; 
        style.fontSize = 16; 
        style.alignment = TextAnchor.MiddleLeft;

        // Отрисовка текста с множителем
        UnityEditor.Handles.Label(transform.position + Vector3.right * 0.6f, $"{miningTimeMultiplier}x", style);
    }
}