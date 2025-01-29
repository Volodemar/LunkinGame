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
        public BaseNode nodeA; 
        public BaseNode nodeB; 
        public float pathLength; 
    }

    // Кеш нод
    private List<BaseNode> cachedNodes = new List<BaseNode>();

    // Кеш путей между нодами
    private List<Path> paths = new List<Path>(); 

    // Кеш всех кротчайших путей из А в Б
    private Dictionary<(BaseNode, BaseNode), List<BaseNode>> pathCache = new Dictionary<(BaseNode, BaseNode), List<BaseNode>>();

    public void Init()
    {
        CacheAllNodes();
    }

    /// <summary>
    /// Вернуть случайную ноду
    /// </summary>
    public BaseNode GetRandomNode()
    {
        if (cachedNodes.Count == 0) 
            return null;

        return cachedNodes[Random.Range(0, cachedNodes.Count)];
    }

    private void CacheAllNodes()
    {
        cachedNodes.Clear();

        BaseNode[] nodes = GetComponentsInChildren<BaseNode>(true);

        cachedNodes.AddRange(nodes);
    }

    private void CacheAllShortestPaths()
    {
        pathCache.Clear();
        foreach (var startNode in cachedNodes)
        {
            foreach (var endNode in cachedNodes)
            {
                if (startNode != endNode)
                {
                    List<BaseNode> shortestPath = FindShortestPath(startNode, endNode);
                    pathCache[(startNode, endNode)] = shortestPath;
                }
            }
        }
    }

    private List<BaseNode> FindShortestPath(BaseNode start, BaseNode end)
    {
        return new List<BaseNode>(); 
    }

    private List<BaseNode> ReconstructPath(Dictionary<BaseNode, BaseNode> cameFrom, BaseNode current)
    {
        var totalPath = new List<BaseNode> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            totalPath.Insert(0, current);
        }
        return totalPath;
    }

    private float Heuristic(BaseNode a, BaseNode b)
    {
        return Vector3.Distance(a.transform.position, b.transform.position);
    }

    public float GetPathLength(BaseNode a, BaseNode b)
    {
        foreach (var path in paths)
        {
            if ((path.nodeA == a && path.nodeB == b) || (path.nodeA == b && path.nodeB == a))
            {
                return path.pathLength;
            }
        }
        return float.MaxValue; 
    }

    public List<BaseNode> GetCachedPath(BaseNode start, BaseNode end)
    {
        if (pathCache.TryGetValue((start, end), out var path))
        {
            return path;
        }
        return new List<BaseNode>(); 
    }

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