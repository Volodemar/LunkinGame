using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Управление поездом
/// </summary>
public class Train : MonoBehaviour
{
    public float speed = 5f; 

    private PathManager pathManager;
    private BaseNode currentNode;
    private BaseNode targetNode;
    private List<BaseNode> path;

    private float _progress = 0f;
    private int _currentWaypoint = 0;

    // Замедление движения поездов для лучшего наблюдения
    private const float _movementSpeedFactor = 0.1f;

    GameLevelController _gameLevel;

    public void Init(PathManager pathManager, BaseNode startNode)
    {
        _gameLevel = LevelController.Instance as GameLevelController;

        this.pathManager = pathManager;

        this.currentNode = startNode;

        this.targetNode = pathManager.GetRandomNode();

        this.path = pathManager.GetCachedShortPath(currentNode, targetNode);
    }

    private void Update()
    {
        if (path == null) 
            return;

        int nextWaypoint = _currentWaypoint + 1;

        if (nextWaypoint > path.Count-1)
        {
            // Достигли конца пути 
            _currentWaypoint = 0;
            _progress = 0;

            // Запрашиваем новый путь из текущей позиции
            currentNode = targetNode;
            targetNode = pathManager.GetRandomNode();
            path = pathManager.GetCachedShortPath(currentNode, targetNode);

        }
        else
        {
            float pathLength = pathManager.GetPathLength(path[_currentWaypoint], path[_currentWaypoint + 1]);

            _progress += Time.deltaTime * _movementSpeedFactor * speed / pathLength;
            _progress = Mathf.Clamp01(_progress);

            Vector3 startPos = path[_currentWaypoint].transform.position;
            Vector3 endPos = path[nextWaypoint].transform.position;
            this.transform.position = Vector3.Lerp(startPos, endPos, _progress);

            if (_progress >= 1f)
            {
                // Достигли ноды, выбираем новую цель
                _currentWaypoint = nextWaypoint;
                _progress = 0;
            }
        }
    }
}