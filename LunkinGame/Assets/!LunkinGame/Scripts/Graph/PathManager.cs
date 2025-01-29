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

    // Настроенные пути графа 
    public List<Path> paths = new List<Path>(); 

    // Кеш нод для оптимизации
    private List<BaseNode> cachedNodes = new List<BaseNode>();

    // Кеш всех кротчайших путей из А в Б для оптимизации
    private Dictionary<(BaseNode, BaseNode), List<BaseNode>> pathShortCache = new Dictionary<(BaseNode, BaseNode), List<BaseNode>>();

    public void Init()
    {
        CacheAllNodes();

        InitializeNodes();

        CacheAllShortestPaths();
    }

    private void CacheAllNodes()
    {
        cachedNodes.Clear();

        BaseNode[] nodes = GetComponentsInChildren<BaseNode>(true);

        cachedNodes.AddRange(nodes);
    }

    private void InitializeNodes()
    {
        // Ноды заполняют данные о соседних нодах на основе путей графа
        foreach(BaseNode node in cachedNodes)
        {
            node.Init(this);
        }
    }

    private void CacheAllShortestPaths()
    {
        pathShortCache.Clear();

        foreach (var startNode in cachedNodes)
        {
            foreach (var endNode in cachedNodes)
            {
                if (startNode != endNode)
                {
                    List<BaseNode> shortestPath = FindShortestPath(startNode, endNode);

                    pathShortCache[(startNode, endNode)] = shortestPath;
                }
            }
        }
    }

    /// <summary>
    /// Метод для поиска кратчайшего пути startNode до endNode
    /// </summary>
    public List<BaseNode> FindShortestPath(BaseNode startNode, BaseNode endNode)
    {
        List<List<BaseNode>> allPaths = FindAllPaths(startNode, endNode);

        List<BaseNode> shortestPath = null;
        float minPathLength = float.MaxValue;

        float currentPathLength = 0;
        foreach (List<BaseNode> path in allPaths)
        {
            currentPathLength = 0;
            for (int i = 0; i < path.Count - 1; i++)
            {
                currentPathLength += GetPathLength(path[i], path[i + 1]);
            }

            if (currentPathLength < minPathLength)
            {
                minPathLength = currentPathLength;
                shortestPath = path;
            }
        }

        return shortestPath;
    }

    /// <summary>
    /// Метод для поиска всех путей от startNode до endNode
    /// </summary>
    private List<List<BaseNode>> FindAllPaths(BaseNode startNode, BaseNode endNode)
    {
        List<List<BaseNode>> allPaths = new List<List<BaseNode>>();
        List<BaseNode> currentPath = new List<BaseNode>();
        HashSet<BaseNode> visited = new HashSet<BaseNode>();

        DFS(startNode, endNode, currentPath, allPaths, visited);

        return allPaths;
    }

    /// <summary>
    /// Рекурсивный метод поиска в глубину
    /// </summary>
    private void DFS(BaseNode current, BaseNode target, List<BaseNode> currentPath, List<List<BaseNode>> allPaths, HashSet<BaseNode> visited)
    {
        visited.Add(current);
        currentPath.Add(current);

        if (current == target)
        {
            // Находим путь до текущей точки и добавляем его в список всех путей
            allPaths.Add(new List<BaseNode>(currentPath));
        }
        else
        {
            // Инициализируем соседей текущего узла, если они ещё не инициализированы
            if (current.neighborNodes == null || current.neighborNodes.Length == 0)
            {
                current.Init(this);
            }

            foreach (var neighbor in current.neighborNodes)
            {
                if (!visited.Contains(neighbor))
                {
                    DFS(neighbor, target, currentPath, allPaths, visited);
                }
            }
        }

        // Отмена посещения текущего узла и удаление его из текущего пути
        visited.Remove(current);
        currentPath.RemoveAt(currentPath.Count - 1);
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

    public List<BaseNode> GetCachedShortPath(BaseNode start, BaseNode end)
    {
        if (pathShortCache.TryGetValue((start, end), out var path))
        {
            return path;
        }
        return new List<BaseNode>(); 
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

    /// <summary>
    /// Выводим найденные пути в лог
    /// </summary>
    private void DebugLogPrintPaths(List<List<BaseNode>> paths)
    {
		foreach (var path in paths)
		{
			string pathStr = "";
			foreach (var node in path)
			{
				pathStr += node.name + " -> ";
			}
			pathStr = pathStr.TrimEnd(' ', '-', '>');
			Debug.Log("Путь: " + pathStr);
		}
	}

    /// <summary>
    /// Вывод пути в лог
    /// </summary>
    private void DebugLogPrintPath(List<BaseNode> path)
    {
		string pathStr = "";
		foreach (var node in path)
		{
			pathStr += node.name + " -> ";
		}
		pathStr = pathStr.TrimEnd(' ', '-', '>');
		Debug.Log("Путь: " + pathStr);
	}
}