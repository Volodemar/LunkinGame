using UnityEngine;

/// <summary>
/// Выводит количество собранных ресурсов
/// </summary>	
public class DisplayTotalText : MonoBehaviour
{
	private void OnGUI()
    {
        // Стиль
        GUIStyle style = new GUIStyle();
        style.fontSize = 24;
        style.normal.textColor = Color.white;

        // Позиция текста
        float x = Screen.width - 200; 
        float y = 10; 

        GUI.Label(new Rect(x, y, 200, 50), $"Total: {LevelData.TotalResources}", style);
    }
}
