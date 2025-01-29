using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Описание нового класса
/// </summary>	
public class PathManager : MonoBehaviour
{
    [System.Serializable]
    public class Path
    {
        public BaseNode nodeA; // Нода A
        public BaseNode nodeB; // Нода B
        public float pathLength; // Длина пути
    }

    public List<Path> paths = new List<Path>(); 

    private void OnDrawGizmos()
    {
        foreach (var path in paths)
        {
            if (path.nodeA != null && path.nodeB != null)
            {
                // Отрисовка линии между нодами
                Gizmos.color = Color.black;
                Gizmos.DrawLine(path.nodeA.transform.position, path.nodeB.transform.position);

                // Стиль
                GUIStyle style = new GUIStyle();
                style.normal.textColor = Color.gray; 
                style.fontSize = 13; 
                style.alignment = TextAnchor.MiddleCenter;

                // Рассчитываем красивую позицию текста
                Vector3 midpoint = (path.nodeA.transform.position + path.nodeB.transform.position) / 2;
                Vector3 lineDirection = (path.nodeB.transform.position - path.nodeA.transform.position).normalized;
                Vector3 perpendicular = new Vector3(-lineDirection.y, lineDirection.x, lineDirection.z).normalized;
                Vector3 textPosition = midpoint + perpendicular * 0.25f;

                // Отрисовка текста с длиной пути
                UnityEditor.Handles.Label(textPosition, $"{path.pathLength}", style);
            }
        }
    }
}