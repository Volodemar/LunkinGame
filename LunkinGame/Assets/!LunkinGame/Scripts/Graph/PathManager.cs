using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.Linq;

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
    [OnValueChanged("OnSectionsChanged")]
    public List<Path> sections = new List<Path>(); 

    // Кеш нод для оптимизации
    private List<BaseNode> _cachedNodes = new List<BaseNode>();

    // Кеш всех кротчайших путей из А в Б для оптимизации
    private Dictionary<(BaseNode, BaseNode), List<BaseNode>> _pathShortCache = new Dictionary<(BaseNode, BaseNode), List<BaseNode>>();

	GameLevelController _gameLevel;

    public void Init()
    {
        _gameLevel = LevelController.Instance as GameLevelController;

        CacheAllNodes();

        InitializeNodes();

        CacheAllShortestPaths();
    }

    private void CacheAllNodes()
    {
        _cachedNodes.Clear();

        BaseNode[] nodes = GetComponentsInChildren<BaseNode>(true);

        _cachedNodes.AddRange(nodes);
    }

    private void InitializeNodes()
    {
        foreach (BaseNode node in _cachedNodes)
        {
            node.Init(this);
        }
    }

    private void CacheAllShortestPaths()
    {
        _pathShortCache.Clear();

        foreach (var startNode in _cachedNodes)
        {
            foreach (var endNode in _cachedNodes)
            {
                if (startNode != endNode)
                {
                    List<BaseNode> shortestPath = FindShortestPath(startNode, endNode);

                    _pathShortCache[(startNode, endNode)] = shortestPath;
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
                currentPathLength += GetPathSectionLength(path[i], path[i + 1]);
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

        DepthFirstSearch(startNode, endNode, currentPath, allPaths, visited);

        return allPaths;
    }

    /// <summary>
    /// Рекурсивный метод поиска пути в глубину
    /// </summary>
    private void DepthFirstSearch(BaseNode current, BaseNode target, List<BaseNode> currentPath, List<List<BaseNode>> allPaths, HashSet<BaseNode> visited)
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
                    DepthFirstSearch(neighbor, target, currentPath, allPaths, visited);
                }
            }
        }

        // Отмена посещения текущего узла и удаление его из текущего пути
        visited.Remove(current);
        currentPath.RemoveAt(currentPath.Count - 1);
    }

    /// <summary>
    /// Возвращает ближайшие ноды
    /// </summary>
    public List<BaseNode> GetNeighbors(BaseNode current)
    {
        List<BaseNode> neighbors = new List<BaseNode>();

        foreach (var section in sections)
        {
            if (section.nodeA == current)
            {
                neighbors.Add(section.nodeB);
            }
            else if (section.nodeB == current)
            {
                neighbors.Add(section.nodeA);
            }
        }

        return neighbors;
    }

    /// <summary>
    /// Возвращает длину секции пути
    /// </summary>
    public float GetPathSectionLength(BaseNode a, BaseNode b)
    {
        foreach (var section in sections)
        {
            if ((section.nodeA == a && section.nodeB == b) || (section.nodeA == b && section.nodeB == a))
            {
                return section.pathLength;
            }
        }
        return float.MaxValue; 
    }

    /// <summary>
    /// Возвращает длину пути
    /// </summary>
    public float GetFullPathLength(List<BaseNode> pathNodes)
    {
        float pathLength = 0;
        for (int i = 0; i < pathNodes.Count - 1; i++)
        {
            pathLength += GetPathSectionLength(pathNodes[i], pathNodes[i + 1]);
        }

        return pathLength;
    }

    /// <summary>
    /// Возвращает кратчайший путь по двум точкам, если такой имеется в кеше
    /// </summary>
    public List<BaseNode> GetCachedShortPath(BaseNode start, BaseNode end)
    {
        if (_pathShortCache.TryGetValue((start, end), out var path))
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
        if (_cachedNodes.Count == 0) 
            return null;

        return _cachedNodes[Random.Range(0, _cachedNodes.Count)];
    }

    /// <summary>
    /// Возвращает оптимальную шахту для добычи текущим поездом из текущей позиции
    /// </summary>
    public BaseNode GetBestMineNode(BaseNode currentNode, float trainSpeed, float trainSpeedMine)
    {
        List<Mine> mines = new List<Mine>();
        List<Base> bases = new List<Base>();

        // Разделяем ноды на шахты и базы
        foreach (BaseNode node in _cachedNodes)
        {
            Mine currentMine = node as Mine;
            Base currentBase = node as Base;
        
            if (currentMine != null)
            {
                mines.Add(currentMine);
            }
            else if(currentBase != null)
            {
                bases.Add(currentBase);
            }
        }

        BaseNode bestMineNode = null;
        float minDeliveryTime = float.MaxValue;

        // Перебираем все шахты
        foreach (Mine theMine in mines)
        {
            // Перебираем все базы
            foreach (Base theBase in bases)
            {
                // Получаем путь от текущей позиции до шахты
                List<BaseNode> pathToMine = GetCachedShortPath(currentNode, theMine);

                // Если поезда не могут пропускать шахты, то поезд выкопает лишний груз до конечной точки пути
                if (!_gameLevel.IsTrainСanSkipMine)
                {
				    // Проверяем, что на пути до шахты нет других шахт
				    bool hasOtherMinesOnPath = pathToMine.Any(node => node is Mine && node != theMine);
				    if (hasOtherMinesOnPath)
				    {
					    continue; // Пропускаем т.к., на пути есть другая шахта
				    }
                }

				float distanceToMine = GetFullPathLength(pathToMine);

                // Получаем путь от шахты до базы
                List<BaseNode> pathToBase = GetCachedShortPath(theMine, theBase);
                float distanceToBase = GetFullPathLength(pathToBase);

                // Рассчитываем общее время доставки
                float timeToMine = distanceToMine / trainSpeed;
                float timeMining = trainSpeedMine * theMine.miningTimeMultiplier;
                float timeToBase = distanceToBase / trainSpeed;
                float deliveryTime = timeToMine + timeMining + timeToBase;

                // Поиск лучшего времени доставки по маршруту
                if (deliveryTime < minDeliveryTime)
                {
                    minDeliveryTime = deliveryTime;
                    bestMineNode = theMine;
                }
            }
        }

        // Возвращаем лучшую шахту
        return bestMineNode;
    }

    /// <summary>
    /// Возвращает оптимальную базу для доставки груза из текущей позиции
    /// </summary>
    public BaseNode GetBestBaseNode(BaseNode currentNode, float trainSpeed, float trainSpeedMine)
    {
        List<Base> bases = new List<Base>();

        // Получаем все базы
        foreach (BaseNode node in _cachedNodes)
        {
            Base currentBase = node as Base;
        
            if(currentBase != null)
            {
                bases.Add(currentBase);
            }
        }

        BaseNode bestBaseNode = null;
        float maxDeliveryEfficiency = 0;

        // Перебираем все базы
        foreach (Base theBase in bases)
        {
            // Получаем путь от текущей позиции до базы
            List<BaseNode> pathToBase = GetCachedShortPath(currentNode, theBase);

            // Если поезда не могут пропускать базы, то поезд может потерять груз до конечной точки пути
            if (!_gameLevel.IsTrainСanSkipBase)
            {
				// Проверяем, что на пути до базы нет других баз
				bool hasOtherBaseOnPath = pathToBase.Any(node => node is Base && node != theBase);
				if (hasOtherBaseOnPath)
				{
					continue; // Пропускаем т.к. на пути есть другая база
				}
			}

            float distanceToBase = GetFullPathLength(pathToBase);

            float deliveryEfficiency = 0;

            Mine theMine = currentNode as Mine;
            if (theMine != null)
            {
                // Если текущая позиция это шахта то формула (эффективность доставки)
                float timeMining = trainSpeedMine * theMine.miningTimeMultiplier;
                float timeDelivery = (distanceToBase / trainSpeed) + timeMining;
                deliveryEfficiency = theBase.resourceMultiplier / timeDelivery;
            }
            else
            {
                // Если поезд везет груз в середине пути то формула (эффективность доставки)
                float timeDelivery = distanceToBase / trainSpeed;
                deliveryEfficiency = theBase.resourceMultiplier / timeDelivery;
            }

            // Поиск лучшего времени доставки по маршруту
            if (deliveryEfficiency > maxDeliveryEfficiency)
            {
                maxDeliveryEfficiency = deliveryEfficiency;
                bestBaseNode = theBase;
            }
        }

        // Возвращаем лучшую базу
        return bestBaseNode;
    }

    // Метод, вызывается при изменении путей, для перерасчета коротких путей и добаленных нод
    private void OnSectionsChanged()
    {
        CacheAllNodes();

        InitializeNodes();

        CacheAllShortestPaths();
    }

    private void OnDrawGizmos()
    {
		foreach (var section in sections)
		{
			if (section.nodeA != null && section.nodeB != null)
			{
				// Отрисовка линии между нодами
				Gizmos.color = Color.black;
				Gizmos.DrawLine(section.nodeA.transform.position, section.nodeB.transform.position);

				// Стиль
				GUIStyle style = new GUIStyle();
				style.normal.textColor = Color.gray;
				style.fontSize = 13;
				style.alignment = TextAnchor.MiddleCenter;

				// Рассчитываем красивую позицию текста
				Vector3 midpoint = (section.nodeA.transform.position + section.nodeB.transform.position) / 2;
				Vector3 lineDirection = (section.nodeB.transform.position - section.nodeA.transform.position).normalized;
				Vector3 perpendicular = new Vector3(-lineDirection.y, lineDirection.x, lineDirection.z).normalized;
				Vector3 textPosition = midpoint + perpendicular * 0.25f;

				// Отрисовка текста с длиной пути
				UnityEditor.Handles.Label(textPosition, $"{section.pathLength}", style);
			}
		}
	}
}